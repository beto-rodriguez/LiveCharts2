{{ render "~/shared/genericSampleSimpleHeader.md" }}

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

{{ render "~/shared/relatedTo.md" }}