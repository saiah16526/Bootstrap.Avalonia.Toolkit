using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace Bootstrap.Avalonia.ToolKit.Properties;

/// <summary>
/// Static Utility Class for Bootstrap-style Spacing (gap) via Dependency Properties.
/// </summary>
public static class BsSpacing
{
    private const string BaseFontSizeKey = "BsBodyFontSize";

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsEnabled", typeof(BsSpacing), false);

    public static bool GetIsEnabled(Control instance) => instance.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(Control instance, bool value) => instance.SetValue(IsEnabledProperty, value);

    static BsSpacing()
    {
        IsEnabledProperty.Changed.AddClassHandler<Control>((instance, eventArgs) =>
        {
            if (eventArgs.NewValue is true)
            {
                instance.Classes.CollectionChanged += (_, _) => InvokeSpacingUpdate(instance);
                InvokeSpacingUpdate(instance);
            }
        });
    }

    private static void InvokeSpacingUpdate(Control instance)
    {
        // Looking for "gap-" pattern
        var gapClass = instance.Classes.FirstOrDefault(c => c.StartsWith("gap-"));
        if (gapClass == null) return;

        double remUnit = QueryRootFontSize(instance);
        if (TryParseGapMagnitude(gapClass, remUnit, out double magnitude))
        {
            ApplySpacing(instance, magnitude);
        }
    }

    /// <summary>
    /// Routes the calculated magnitude to the specific Panel's spacing properties.
    /// </summary>
    private static void ApplySpacing(Control instance, double value)
    {
        switch (instance)
        {
            case StackPanel sp:
                sp.Spacing = value;
                break;

            case DockPanel dp:
                dp.HorizontalSpacing = value;
                dp.VerticalSpacing = value;
                break;

            case Grid grid:
                grid.RowSpacing = value;
                grid.ColumnSpacing = value;
                break;

            case UniformGrid ug:
                ug.ColumnSpacing = value;
                ug.RowSpacing = value;
                break;

            case WrapPanel wp:
                wp.ItemSpacing = value;
                wp.LineSpacing = value;
                break;
        }
    }

    private static bool TryParseGapMagnitude(string identifier, double remUnit, out double magnitude)
    {
        magnitude = 0;
        var parts = identifier.Split('-');
        
        if (parts.Length < 2) return false;

        if (double.TryParse(parts[^1], out double numericLiteral))
        {
            // Bootstrap spacing scale: 0=0, 1=.25rem, 2=.5rem, 3=1rem, 4=1.5rem, 5=3rem
            double coefficient = numericLiteral switch
            {
                0 => 0,
                1 => 0.25,
                2 => 0.5,
                3 => 1.0,
                4 => 1.5,
                5 => 3.0,
                _ => 0
            };
            magnitude = remUnit * coefficient;
            return true;
        }

        return false;
    }

    private static double QueryRootFontSize(Control instance)
    {
        if (Application.Current?.TryGetResource(BaseFontSizeKey, instance.ActualThemeVariant, out var resource) == true && resource is double scalar)
            return scalar;
        
        return 14.0; 
    }
}