# {{ name | to_title_case }}

{{~ if xaml ~}}

:::info
The `[ObservableObject]`, `[ObservableProperty]` and `[RelayCommand]` attributes come from the 
[CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm/) package, you can read more about it 
[here]({{ website_url }}/docs/{{ platform }}/{{ version }}/About.About%20this%20samples).
:::

{{~ end ~}}

{{~ if wpf || avalonia || uno || winui  ~}}

:::info
This web site wraps every sample using a `UserControl` instance, but LiveCharts controls can be used inside any container.
:::

{{~ end ~}}


{{~ if  maui ~}}

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
    <img src="{{ assets_url }}/docs/{{ unique_name }}/all.png" alt="sample image" />
</div>

{{~ if mvvm ~}}
## View model

```csharp
{{ full_name | get_vm_from_docs }}
```
{{~ end ~}}

## {{~ view_title ~}}

```
{{ full_name | get_view_from_docs }}
```

{{ render "~/shared/relatedTo.md" }}