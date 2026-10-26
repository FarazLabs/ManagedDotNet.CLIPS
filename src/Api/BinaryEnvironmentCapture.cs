namespace ManagedDotNet.CLIPS.Api;

/// <summary>
/// Represents a binary snapshot of the CLIPS environment state (instances and facts).
/// </summary>
public class BinaryEnvironmentCapture
{
    /// <summary>
    /// Binary data representing saved instances from CLIPS bsave-instances command.
    /// </summary>
    public byte[]? InstancesData { get; set; }

    /// <summary>
    /// Binary data representing saved facts from CLIPS bsave-facts command.
    /// </summary>
    public byte[]? FactsData { get; set; }
}

