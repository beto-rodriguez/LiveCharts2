{{ render this "~/shared/genericSampleSimpleHeader.md" }}

## View model

```
{{ full_name | get_vm_from_docs }}
```

## {{~ view_title ~}}

```
{{ full_name | get_view_from_docs }}
```

{{~ if xaml~}}
## StringToPaintConverter.cs
{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/Pies/Nested/StringToPaintConverter.cs" ~}}
{{~ end ~}}

{{ render this "~/shared/relatedTo.md" }}