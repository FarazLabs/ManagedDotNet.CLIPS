using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using CLIPSNET;
using ManagedDotNet.CLIPS.Abstractions;
using ManagedDotNet.CLIPS.Api;
using ManagedDotNet.CLIPS.Api.Values;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManagedDotNet.CLIPS.Core;

public class ManagedCLIPSEnvironment : IManagedCLIPSEnvironment, IDisposable
{
    private readonly ILogger<IManagedCLIPSEnvironment> _logger;
    private readonly CLIPSNET.Environment _environment; // Use the wrapper's Environment
    private readonly MemoryRouter _memoryRouter;

    internal ManagedCLIPSEnvironment(ILogger<IManagedCLIPSEnvironment> logger)
    {
        _logger = logger;
        _environment = new CLIPSNET.Environment();
        // instantiate the memory router and add to the environment
        _memoryRouter = new MemoryRouter();
        _environment.AddRouter(_memoryRouter); // FIX: Only pass the router instance
        _environment.Build(Constants.GET_INSTANCE_LIST_DEFINITION);
        _environment.Build(Constants.PRETTY_PRINT_INSTANCE_TO_ROUTER_DEFINITION);
    }


    #region API
    

    /// <summary>
    /// Evaluates a CLIPS expression in the current environment.
    /// </summary>
    /// <param name="expression">A valid CLIPS expression (e.g., <c>(+ 1 2)</c>).</param>
    /// <returns>
    /// The result as a <c>CLIPSPrimitiveValue</c>, preserving CLIPS types
    /// (numbers, symbols, strings, multifields, etc.) while hiding the underlying implementation.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="expression"/> is empty or invalid.</exception>
    public CLIPSValue Eval(string expression)
    {
        if (string.IsNullOrEmpty(expression))
            throw new ArgumentException("Expression cannot be null or empty", nameof(expression));

        PrimitiveValue? result = _environment.Eval(expression);

        if (result == null)
            throw new InvalidOperationException("CLIPS evaluation returned null");

        CLIPSValue value = result.ToCLIPSValue();

        return value;
    }

    /// <summary>
    /// Evaluates a CLIPS expression and returns the result cast to the specified CLIPS value type.
    /// </summary>
    /// <typeparam name="T">The CLIPS value type to cast the result to (e.g., CLIPSIntegerValue, CLIPSStringValue, etc.)</typeparam>
    /// <param name="expression">A valid CLIPS expression (e.g., <c>(+ 1 2)</c>).</param>
    /// <returns>The result cast to the specified CLIPS value type.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="expression"/> is empty or invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the result cannot be cast to the specified type.</exception>
    public T Eval<T>(string expression) where T : CLIPSValue
    {
        CLIPSValue result = Eval(expression);
        
        if (result is T castResult)
        {
            return castResult;
        }
        
        throw new InvalidOperationException($"CLIPS evaluation result of type {result.Type} cannot be cast to {typeof(T).Name}");
    }

    
    public bool Load(string filePath)
    { 
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"CLIPS file not found: {filePath}");

