# {{ name | to_title_case }}

{{~ if xaml ~}}

:::info
This sample uses C# 13 preview features such as partial properties, it also uses features from the
[CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm/) package, you can learn more about it 
[here]({{ website_url }}/docs/{{ platform }}/{{ version }}/About.About%20this%20samples).
:::

{{~ end ~}}

{{~ if wpf || avalonia || uno || winui  ~}}

:::info
This web site wraps every sample using a `UserControl` instance, but LiveCharts controls can be used inside any container.
:::

{{~ end ~}}


{{~ if maui ~}}

:::info
This web site wraps every sample using a `ContentPage` instance, but LiveCharts controls can be used inside any container.
:::

{{~ end ~}}


{{~ if winforms ~}}

:::info
This web site builds the control from code behind but you could also grab it from the toolbox.
:::

{{~ end ~}}

<div class="text-center sample-img">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result.png" alt="sample image" />
</div>
