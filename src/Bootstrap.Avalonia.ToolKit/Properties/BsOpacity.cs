namespace Bootstrap.Avalonia.ToolKit.Properties;

/// <summary>
/// Static Utility Class for Mutating Alpha-Channel Transparency via Attached Dependency Properties.
/// </summary>
public static class BsOpacity
{
    /// <summary>
    /// The RegisterAttached Boolean Flag for the Visual Component Instance.
    /// </summary>
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Visual, bool>("IsEnabled", typeof(BsOpacity));

    /// <summary>
    /// Static Constructor for Event Delegate Registration within the Global Class Handler.
    /// </summary>
    static BsOpacity()
    {
        // Hooking Global Class Handler to the PropertyChanged Observable.
        IsEnabledProperty.Changed.AddClassHandler<Visual>((instance, eventArgs) =>
        {
            if (eventArgs.NewValue is true)
            {
                // Registering a Callback to the CollectionChanged Event for Dynamic Attribute Updates.
                instance.Classes.CollectionChanged += (_, _) => InvokeOpacityUpdate(instance); 
                InvokeOpacityUpdate(instance);
            }
        });
    }

    /// <summary>
    /// Logic Implementation for Parsing Identifier Tokens and Mutating the Alpha Scalar.
    /// </summary>
    private static void InvokeOpacityUpdate(Visual instance)
    {
        if (instance.TemplatedParent != null) return;

        // Iterating through the StyleClass Collection.
        foreach (var identifier in instance.Classes)
        {
            var segment = identifier.AsSpan();
            
            // Buffer Validation for the 'opacity-' Prefix (Offset 8).
            if (segment is ['o', 'p', 'a', 'c', 'i', 't', 'y', '-', .. var payload])
            {
                // Parsing the Payload Segment into a Double-Precision Floating Point.
                if (double.TryParse(payload.ToString(), out double numericLiteral))
                {
                    // Scalar Normalization: Converting Percentage Integer to a 0.0-1.0 Range.
                    instance.Opacity = numericLiteral / 100.0;
                    
                    // Exclusive Assignment: Breaking the loop as Opacity is a Single-Value Property.
                    break;
                }
            }
        }
    }
}