        try
        {
            // Escape backslashes for CLIPS
            string escapedPath = filePath.Replace("\\", "\\\\");
            string loadCommand = $"(load \"{escapedPath}\")";
            
            _logger.LogInformation("Loading CLIPS file: {FilePath}", filePath);
            _environment.Eval(loadCommand);
            
            _logger.LogInformation("Successfully loaded CLIPS file: {FilePath}", filePath);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load CLIPS file: {FilePath}", filePath);
            return false;
        }
    }

    public bool Reset()
    {
        _logger.LogInformation($"{nameof(IManagedCLIPSEnvironment)}: resetting");

        try
        {
            // reset clips :
            // retracts facts
            // deletes instances
            // re-sets global variables to their original values
            // asserts deffacts 
            // makes definstances
            _environment.Reset();

            _logger.LogInformation($"{nameof(IManagedCLIPSEnvironment)}: reset completed");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(IManagedCLIPSEnvironment)}: failed to reset the environment");
            return false;
        }
    }

    /// <summary>
    /// Asserts a fact into the working memory.
    /// </summary>
    /// <param name="fact">The fact to assert.</param>
    /// <remarks>
    /// The fact becomes eligible for rule matching immediately.
    /// Use <see cref="Fire"/> to run the agenda and trigger activations.
    /// </remarks>
    public void Assert(IFact fact)
    {
        _logger.LogInformation($"{nameof(IManagedCLIPSEnvironment)}: asserting");

        IEnumerable<string> clipsFacts = fact.Serialize();

        // cast and assert
        foreach (string f in clipsFacts)
        {
            _environment.AssertString(f);
        }
    }

    /// <summary>
    /// Fires the rules agenda until quiescence or a halt condition.
    /// </summary>
    /// <remarks>
    /// Executes rule activations generated from the current working memory.
    /// Implementation may stop on explicit halts or when no activations remain.
    /// </remarks>
    public void Fire()
    {

        _logger.LogInformation($"{nameof(IManagedCLIPSEnvironment)}: firing");

        _environment.Run();
    }

    /// <summary>
    /// Creates a restorable snapshot of the engine state for specified modules.P
    /// </summary>
    /// <param name="moduleNames">Array of module names to capture. If null, captures all available modules. Throws exception if any specified module is not found.</param>
    /// <returns>
    /// A <see cref="Api.EnvironmentCapture"/> capturing the current state (i.e., facts and instances) for the specified modules
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if any specified module name is not found in the environment.</exception>
    /// <remarks>
    /// Use <see cref="Restore"/> to revert the engine to this state later.
    /// Modules are captured in the order specified and restored in the same order.
    /// </remarks>
    public EnvironmentCapture Capture()
    {
        string[] modules = GetModules();

        var capture = new EnvironmentCapture();

        // Validate specified modules exist
        for (int i = 0; i < modules.Length; i++)
        {
            string moduleName = modules[i];

            var facts = GetFacts(moduleName);
            var instances = GetInstances(moduleName);

            var moduleCapture = new ModuleCapture(
                name : moduleName,
                facts : facts,
                instances: instances,
                captureOrder: i + 1
            );
            
            capture.Modules.Add(moduleCapture);
        }

        return capture;
    }

    public bool Restore(EnvironmentCapture environmentCapture)
    {
        if (environmentCapture == null) throw new ArgumentNullException(nameof(environmentCapture));

        Reset();

        _logger.LogInformation("Loading...");

        // Restore facts and instances for each module in index order
        foreach (var moduleCapture in environmentCapture.Modules.OrderByDescending(m => m.CaptureOrder))
        {
            // Set the current module
            _environment.Eval($"(set-current-module {moduleCapture.ModuleName})");

            // First, load all facts for this module (safely skip system facts)
            foreach (string fact in moduleCapture.Facts)
            {
                if (!string.IsNullOrEmpty(fact) && 
                    fact != "(initial-fact)" && 
                    !fact.StartsWith("(initial-fact"))
                {
                    _environment.AssertString(fact);
                }
            }

            // Then, load all instances for this module (safely skip system instances)
            foreach (string prettyPrintedInstance in moduleCapture.Instances)
            {
                if 
                (
                    !string.IsNullOrEmpty(prettyPrintedInstance) &&
                    !prettyPrintedInstance.StartsWith("initial-object")
                )
                {
                    var expression = $"(make-instance {prettyPrintedInstance})";
                    _environment.Eval(expression);
                }
            }

            // Module is now fully loaded, move to next module
        }

        return true;
    }


    #endregion


    public string[] GetFacts(string moduleName)
    { 
        List<string> result = new List<string>();

        // Set the current module
        _environment.Eval($"(set-current-module {moduleName})");

        // retrieve the list of facts from the specified module
        string getFactsCommand = $"(get-fact-list {moduleName})";
        MultifieldValue list = (MultifieldValue)_environment.Eval(getFactsCommand);

        // iterate over the list of fact 
        foreach (FactAddressValue fact in list)
        {
            var index = fact.FactIndex; // FIX: use property, not method

            // pretty-print the clips instance to the provided logical name associated 
            // with the previously added router (within the constructor)
            _environment.Eval($"(ppfact {index} {MemoryRouter.LOGICAL_NAME}))");

            // capture the pretty-print representation of the instance
            // (car (model bmw) (color blue) ...)
            string prettyFact = _memoryRouter.GetCapturedOutput().Replace("\r", "").Replace("\n", "");
            result.Add(prettyFact);
        }

        return result.ToArray();
    }

    public string[] GetInstances(string moduleName)
    {
        var result = new List<string>();

        // Set the current module
        _environment.Eval($"(set-current-module {moduleName})");


        // Retrieve a list of instances from the specified module. the method has been previously defined within the constcutor 
        string get_instance_list = $"({Constants.GET_INSTANCE_LIST} {moduleName})";
        MultifieldValue instances = (MultifieldValue) (_environment.Eval(get_instance_list));

        foreach (var instance in instances)
        {
            // pretty-print the clips instance to the provided logical name associated 
            // with the previously added router (within the constructor)
            var pretty_print_instance_to_router = $"(send {instance} {Constants.PRETTY_PRINT_INSTANCE_TO_ROUTER} {MemoryRouter.LOGICAL_NAME})";
            _environment.Eval(pretty_print_instance_to_router);

            // capture the pretty-print representation of the instance
            // gen1 of CLASS1 (slot1 val1) (slot2 val2) ...
            string prettyInstance = _memoryRouter.GetCapturedOutput().Replace("\r", "").Replace("\n", "");
            result.Add(prettyInstance);
        }

        return result.ToArray();
    }
    
    public string[] GetModules()
    {
        var result = new List<string>();

        try
        {
            // Get the list of all modules
            MultifieldValue modules = (MultifieldValue)_environment.Eval("(list-defmodules)");

            foreach (var module in modules)
            {
                if (module != null)
                {
                    result.Add(module.ToString()!);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get module list");
        }
        

        return result.ToArray();
    }


    /// <summary>
    /// Creates a binary restorable snapshot of the engine state (instances and facts).
    /// </summary>
    public byte[] BinaryCapture()
    {
        BinaryEnvironmentCapture binaryCapture = new BinaryEnvironmentCapture();
        
        string? tempInstancesPath = null;
        string? tempFactsPath = null;

        try
        {
            // Create temporary files for instances and facts
            tempInstancesPath = Path.GetTempFileName();
            tempFactsPath = Path.GetTempFileName();

            // Save instances to temporary file
            bool instancesSaved = BsaveInstances(tempInstancesPath);
            if (instancesSaved && File.Exists(tempInstancesPath))
            {
                binaryCapture.InstancesData = File.ReadAllBytes(tempInstancesPath);
            }

            // Save facts to temporary file
            bool factsSaved = BsaveFacts(tempFactsPath);
            if (factsSaved && File.Exists(tempFactsPath))
            {
                binaryCapture.FactsData = File.ReadAllBytes(tempFactsPath);
            }

            // Serialize BinaryEnvironmentCapture to bytes using Newtonsoft.Json
            string jsonString = JsonConvert.SerializeObject(binaryCapture);
            byte[] serializedBytes = Encoding.UTF8.GetBytes(jsonString);

            _logger.LogInformation("Binary capture completed successfully");
            return serializedBytes;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create binary capture");
            throw;
        }
        finally
        {
            // Clean up temporary files
            try
            {
                if (tempInstancesPath != null && File.Exists(tempInstancesPath))
                {
                    File.Delete(tempInstancesPath);
                }
                if (tempFactsPath != null && File.Exists(tempFactsPath))
                {
                    File.Delete(tempFactsPath);
                }
            }
            catch (Exception cleanupEx)
            {
                _logger.LogWarning(cleanupEx, "Failed to clean up temporary files during binary capture");
            }
        }
    }

    /// <summary>
    /// Restores the engine from a binary checkpoint.
    /// </summary>
    public bool BinaryRestore(byte[] binaryCaptureData)
    {
        if (binaryCaptureData == null)
            throw new ArgumentNullException(nameof(binaryCaptureData));

        BinaryEnvironmentCapture? binaryCapture = null;
        string? tempInstancesPath = null;
        string? tempFactsPath = null;

        try
        {
            // Deserialize byte array back to BinaryEnvironmentCapture using Newtonsoft.Json
            string jsonString = Encoding.UTF8.GetString(binaryCaptureData);
            binaryCapture = JsonConvert.DeserializeObject<BinaryEnvironmentCapture>(jsonString);

            if (binaryCapture == null)
            {
                _logger.LogError("Failed to deserialize binary capture data");
                return false;
            }

            // Create temporary files for instances and facts
            tempInstancesPath = Path.GetTempFileName();
            tempFactsPath = Path.GetTempFileName();

            // Write binary data to temporary files
            if (binaryCapture.InstancesData != null && binaryCapture.InstancesData.Length > 0)
            {
                File.WriteAllBytes(tempInstancesPath, binaryCapture.InstancesData);
                
                // Load instances from binary file using bload-instances
                string escapedInstancesPath = tempInstancesPath.Replace("\\", "\\\\");
                string bloadInstancesCommand = $"(bload-instances \"{escapedInstancesPath}\")";
                
                _logger.LogInformation("Restoring instances from binary file");
                _environment.Eval(bloadInstancesCommand);
            }

            if (binaryCapture.FactsData != null && binaryCapture.FactsData.Length > 0)
            {
                File.WriteAllBytes(tempFactsPath, binaryCapture.FactsData);
                
                // Load facts from binary file using bload-facts
                string escapedFactsPath = tempFactsPath.Replace("\\", "\\\\");
                string bloadFactsCommand = $"(bload-facts \"{escapedFactsPath}\")";
                
                _logger.LogInformation("Restoring facts from binary file");
                _environment.Eval(bloadFactsCommand);
            }

            _logger.LogInformation("Binary restore completed successfully");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to restore from binary capture");
            return false;
        }
        finally
        {
            // Clean up temporary files
            try
            {
                if (tempInstancesPath != null && File.Exists(tempInstancesPath))
                {
                    File.Delete(tempInstancesPath);
                }
                if (tempFactsPath != null && File.Exists(tempFactsPath))
                {
                    File.Delete(tempFactsPath);
                }
            }
            catch (Exception cleanupEx)
            {
                _logger.LogWarning(cleanupEx, "Failed to clean up temporary files during binary restore");
            }
        }
    }


    /// <summary>
    /// Saves instances to a binary file using CLIPS bsave-instances command.
    /// </summary>
    public bool BsaveInstances(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            // Escape backslashes for CLIPS
            string escapedPath = filePath.Replace("\\", "\\\\");
            string bsaveCommand = $"(bsave-instances \"{escapedPath}\")";

            _logger.LogInformation("Saving instances to binary file: {FilePath}", filePath);
            _environment.Eval(bsaveCommand);

            _logger.LogInformation("Successfully saved instances to binary file: {FilePath}", filePath);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save instances to binary file: {FilePath}", filePath);
            return false;
        }
    }

    /// <summary>
    /// Saves facts to a binary file using CLIPS bsave-facts command.
    /// </summary>
    public bool BsaveFacts(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            // Escape backslashes for CLIPS
            string escapedPath = filePath.Replace("\\", "\\\\");
            string bsaveCommand = $"(bsave-facts \"{escapedPath}\")";

            _logger.LogInformation("Saving facts to binary file: {FilePath}", filePath);
            _environment.Eval(bsaveCommand);

            _logger.LogInformation("Successfully saved facts to binary file: {FilePath}", filePath);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save facts to binary file: {FilePath}", filePath);
            return false;
        }
    }

    public void Dispose()
    {
        _environment?.Dispose();
    }
}
