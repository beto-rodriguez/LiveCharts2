<!--
To get help on editing this file, see https://github.com/beto-rodriguez/LiveCharts2/blob/dev/docs/readme.md
content is normally pulled from the examples in the repository.
-->

{{ render "~/shared/genericSampleSimpleHeader.md" }}

{{~ if mvvm ~}}
## View model

```csharp
{{ render_current_directory_view_model }}
```
{{~ end ~}}

## MyGeometry.cs

```csharp
{{~ render "~/../samples/ViewModelsSamples/Scatter/Custom/MyGeometry.cs" ~}}
```

## {{~ view_title ~}}

```
{{ render_current_directory_view }}
```

{{~ if xaml2006 ~}}
## CustomSeries.cs

```csharp
{{~ render $"~/../samples/{platform_samples_folder}/Scatter/Custom/CustomSeries.cs" ~}}
```
{{~ end ~}}

{{~ if related_to != null ~}}

### Articles you might also find useful:

{{~ for r in related_to ~}}

<div>
<a href="{{ compile this r.url }}">
{{ r.name }}
</a>
</div>

{{~ end ~}}

{{~ end ~}}
