{{ render "~/shared/genericSampleJustGifHeader.md" }}

## View model

```
{{ full_name | get_vm_from_docs }}
```

## {{~ view_title ~}}

```
{{ full_name | get_view_from_docs }}
```

{{~ if related_to != null ~}}

### Articles you might also find useful:

{{~ for r in related_to ~}}

<div>
<a href="{{ compile this r.url }}">
{{ r.name }}
</a>
</div>

{{~ end ~}}

{{~ end ~}}