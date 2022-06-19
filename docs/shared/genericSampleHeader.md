# {{ name | to_title_case }}

:::info
Hover over the image to see the chart animation
:::

{{~ if xaml ~}}

:::info
The `[ObservableObject]`, `[ObservableProperty]` and `[ICommand]` attributes come from the 
[CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm/) package, you can read more about it 
[here]({{ website_url }}/docs/{{ platform }}/{{ version }}/About.About%20this%20samples).
:::

{{~ end ~}}

{{~ if wpf || avalonia || uno || winui  ~}}

:::info
This web site wraps every sample using a `UserControl` instance, but LiveCharts controls can be used inside any container.
:::

{{~ end ~}}


{{~ if xamarin || maui ~}}

:::info
This web site wraps every sample using a `ContentPage` instance, but LiveCharts controls can be used inside any container.
:::

{{~ end ~}}


{{~ if winforms ~}}

:::info
This web site builds the control from code behind but you could also grab it from the toolbox,
this sample also uses a ViewModel to populate the properties of the control(s) in this sample.
:::

{{~ end ~}}

<div class="position-relative text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result.png" class="static" alt="sample image" />
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result.gif" alt="sample image" />
</div>