using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Bootstrap.Avalonia.ToolKit.Extensions;

namespace Bootstrap.Avalonia.ToolKit.Properties;

/// <summary>
/// Static Utility Class implementing an Attached Dependency Property for Background Logic.
/// </summary>
public static class BsBackground
{
    /// <summary>
    /// The RegisterAttached Boolean Flag for the Dependency Object.
    /// </summary>
    public static readonly AttachedProperty<bool> IsEnabledProperty
        = AvaloniaProperty.RegisterAttached<Control, bool>("IsEnabled", typeof(BsBackground), false);

    /// <summary>
    /// Static Constructor for Metadata Registration and Event Handler Assignment.
    /// </summary>
    static BsBackground()
    {
        // Hooking Global Class Handler to the PropertyChanged Observable.
        IsEnabledProperty.Changed.AddClassHandler<Control>((instance, eventArgs) =>
        {
            if (eventArgs.NewValue is true)
            {
                // Registering a Callback to the CollectionChanged Event for Dynamic Class Updates.
                instance.Classes.CollectionChanged += (_, __) => InvokeBackgroundUpdate(instance);
            }
            InvokeBackgroundUpdate(instance);
        });
    }

    /// <summary>
    /// Getter Accessor for the Attached Boolean Property.
    /// </summary>
    public static bool GetIsEnabled(Control instance)
    {
        return instance.GetValue(IsEnabledProperty);
    }

    /// <summary>
    /// Setter Mutator for the Attached Boolean Property.
    /// </summary>
    public static void SetIsEnabled(Control instance, bool value)
    {
        instance.SetValue(IsEnabledProperty, value);
    }

    /// <summary>
    /// Logic Implementation for Resource Resolution and Property Assignment.
    /// </summary>
    private static void InvokeBackgroundUpdate(Control instance)
    {
        // Iterating through the StyleClass Collection.
        foreach (var identifier in instance.Classes)
        {
            // Parsing the String Buffer for the 'bg-' Prefix via ReadOnlySpan.
            if (identifier.AsSpan() is ['b', 'g', '-', .. var payload])
            {
                // Serializing the Payload to PascalCase String Format.
                string resourceKey = payload.ToBsPascalCase();
                
                // Querying the Global ResourceDictionary for the IBrush Interface.
                if (Application.Current?.TryGetResource(resourceKey, instance.ActualThemeVariant, out var resource) == true 
                    && resource is IBrush brushInstance)
                {
                    // Invoking the Setter for the Background Property.
                    instance.Set("Background", brushInstance);
                    break; // Early return on successful resolution.
                }
            }
        }
    }
}