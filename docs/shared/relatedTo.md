<!--
To get help on editing this file, see https://github.com/beto-rodriguez/LiveCharts2/blob/dev/docs/readme.md
content is normally pulled from the examples in the repository.
-->

{{~ if related_to != null ~}}

#### Articles you might also find useful:

{{~ for r in related_to ~}}

<div class="my-2">
<a href="{{ compile this r.url }}">
<div class="d-inline-block p-3 bg-light shadow-sm">
<b>{{ r.name }}</b>
</div>
</a>

</div>

{{~ end ~}}

{{~ end ~}}
