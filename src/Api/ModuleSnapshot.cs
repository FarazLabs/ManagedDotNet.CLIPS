namespace ManagedDotNet.Clips.Api;

public class ModuleSnapshot
{
    public string ModuleName { get; set; } = string.Empty;
    public string[] Facts { get; set; } 
    public string[] Instances { get; set; } 
    public int CaptureOrder { get; set; }

    public ModuleSnapshot(string name, string[] facts, string[] instances, int captureOrder)
    {
        ModuleName = name;
        Facts = facts;
        Instances = instances;
        CaptureOrder = captureOrder;
    }
}
