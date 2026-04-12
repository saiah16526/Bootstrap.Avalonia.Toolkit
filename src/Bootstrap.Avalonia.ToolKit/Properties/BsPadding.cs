using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Bootstrap.Avalonia.ToolKit.Properties;

public static class BsPadding
{
    private const string BaseFontSizeKey = "BsBodyFontSize";
    private static readonly ConcurrentDictionary<Type, AvaloniaProperty?> PropertyIdentifierCache = new();

    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsEnabled", typeof(BsPadding), false);

    public static bool GetIsEnabled(Control instance) => instance.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(Control instance, bool value) => instance.SetValue(IsEnabledProperty, value);

    static BsPadding()
    {
        IsEnabledProperty.Changed.AddClassHandler<Control>((instance, eventArgs) =>
        {
            if (eventArgs.NewValue is true)
            {
                // Registering a Callback to the CollectionChanged Observable.
                instance.Classes.CollectionChanged += (_, _) => InvokePaddingUpdate(instance); 
                InvokePaddingUpdate(instance);
            }
        });
    }

    /// <summary>
    /// Implementation Logic for Scalar Calculation and Property Assignment via Reflected Metadata.
    /// </summary>
    private static void InvokePaddingUpdate(Control instance)
    {
        // 1. COMPOSITION GUARD:
        // We verify the 'TemplatedParent' property. 
        // If this is non-null, the control is a sub-component of a larger template 
        // (e.g., the Border inside a Button). We ignore these to prevent 'Double Padding'.
        if (instance.TemplatedParent != null || !instance.Classes.Any(c => c is ['p', '-' or 'x' or 'y' or 't' or 'b' or 's' or 'e', ..]))
        {
            return;
        }

        // 2. Metadata Resolution for the 'Padding' Identifier.
        var targetProperty = ResolvePaddingMetadata(instance.GetType());
        if (targetProperty == null) return;

        double remUnit = QueryRootFontSize(instance);
        
        // Initializing Cartesian Coordinates for the Thickness Vector.
        double xMin = 0, yMin = 0, xMax = 0, yMax = 0;
        foreach (var identifier in instance.Classes)
        {
            var segment = identifier.AsSpan();

            if (segment is ['p', ..])
            {
                if (TryParseSpatialMagnitude(segment, remUnit, out double magnitude, out var axis))
                {
                    switch (axis)
                    {
                        case "all": xMin = yMin = xMax = yMax = magnitude; break;
                        case "t": yMin = magnitude; break;
                        case "b": yMax = magnitude; break;
                        case "s": xMin = magnitude; break;
                        case "e": xMax = magnitude; break;
                        case "x": xMin = xMax = magnitude; break;
                        case "y": yMin = yMax = magnitude; break;
                    }
                }
            }
        }

        // 3. Direct Invocation of the Property Mutator via SetValue.
        instance.SetValue(targetProperty, new Thickness(xMin, yMin, xMax, yMax));
    }

    private static AvaloniaProperty? ResolvePaddingMetadata(Type runtimeType)
    {
        return PropertyIdentifierCache.GetOrAdd(runtimeType, typeKey =>
        {
            return AvaloniaPropertyRegistry.Instance.GetRegistered(typeKey)
                .FirstOrDefault(metadata => metadata.Name == "Padding");
        });
    }

    private static bool TryParseSpatialMagnitude(ReadOnlySpan<char> segment, double remUnit, out double magnitude, out string axis)
    {
        magnitude = 0;
        axis = "all";

        int delimiterIndex = segment.IndexOf('-');
        if (delimiterIndex == -1) return false;

        var prefixSegment = segment[..delimiterIndex];
        var literalSegment = segment[(delimiterIndex + 1)..];

        axis = prefixSegment switch 
        { 
            "p" => "all", "pt" => "t", "pb" => "b", "ps" => "s", "pe" => "e", "px" => "x", "py" => "y", _ => "none" 
        };

        if (axis == "none") return false;

        if (double.TryParse(literalSegment.ToString(), out double numericLiteral))
        {
            double coefficient = numericLiteral switch { 0=>0, 1=>0.25, 2=>0.5, 3=>1.0, 4=>1.5, 5=>3.0, _=>0 };
            magnitude = remUnit * coefficient;
            return true;
        }

        return false;
    }

    private static double QueryRootFontSize(Control instance)
    {
        if (Application.Current?.TryGetResource(BaseFontSizeKey, instance.ActualThemeVariant, out var resource) == true && resource is double scalar)
            return scalar;
        return 14.0;
    }
}