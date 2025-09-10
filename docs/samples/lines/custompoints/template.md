{{ render this "~/shared/genericSampleSimpleHeader.md" }}

## View model

```
{{ full_name | get_vm_from_docs }}
```

## MyGeometry.cs

{{~ "~/../samples/ViewModelsSamples/Lines/Custom/MyGeometry.cs" | render_file_as_code ~}}

## {{~ view_title ~}}

```
{{ full_name | get_view_from_docs }}
```

{{~ if xaml2006 ~}}
## CustomSeries.cs
{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/Lines/CustomPoints/CustomArrowLineSeries.cs" ~}}
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