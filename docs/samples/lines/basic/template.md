{{ render "~/shared/genericSampleHeader.md" }}

{{~ if mvvm ~}}
## View model

```csharp
{{ full_name | get_vm_from_docs }}
```
{{~ end ~}}

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
