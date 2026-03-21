using Avalonia;
using Avalonia.Layout;

namespace Bootstrap.Avalonia.Toolkit.Styles;

public static class BsAlignSelf
{
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Layoutable, bool>("IsEnabled", typeof(BsAlignSelf), false);

    static BsAlignSelf()
    {
        IsEnabledProperty.Changed.AddClassHandler<Layoutable>((element, args) =>
        {
            if (args.NewValue is true)
            {
                element.Classes.CollectionChanged += (_, _) => UpdateAlignment(element); 
                UpdateAlignment(element);
            }
        });
    }

    private static void UpdateAlignment(Layoutable element)
    {
        foreach (var className in element.Classes)
        {
            var span = className.AsSpan();

            // Handle Horizontal: align-self-start, align-self-center, align-self-end, align-self-stretch
            if (span.StartsWith("align-self-".AsSpan()))
            {
                var type = span[11..];
                
                // We check if it's a vertical or horizontal keyword
                if (type.SequenceEqual("start".AsSpan())) element.HorizontalAlignment = HorizontalAlignment.Left;
                else if (type.SequenceEqual("center".AsSpan())) element.HorizontalAlignment = HorizontalAlignment.Center;
                else if (type.SequenceEqual("end".AsSpan())) element.HorizontalAlignment = HorizontalAlignment.Right;
                else if (type.SequenceEqual("stretch".AsSpan())) element.HorizontalAlignment = HorizontalAlignment.Stretch;
                
                // Handle Vertical: v-align-self-start, etc (Custom prefix for disambiguation)
                else if (type.StartsWith("v-".AsSpan()))
                {
                    var vType = type[2..];
                    if (vType.SequenceEqual("start".AsSpan())) element.VerticalAlignment = VerticalAlignment.Top;
                    else if (vType.SequenceEqual("center".AsSpan())) element.VerticalAlignment = VerticalAlignment.Center;
                    else if (vType.SequenceEqual("end".AsSpan())) element.VerticalAlignment = VerticalAlignment.Bottom;
                    else if (vType.SequenceEqual("stretch".AsSpan())) element.VerticalAlignment = VerticalAlignment.Stretch;
                }
            }
        }
    }
}