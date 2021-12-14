{{ render this "~/shared/genericSampleJustGifHeader.md" }}

## View model

```
{{ full_name | get_vm_from_docs }}
```

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

{{~ if xaml ~}}
## View code behind

{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/Events/AddPointOnClick/$PlatformViewCodeBehindFile" ~}}
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