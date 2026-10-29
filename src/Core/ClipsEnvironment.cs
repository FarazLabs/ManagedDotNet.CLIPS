using System.Text;
using ManagedDotNet.Clips.Abstractions;
using ManagedDotNet.Clips.Api;
using ManagedDotNet.Clips.Api.Models;
using ManagedDotNet.Clips.Api.Models.Values;
using ManagedDotNet.Clips.Interop;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManagedDotNet.Clips.Core;

public class ClipsEnvironment : IClipsEnvironment, IDisposable
{
    private readonly ILogger _logger;
    private IntPtr _environment;

    internal ClipsEnvironment(ILogger logger)
    {
        _logger = logger;
        _environment = ClipsNative.Clips_CreateEnvironment();
        if (_environment == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to create CLIPS environment");
        }
    }

    public ClipsValue Eval(string expression)
    {
        if (string.IsNullOrEmpty(expression))
        {
            throw new ArgumentException("Expression cannot be null or empty", nameof(expression));
        }

        EnsureAlive();

        if (ClipsNative.Clips_Eval(_environment, expression, out ClipsPInvokeValue native) == 0)
        {
            throw new InvalidOperationException($"CLIPS evaluation failed for expression: {expression}");
        }

        try
        {
            return ClipsValueMapper.ToManaged(ref native);
        }
        finally
        {
            ClipsNative.Clips_FreeValue(ref native);
        }
    }

    public T Eval<T>(string expression) where T : ClipsValue
    {
        ClipsValue result = Eval(expression);

        if (result is T castResult)
        {
            return castResult;
        }

        throw new InvalidOperationException($"CLIPS evaluation result of type {result.Type} cannot be cast to {typeof(T).Name}");
    }

