using Avalonia;

namespace Bootstrap.Avalonia.Toolkit.Styles;

public static class BsOpacity
{
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Visual, bool>("IsEnabled", typeof(BsOpacity), false);

    static BsOpacity()
    {
        IsEnabledProperty.Changed.AddClassHandler<Visual>((visual, args) =>
        {
            if (args.NewValue is true)
            {
                visual.Classes.CollectionChanged += (_, _) =>  UpdateOpacity(visual); 
                UpdateOpacity(visual);
            }
        });
    }

    private static void UpdateOpacity(Visual visual)
    {
        foreach (var className in visual.Classes)
        {
            var span = className.AsSpan();
            if (span is ['o', 'p', 'a', 'c', 'i', 't', 'y', '-', .. var valPart])
            {
                if (double.TryParse(valPart.ToString(), out double n))
                {
                    visual.Opacity = n / 100.0;
                    break;
                }
            }
        }
    }
}