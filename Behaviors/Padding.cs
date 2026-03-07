using Avalonia;
using Avalonia.Controls;

namespace Bootstrap.Avalonia.Toolkit.Behaviors;

/// <summary>
///     Provides Bootstrap-style padding utilities for Avalonia controls.
///     When <see cref="EnableProperty" /> is true, the class parses the control's CSS-style classes
///     (e.g., "p-3", "pt-2") and applies the corresponding Padding.
/// </summary>
public static class Padding
{
    /// <summary>
    ///     Defines the Enabled attached property. When set to true, the control will listen
    ///     for class changes to apply padding values.
    /// </summary>
    public static readonly AttachedProperty<bool> EnableProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("Enabled", typeof(Padding));

    /// <summary>
    ///     Map of Bootstrap spacer levels (0-5) to multiplier values.
    ///     Values are typically based on a base unit (e.g., 1rem or 16px).
    /// </summary>
    private static readonly Dictionary<string, double> SpacerValues = new()
    {
        ["0"] = 0,
        ["1"] = 0.25,
        ["2"] = 0.5,
        ["3"] = 1,
        ["4"] = 1.5,
        ["5"] = 3
    };

    /// <summary>
    ///     Static constructor to register a global change handler for the EnableProperty.
    /// </summary>
    static Padding()
    {
        EnableProperty.Changed.AddClassHandler<Control>((c, e) =>
        {
            if (e.NewValue is true)
            {
                // Subscribe to class changes to allow dynamic updates at runtime
                c.Classes.CollectionChanged += (_, _) => Parse(c);
                Parse(c);
            }
        });
    }

    /// <summary>Sets the value of the Enabled attached property.</summary>
    public static void SetEnable(Control c, bool v)
    {
        c.SetValue(EnableProperty, v);
    }

    /// <summary>Gets the value of the Enabled attached property.</summary>
    public static bool GetEnable(Control c)
    {
        return c.GetValue(EnableProperty);
    }

    /// <summary>
    ///     Parses the Control's classes and applies a <see cref="Thickness" /> to the Padding property.
    ///     Supports: p, px, py, ps, pe, pt, pb.
    /// </summary>
    /// <param name="c">The control to inspect and modify.</param>
    private static void Parse(Control c)
    {
        if (c.Classes.Count == 0) return;

        double l = 0, t = 0, r = 0, b = 0;
        var modified = false;

        foreach (var cls in c.Classes)
        {
            if (string.IsNullOrEmpty(cls) || !cls.Contains('-')) continue;

            var parts = cls.Split('-');
            if (parts.Length != 2) continue;

            if (!SpacerValues.TryGetValue(parts[1], out var val)) continue;
            var topLevel = TopLevel.GetTopLevel(c);
            double baseSize = 14.0; // Default fallback

            if (topLevel != null)
            {
                // Fetch the Window/Root font size
                baseSize = topLevel.FontSize;
            }

            // Now apply your multiplier
            val *= baseSize;

            modified = true;
            // Map Bootstrap abbreviations to Thickness sides
            switch (parts[0])
            {
                case "p": l = t = r = b = val; break; // All sides
                case "px": l = r = val; break; // Horizontal
                case "py": t = b = val; break; // Vertical
                case "ps": l = val; break; // Start (Left)
                case "pe": r = val; break; // End (Right)
                case "pt": t = val; break; // Top
                case "pb": b = val; break; // Bottom
            }
        }

        if (!modified) return;

        // Apply the calculated thickness to supported control types
        switch (c)
        {
            case Border border:
                border.Padding = new Thickness(l, t, r, b);
                break;
            case Button button:
                button.Padding = new Thickness(l, t, r, b);
                break;
        }
    }
}