{{ render "~/shared/genericSampleSimpleHeader.md" }}

## View model

```csharp
{{ full_name | get_vm_from_docs }}
```

## {{~ view_title ~}}

```csharp
{{ full_name | get_view_from_docs }}
```

{{~ if xaml~}}
## StringToPaintConverter.cs

```csharp
{{~ render $"~/../samples/{samples_folder}/Pies/Nested/StringToPaintConverter.cs" ~}}
```
{{~ end ~}}

{{ render "~/shared/relatedTo.md" }}