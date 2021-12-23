# {{ name | to_title_case }}

:::info
Hover over the image to see the chart animation
:::

{{~ if wpf || avalonia ~}}

:::info
Notice this web site wraps every sample using the `UserControl` class, but LiveCharts controls can be used inside any container, 
this sample also follows a Model-View-* pattern.
:::

{{~ end ~}}

{{~ if xamarin ~}}

:::info
Notice this web site wraps every sample using the `ContentPage` class, but LiveCharts controls can be used inside any container, 
this sample also follows a Model-View-* pattern.
:::

{{~ end ~}}

{{~ if winforms ~}}

:::info
Notice this web site builds the control from code behind but you could also grab it from the toolbox,
this sample also uses a ViewModel to populate the properties of the control(s) in this sample.
:::

{{~ end ~}}

<div class="position-relative text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result.png" class="static" alt="basic line" />
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result.gif" alt="basic line" />
</div>