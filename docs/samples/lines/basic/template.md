{{ render this "~/shared/genericSampleHeader.md" }}

## View Model

```
{{ full_name | get_vm_from_docs }}
```

## {{~ view_title ~}}

```
{{ full_name | get_view_from_docs }}
```

{{~ if xaml2006 ~}}
## CustomGeometryPointColumnSeries.cs
{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/Lines/Basic/CustomStarLineSeries.cs" ~}}
{{~ end ~}}

{{ render this "~/shared/relatedTo.md" }}
