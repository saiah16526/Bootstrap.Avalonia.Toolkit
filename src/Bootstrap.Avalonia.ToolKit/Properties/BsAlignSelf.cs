
namespace Bootstrap.Avalonia.ToolKit.Properties;

/// <summary>
/// Static Utility Class for Mutating Horizontal and Vertical Alignment Metadata via Attached Dependency Properties.
/// </summary>
public static class BsAlignSelf
{
    /// <summary>
    /// The RegisterAttached Boolean Flag for the Layoutable Component Instance.
    /// </summary>
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Layoutable, bool>("IsEnabled", typeof(BsAlignSelf), false);

    /// <summary>
    /// Static Constructor for Event Delegate Registration within the Global Class Handler.
    /// </summary>
    static BsAlignSelf()
    {
        IsEnabledProperty.Changed.AddClassHandler<Layoutable>((instance, eventArgs) =>
        {
            if (eventArgs.NewValue is true)
            {
                // Assigning a Callback to the CollectionChanged Observable for Runtime Metadata Mutation.
                instance.Classes.CollectionChanged += (_, _) => InvokeAlignmentUpdate(instance); 
                InvokeAlignmentUpdate(instance);
            }
        });
    }

    /// <summary>
    /// Implementation Logic for Parsing Identifier Tokens and Mutating Alignment Properties.
    /// </summary>
    private static void InvokeAlignmentUpdate(Layoutable instance)
    {
        if (instance.TemplatedParent != null) return;
        
        // Iterating through the StyleClass Collection.
        foreach (var identifier in instance.Classes)
        {
            var segment = identifier.AsSpan();

            // Prefix Validation: Targeting the 'align-self-' Buffer Segment (Offset 11).
            if (segment.StartsWith("align-self-".AsSpan()))
            {
                var payload = segment[11..];
                
                // Logic Branch: Mapping Payload Tokens to HorizontalAlignment Enumeration.
                if (payload.SequenceEqual("start".AsSpan())) instance.HorizontalAlignment = HorizontalAlignment.Left;
                else if (payload.SequenceEqual("center".AsSpan())) instance.HorizontalAlignment = HorizontalAlignment.Center;
                else if (payload.SequenceEqual("end".AsSpan())) instance.HorizontalAlignment = HorizontalAlignment.Right;
                else if (payload.SequenceEqual("stretch".AsSpan())) instance.HorizontalAlignment = HorizontalAlignment.Stretch;
                
                // Logic Branch: Mapping Nested 'v-' Prefix to VerticalAlignment Enumeration.
                else if (payload.StartsWith("v-".AsSpan()))
                {
                    var verticalPayload = payload[2..];
                    if (verticalPayload.SequenceEqual("top".AsSpan())) instance.VerticalAlignment = VerticalAlignment.Top;
                    else if (verticalPayload.SequenceEqual("center".AsSpan())) instance.VerticalAlignment = VerticalAlignment.Center;
                    else if (verticalPayload.SequenceEqual("bottom".AsSpan())) instance.VerticalAlignment = VerticalAlignment.Bottom;
                    else if (verticalPayload.SequenceEqual("stretch".AsSpan())) instance.VerticalAlignment = VerticalAlignment.Stretch;
                }
                
            }
        }
    }
}