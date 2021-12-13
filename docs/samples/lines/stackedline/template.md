{{ render this "~/shared/genericSampleHeader.md" }}

{{~ if xaml ~}}
## XAML
{{~ end ~}}

{{~ if winforms ~}}
## Form code behind
{{~ end ~}}

{{~~ if blazor~}}
## HTML
{{~ end~}}

```
{{ "~/samples/" | get_view_from_docs }}
```

## View model

```
{{ "" | get_vm_from_docs }}
```