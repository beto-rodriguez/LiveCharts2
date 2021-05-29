# {{ name | to_title_case }}

![sample image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/{{ unique_name }}/result.gif)

## View model

```
{{ full_name | get_vm_from_docs }}
```

{{~ if xaml ~}}
## XAML
{{~ end ~}}

{{~ if winforms ~}}
## Form code behind
{{~ end ~}}

```
{{ full_name | get_view_from_docs }}
```