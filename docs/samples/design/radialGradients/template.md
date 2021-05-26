# {{ name | to_title_case }}

<p align="center">
  <img src="https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/{{ full_name }}/result.png" />
</p>

{{~ if xaml ~}}
## XAML
{{~ end ~}}

{{~ if winforms ~}}
## Form code behind
{{~ end ~}}

```
{{ full_name | get_view_from_docs }}
```

## View model

```
{{ full_name | get_vm_from_docs }}
```