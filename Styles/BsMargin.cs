using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Bootstrap.Avalonia.Toolkit.Helpers;

namespace Bootstrap.Avalonia.Toolkit.Styles;

public static class BsMargin
{
    private const string BaseFontSizeKey = "BsFontSize";

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Layoutable, bool>("IsEnabled", typeof(BsMargin), false);

    public static bool GetIsEnabled(Layoutable element) => element.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(Layoutable element, bool value) => element.SetValue(IsEnabledProperty, value);

    static BsMargin()
    {
        // Layoutable is the parent of almost all UI elements in Avalonia
        IsEnabledProperty.Changed.AddClassHandler<Layoutable>((element, args) =>
        {
            if (args.NewValue is true)
            {
                element.Classes.CollectionChanged += (_, _) => UpdateMarginStyles(element);
                UpdateMarginStyles(element);
            }
        });
    }

    private static void UpdateMarginStyles(Layoutable element)
    {
        double rem = GetBaseFontSize(element);
        
        // Bootstrap standard: m-1 is 0.25rem, m-3 is 1rem, m-5 is 3rem
        double l = 0, t = 0, r = 0, b = 0;

        foreach (var className in element.Classes)
        {
            var span = className.AsSpan();

            // Match 'm-', 'mt-', 'mb-', 'ms-', 'me-', 'mx-', 'my-'
            if (span is [('m' or 'p'), .. var rest]) 
            {
                // We only handle 'm' here; if you add BsPadding, you'd check for 'p'
                if (span[0] != 'm') continue;

                if (TryParseMargin(span, rem, out double val, out var side))
                {
                    switch (side)
                    {
                        case "all": l = t = r = b = val; break;
                        case "t": t = val; break;
                        case "b": b = val; break;
                        case "s": l = val; break; // start
                        case "e": r = val; break; // end
                        case "x": l = r = val; break;
                        case "y": t = b = val; break;
                    }
                }
            }
        }

        element.Margin = new Thickness(l, t, r, b);
    }

    private static bool TryParseMargin(ReadOnlySpan<char> span, double rem, out double value, out string side)
    {
        value = 0;
        side = "all";

        // Logic for: m-n, mt-n, mx-n, etc.
        var parts = span.ToString().Split('-');
        if (parts.Length < 2) return false;

        // Determine the side
        side = parts[0] switch { "m"=>"all", "mt"=>"t", "mb"=>"b", "ms"=>"s", "me"=>"e", "mx"=>"x", "my"=>"y", _=>"none" };
        if (side == "none") return false;

        // Determine the value multiplier (0-5 or auto)
        string valStr = parts[^1];
        if (valStr == "auto")
        {
            // Note: Avalonia Margin doesn't support 'auto' like CSS. 
            // Usually, HorizontalAlignment/VerticalAlignment handles 'auto' margins.
            value = 0; 
            return true;
        }

        if (double.TryParse(valStr, out double n))
        {
            // Bootstrap 5 Scales: 0=0, 1=.25rem, 2=.5rem, 3=1rem, 4=1.5rem, 5=3rem
            double multiplier = n switch { 0=>0, 1=>0.25, 2=>0.5, 3=>1.0, 4=>1.5, 5=>3.0, _=>0 };
            value = rem * multiplier;
            return true;
        }

        return false;
    }

    private static double GetBaseFontSize(StyledElement element)
    {
        var theme = (element as Control)?.ActualThemeVariant;
        if (Application.Current?.TryGetResource(BaseFontSizeKey, theme, out var res) == true && res is double size)
            return size;
        return 14.0;
    }
}