using ManagedDotNet.Clips.Abstractions;
using ManagedDotNet.Clips.Api.Models;
using ManagedDotNet.Clips.Api.Models.Values;

namespace ManagedDotNet.Clips.Api;

public interface IClipsEnvironment
{
    /// <summary>
    /// Evaluates a CLIPS expression in the current environment.
    /// </summary>
    public ClipsValue Eval(string expression);

    /// <summary>
    /// Evaluates a CLIPS expression and returns the result cast to the specified CLIPS value type.
    /// </summary>
    /// <typeparam name="T">The CLIPS value type to cast the result to (e.g., ClipsIntegerValue, ClipsStringValue, etc.)</typeparam>
    /// <param name="expression">A valid CLIPS expression (e.g., <c>(+ 1 2)</c>).</param>
    /// <returns>The result cast to the specified CLIPS value type.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="expression"/> is empty or invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the result cannot be cast to the specified type.</exception>
    public T Eval<T>(string expression) where T : ClipsValue;


    /// <summary>
    /// Loads a CLIPS file into the environment. (load filepath)
    /// </summary>
    /// <param name="filePath">The path to the CLIPS file to load.</param>
    /// <returns>True if the file was loaded successfully, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="filePath"/> is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    public bool Load(string filePath);

    /// <summary>
    /// (reset) the CLIPS environment
    /// </summary>
    /// <returns></returns>
    public bool Reset();


    /// <summary>
    /// Asserts a fact into the working memory.
    /// </summary>
    /// <param name="fact">The fact to assert.</param>
    /// <remarks>
    /// The fact becomes eligible for rule matching immediately.
    /// Use <see cref="Fire"/> to run the agenda and trigger activations.
    /// </remarks>
    public void Assert(IFact fact);

    /// <summary>
    /// Fires the rules agenda until quiescence or a halt condition.
    /// </summary>
    /// <remarks>
    /// Executes rule activations generated from the current working memory.
    /// Implementation may stop on explicit halts or when no activations remain.
    /// </remarks>
    public void Fire();

    /// <summary>
    /// Creates a restorable snapshot of the engine state
    /// </summary>
    /// <returns>
    /// A <see cref="Models.PrettyCapture"/> capturing the current state (i.e., facts and instances) for the specified modules
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if any specified module name is not found in the environment.</exception>
    /// <remarks>
    /// Use <see cref="RestorePretty"/> to revert the engine to this state later.
    /// Modules are captured in the order specified and restored in the same order.
    /// </remarks>
    public PrettyCapture CapturePretty();


    /// <summary>
    /// Restores the engine to a previously created checkpoint.
    /// </summary>
    /// <param name="captured">The checkpoint to restore.</param>
    /// <returns>
    /// <c>true</c> if the checkpoint was applied successfully; otherwise, <c>false</c>
    /// (e.g., incompatible or corrupted checkpoint).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="captured"/> is null.</exception>
    /// <remarks>
    /// Restoring replaces current working memory with the checkpoint's contents.
    /// </remarks>
    public bool RestorePretty(PrettyCapture captured);



    /// <summary>
    /// Creates a binary restorable snapshot of the engine state (instances and facts).
    /// </summary>
    /// <returns>
    /// A <see cref="Models.BinaryCapture"/> representing the binary capture of the current state.
    /// The bytes are serialized using Newtonsoft.Json in binary format.
    /// </returns>
    /// <remarks>
    /// Use <see cref="RestoreBinary"/> to revert the engine to this state later.
    /// Binary capture is more efficient than text-based capture for large environments.
    /// </remarks>
    public BinaryCapture CaptureBinary();

    /// <summary>
    /// Restores the engine from a binary checkpoint.
    /// </summary>
    /// <param name="capturedBinary">The binary checkpoint data to restore.</param>
    /// <returns>
    /// <c>true</c> if the checkpoint was applied successfully; otherwise, <c>false</c>
    /// (e.g., incompatible or corrupted checkpoint).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="capturedBinary"/> is null.</exception>
    /// <remarks>
    /// Restoring replaces current working memory with the checkpoint's contents.
    /// </remarks>
    public bool RestoreBinary(byte[] capturedBinary);



    public string[] GetInstances(string moduleName);

    public string[] GetFacts(string moduleName);


}

