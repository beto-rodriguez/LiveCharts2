{{ render "~/shared/genericSampleSimpleHeader.md" }}

{{~ if mvvm ~}}
## View model

```csharp
{{ render_current_directory_view_model }}
```
{{~ end ~}}

## {{~ view_title ~}}

```csharp
{{ render_current_directory_view }}
```

{{~ if xaml~}}
## StringToPaintConverter.cs

```csharp
{{~ render $"~/../samples/{platform_samples_folder}/Pies/Nested/StringToPaintConverter.cs" ~}}
```
{{~ end ~}}

{{ render "~/shared/relatedTo.md" }}