using System;
using System.Collections.Concurrent;
using Avalonia;
using Avalonia.Controls;
using Bootstrap.Avalonia.Toolkit.Helpers;

namespace Bootstrap.Avalonia.Toolkit.Styles;

public static class BsPadding
{
    private const string BaseFontSizeKey = "BsFontSize";
    
    // Cache properties to avoid repeated reflection/lookup
    private static readonly ConcurrentDictionary<Type, AvaloniaProperty?> PaddingPropertyCache = new();

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<StyledElement, bool>("IsEnabled", typeof(BsPadding), false);

    public static bool GetIsEnabled(StyledElement element) => element.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(StyledElement element, bool value) => element.SetValue(IsEnabledProperty, value);

    static BsPadding()
    {
        IsEnabledProperty.Changed.AddClassHandler<StyledElement>((element, args) =>
        {
            if (args.NewValue is true)
            {
                element.Classes.CollectionChanged += (_, _) =>  UpdatePaddingStyles(element); 
                UpdatePaddingStyles(element);
            }
        });
    }

    private static void UpdatePaddingStyles(AvaloniaObject obj)
    {
        // 1. Efficiently find the Padding property for this specific type
        var paddingProp = GetPaddingProperty(obj.GetType());
        if (paddingProp == null) return;

        if (obj is not StyledElement element) return;

        double rem = GetBaseFontSize(element);
        double l = 0, t = 0, r = 0, b = 0;

        foreach (var className in element.Classes)
        {
            var span = className.AsSpan();

            if (span is ['p', .. var rest])
            {
                if (TryParsePadding(span, rem, out double val, out var side))
                {
                    switch (side)
                    {
                        case "all": l = t = r = b = val; break;
                        case "t": t = val; break;
                        case "b": b = val; break;
                        case "s": l = val; break;
                        case "e": r = val; break;
                        case "x": l = r = val; break;
                        case "y": t = b = val; break;
                    }
                }
            }
        }

        // 2. Direct set via the found property—no type casting needed here
        obj.SetValue(paddingProp, new Thickness(l, t, r, b));
    }

    private static AvaloniaProperty? GetPaddingProperty(Type type)
    {
        // Check cache first for lightning-fast lookups
        return PaddingPropertyCache.GetOrAdd(type, t =>
        {
            // Search for any property named "Padding" registered on this type or its parents
            return AvaloniaPropertyRegistry.Instance.GetRegistered(t)
                .FirstOrDefault(p => p.Name == "Padding");
        });
    }

    private static bool TryParsePadding(ReadOnlySpan<char> span, double rem, out double value, out string side)
    {
        value = 0;
        side = "all";

        int dashIndex = span.IndexOf('-');
        if (dashIndex == -1) return false;

        var prefix = span[..dashIndex];
        var valStr = span[(dashIndex + 1)..];

        side = prefix switch 
        { 
            "p" => "all", "pt" => "t", "pb" => "b", "ps" => "s", "pe" => "e", "px" => "x", "py" => "y", _ => "none" 
        };

        if (side == "none") return false;

        // Bootstrap 5 Scales
        if (double.TryParse(valStr.ToString(), out double n))
        {
            double multiplier = n switch { 0=>0, 1=>0.25, 2=>0.5, 3=>1.0, 4=>1.5, 5=>3.0, _=>0 };
            value = rem * multiplier;
            return true;
        }

        return false;
    }

    private static double GetBaseFontSize(StyledElement element)
    {
        if (Application.Current?.TryGetResource(BaseFontSizeKey, (element as Control)?.ActualThemeVariant, out var res) == true && res is double size)
            return size;
        return 14.0;
    }
}