using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;

namespace Bootstrap.Avalonia.Toolkit.Behaviors;

/// <summary>
///     Provides Bootstrap-style Margin utilities for Avalonia controls using attached properties.
///     When enabled, it parses a control's <see cref="Classes" /> for margin shorthand (e.g., "mt-3", "mx-2").
/// </summary>
public static class Margin
{
    /// <summary>
    ///     Attached property to enable or disable the Bootstrap margin behavior on a control.
    /// </summary>
    public static readonly AttachedProperty<bool> EnableProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("Enabled", typeof(Margin));

    /// <summary>
    ///     Dictionary mapping Bootstrap spacer levels (0-5) to numeric values.
    ///     These are multipliers typically applied to a base spacing unit.
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
    ///     Static constructor to register a global listener for the EnableProperty.
    ///     Hooks into the Classes collection to update margins dynamically.
    /// </summary>
    static Margin()
    {
        EnableProperty.Changed.AddClassHandler<Control>((c, e) =>
        {
            if (e.NewValue is true)
            {
                // Reparse whenever the Classes collection changes (e.g., adding/removing classes via code)
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
    ///     Scans the control's class list and applies the corresponding <see cref="Thickness" />
    ///     to the <see cref="Control.Margin" /> property.
    /// </summary>
    /// <remarks>
    ///     Supported Prefixes:
    ///     m  - All sides
    ///     mx - Left and Right
    ///     my - Top and Bottom
    ///     mt - Top
    ///     mb - Bottom
    ///     ms - Start (Left)
    ///     me - End (Right)
    /// </remarks>
    private static void Parse(Control c)
    {
        if (c.Classes.Count == 0) return;

        double l = 0, t = 0, r = 0, b = 0;
        var isModified = false;

        foreach (var cls in c.Classes)
        {
            if (string.IsNullOrEmpty(cls) || !cls.Contains('-')) continue;

            var parts = cls.Split('-');
            if (parts.Length != 2) continue;

            if (!SpacerValues.TryGetValue(parts[1], out var val)) continue;
            var topLevel = TopLevel.GetTopLevel(c);
            double baseSize = 14.0; 

            if (topLevel != null)
            {
                baseSize = topLevel.FontSize;
            }

            val *= baseSize;
            isModified = true;
            switch (parts[0])
            {
                case "m": l = t = r = b = val; break;
                case "mx": l = r = val; break;
                case "my": t = b = val; break;
                case "ms": l = val; break;
                case "me": r = val; break;
                case "mt": t = val; break;
                case "mb": b = val; break;
            }
        }

        // Apply the new thickness only if a valid Bootstrap class was found
        if (isModified) c.Margin = new Thickness(l, t, r, b);
    }
}