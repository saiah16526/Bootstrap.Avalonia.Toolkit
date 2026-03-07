
# 🚀 Avalonia.Bootstrap

**Build beautiful Avalonia UIs faster.** A lightweight, class-based styling system inspired by Bootstrap, designed specifically for modern Avalonia applications.

---

## ✨ Features

* **Class-Based Styling**: Stop writing custom setters for every button. Use `Classes="btn-primary"`.
* **IntelliSense Ready**: Custom helper files provide full autocomplete in VS Code, Visual Studio, and Rider.
* **Lightweight**: Only includes CSS-like styles—no heavy dependencies.
* **Theme Aware**: Works perfectly alongside Avalonia's `FluentTheme`.

---

## 📦 1. Installation

Install the core package via the .NET CLI:

```bash
dotnet add package Avalonia.Bootstrap

```

---

## 🛠️ 2. Quick Start

### Step A: Register the Theme

In your **`App.axaml`**, include the Bootstrap styles. This pulls the actual CSS-like logic from the NuGet package.

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://Avalonia.Bootstrap/Assets/Theme.axaml" />
</Application.Styles>

```

### Step B: Enable Autocomplete (Hints)

Since NuGet files are "virtual," the IDE needs a local map to show you hints while you type.

1. **Download** the [Bootstrap.Autocomplete.axaml](https://www.google.com/search?q=https://raw.githubusercontent.com/YourUsername/Repo/main/Bootstrap.Autocomplete.axaml) helper file.
2. **Drop it** into your project (e.g., in an `/Assets` folder).
3. **Set Build Action**: Right-click the file → **Properties** → **Build Action** = `AvaloniaResource`.

---

## 🎨 3. Usage Examples

Once set up, you can style your UI using simple class names.

### Buttons

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <Button Classes="btn-primary" Content="Submit" />
    <Button Classes="btn-outline" Content="Cancel" />
    <Button Classes="btn-danger btn-sm" Content="Delete" />
</StackPanel>

```

### Typography & Cards

```xml
<Border Classes="card">
    <StackPanel>
        <TextBlock Classes="h1" Text="Welcome to my App" />
        <TextBlock Classes="text-muted" Text="This is a bootstrap-styled card." />
    </StackPanel>
</Border>

```

---

## 📚 4. Class Reference

| Category | Classes |
| --- | --- |
| **Buttons** | `btn-primary`, `btn-secondary`, `btn-success`, `btn-danger`, `btn-outline` |
| **Button Sizing** | `btn-lg`, `btn-sm` |
| **Typography** | `h1`, `h2`, `h3`, `h4`, `h5`, `h6`, `text-muted`, `text-center` |
| **Containers** | `card`, `container`, `p-3` (padding), `m-2` (margin) |

---

## 🐧 Linux (Fedora) Development

If you are updating the package locally while developing, remember to clear your NuGet cache to see changes:

```bash
dotnet nuget locals all --clear
dotnet restore

```

Would you like me to help you create a "Preview" section showing what the actual UI looks like for the "Buttons" and "Cards"?**
