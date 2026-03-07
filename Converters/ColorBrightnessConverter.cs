using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Bootstrap.Avalonia.Toolkit.Converters;

public class ColorBrightnessConverter : IValueConverter
{
    /// <summary>
    ///     Provides dynamically adjusted brushes for a button's interactive states
    ///     by modifying brightness for pointer-over (hover) and pressed effects.
    /// </summary>
    /// <param name="value">The input color or brush to be adjusted.</param>
    /// <param name="targetType">The type of object expected as the output (usually Brush or Color).</param>
    /// <param name="parameter">
    ///     Optional parameter to control brightness adjustment. Typically, a double
    ///     representing the amount to darken (&lt;0) or lighten (&gt;0) the color.
    /// </param>
    /// <param name="culture">The culture information for any culture-specific conversions.</param>
    /// <returns>A brush or color with adjusted brightness for the interactive state.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var baseColor = value switch
        {
            Color c => c,
            ISolidColorBrush s => s.Color,
            _ => default
        };
        if (baseColor == default) return value;
        if (!TryGetBrightnessFactor(parameter, out var factor)) return value;

        // 3. Calculate new RGB values
        var r = Clamp((int)(baseColor.R * (1 + factor)));
        var g = Clamp((int)(baseColor.G * (1 + factor)));
        var b = Clamp((int)(baseColor.B * (1 + factor)));

        var newColor = Color.FromArgb(baseColor.A, r, g, b);

        if (targetType.IsAssignableFrom(typeof(IBrush)))
            return new SolidColorBrush(newColor);

        return newColor;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }

    private static bool TryGetBrightnessFactor(object? parameter, out double factor)
    {
        if (parameter is double d)
        {
            factor = d;
            return true;
        }

        if (parameter is string s &&
            double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out factor)) return true;

        factor = 0;
        return false;
    }

    private static byte Clamp(int val)
    {
        return (byte)Math.Clamp(val, 0, 255);
    }
}