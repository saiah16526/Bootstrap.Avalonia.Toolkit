using Avalonia.Controls.Primitives;

namespace Bootstrap.Avalonia.ToolKit.Properties;

/// <summary>
/// Static Utility Class for Mutating Corner Radius Geometric Vectors via Attached Dependency Properties.
/// </summary>
public static class BsRounded
{
    private const string BaseFontSizeKey = "BsBodyFontSize";

    /// <summary>
    /// The RegisterAttached Boolean Flag for the Control Component Instance.
    /// </summary>
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsEnabled", typeof(BsRounded), false);

    /// <summary>
    /// Getter Accessor for the IsEnabled Dependency Identifier.
    /// </summary>
    public static bool GetIsEnabled(Control instance) => instance.GetValue(IsEnabledProperty);
    
    /// <summary>
    /// Setter Mutator for the IsEnabled Dependency Identifier.
    /// </summary>
    public static void SetIsEnabled(Control instance, bool value) => instance.SetValue(IsEnabledProperty, value);

    /// <summary>
    /// Static Constructor for Event Delegate Registration within the Global Class Handler.
    /// </summary>
    static BsRounded()
    {
        IsEnabledProperty.Changed.AddClassHandler<Control>((instance, eventArgs) =>
        {
            if (eventArgs.NewValue is true)
            {
                // Registering a Callback to the CollectionChanged Observable for Runtime Metadata Mutation.
                instance.Classes.CollectionChanged += (_, _) => InvokeRoundedUpdate(instance); 
                InvokeRoundedUpdate(instance);
            }
        });
    }

    /// <summary>
    /// Implementation Logic for Parsing Identifier Tokens and Mutating CornerRadius Geometry.
    /// </summary>
    private static void InvokeRoundedUpdate(Control instance)
    {
        // 1. COMPOSITION GUARD: Prevent styling internal template parts (e.g., Border inside a Button).
        if (instance.TemplatedParent != null) return;

        // 2. INHIBITION GUARD: If no 'rounded' class is present, abort to protect inline XAML values.
        bool hasRoundedClass = false;
        foreach (var c in instance.Classes) { if (c.StartsWith("rounded")) { hasRoundedClass = true; break; } }
        if (!hasRoundedClass) return;

        double remUnit = QueryRootFontSize(instance);
        
        // Bootstrap standard: 'rounded' base is 0.375rem.
        double baseArcMagnitude = remUnit * 0.375;

        // Initializing Cartesian Arc Magnitudes (Top-Left, Top-Right, Bottom-Left, Bottom-Right).
        double tl = 0, tr = 0, bl = 0, br = 0;

        foreach (var identifier in instance.Classes)
        {
            var segment = identifier.AsSpan();

            if (segment.StartsWith("rounded".AsSpan()))
            {
                // Logic Branch: Perfect Circular Geometric Constraints.
                if (segment.SequenceEqual("rounded-circle".AsSpan()))
                {
                    tl = tr = bl = br = 999;
                }
                // Logic Branch: Capsule/Pill Geometry.
                else if (segment.SequenceEqual("rounded-pill".AsSpan()))
                {
                    tl = tr = bl = br = 999; 
                }
                // Logic Branch: Universal Arc Magnitude.
                else if (segment.SequenceEqual("rounded".AsSpan()))
                {
                    tl = tr = bl = br = baseArcMagnitude;
                }
                // Logic Branch: Explicit Scalar Multipliers (rounded-0 to rounded-5).
                else if (segment.Length == 9 && segment.StartsWith("rounded-".AsSpan()) && char.IsDigit(segment[8]))
                {
                    double multiplier = char.GetNumericValue(segment[8]);
                    // BS5 normalization: Multiplier relative to the 3.0 scale constant.
                    tl = tr = bl = br = (baseArcMagnitude * multiplier) / 1.0; 
                }
                // Logic Branch: Directional Arc Assignment (top, bottom, start, end).
                else
                {
                    ApplyDirectionalArc(segment, baseArcMagnitude, ref tl, ref tr, ref bl, ref br);
                }
            }
        }

        // Committing the calculated CornerRadius Vector to the Instance.
        InvokeCornerRadiusMutator(instance, new CornerRadius(tl, tr, br, bl));
    }

    /// <summary>
    /// Internal Parser for Translating Directional Tokens into Specific Corner Arc Magnitudes.
    /// </summary>
    private static void ApplyDirectionalArc(ReadOnlySpan<char> segment, double baseArc, ref double tl, ref double tr, ref double bl, ref double br)
    {
        double magnitude = baseArc;
        
        // Check for trailing numeric scalar (e.g., rounded-top-3).
        if (segment.Length > 0 && char.IsDigit(segment[^1]))
        {
            magnitude = (baseArc * char.GetNumericValue(segment[^1])) / 1.0;
        }

        if (segment.Contains("top".AsSpan(), StringComparison.OrdinalIgnoreCase)) { tl = tr = magnitude; }
        else if (segment.Contains("bottom".AsSpan(), StringComparison.OrdinalIgnoreCase)) { bl = br = magnitude; }
        else if (segment.Contains("start".AsSpan(), StringComparison.OrdinalIgnoreCase)) { tl = bl = magnitude; }
        else if (segment.Contains("end".AsSpan(), StringComparison.OrdinalIgnoreCase)) { tr = br = magnitude; }
    }

    /// <summary>
    /// Dispatches the CornerRadius assignment to the appropriate Dependency Property.
    /// </summary>
    private static void InvokeCornerRadiusMutator(AvaloniaObject obj, CornerRadius radius)
    {
        if (obj is Border b) b.CornerRadius = radius;
        else if (obj is Button btn) btn.CornerRadius = radius;
        else if (obj is TemplatedControl tc) tc.SetValue(TemplatedControl.CornerRadiusProperty, radius);
    }

    private static double QueryRootFontSize(Control instance)
    {
        var themeVariant = instance.ActualThemeVariant;
        if (Application.Current?.TryGetResource(BaseFontSizeKey, themeVariant, out var resource) == true && resource is double scalar)
            return scalar;
        return 14.0;
    }
}