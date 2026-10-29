namespace ManagedDotNet.Clips.Api.Models;

public class PrettyCapture
{
    public List<PrettyModuleCapture> Modules { get; set; } = new List<PrettyModuleCapture>();
}

public class PrettyModuleCapture
{
    public string ModuleName { get; set; } = string.Empty;
    public string[] Facts { get; set; }
    public string[] Instances { get; set; }
    public int CaptureOrder { get; set; }

    public PrettyModuleCapture(string name, string[] facts, string[] instances, int captureOrder)
    {
        ModuleName = name;
        Facts = facts;
        Instances = instances;
        CaptureOrder = captureOrder;
    }
}
