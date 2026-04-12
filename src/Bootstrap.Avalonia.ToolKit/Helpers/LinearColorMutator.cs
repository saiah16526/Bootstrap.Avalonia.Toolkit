using Avalonia.Media;

namespace Bootstrap.Avalonia.ToolKit.Helpers;

/// <summary>
/// Static Provider for Linear Interpolation and Scalar Transformation of RGB Color Structures.
/// </summary>
public static class LinearColorMutator
{
    /// <summary>
    /// Translates the RGB Vector toward the White Point (255, 255, 255).
    /// </summary>
    public static Color Lighten(Color baseColor, float magnitude)
    {
        // Linear Interpolation: Current + (Target - Current) * Delta
        byte r = (byte)Math.Min(255, baseColor.R + (255 - baseColor.R) * magnitude);
        byte g = (byte)Math.Min(255, baseColor.G + (255 - baseColor.G) * magnitude);
        byte b = (byte)Math.Min(255, baseColor.B + (255 - baseColor.B) * magnitude);

        return Color.FromArgb(baseColor.A, r, g, b);
    }

    /// <summary>
    /// Scales the RGB Vector toward the Origin Point (0, 0, 0).
    /// </summary>
    public static Color Darken(Color baseColor, float coefficient)
    {
        // Scalar Multiplication: Current * (1 - Reduction)
        float inverseMagnitude = 1 - coefficient;

        byte r = (byte)(baseColor.R * inverseMagnitude);
        byte g = (byte)(baseColor.G * inverseMagnitude);
        byte b = (byte)(baseColor.B * inverseMagnitude);

        return Color.FromArgb(baseColor.A, r, g, b);
    }
}