# {{ name | to_title_case }}

{{~ if wpf || avalonia ~}}

:::info
Notice this web site wraps every sample using the `UserControl` class, but LiveCharts controls can be used inside any container, 
this sample also follows a Model-View-* pattern.
:::

{{~ end ~}}

{{~ if xamarin ~}}

:::info
Notice this web site wraps every sample using the `ContentPage` class, but LiveCharts controls can be used inside any container, 
this sample also follows a Model-View-* pattern.
:::

{{~ end ~}}

{{~ if winforms ~}}

:::info
Notice this web site builds the control from code behind but you could also grab it from the toolbox,
this sample also uses a ViewModel to populate the properties of the control(s) in this sample.
:::

{{~ end ~}}

<div class="text-center">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result2.png" alt="sample image" />
</div>

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

{{~ if blazor ~}}
## HTML
{{~ end~}}

```
{{ full_name | get_view_from_docs }}
```

{{~ if xaml ~}}
{{~ render_params_file_as_code this "~/../samples/$PlatformSamplesFolder/General/ChartToImage/$PlatformViewCodeBehindFile" ~}}
{{~ end ~}}

{{ render this "~/shared/relatedTo.md" }}