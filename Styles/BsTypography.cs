using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Bootstrap.Avalonia.Toolkit.Helpers;

namespace Bootstrap.Avalonia.Toolkit.Styles;

public static class BsTypography
{
    private const string BaseFontSizeKey = "BsFontSize";

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<StyledElement, bool>("IsEnabled", typeof(BsTypography));

    public static bool GetIsEnabled(StyledElement element) => element.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(StyledElement element, bool value) => element.SetValue(IsEnabledProperty, value);

    static BsTypography()
    {
        IsEnabledProperty.Changed.AddClassHandler<StyledElement>((element, args) =>
        {
            if (args.NewValue is true)
            {
                element.Classes.CollectionChanged += (_, _) => UpdateControlStyles(element); 
                UpdateControlStyles(element);
            }
        });
    }

    private static void UpdateControlStyles(AvaloniaObject obj)
    {
        if (obj is not StyledElement element) return;

        double baseSize = GetBaseFontSize(element);

        foreach (var className in element.Classes)
        {
            var span = className.AsSpan();

            if (span is ['t', 'e', 'x', 't', '-', .. var txt] && !txt.StartsWith("align".AsSpan()) && !txt.StartsWith("decoration".AsSpan()))
            {
                if (TryGetResource<IBrush>(element, txt.ToBsPascalCase(), out var brush))
                    obj.SetValue(TextElement.ForegroundProperty, brush);
            }
            
            else if (span is ['f', 'o', 'n', 't', '-', .. var fontName] || span.SequenceEqual("pre".AsSpan()) || span.SequenceEqual("code".AsSpan()))
            {
                bool isMonospace = span switch
                {
                    ['f', 'o', 'n', 't', '-', 'm', 'o', 'n', 'o', 's', 'p', 'a', 'c', 'e'] => true,
                    _ when span.SequenceEqual("pre".AsSpan()) => true,
                    _ when span.SequenceEqual("code".AsSpan()) => true,
                    _ => false
                };

                if (isMonospace)
                {
                    if (TryGetResource<FontFamily>(element, "BsFontMonospace", out var fontFamily))
                    {
                        obj.SetValue(TextElement.FontFamilyProperty, (object?)fontFamily);
                    }
                }
            }
            
            
            else if (span is ['f', 'w', '-', .. var weight])
            {
                var fw = weight.ToString().ToLower() switch
                {
                    "light"    => FontWeight.Light,
                    "semibold" => FontWeight.SemiBold,
                    "bold"     => FontWeight.Bold,
                    "black"    => FontWeight.Black,
                    _          => FontWeight.Normal
                };
                obj.SetValue(TextElement.FontWeightProperty, fw);
            }

            else if (TryCalculateSize(span, baseSize, out double finalSize))
            {
                obj.SetValue(TextElement.FontSizeProperty, finalSize);
            }
            
            else if (span is ['t', 'e', 'x', 't', '-', 'a', 'l', 'i', 'g', 'n', '-', .. var align])
            {
                var ta = align.ToString().ToLower() switch
                {
                    "start"  => TextAlignment.Left,
                    "center" => TextAlignment.Center,
                    "end"    => TextAlignment.Right,
                    _        => TextAlignment.Left
                };
                obj.SetValue(TextBlock.TextAlignmentProperty, ta);
            }
        }
    }

    private static bool TryCalculateSize(ReadOnlySpan<char> span, double @base, out double result)
    {
        result = 0;
        double ratio = 0;

        if (span is ['h', var hn])
        {
            ratio = hn switch { '1' => 2.5, '2' => 2.0, '3' => 1.75, '4' => 1.5, '5' => 1.25, '6' => 1.0, _ => 0 };
        }
        else if (span is ['f', 's', '-', var fn])
        {
            ratio = fn switch { '1' => 2.5, '2' => 2.0, '3' => 1.75, '4' => 1.5, '5' => 1.25, '6' => 1.0, _ => 0 };
        }
        else if (span is ['d', 'i', 's', 'p', 'l', 'a', 'y', '-', var dn])
        {
            ratio = dn switch { '1' => 5.0, '2' => 4.5, '3' => 4.0, '4' => 3.5, '5' => 3.0, '6' => 2.5, _ => 0 };
        }
        else if (span.SequenceEqual("lead".AsSpan()))  ratio = 1.25;
        else if (span.SequenceEqual("small".AsSpan())) ratio = 0.875;

        if (ratio > 0)
        {
            result = @base * ratio;
            return true;
        }
        return false;
    }

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
        if (Application.Current?.TryGetResource(key, theme, out var res) == true && res is T val)
        {
            result = val;
            return true;
        }
        return false;
    }
}