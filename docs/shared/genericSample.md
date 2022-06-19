{{ render this "~/shared/genericSampleHeader.md" }}

## View Model

```
{{ full_name | get_vm_from_docs }}
```

{{~ if xaml ~}}
## XAML
{{~ end ~}}

{{~ if winforms ~}}
## Code Behind
{{~ end ~}}

{{~ if blazor~}}
## HTML
{{~ end~}}

```
{{ full_name | get_view_from_docs }}
```

{{ render this "~/shared/relatedTo.md" }}
