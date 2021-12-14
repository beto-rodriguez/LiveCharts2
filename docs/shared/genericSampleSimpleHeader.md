# {{ name | to_title_case }}

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

<div class="text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result.png"alt="basic line" />
</div>