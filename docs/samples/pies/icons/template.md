{{ render "~/shared/genericSampleSimpleHeader.md" }}

{{~ if mvvm ~}}
## View model

```csharp
{{ full_name | get_vm_from_docs }}
```
{{~ end ~}}

## {{~ view_title ~}}

```csharp
{{ full_name | get_view_from_docs }}
```

{{~ if !blazor ~}}
## CustomPieSeries.cs

```csharp
{{~ render $"~/../samples/{platform_samples_folder}/Pies/Icons/CustomPieSeries.cs" ~}}
```

## SvgLabel.cs

```csharp
{{~ render $"~/../samples/{platform_samples_folder}/Pies/Icons/SvgLabel.cs" ~}}
```
{{~ end ~}}

{{ render "~/shared/relatedTo.md" }}