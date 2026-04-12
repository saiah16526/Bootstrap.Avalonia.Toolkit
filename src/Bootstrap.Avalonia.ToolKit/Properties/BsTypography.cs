using Avalonia.Controls.Documents;
using Avalonia.Media;
using Bootstrap.Avalonia.ToolKit.Extensions;

namespace Bootstrap.Avalonia.ToolKit.Properties;

/// <summary>
/// Static Utility Class for Mutating Typographic Metadata and Font-Scalar Logic via Attached Dependency Properties.
/// </summary>
public static class BsTypography
{
    private const string BaseFontSizeKey = "BsBodyFontSize";

    /// <summary>
    /// The RegisterAttached Boolean Flag for the StyledElement Component Instance.
    /// </summary>
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<StyledElement, bool>("IsEnabled", typeof(BsTypography));

    /// <summary>
    /// Getter Accessor for the IsEnabled Dependency Identifier.
    /// </summary>
    public static bool GetIsEnabled(StyledElement instance) => instance.GetValue(IsEnabledProperty);
    
    /// <summary>
    /// Setter Mutator for the IsEnabled Dependency Identifier.
    /// </summary>
    public static void SetIsEnabled(StyledElement instance, bool value) => instance.SetValue(IsEnabledProperty, value);

    /// <summary>
    /// Static Constructor for Event Delegate Registration within the Global Class Handler.
    /// </summary>
    static BsTypography()
    {
        IsEnabledProperty.Changed.AddClassHandler<StyledElement>((instance, eventArgs) =>
        {
            if (eventArgs.NewValue is true)
            {
                // Assigning a Callback to the CollectionChanged Observable for Runtime Metadata Mutation.
                instance.Classes.CollectionChanged += (_, _) => InvokeTypographyUpdate(instance); 
                InvokeTypographyUpdate(instance);
            }
        });
    }

    /// <summary>
    /// Implementation Logic for Parsing Identifier Tokens and Mutating Typographic Dependency Properties.
    /// </summary>
    private static void InvokeTypographyUpdate(AvaloniaObject obj)
    {
        if (obj is not StyledElement instance) return;

        // Resolving the Root Floating-Point Scalar for Relative Font Scaling.
        double remUnit = QueryRootFontSize(instance);

        foreach (var identifier in instance.Classes)
        {
            var segment = identifier.AsSpan();

            // Logic Branch: Foreground IBrush Resolution (Prefix 'text-' excluding alignment/decoration).
            if (segment is ['t', 'e', 'x', 't', '-', .. var colorPayload] 
                && !colorPayload.StartsWith("align".AsSpan()) 
                && !colorPayload.StartsWith("decoration".AsSpan()))
            {
                if (TryQueryResource<IBrush>(instance, colorPayload.ToBsPascalCase(), out var brushInstance))
                {
                    obj.SetValue(TextElement.ForegroundProperty, brushInstance);
                }
            }
            
            // Logic Branch: FontFamily Mapping (Monospace identifiers).
            else if (segment is ['f', 'o', 'n', 't', '-', ..] || segment.SequenceEqual("pre".AsSpan()) || segment.SequenceEqual("code".AsSpan()))
            {
                bool isMonospaceTarget = segment switch
                {
                    ['f', 'o', 'n', 't', '-', 'm', 'o', 'n', 'o', 's', 'p', 'a', 'c', 'e'] => true,
                    _ when segment.SequenceEqual("pre".AsSpan()) => true,
                    _ when segment.SequenceEqual("code".AsSpan()) => true,
                    _ => false
                };

                if (isMonospaceTarget && TryQueryResource<FontFamily>(instance, "JetBrainsMono", out var fontFamilyInstance))
                {
                    obj.SetValue(TextElement.FontFamilyProperty, (object?)fontFamilyInstance);
                }
            }
            
            // Logic Branch: FontWeight Enumeration Mapping (Prefix 'fw-').
            else if (segment is ['f', 'w', '-', .. var weightPayload])
            {
                var fontWeight = weightPayload.ToString().ToLower() switch
                {
                    "lighter"    => FontWeight.ExtraLight,
                    "light"    => FontWeight.Light,
                    "medium"    => FontWeight.Medium,
                    "semibold" => FontWeight.SemiBold,
                    "bold"     => FontWeight.Bold,
                    "bolder"     => FontWeight.ExtraBold,
                    "black"    => FontWeight.Black,
                    _          => FontWeight.Normal
                };
                obj.SetValue(TextElement.FontWeightProperty, fontWeight);
            }

            // Logic Branch: Scalar Magnitude Calculation (Headers, fs-, Display, etc.).
            else if (TryCalculateFontScalar(segment, remUnit, out double computedSize))
            {
                obj.SetValue(TextElement.FontSizeProperty, computedSize);
            }
            
            // Logic Branch: TextAlignment Enumeration Mapping (Prefix 'text-align-').
            else if (segment is ['t', 'e', 'x', 't', '-', 'a', 'l', 'i', 'g', 'n', '-', .. var alignmentPayload])
            {
                var textAlignment = alignmentPayload.ToString().ToLower() switch
                {
                    "start"  => TextAlignment.Left,
                    "center" => TextAlignment.Center,
                    "end"    => TextAlignment.Right,
                    _        => TextAlignment.Left
                };
                obj.SetValue(TextBlock.TextAlignmentProperty, textAlignment);
            }
        }
    }

    /// <summary>
    /// Internal Parser for Translating Typographic Tokens into Normalized Numeric Scalars.
    /// </summary>
    private static bool TryCalculateFontScalar(ReadOnlySpan<char> segment, double remUnit, out double magnitude)
    {
        magnitude = 0;
        double coefficient = 0;

        if (segment is ['h', var hIndex])
        {
            coefficient = hIndex switch { '1' => 2.5, '2' => 2.0, '3' => 1.75, '4' => 1.5, '5' => 1.25, '6' => 1.0, _ => 0 };
        }
        else if (segment is ['f', 's', '-', var fsIndex])
        {
            coefficient = fsIndex switch { '1' => 2.5, '2' => 2.0, '3' => 1.75, '4' => 1.5, '5' => 1.25, '6' => 1.0, _ => 0 };
        }
        else if (segment is ['d', 'i', 's', 'p', 'l', 'a', 'y', '-', var dIndex])
        {
            coefficient = dIndex switch { '1' => 5.0, '2' => 4.5, '3' => 4.0, '4' => 3.5, '5' => 3.0, '6' => 2.5, _ => 0 };
        }
        else if (segment.SequenceEqual("lead".AsSpan()))  coefficient = 1.25;
        else if (segment.SequenceEqual("small".AsSpan())) coefficient = 0.875;
        else if (segment.SequenceEqual("smaller".AsSpan())) coefficient = 0.75;

        if (coefficient > 0)
        {
            magnitude = remUnit * coefficient;
            return true;
        }
        return false;
    }

    private static double QueryRootFontSize(StyledElement instance)
    {
        if (TryQueryResource<double>(instance, BaseFontSizeKey, out var scalar))
            return scalar;
        return 14.0;
    }

    private static bool TryQueryResource<T>(StyledElement instance, string identifier, out T? resource)
    {
        resource = default;
        var themeVariant = (instance as Control)?.ActualThemeVariant;
        if (Application.Current?.TryGetResource(identifier, themeVariant, out var rawResource) == true && rawResource is T typedResource)
        {
            resource = typedResource;
            return true;
        }
        return false;
    }
}