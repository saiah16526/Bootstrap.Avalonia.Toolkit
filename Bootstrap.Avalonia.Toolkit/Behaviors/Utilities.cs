using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Bootstrap.Avalonia.Toolkit.Behaviors;

/// <summary>
/// Provides Bootstrap-style utility classes for Avalonia Controls, 
/// including Margins, Padding, Borders, and Corner Radii.
/// </summary>
public static class Utilities
{
    #region Attached Properties

    public static readonly AttachedProperty<bool> EnableProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("Enable", typeof(Utilities));

    public static bool GetEnable(Control c) => c.GetValue(EnableProperty);
    public static void SetEnable(Control c, bool v) => c.SetValue(EnableProperty, v);

    #endregion

    #region Lookup Tables

    // Standard Bootstrap spacing multipliers (0 to 3x the base font size)
    private static readonly Dictionary<string, double> SpacingMultipliers = new()
    {
        ["0"] = 0.0, ["1"] = 0.25, ["2"] = 0.5, ["3"] = 1.0, ["4"] = 1.5, ["5"] = 3.0, ["default"] = 1.0
    };

    // Specific rounding values based on Bootstrap 5 constants
    private static readonly Dictionary<string, double> RoundingValues = new()
    {
        ["0"] = 0.0, ["1"] = 3.5, ["2"] = 5.25, ["3"] = 7.0, ["4"] = 14.0, ["5"] = 28.0, ["pill"] = 700.0, ["circle"] = 700.0
    };

    #endregion

    static Utilities()
    {
        // Global listener: When 'Enable' is set to true, monitor class changes
        EnableProperty.Changed.AddClassHandler<Control>((control, args) =>
        {
            if (args.NewValue is true)
            {
                control.Classes.CollectionChanged += (_, _) => UpdateControlStyles(control);
                UpdateControlStyles(control);
            }
        });
    }

    private static void UpdateControlStyles(Control control)
    {
        if (control.Classes.Count == 0) return;

        // Base sizing: Usually 14px or 16px depending on the app theme
        double baseScale = 16.0;
        if (Application.Current?.TryGetResource("BsBodyFontSize", out var res) == true)
            baseScale = res is double d ? d : 16.0;

        // Local accumulators for the different box-model properties
        double marginLeft = 0, marginTop = 0, marginRight = 0, marginBottom = 0;
        double padLeft = 0, padTop = 0, padRight = 0, padBottom = 0;
        double borderLeft = 0, borderTop = 0, borderRight = 0, borderBottom = 0;
        double radiusTl = 0, radiusTr = 0, radiusBr = 0, radiusBl = 0;

        bool hasMargin = false, hasPadding = false, hasBorder = false, hasRounding = false;

        foreach (var className in control.Classes)
        {
            var segments = className.Split('-');
            if (segments.Length < 2) continue;

            string prefix = segments[0];
            string suffix = segments[^1];

            // 1. Handle Spacing (Margin/Padding)
            if (prefix.StartsWith("m") || prefix.StartsWith("p"))
            {
                if (!SpacingMultipliers.TryGetValue(suffix, out var multiplier)) continue;
                double value = multiplier * baseScale;

                if (prefix.StartsWith("m"))
                {
                    hasMargin = true;
                    MapBoxSide(prefix, value, ref marginLeft, ref marginTop, ref marginRight, ref marginBottom);
                }
                else
                {
                    hasPadding = true;
                    MapBoxSide(prefix, value, ref padLeft, ref padTop, ref padRight, ref padBottom);
                }
            }
            // 2. Handle Borders (border-top-2, etc)
            else if (prefix == "border")
            {
                hasBorder = true;
                string position = segments.Length == 3 ? segments[1] : "all";
                // Borders use spacing multipliers if numeric, otherwise default to 1px
                double width = SpacingMultipliers.TryGetValue(suffix, out var m) ? m : 1.0;
                
                MapPositionToSide(position, width, ref borderLeft, ref borderTop, ref borderRight, ref borderBottom);
            }
            // 3. Handle Rounding (rounded-top-sm, etc)
            else if (prefix == "rounded")
            {
                hasRounding = true;
                string position = segments.Length == 3 ? segments[1] : "all";
                if (RoundingValues.TryGetValue(suffix, out var radius))
                {
                    MapPositionToSide(position, radius, ref radiusTl, ref radiusTr, ref radiusBr, ref radiusBl);
                }
            }
        }

        ApplyCalculatedStyles(control, 
            hasMargin, new Thickness(marginLeft, marginTop, marginRight, marginBottom),
            hasPadding, new Thickness(padLeft, padTop, padRight, padBottom),
            hasBorder, new Thickness(borderLeft, borderTop, borderRight, borderBottom),
            hasRounding, new CornerRadius(radiusTl, radiusTr, radiusBr, radiusBl));
    }

    /// <summary>
    /// Maps shorthand keys like 'mx', 'pt', 'ms' to the correct sides of a Thickness variable set.
    /// </summary>
    private static void MapBoxSide(string key, double val, ref double l, ref double t, ref double r, ref double b)
    {
        if (key.EndsWith("x")) { l = r = val; }
        else if (key.EndsWith("y")) { t = b = val; }
        else if (key.EndsWith("s")) { l = val; }
        else if (key.EndsWith("e")) { r = val; }
        else if (key.EndsWith("t")) { t = val; }
        else if (key.EndsWith("b")) { b = val; }
        else { l = t = r = b = val; }
    }

    /// <summary>
    /// Maps descriptive positions like 'top', 'start', 'all' to side variables.
    /// </summary>
    private static void MapPositionToSide(string pos, double val, ref double l, ref double t, ref double r, ref double b)
    {
        switch (pos)
        {
            case "top": t = r = val; if (pos == "top") { l = t = r = val; b = 0; } break; // For rounded-top
            case "bottom": l = b = r = val; t = 0; break;
            case "start": l = val; break;
            case "end": r = val; break;
            default: l = t = r = b = val; break;
        }
    }

    /// <summary>
    /// Final application of values to the Avalonia Control based on its supported properties.
    /// </summary>
    private static void ApplyCalculatedStyles(Control c, bool hM, Thickness m, bool hP, Thickness p, bool hB, Thickness b, bool hR, CornerRadius r)
    {
        if (hM) c.Margin = m;
        
        if (hP)
        {
            if (c is TemplatedControl tc) tc.Padding = p;
            else if (c is Border bord) bord.Padding = p;
        }

        if (hB && c is Border borderThickness) borderThickness.BorderThickness = b;

        if (hR)
        {
            if (c is Border bor) bor.CornerRadius = r;
            else if (c is Button btn) btn.CornerRadius = r;
        }
    }
}