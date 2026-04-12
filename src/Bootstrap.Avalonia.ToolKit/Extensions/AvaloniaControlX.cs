using System.Reflection;
using Avalonia.Media;

namespace Bootstrap.Avalonia.ToolKit.Extensions;

/// <summary>
/// Static Extension Provider for Metadata-driven Property Manipulation via Reflection.
/// </summary>
public static class AvaloniaControlX
{
    /// <summary>
    /// Executes a Late-bound Assignment on a Dependency Object Property via its Identifier.
    /// </summary>
    /// <param name="instance">The Target Object Instance for the Operation.</param>
    /// <param name="identifier">The String Key representing the Property Metadata.</param>
    /// <param name="payload">The Object Reference to be assigned to the Property.</param>
    public static void Set(this Control instance, string? identifier, Object? payload)
    {
        // Validation Guard: Abort if the Pointer or the Identifier is Null.
        if (identifier == null || payload == null) return;

        // Querying the Runtime Type Metadata for the specified PropertyInfo.
        PropertyInfo? propertyMetadata = instance.GetType().GetProperty(identifier);

        // Conditional Invocation: Execute the Setter if the Metadata Reference is not Null.
        if (propertyMetadata != null)
        {
            // Invoking the Property Mutator via the Reflection API.
            propertyMetadata.SetValue(instance, payload);
        }
    }
}