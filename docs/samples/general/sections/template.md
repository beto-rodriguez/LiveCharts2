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

You can also create lines as sections, in the next example we set the same value for both
`Yi` and `Yj` and for the `Xi` and `Xj` we use the default value (`null` or `double.NaN`):

<div class="text-center sample-img">
    <img src="{{ assets_url }}/docs/{{ unique_name }}/result2.png" alt="sample image" />
</div>

```csharp
{{~ render  $"~/../samples/{samples_folder}/General/Sections2{view_extension}"  ~}}
```

{{ render "~/shared/relatedTo.md" }}