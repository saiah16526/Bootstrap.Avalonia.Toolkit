using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Bootstrap.Avalonia.Toolkit.Helpers;

namespace Bootstrap.Avalonia.Toolkit.Styles;

public static class BsButton
{
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Button, bool>("IsEnabled", typeof(BsButton), false);

    public static bool GetIsEnabled(Button element) => element.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(Button element, bool value) => element.SetValue(IsEnabledProperty, value);

    static BsButton()
    {
        IsEnabledProperty.Changed.AddClassHandler<Button>((btn, args) =>
        {
            if (args.NewValue is true)
            {
                btn.Classes.CollectionChanged += (_, _) => UpdateButtonStyles(btn); 
                UpdateButtonStyles(btn);
            }
        });
    }

    private static void UpdateButtonStyles(Button btn)
    {
        foreach (var className in btn.Classes)
        {
            var span = className.AsSpan();

            if (span is ['b', 't', 'n', '-', .. var rest])
            {
                bool isOutline = rest.StartsWith("outline-".AsSpan());
                ReadOnlySpan<char> colorName = isOutline ? rest[8..] : rest;
                
                string key = colorName.ToBsPascalCase();
                if (Application.Current?.TryGetResource(key, btn.ActualThemeVariant, out var res) == true && res is ISolidColorBrush baseBrush)
                {
                    if (isOutline)
                        ApplyOutlineStyles(btn, baseBrush);
                    else
                        ApplySolidStyles(btn, baseBrush);
                    break; 
                }
            }
        }
    }

    private static void ApplySolidStyles(Button btn, ISolidColorBrush baseBrush)
    {
        var color = baseBrush.Color;
        
        var hoverColor = ColorHelper.Lighten(color, 0.2f);
        var pressedColor = ColorHelper.Darken(color, 0.2f);

        btn.Resources["ButtonBackground"] = baseBrush;
        btn.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(hoverColor);
        btn.Resources["ButtonBackgroundPressed"] = new SolidColorBrush(pressedColor);
        btn.Resources["ButtonForeground"] = GetContrastForeground(color);
        btn.Resources["ButtonForegroundPointerOver"] = GetContrastForeground(color);
        btn.Resources["ButtonForegroundPressed"] = GetContrastForeground(color);
    }

    private static void ApplyOutlineStyles(Button btn, ISolidColorBrush baseBrush)
    {
        var color = baseBrush.Color;
        var transparent = new SolidColorBrush(Colors.Transparent);

        btn.Resources["ButtonBackground"] = transparent;
        btn.Resources["ButtonBorderBrush"] = baseBrush;
        btn.Resources["ButtonBorderThickness"] = new Thickness(1);
        
        btn.Resources["ButtonBackgroundPointerOver"] = baseBrush;
        btn.Resources["ButtonForeground"] = baseBrush;
        btn.Resources["ButtonForegroundPointerOver"] = GetContrastForeground(color);
        btn.Resources["ButtonForegroundPressed"] = GetContrastForeground(color);
    }

    private static IBrush GetContrastForeground(Color background)
    {
        var yiq = ((background.R * 299) + (background.G * 587) + (background.B * 114)) / 1000;
        return yiq >= 128 ? Brushes.Black : Brushes.White;
    }
}