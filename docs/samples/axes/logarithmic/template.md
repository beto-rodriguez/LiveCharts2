{{ render "~/shared/genericSampleSimpleHeader.md" }}

## View model

```
{{ full_name | get_vm_from_docs }}
```

## LogarithmicPoint.cs

```csharp
{{~ render "~/../samples/ViewModelsSamples/Axes/Logarithmic/LogarithmicPoint.cs" ~}}
```

## {{~ view_title ~}}

```
{{ full_name | get_view_from_docs }}
```

{{~ if xaml2006 ~}}
## LogarithmicSeries.cs

```csharp
{{~ render $"~/../samples/{samples_folder}/Axes/Logarithmic/LogarithmicSeries.cs" ~}}
```
{{~ end ~}}

{{ render "~/shared/relatedTo.md" }}