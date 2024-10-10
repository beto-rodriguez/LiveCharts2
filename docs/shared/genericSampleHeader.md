# {{ name | to_title_case }}

<div class="position-relative text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result.png" class="static" alt="sample image" />
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result.gif" alt="sample image" />
</div>

:::tip
Hover over the image to see the chart animation
:::

{{~ if xaml ~}}

:::info
The `ObservableObject` class, `ObservableProperty` and `RelayCommand` attributes, come from the 
[CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm/) package, you can read more about it 
[here]({{ website_url }}/docs/{{ platform }}/{{ version }}/About.About%20this%20samples).
:::

{{~ end ~}}