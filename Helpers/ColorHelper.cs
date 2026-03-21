using Avalonia.Media;

namespace Bootstrap.Avalonia.Toolkit.Helpers;

public static class ColorHelper
{
    public static Color Lighten(Color color, float amount) => 
        Color.FromArgb(color.A, 
            (byte)Math.Min(255, color.R + (255 - color.R) * amount),
            (byte)Math.Min(255, color.G + (255 - color.G) * amount),
            (byte)Math.Min(255, color.B + (255 - color.B) * amount));

    public static Color Darken(Color color, float amount) => 
        Color.FromArgb(color.A, 
            (byte)(color.R * (1 - amount)),
            (byte)(color.G * (1 - amount)),
            (byte)(color.B * (1 - amount)));
}