using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Bootstrap.Avalonia.Toolkit.Helpers;

namespace Bootstrap.Avalonia.Toolkit.Styles;

public static class BsRounded
{
    private const string BaseFontSizeKey = "BsFontSize";

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<StyledElement, bool>("IsEnabled", typeof(BsRounded), false);

    public static bool GetIsEnabled(StyledElement element) => element.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(StyledElement element, bool value) => element.SetValue(IsEnabledProperty, value);

    static BsRounded()
    {
        IsEnabledProperty.Changed.AddClassHandler<StyledElement>((element, args) =>
        {
            if (args.NewValue is true)
            {
                element.Classes.CollectionChanged += (_, _) => UpdateRoundedStyles(element); 
                UpdateRoundedStyles(element);
            }
        });
    }

    private static void UpdateRoundedStyles(AvaloniaObject obj)
    {
        if (obj is not StyledElement element) return;

        double rem = GetBaseFontSize(element);
        // Bootstrap standard: 'rounded' is 0.375rem
        double baseRadius = rem * 0.375;

        double tl = 0, tr = 0, bl = 0, br = 0;

        foreach (var className in element.Classes)
        {
            var span = className.AsSpan();

            if (span.StartsWith("rounded".AsSpan()))
            {
                // rounded-circle (999 is standard for perfect circles in UI)
                if (span.SequenceEqual("rounded-circle".AsSpan()))
                {
                    tl = tr = bl = br = 999;
                }
                // rounded-pill (usually 50rem, effectively half-height)
                else if (span.SequenceEqual("rounded-pill".AsSpan()))
                {
                    tl = tr = bl = br = 50; 
                }
                // rounded (all sides)
                else if (span.SequenceEqual("rounded".AsSpan()))
                {
                    tl = tr = bl = br = baseRadius;
                }
                // rounded-0 to rounded-5
                else if (span is ['r', 'o', 'u', 'n', 'd', 'e', 'd', '-', var n] && char.IsDigit(n))
                {
                    double multiplier = char.GetNumericValue(n);
                    tl = tr = bl = br = (baseRadius * multiplier) / 3.0; // BS5 scales relative to base
                }
                // Directional: rounded-top, rounded-start-2, etc.
                else
                {
                    ApplyDirectionalRounded(span, baseRadius, ref tl, ref tr, ref bl, ref br);
                }
            }
        }

        SetCornerRadius(obj, new CornerRadius(tl, tr, br, bl));
    }

    private static void ApplyDirectionalRounded(ReadOnlySpan<char> span, double @base, ref double tl, ref double tr, ref double bl, ref double br)
    {
        // Extract direction and numeric value if present (e.g., rounded-top-3)
        double radius = @base;
        if (span.Length > 0 && char.IsDigit(span[^1]))
        {
            radius = (@base * char.GetNumericValue(span[^1])) / 3.0;
        }

        if (span.Contains("top".AsSpan(), StringComparison.OrdinalIgnoreCase)) { tl = tr = radius; }
        else if (span.Contains("bottom".AsSpan(), StringComparison.OrdinalIgnoreCase)) { bl = br = radius; }
        else if (span.Contains("start".AsSpan(), StringComparison.OrdinalIgnoreCase)) { tl = bl = radius; }
        else if (span.Contains("end".AsSpan(), StringComparison.OrdinalIgnoreCase)) { tr = br = radius; }
    }

    private static void SetCornerRadius(AvaloniaObject obj, CornerRadius radius)
    {
        // Manual property check to cover all major Avalonia types
        if (obj is Border b) b.CornerRadius = radius;
        else if (obj is Button btn) btn.CornerRadius = radius;
        else if (obj is TemplatedControl tc) tc.SetValue(TemplatedControl.CornerRadiusProperty, radius);
    }

    private static double GetBaseFontSize(StyledElement element)
    {
        var theme = (element as Control)?.ActualThemeVariant;
        if (Application.Current?.TryGetResource(BaseFontSizeKey, theme, out var res) == true && res is double size)
            return size;
        return 14.0;
    }
}