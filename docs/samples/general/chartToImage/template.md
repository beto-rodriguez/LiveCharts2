# {{ name | to_title_case }}

{{~ if wpf || avalonia ~}}

:::info
Notice this web site wraps every sample using the `UserControl` class, but LiveCharts controls can be used inside any container, 
this sample also follows a Model-View-* pattern.
:::

{{~ end ~}}

{{~ if winforms ~}}

:::info
Notice this web site builds the control from code behind but you could also grab it from the toolbox,
this sample also uses a ViewModel to populate the properties of the control(s) in this sample.
:::

{{~ end ~}}

## View model

```csharp
{{ full_name | get_vm_from_docs }}
```

## {{~ view_title ~}}

Having the previous data (ViewModel), we add 3 charts to the UI, a `CartesianChart`, a `PieChart` and a `GeoMap`.

```
{{ full_name | get_view_from_docs }}
```

You will get the following plot in the UI.

<div class="text-center sample-img">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result2.png" alt="sample image" />
</div>

{{~ if xaml ~}}

You can take any control in the UI and build an image from it, in the next example, for simplicity, we build an image of our charts once the 
view is loaded.

```csharp
{{~ render $"~/../samples/{samples_folder}/General/ChartToImage{view_code}" ~}}
```
{{~ end ~}}

## Build an image in the server side or console app

LiveCharts can render images without the need of any UI framework, you can build images in the server side or in a console 
application as far as you install the SkiaSharp view package, it is available from NuGet:

{{ "LiveChartsCore.SkiaSharpView" | from_nuget }}

:::info
Notice any view of LiveCharts (WPF, WinForms, Maui, etc..) has a dependency on
LiveChartsCore.SkiaSharpView package, thus you do not need to install this package again if you are already
using LiveCharts to render any control in the UI.
:::

The next code block build a `CartesianChart`, a `PieChart` and a `GeoMap` chart as images, to reproduce this sample
create a new ConsoleApplication in VisualStudio 2022, select .Net 6.0 as the framework.

:::info
The Net 6.0 template is much cleaner than previous console app templates, notice also that LiveCharts is available in 
.NET 5.0, .NET core 3.1 or .NET framework (classic) 4.6.2 or greater.
:::

![image]({{ assets_url }}/docs/_assets/console.png)

Finally build the images in the `Program.cs` file.

```csharp
{{~ "~/../samples/ConsoleSample/ConsoleSample/Program.cs" ~}}
```

:::tip
Notice that the previous code also works in an `ASP.net` project, as far as you are using .NET core 3.1 or greater
:::

{{ render "~/shared/relatedTo.md" }}