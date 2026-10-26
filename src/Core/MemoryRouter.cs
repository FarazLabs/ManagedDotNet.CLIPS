using System.Text;
using CLIPSNET;

namespace ManagedDotNet.CLIPS.Core;
/*************************************************************************/
/*                          IMPORTANT                                   */
/*  inscom.c has been edited to use "memory" as default logical name    */
/************************************************************************/

/// <summary>
/// inscom.c has been edited to use "memory" as logical name
/// </summary>
internal class MemoryRouter : Router
{
    public const string LOGICAL_NAME = "memory";

    private readonly StringBuilder _output;

    public MemoryRouter()
        : base()
    {
        _output = new StringBuilder();
    }

    public override bool Query(string logicalName)
    {
        // Intercept output for "stdout", as "save-facts" uses it
        return logicalName == LOGICAL_NAME;
    }

    public override void Write(string logicalName, string printString)
    {
        // Capture the output into a memory buffer (StringBuilder)
        _output.Append(printString);
    }

    public override int Read(string logicalName)
    {
        // Not used for this case
        return -1;
    }

    public override int Unread(string logicalName, int theChar)
    {
        // Not used for this case
        return 0;
    }

    // Method to retrieve captured output (e.g., facts)
    public string GetCapturedOutput()
    {
        return _output.ToString();
    }

    // Method to clear captured output
    public void ClearOutput()
    {
        _output.Clear();
    }
}