    public bool Load(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"CLIPS file not found: {filePath}");
        }

        EnsureAlive();

        try
        {
            _logger.LogInformation("Loading CLIPS file: {FilePath}", filePath);
            int ok = ClipsNative.Clips_Load(_environment, filePath);
            if (ok == 0)
            {
                _logger.LogError("Failed to load CLIPS file: {FilePath}", filePath);
                return false;
            }

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
        _logger.LogInformation("{Name}: resetting", nameof(IClipsEnvironment));

        try
        {
            EnsureAlive();
            ClipsNative.Clips_Reset(_environment);
            _logger.LogInformation("{Name}: reset completed", nameof(IClipsEnvironment));
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Name}: failed to reset the environment", nameof(IClipsEnvironment));
            return false;
        }
    }

    public void Assert(IFact fact)
    {
        _logger.LogInformation("{Name}: asserting", nameof(IClipsEnvironment));
        EnsureAlive();

        foreach (string f in fact.Serialize())
        {
            if (ClipsNative.Clips_AssertString(_environment, f) == 0)
            {
                throw new InvalidOperationException($"Failed to assert fact: {f}");
            }
        }
    }

    public void Fire()
    {
        _logger.LogInformation("{Name}: firing", nameof(IClipsEnvironment));
        EnsureAlive();
        ClipsNative.Clips_Run(_environment, -1);
    }

    public PrettyCapture CapturePretty()
    {
        string[] modules = GetModules();
        var capture = new PrettyCapture();

        for (int i = 0; i < modules.Length; i++)
        {
            string moduleName = modules[i];
            var moduleCapture = new PrettyModuleCapture(
                name: moduleName,
                facts: GetFacts(moduleName),
                instances: GetInstances(moduleName),
                captureOrder: i + 1);

            capture.Modules.Add(moduleCapture);
        }

        return capture;
    }

    public bool RestorePretty(PrettyCapture environmentCapture)
    {
        if (environmentCapture == null)
        {
            throw new ArgumentNullException(nameof(environmentCapture));
        }

        Reset();
        _logger.LogInformation("Loading...");

        foreach (var moduleCapture in environmentCapture.Modules.OrderByDescending(m => m.CaptureOrder))
        {
            Eval($"(set-current-module {moduleCapture.ModuleName})");

            foreach (string fact in moduleCapture.Facts)
            {
                if (!string.IsNullOrEmpty(fact) &&
                    fact != "(initial-fact)" &&
                    !fact.StartsWith("(initial-fact", StringComparison.Ordinal))
                {
                    ClipsNative.Clips_AssertString(_environment, fact);
                }
            }

            foreach (string prettyPrintedInstance in moduleCapture.Instances)
            {
                if (!string.IsNullOrEmpty(prettyPrintedInstance) &&
                    !prettyPrintedInstance.StartsWith("initial-object", StringComparison.Ordinal))
                {
                    Eval($"(make-instance {prettyPrintedInstance})");
                }
            }
        }

        return true;
    }

    public string[] GetFacts(string moduleName)
    {
        EnsureAlive();

        if (ClipsNative.Clips_GetFactPPForms(_environment, moduleName, out IntPtr formsPtr, out int count) == 0)
        {
            throw new InvalidOperationException($"Failed to get facts for module '{moduleName}'");
        }

        try
        {
            return ClipsValueMapper.ReadStringArray(formsPtr, count)
                .Select(s => s.Replace("\r", string.Empty).Replace("\n", string.Empty))
                .ToArray();
        }
        finally
        {
            ClipsNative.Clips_FreeStringArray(formsPtr, count);
        }
    }

    public string[] GetInstances(string moduleName)
    {
        EnsureAlive();

        if (ClipsNative.Clips_GetInstancePPForms(_environment, moduleName, out IntPtr formsPtr, out int count) == 0)
        {
            throw new InvalidOperationException($"Failed to get instances for module '{moduleName}'");
        }

        try
        {
            return ClipsValueMapper.ReadStringArray(formsPtr, count)
                .Select(s => s.Replace("\r", string.Empty).Replace("\n", string.Empty))
                .ToArray();
        }
        finally
        {
            ClipsNative.Clips_FreeStringArray(formsPtr, count);
        }
    }

    public string[] GetModules()
    {
        EnsureAlive();

        try
        {
            if (ClipsNative.Clips_GetModuleNames(_environment, out IntPtr namesPtr, out int count) == 0)
            {
                return Array.Empty<string>();
            }

            try
            {
                return ClipsValueMapper.ReadStringArray(namesPtr, count);
            }
            finally
            {
                ClipsNative.Clips_FreeStringArray(namesPtr, count);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get module list");
            return Array.Empty<string>();
        }
    }

    public BinaryCapture CaptureBinary()
    {
        string? tempInstancesPath = null;
        string? tempFactsPath = null;

        try
        {
            tempInstancesPath = Path.GetTempFileName();
            tempFactsPath = Path.GetTempFileName();

            byte[] instancesData = BsaveInstances(tempInstancesPath)
                ? File.ReadAllBytes(tempInstancesPath)
                : [];

            byte[] factsData = BsaveFacts(tempFactsPath)
                ? File.ReadAllBytes(tempFactsPath)
                : [];

            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.Write(instancesData.Length);
            writer.Write(instancesData);

            writer.Write(factsData.Length);
            writer.Write(factsData);

            writer.Flush();

            _logger.LogInformation("Binary capture completed successfully");

            return new BinaryCapture(stream.ToArray());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create binary capture");
            throw;
        }
        finally
        {
            TryDelete(tempInstancesPath);
            TryDelete(tempFactsPath);
        }
    }

    public bool RestoreBinary(byte[] binaryCaptureData)
    {
        ArgumentNullException.ThrowIfNull(binaryCaptureData);

        string? tempInstancesPath = null;
        string? tempFactsPath = null;

        try
        {
            using var stream = new MemoryStream(binaryCaptureData);
            using var reader = new BinaryReader(stream);

            byte[] ReadSection()
            {
                if (stream.Length - stream.Position < sizeof(int))
                {
                    throw new InvalidDataException("Missing section length.");
                }

                int length = reader.ReadInt32();

                if (length < 0 || length > stream.Length - stream.Position)
                {
                    throw new InvalidDataException("Invalid section length.");
                }

                byte[] data = reader.ReadBytes(length);

                if (data.Length != length)
                {
                    throw new InvalidDataException("Incomplete section data.");
                }

                return data;
            }

            byte[] instancesData = ReadSection();
            byte[] factsData = ReadSection();

            if (stream.Position != stream.Length)
            {
                throw new InvalidDataException("Unexpected trailing data.");
            }

            tempInstancesPath = Path.GetTempFileName();
            tempFactsPath = Path.GetTempFileName();

            if (instancesData.Length > 0)
            {
                File.WriteAllBytes(tempInstancesPath, instancesData);

                string escaped = tempInstancesPath.Replace(
                    "\\", "\\\\", StringComparison.Ordinal);

                Eval($"(bload-instances \"{escaped}\")");
            }

            if (factsData.Length > 0)
            {
                File.WriteAllBytes(tempFactsPath, factsData);

                string escaped = tempFactsPath.Replace(
                    "\\", "\\\\", StringComparison.Ordinal);

                Eval($"(bload-facts \"{escaped}\")");
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
            TryDelete(tempInstancesPath);
            TryDelete(tempFactsPath);
        }
    }

    public bool BsaveInstances(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        try
        {
            string escapedPath = filePath.Replace("\\", "\\\\", StringComparison.Ordinal);
            Eval($"(bsave-instances \"{escapedPath}\")");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save instances to binary file: {FilePath}", filePath);
            return false;
        }
    }

    public bool BsaveFacts(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        try
        {
            string escapedPath = filePath.Replace("\\", "\\\\", StringComparison.Ordinal);
            Eval($"(bsave-facts \"{escapedPath}\")");
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
        if (_environment != IntPtr.Zero)
        {
            _ = ClipsNative.Clips_DestroyEnvironment(_environment);
            _environment = IntPtr.Zero;
        }

        GC.SuppressFinalize(this);
    }

    private void EnsureAlive()
    {
        if (_environment == IntPtr.Zero)
        {
            throw new ObjectDisposedException(nameof(ClipsEnvironment));
        }
    }

    private void TryDelete(string? path)
    {
        try
        {
            if (path != null && File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception cleanupEx)
        {
            _logger.LogWarning(cleanupEx, "Failed to clean up temporary file {Path}", path);
        }
    }
}
