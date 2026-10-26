namespace ManagedDotNet.CLIPS.Api;

public class ModuleCapture
{
    public string ModuleName { get; set; } = string.Empty;
    public string[] Facts { get; set; } 
    public string[] Instances { get; set; } 
    public int CaptureOrder { get; set; }

    public ModuleCapture(string name, string[] facts, string[] instances, int captureOrder)
    {
        ModuleName = name;
        Facts = facts;
        Instances = instances;
        CaptureOrder = captureOrder;
    }
}
