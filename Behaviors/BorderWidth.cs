using System.Globalization;
using Avalonia;
using Avalonia.Controls;

namespace Bootstrap.Avalonia.Toolkit.Behaviors;

public static class BorderWidth
{
    public static readonly AttachedProperty<bool> EnableProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("Enable", typeof(BorderWidth));

    // Bootstrap-style fractional values
    private static readonly Dictionary<string, double> SpacerValues = new()
    {
        ["0"] = 0, ["1"] = 0.25, ["2"] = 0.5, ["3"] = 1, ["4"] = 1.5, ["5"] = 3
    };

    static BorderWidth()
    {
        EnableProperty.Changed.AddClassHandler<Control>((c, e) =>
        {
            if (e.NewValue is true)
            {
                c.Classes.CollectionChanged += (_, _) => Parse(c);
                Parse(c);
            }
        });
    }

    public static void SetEnable(Control c, bool v)
    {
        c.SetValue(EnableProperty, v);
    }

    public static bool GetEnable(Control c)
    {
        return c.GetValue(EnableProperty);
    }

    private static void Parse(Control c)
    {
        if (c.Classes.Count == 0) return;
        double l = 0, t = 0, r = 0, bm = 0;
        var modified = false;
        foreach (var cls in c.Classes)
        {
            if (string.IsNullOrEmpty(cls) || !cls.StartsWith("border")) continue;
            modified = true;
            var p = cls.Split('-');
            var pos = "all";
            double val = 1;
            if (p.Length == 2)
            {
                if (double.TryParse(p[1], out var x)) val = x;
                else pos = p[1];
            }
            else if (p.Length == 3)
            {
                pos = p[1];
                double.TryParse(p[2], out val);
            }

            if (SpacerValues.TryGetValue(val.ToString(CultureInfo.InvariantCulture), out var sv))
                if (c.TryGetResource("BodyFontSize", out var value) && value is double size)
                    val = size * sv;
            switch (pos)
            {
                case "all": l = t = r = bm = val; break;
                case "top": t = val; break;
                case "bottom": bm = val; break;
                case "start": l = val; break;
                case "end": r = val; break;
            }
        }

        if (!modified) return;

        switch (c)
        {
            case Border b: b.BorderThickness = new Thickness(l, t, r, bm); break;
            case Button btn: btn.Padding = new Thickness(l, t, r, bm); break;
        }
    }
}