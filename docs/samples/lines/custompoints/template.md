{{ render "~/shared/genericSampleSimpleHeader.md" }}

## View model

```csharp
{{ full_name | get_vm_from_docs }}
```

## MyGeometry.cs

{{~ render "~/../samples/ViewModelsSamples/Lines/Custom/MyGeometry.cs" ~}}

## {{~ view_title ~}}

```
{{ full_name | get_view_from_docs }}
```

{{~ if xaml2006 ~}}
## CustomSeries.cs

```csharp
{{~ render $"~/../samples/{samples_folder}/Lines/CustomPoints/CustomArrowLineSeries.cs" ~}}
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