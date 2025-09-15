{{ render "~/shared/genericSampleSimpleHeader.md" }}

{{~ if mvvm ~}}
## View model

```csharp
{{ full_name | get_vm_from_docs }}
```
{{~ end ~}}

## MyGeometry.cs

```csharp
{{~ render "~/../samples/ViewModelsSamples/Scatter/Custom/MyGeometry.cs" ~}}
```

## {{~ view_title ~}}

```
{{ full_name | get_view_from_docs }}
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
