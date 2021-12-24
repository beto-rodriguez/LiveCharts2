{{ render this "~/shared/genericSampleSimpleHeader.md" }}

## View model

```
{{ full_name | get_vm_from_docs }}
```

## LogarithmicPoint.cs

{{~ "~/../samples/ViewModelsSamples/Axes/Logarithmic/LogarithmicPoint.cs" | render_file_as_code ~}}

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

{{ render this "~/shared/relatedTo.md" }}