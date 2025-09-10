{{ render this "~/shared/genericSampleSimpleHeader.md" }}

## View model

```
{{ full_name | get_vm_from_docs }}
```

## MyGeometry.cs

{{~ "~/../samples/ViewModelsSamples/Bars/Custom/MyGeometry.cs" | render_file_as_code ~}}

{{~ if xaml ~}}
## XAML
{{~ end ~}}

{{~ if winforms ~}}
## Form code behind
{{~ end ~}}

{{~ if blazor~}}
## HTML
{{~ end~}}

```
{{ full_name | get_view_from_docs }}
```

{{~ if xaml2006 ~}}
## LogarithmicSeries.cs
{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/Bars/Custom/CustomSeries.cs" ~}}
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