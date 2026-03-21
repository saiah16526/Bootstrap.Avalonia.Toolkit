using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Bootstrap.Avalonia.Toolkit.Helpers;

namespace Bootstrap.Avalonia.Toolkit.Styles;

public static class BsBorder
{
    private const string BaseFontSizeKey = "BsFontSize";

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<StyledElement, bool>("IsEnabled", typeof(BsBorder), false);

    public static bool GetIsEnabled(StyledElement element) => element.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(StyledElement element, bool value) => element.SetValue(IsEnabledProperty, value);

    static BsBorder()
    {
        IsEnabledProperty.Changed.AddClassHandler<StyledElement>((element, args) =>
        {
            if (args.NewValue is true)
            {
                element.Classes.CollectionChanged += (_, _) => UpdateBorderStyles(element);
                UpdateBorderStyles(element);
            }
        });
    }

    private static void UpdateBorderStyles(AvaloniaObject obj)
    {
        if (obj is not StyledElement element) return;

        double rem = GetBaseFontSize(element);
        double unit = rem / 14.0;

        double left = 0, top = 0, right = 0, bottom = 0;

        foreach (var className in element.Classes)
        {
            var span = className.AsSpan();

            if (span is ['b', 'o', 'r', 'd', 'e', 'r', '-', .. var colorPart] 
                && !char.IsDigit(colorPart[0]) 
                && !IsDirectional(colorPart))
            {
                string key = colorPart.ToBsPascalCase();
                if (TryGetResource<IBrush>(element, key, out var brush))
                {
                    obj.SetValue(TemplatedControl.BorderBrushProperty, brush);
                }
            }

            else if (span.StartsWith("border".AsSpan()))
            {
                if (span.SequenceEqual("border".AsSpan()))
                {
                    left = top = right = bottom = unit;
                    if (TryGetResource<IBrush>(element, "BsBorderColor", out var brush))
                    {
                        obj.SetValue(TemplatedControl.BorderBrushProperty, brush);
                    }
                }
                else if (span is ['b', 'o', 'r', 'd', 'e', 'r', '-', var n] && char.IsDigit(n))
                {
                    double multiplier = char.GetNumericValue(n);
                    left = top = right = bottom = unit * multiplier;
                }
                else if (span is ['b', 'o', 'r', 'd', 'e', 'r', '-', .. var direction])
                {
                    double thickness = direction.EndsWith("-0".AsSpan()) ? 0 : unit;
                    
                    if (direction.StartsWith("top".AsSpan())) top = thickness;
                    else if (direction.StartsWith("bottom".AsSpan())) bottom = thickness;
                    else if (direction.StartsWith("start".AsSpan())) left = thickness;
                    else if (direction.StartsWith("end".AsSpan())) right = thickness;
                }
            }
        }

        obj.SetValue(TemplatedControl.BorderThicknessProperty, new Thickness(left, top, right, bottom));
    }

    private static bool IsDirectional(ReadOnlySpan<char> span) =>
        span.StartsWith("top".AsSpan()) || span.StartsWith("bottom".AsSpan()) || 
        span.StartsWith("start".AsSpan()) || span.StartsWith("end".AsSpan());

    private static double GetBaseFontSize(StyledElement element)
    {
        if (TryGetResource<double>(element, BaseFontSizeKey, out var size))
            return size;
        return 14.0;
    }

    private static bool TryGetResource<T>(StyledElement element, string key, out T? result)
    {
        result = default;
        var theme = (element as Control)?.ActualThemeVariant;
        
        // This cast to (object?) handles the nullability/generic SetValue conflict
        if (Application.Current?.TryGetResource(key, theme, out var res) == true && res is T val)
        {
            result = val;
            return true;
        }
        return false;
    }
}