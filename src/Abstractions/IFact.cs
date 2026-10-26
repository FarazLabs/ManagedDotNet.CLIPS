namespace ManagedDotNet.CLIPS.Abstractions;


/// <summary>
/// Represents an item that can be transformed into one or more facts 
/// suitable for assertion in the rules engine.
/// </summary>
public interface IFact
{
    /// <summary>
    /// Converts the item into an enumerable of facts strings that the rules engine can consume. <br />
    /// e.g., (person (name john) (lname smith)
    /// </summary>
    IEnumerable<string> Serialize();
}
