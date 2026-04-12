using System.Linq;
using Avalonia.Layout;

namespace Bootstrap.Avalonia.ToolKit.Properties;

/// <summary>
/// Static Utility Class for Linear Spatial Translation Logic via Dependency Properties.
/// </summary>
public static class BsMargin
{
    private const string BaseFontSizeKey = "BsBodyFontSize";

    /// <summary>
    /// The RegisterAttached Boolean Flag for the Layoutable Component Instance.
    /// </summary>
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Layoutable, bool>("IsEnabled", typeof(BsMargin), false);

    /// <summary>
    /// Getter Accessor for the IsEnabled Dependency Identifier.
    /// </summary>
    public static bool GetIsEnabled(Layoutable instance) => instance.GetValue(IsEnabledProperty);

    /// <summary>
    /// Setter Mutator for the IsEnabled Dependency Identifier.
    /// </summary>
    public static void SetIsEnabled(Layoutable instance, bool value) => instance.SetValue(IsEnabledProperty, value);

    /// <summary>
    /// Static Constructor for Event Delegate Registration within the Global Class Handler.
    /// </summary>
    static BsMargin()
    {
        IsEnabledProperty.Changed.AddClassHandler<Layoutable>((instance, eventArgs) =>
        {
            if (eventArgs.NewValue is true)
            {
                // Assigning a Callback to the CollectionChanged Observable for Runtime Metadata Mutation.
                instance.Classes.CollectionChanged += (_, _) => InvokeMarginUpdate(instance);
                InvokeMarginUpdate(instance);
            }
        });
    }

    /// <summary>
    /// Implementation Logic for Scalar Calculation and Thickness Structure Assignment.
    /// </summary>
    private static void InvokeMarginUpdate(Layoutable instance)
    {
        if (instance.TemplatedParent != null || !instance.Classes.Any(c => c is ['m', '-' or 'x' or 'y' or 't' or 'b' or 's' or 'e', ..]))
        {
            return;
        }
        
        double remUnit = QueryRootFontSize(instance);
        
        // Initializing Cartesian Coordinates for the Thickness Vector.
        double xMin = 0, yMin = 0, xMax = 0, yMax = 0;

        foreach (var identifier in instance.Classes)
        {
            var segment = identifier.AsSpan();

            // Buffer Validation for Margin-specific Identifiers (Prefix 'm').
            if (segment is ['m', ..]) 
            {
                if (TryParseSpatialMagnitude(segment, remUnit, out double magnitude, out var axis))
                {
                    // State Machine for Coordinate Vector Assignment.
                    switch (axis)
                    {
                        case "all": xMin = yMin = xMax = yMax = magnitude; break;
                        case "t": yMin = magnitude; break;
                        case "b": yMax = magnitude; break;
                        case "s": xMin = magnitude; break; // Start (Left in LTR)
                        case "e": xMax = magnitude; break; // End (Right in LTR)
                        case "x": xMin = xMax = magnitude; break;
                        case "y": yMin = yMax = magnitude; break;
                    }
                }
            }
        }

        // Committing the calculated Thickness Structure to the Instance Heap.
        instance.Margin = new Thickness(xMin, yMin, xMax, yMax);
    }

    /// <summary>
    /// Internal Parser for Serializing Class Identifiers into Numeric Magnitudes.
    /// </summary>
    private static bool TryParseSpatialMagnitude(ReadOnlySpan<char> segment, double remUnit, out double magnitude, out string axis)
    {
        magnitude = 0;
        axis = "all";

        // Tokenizing the Buffer based on the Hyphen Delimiter.
        var tokens = segment.ToString().Split('-');
        if (tokens.Length < 2) return false;

        // Mapping Token Metadata to Directional Constants (t, b, s, e, x, y).
        axis = tokens[0] switch 
        { 
            "m" => "all", 
            "mt" => "t", 
            "mb" => "b", 
            "ms" => "s", 
            "me" => "e", 
            "mx" => "x", 
            "my" => "y", 
            _ => "none" 
        };
        
        if (axis == "none") return false;

        string scalarLiteral = tokens[^1];
        if (scalarLiteral == "auto")
        {
            magnitude = 0; 
            return true;
        }

        if (double.TryParse(scalarLiteral, out double numericLiteral))
        {
            // Applying Multiplier Constants based on the Bootstrap Grid Specification.
            double coefficient = numericLiteral switch { 0=>0, 1=>0.25, 2=>0.5, 3=>1.0, 4=>1.5, 5=>3.0, _=>0 };
            magnitude = remUnit * coefficient;
            return true;
        }

        return false;
    }


    /// <summary>
    /// Resource Dictionary Query for the Base Scalar Floating Point.
    /// </summary>
    private static double QueryRootFontSize(StyledElement instance)
    {
        var themeVariant = (instance as Control)?.ActualThemeVariant;
        if (Application.Current?.TryGetResource(BaseFontSizeKey, themeVariant, out var resource) == true && resource is double scalar)
            return scalar;
        
        return 14.0; // Default System Constant.
    }
}