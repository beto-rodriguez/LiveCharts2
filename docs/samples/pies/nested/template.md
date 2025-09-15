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

{{~ if xaml~}}
## StringToPaintConverter.cs

```csharp
{{~ render $"~/../samples/{platform_samples_folder}/Pies/Nested/StringToPaintConverter.cs" ~}}
```
{{~ end ~}}

{{ render "~/shared/relatedTo.md" }}