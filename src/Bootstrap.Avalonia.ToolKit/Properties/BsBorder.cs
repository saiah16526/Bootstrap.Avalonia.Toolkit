using System.Collections.Concurrent;
using System.Linq;
using Avalonia.Media;
using Bootstrap.Avalonia.ToolKit.Extensions;

namespace Bootstrap.Avalonia.ToolKit.Properties;

/// <summary>
/// Static Utility Class for Mutating Border Brush and Thickness Metadata via Geometric Vector Resolution.
/// </summary>
public static class BsBorder
{
    private const string BaseFontSizeKey = "BsBodyFontSize";
    private static readonly ConcurrentDictionary<Type, AvaloniaProperty?> BrushPropertyCache = new();
    private static readonly ConcurrentDictionary<Type, AvaloniaProperty?> ThicknessPropertyCache = new();

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsEnabled", typeof(BsBorder), false);

    static BsBorder()
    {
        IsEnabledProperty.Changed.AddClassHandler<Control>((instance, eventArgs) =>
        {
            if (eventArgs.NewValue is true)
            {
                instance.Classes.CollectionChanged += (_, _) => InvokeBorderUpdate(instance);
                InvokeBorderUpdate(instance);
            }
        });
    }

    private static void InvokeBorderUpdate(Control instance)
    {
        if (instance.TemplatedParent != null) return;

        bool hasBorderClass = false;
        foreach (var c in instance.Classes) { if (c.StartsWith("border")) { hasBorderClass = true; break; } }
        if (!hasBorderClass) return;

        var brushProperty = ResolveMetadata(instance.GetType(), "BorderBrush", BrushPropertyCache);
        var thicknessProperty = ResolveMetadata(instance.GetType(), "BorderThickness", ThicknessPropertyCache);
        if (brushProperty == null || thicknessProperty == null) return;

        double remUnit = QueryRootFontSize(instance);
        double normalizedUnit = remUnit / 14.0; 

        double xMin = 0, yMin = 0, xMax = 0, yMax = 0;

        foreach (var identifier in instance.Classes)
        {
            var segment = identifier.AsSpan();

            // Logic Branch: Border Color Assignment
            if (segment is ['b', 'o', 'r', 'd', 'e', 'r', '-', .. var colorPayload] 
                && !char.IsDigit(colorPayload[0]) 
                && !IsDirectionalPayload(colorPayload))
            {
                string resourceKey = colorPayload.ToBsPascalCase();
                if (TryQueryResource<IBrush>(instance, resourceKey, out var brushInstance))
                    instance.SetValue(brushProperty, brushInstance);
            }

            // Logic Branch: Border Magnitude and Directional Logic
            else if (segment.StartsWith("border".AsSpan()))
            {
                if (segment.SequenceEqual("border".AsSpan()))
                {
                    xMin = yMin = xMax = yMax = normalizedUnit;
                    if (TryQueryResource<IBrush>(instance, "BsBorderColor", out var defaultBrush))
                        instance.SetValue(brushProperty, defaultBrush);
                }
                // Case: Universal Scalar (border-0 to border-5)
                else if (segment.Length == 8 && segment.StartsWith("border-".AsSpan()) && char.IsDigit(segment[7]))
                {
                    xMin = yMin = xMax = yMax = normalizedUnit * char.GetNumericValue(segment[7]);
                }
                // Case: Directional Segmentation (border-top, border-top-5, border-end-0, etc.)
                else if (segment is ['b', 'o', 'r', 'd', 'e', 'r', '-', .. var directionPayload])
                {
                    double magnitude = ExtractMagnitude(directionPayload, normalizedUnit);
                    
                    if (directionPayload.StartsWith("top".AsSpan())) yMin = magnitude;
                    else if (directionPayload.StartsWith("bottom".AsSpan())) yMax = magnitude;
                    else if (directionPayload.StartsWith("start".AsSpan())) xMin = magnitude;
                    else if (directionPayload.StartsWith("end".AsSpan())) xMax = magnitude;
                }
            }
        }

        instance.SetValue(thicknessProperty, new Thickness(xMin, yMin, xMax, yMax));
    }

    /// <summary>
    /// Parses the directional payload to determine the specific scalar magnitude.
    /// handles "top", "top-0", "top-5", etc.
    /// </summary>
    private static double ExtractMagnitude(ReadOnlySpan<char> payload, double unit)
    {
        // Check for specific scalar suffix (e.g., "-5" at the end of "top-5")
        if (payload.Length >= 3 && payload[^2] == '-' && char.IsDigit(payload[^1]))
        {
            return unit * char.GetNumericValue(payload[^1]);
        }
        
        // Handle explicit zero-out (e.g., "top-0")
        if (payload.EndsWith("-0".AsSpan())) return 0;

        // Default to 1 unit if only the direction is specified (e.g., "border-top")
        return unit;
    }

    private static bool IsDirectionalPayload(ReadOnlySpan<char> payload) =>
        payload.StartsWith("top".AsSpan()) || payload.StartsWith("bottom".AsSpan()) || 
        payload.StartsWith("start".AsSpan()) || payload.StartsWith("end".AsSpan());

    private static AvaloniaProperty? ResolveMetadata(Type runtimeType, string propertyName, ConcurrentDictionary<Type, AvaloniaProperty?> cache)
    {
        return cache.GetOrAdd(runtimeType, typeKey =>
        {
            return AvaloniaPropertyRegistry.Instance.GetRegistered(typeKey)
                .FirstOrDefault(metadata => metadata.Name == propertyName);
        });
    }

    private static double QueryRootFontSize(Control instance)
    {
        if (TryQueryResource<double>(instance, BaseFontSizeKey, out var scalar))
            return scalar;
        return 14.0;
    }

    private static bool TryQueryResource<T>(Control instance, string key, out T? resource)
    {
        resource = default;
        var theme = instance.ActualThemeVariant;
        if (Application.Current?.TryGetResource(key, theme, out var rawResource) == true && rawResource is T typedResource)
        {
            resource = typedResource;
            return true;
        }
        return false;
    }
}