# 🌟 Bootstrap.Avalonia.Toolkit

**Bringing the power, speed, and familiarity of utility-first design to Avalonia XAML desktop apps.**

<div align="center">

| **Author**       | **License** | **NuGet Status**                                                    | **Platform Support**                     |
| ---------------- | ----------- | ------------------------------------------------------------------- | ---------------------------------------- |
| **Saiah Albert** | MIT         | ![NuGet](https://img.shields.io/nuget/v/Bootstrap.Avalonia.Toolkit) | Linux (Fedora Optimized), Windows, macOS |

</div>

---

### 📝 Overview

**Bootstrap.Avalonia.Toolkit** is a modern styling library for Avalonia developers who want to **rapidly prototype desktop UIs with minimal overhead**.

Inspired by the utility-first philosophy of Bootstrap, it provides **clean, modular XAML classes** for:

* Spacing (margin, padding)
* Typography (headings, font weight, colors, alignment)
* Borders & rounded corners
* Components (cards, buttons, badges, progress bars)

By using these utility classes, you can **build readable, maintainable interfaces** without long `<Style>` blocks or resource dictionaries. Perfect for dashboards, system monitors, and productivity apps.

---

### 🛠 Installation & Integration

#### 1. Install via NuGet

```bash
dotnet add package Bootstrap.Avalonia.Toolkit
```

#### 2. Add Global Styles

Include the toolkit in your `App.axaml` so all classes are available throughout your app:

```xml
<Application.Styles>
    <FluentTheme />

    <!-- Full development version -->
    <StyleInclude Source="avares://Bootstrap.Avalonia.Toolkit/Styles/Bootstrap.axaml" />

    <!-- Minified version for production -->
    <StyleInclude Source="avares://Bootstrap.Avalonia.Toolkit/Styles/Bootstrap.Min.axaml" />

    <!-- Codepoint map for icons -->
    <StyleInclude Source="avares://Bootstrap.Avalonia.Toolkit/Styles/Bootstrap.Map.axaml" />
</Application.Styles>
```

#### 3. Declare Toolkit Namespace

In your Window or UserControl XAML:

```xml
xmlns:b="clr-namespace:Bootstrap.Avalonia.Toolkit;assembly=Bootstrap.Avalonia.Toolkit"
```

---

### ⚠️ Troubleshooting & Common Fixes

| Issue                    | Root Cause               | Solution                                                                                                |
| ------------------------ | ------------------------ | ------------------------------------------------------------------------------------------------------- |
| **Missing IntelliSense** | IDE cache / metadata lag | Clean your solution and delete `.vs` or `.idea`. Rebuild the project to refresh XAML indexing.          |
| **Styles Not Applying**  | Load order               | Make sure the Toolkit `StyleInclude` comes **after** your base theme (`FluentTheme` or `SimpleTheme`).  |
| **Resource Mismatch**    | Version mismatch         | Use the latest `.axaml` files from `/src` on GitHub for up-to-date fixes before the next NuGet release. |

---

### ✨ Features & Utilities

| 🏗 Utilities                                          | 🎨 Typography                                                | 🧱 Components                                                   |
| ----------------------------------------------------- | ------------------------------------------------------------ | --------------------------------------------------------------- |
| **Spacing:** `m-1` → `m-5`, `p-1` → `p-5`             | **Headings:** `h1`–`h6`                                      | **Buttons:** `btn-primary`, `btn-outline-secondary`             |
| **Borders & Radius:** `border`, `rounded`, `border-2` | **Font Weight:** `fw-bold`, `fw-light`                       | **Badges:** Rounded status indicators                           |
|                                                       | **Text Colors:** `text-primary`, `text-muted`, `text-danger` | **Cards:** Modular containers for widgets                       |
|                                                       | **Text Alignment:** `text-center`, `text-start`              | **Progress Bars:** `progress-success`, `progress-warning`, etc. |

> These classes are designed to **streamline styling directly in XAML**, keeping interfaces readable and maintainable without heavy `<Style>` blocks.

---

### 🖼 Example: Dashboard Widget

```xml
<Border Classes="card p-3 m-2">
    <StackPanel>
        <TextBlock Classes="h4 text-primary" Text="CPU Usage" />
        <ProgressBar Value="65" Classes="progress-success" />
        <TextBlock Classes="text-muted small" Text="Updated: Just now" />
    </StackPanel>
</Border>
```

> Notice how `p-3` and `m-2` handle spacing instantly, while `text-primary` and `progress-success` style the content cleanly—all without extra styles.

---

### 💡 Why Use Bootstrap.Avalonia.Toolkit?

* **Rapid Prototyping:** Apply styles directly in XAML with minimal setup.
* **Consistency:** Uniform spacing, colors, and typography across your app.
* **Modularity:** Components like cards, buttons, and badges are reusable.
* **Open Source:** Contributions are welcome, helping grow the Avalonia community.

---

### 🤝 Contributing & Community

The full source code, sample projects, and development assets are hosted on GitHub. If you encounter bugs or want to contribute new utility classes, please visit the repository:

https://github.com/saiah16526/Bootstrap.Avalonia.Toolkit

Clone the repo: git clone https://github.com/saiah16526/Bootstrap.Avalonia.Toolkit.git

Contribute: Submit a Pull Request or open an Issue for feature requests.

> Your help can make Avalonia **the most modern, designer-friendly UI framework for .NET desktop apps**.

