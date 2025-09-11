{{ render "~/shared/genericSampleHeader.md" }}

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

```csharp
{{~ render $"~/../samples/{samples_folder}/Lines/Basic/CustomStarLineSeries.cs" ~}}
```
{{~ end ~}}

{{ render "~/shared/relatedTo.md" }}
