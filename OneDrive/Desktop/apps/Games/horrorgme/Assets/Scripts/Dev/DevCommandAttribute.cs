using System;

/// <summary>
/// Attribute to register a developer console command. Apply to public methods on a MonoBehaviour (instance)
/// or static methods. Supported method signatures:
/// - string Handler(string[] args)
/// - void Handler(string[] args)
/// Any return value string will be printed to the console output.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class DevCommandAttribute : Attribute
{
    /// <summary>
    /// Canonical command name (lowercase recommended).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Short description for help/registry list.
    /// </summary>
    public string Description { get; }

    public DevCommandAttribute(string name, string description = "")
    {
        Name = name;
        Description = description;
    }
}


