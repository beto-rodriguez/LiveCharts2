{{ render this "~/shared/genericSampleSimpleHeader.md" }}

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

## CustomPieSeries.cs

{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/Pies/Icons/CustomPieSeries.cs" ~}}

## SvgLabel.cs

{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/Pies/Icons/SvgLabel.cs" ~}}

{{ render this "~/shared/relatedTo.md" }}