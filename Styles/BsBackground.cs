using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Bootstrap.Avalonia.Toolkit.Helpers;

namespace Bootstrap.Avalonia.Toolkit.Styles;

public static class BsBackground
{

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>(
            "IsEnabled", 
            typeof(BsBackground), 
            defaultValue: false);

    public static bool GetIsEnabled(Control control) => 
        control.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(Control control, bool value) => 
        control.SetValue(IsEnabledProperty, value);

    
    static BsBackground()
    {
        IsEnabledProperty.Changed.AddClassHandler<Control>(((control, args) =>
        {
            if (args.NewValue is true)
            {
                control.Classes.CollectionChanged += (_, _) => UpdateControlStyles(control);
                UpdateControlStyles(control);
            }
        }));
    }

    private static void UpdateControlStyles(Control control)
    {
        foreach (var className in control.Classes)
        {
            if (className.AsSpan() is ['b', 'g', '-', .. var rest])
            {
                string key = rest.ToBsPascalCase();
                if (Application.Current?.TryGetResource(key, control.ActualThemeVariant, out var res) == true 
                    && res is IBrush brush)
                {
                    control.SetValue(TemplatedControl.BackgroundProperty, brush);
                    break;
                }
            }
        }
    }
    
}