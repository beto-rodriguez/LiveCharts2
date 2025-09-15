# {{ name | to_title_case }}

{{~ if xaml ~}}
:::info
The `[ObservableObject]`, `[ObservableProperty]` and `[RelayCommand]` attributes come from the 
[CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm/) package, you can read more about it 
[here]({{ website_url }}/docs/{{ platform }}/{{ version }}/About.About%20this%20samples).
:::
{{~ end ~}}


{{~ if winforms ~}}
:::info
This web site builds the control from code behind but you could also grab it from the toolbox.
:::
{{~ end ~}}

<div class="text-center sample-img">
    <img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/dev/docs/{{ unique_name }}/all.png" alt="sample image" />
</div>

{{~ if mvvm ~}}
## View model

```csharp
{{ render_current_directory_view_model }}
```
{{~ end ~}}

## {{~ view_title ~}}

```
{{ render_current_directory_view }}
```

{{ render "~/shared/relatedTo.md" }}
