# {{ name | to_title_case }}

<div class="text-center sample-img">
    <img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/dev/docs/{{ unique_name }}/result.png" alt="sample image" />
</div>

When you need to specify both, X and Y coordinates, you can use the 
[ObservablePoint](https://livecharts.dev/api/{{ version }}/LiveChartsCore.Defaults.ObservablePoint) class, 
in other examples in this repository you will notice that the library can also plot primitive types such as
`int` or `double`, the library (usually) uses the index of the element in the array as the `X` coordinate
and the value as the `Y` coordinate, so even we are passing an array of primitives, the library is mapping 
that object to and `(X, Y)` point, the library can build charts from any object, but we much teach LiveCharts
how to handle that object, if you want to learn more, please read the 
[Mappers article](https://livecharts.dev/docs/{{ platform }}/{{ version }}/Overview.Mappers).

{{~ if mvvm ~}}
## View Model

```csharp
{{ render_current_directory_view_model }}
```
{{~ end ~}}

## {{~ view_title ~}}

```
{{ render_current_directory_view }}
```

{{ render "~/shared/relatedTo.md" }}
