using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Bootstrap.Avalonia.ToolKit.Extensions;
using Bootstrap.Avalonia.ToolKit.Helpers;

namespace Bootstrap.Avalonia.ToolKit.Properties;

/// <summary>
/// Static Utility Class for Mutating Button Component Metadata and State-Dependent Visual Resources.
/// </summary>
public static class BsButton
{
    private const string LightenScalarKey = "BsLightenRatio";
    private const string DarkenScalarKey = "BsDarkenRatio";

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Button, bool>("IsEnabled", typeof(BsButton), false);

    static BsButton()
    {
        IsEnabledProperty.Changed.AddClassHandler<Button>((instance, eventArgs) =>
        {
            if (eventArgs.NewValue is true)
            {
                instance.Classes.CollectionChanged += (_, _) => InvokeButtonUpdate(instance); 
                InvokeButtonUpdate(instance);
            }
        });
    }

    private static void InvokeButtonUpdate(Button instance)
    {
        // Resolving the Multiplier Constants from the Global Resource Dictionary.
        float lightenRatio = QueryRatioResource(instance, LightenScalarKey, 0.2f);
        float darkenRatio = QueryRatioResource(instance, DarkenScalarKey, 0.2f);

        foreach (var identifier in instance.Classes)
        {
            var segment = identifier.AsSpan();
            if (identifier is "btn") continue;
            if (segment is ['b', 't', 'n', '-', .. var payload])
            {
                bool isOutlineState = payload.StartsWith("outline-".AsSpan());
                ReadOnlySpan<char> colorIdentifier = isOutlineState ? payload[8..] : payload;
                
                string resourceKey = colorIdentifier.ToBsPascalCase();
                
                if (Application.Current?.TryGetResource(resourceKey, instance.ActualThemeVariant, out var resource) == true 
                    && resource is ISolidColorBrush brushInstance)
                {
                    if (isOutlineState)
                        ApplyOutlineResourceOverrides(instance, brushInstance);
                    else
                        ApplySolidResourceOverrides(instance, brushInstance, lightenRatio, darkenRatio);
                    
                    break; 
                }
            }
        }
    }

    private static void ApplySolidResourceOverrides(Button instance, ISolidColorBrush baseBrush, float lightenRatio, float darkenRatio)
    {
        var rgbValue = baseBrush.Color;
        
        // Applying the Dynamic Ratios retrieved from the Resource Metadata.
        var hoverHex = LinearColorMutator.Lighten(rgbValue, lightenRatio);
        var pressedHex = LinearColorMutator.Darken(rgbValue, darkenRatio);

        instance.Resources["ButtonBackground"] = baseBrush;
        instance.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(hoverHex);
        instance.Resources["ButtonBackgroundPressed"] = new SolidColorBrush(pressedHex);
        
        var contrastBrush = QueryContrastForeground(rgbValue);
        instance.Resources["ButtonForeground"] = contrastBrush;
        instance.Resources["ButtonForegroundPointerOver"] = contrastBrush;
        instance.Resources["ButtonForegroundPressed"] = contrastBrush;
    }

    private static void ApplyOutlineResourceOverrides(Button instance, ISolidColorBrush baseBrush)
    {
        var rgbValue = baseBrush.Color;
        instance.Resources["ButtonBackground"] = new SolidColorBrush(Colors.Transparent);
        instance.Resources["ButtonBorderBrush"] = baseBrush;
        instance.Resources["ButtonBorderThickness"] = new Thickness(1);
        
        instance.Resources["ButtonBackgroundPointerOver"] = baseBrush;
        instance.Resources["ButtonForeground"] = baseBrush;
        instance.Resources["ButtonForegroundPointerOver"] = QueryContrastForeground(rgbValue);
        instance.Resources["ButtonForegroundPressed"] = QueryContrastForeground(rgbValue);
    }

    /// <summary>
    /// Queries the Resource Dictionary for a Single-Precision Floating Point Ratio.
    /// </summary>
    private static float QueryRatioResource(Control instance, string key, float defaultFallback)
    {
        if (Application.Current?.TryGetResource(key, instance.ActualThemeVariant, out var resource) == true)
        {
            if (resource is float f) return f;
            if (resource is double d) return (float)d;
        }
        return defaultFallback;
    }

    private static IBrush QueryContrastForeground(Color background)
    {
        var luminance = ((background.R * 299) + (background.G * 587) + (background.B * 114)) / 1000;
        return luminance >= 128 ? Brushes.Black : Brushes.White;
    }
}