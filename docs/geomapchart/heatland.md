# HeatLand shape

The `HeatLand` class assigns a numeric value to a land area in a map, then the chart will create a heat map based on all the `HeatLand` instances,
the main properties of the `HeatLand` class are the `Name` and the `Value` properties, 

## The Name property

The `Name` is of type `string` and it is a unique identifier
for each lane in the map, for example in the case of a world map, the unique name for Japan is "jpn", for France is "fra" and for Brazil is "bra",
every country has a unique identifier.

## The value property

The `Value` is the numeric value for the lane, then the chart will create a heat map based on the `GeoMap.HeatMap` property, where the range of all the 
values in the all the `HeatLand` instances in the chart will be mapped lineally to a color according to the `GeoMap.HeatMap` property.

{{~ if xaml || blazor ~}}
<pre><code>Shapes = new HeatLand[]
{
    new HeatLand { Name= "jpn", Value = 15 },
    new HeatLand { Name = "usa", Value = 15 },
    new HeatLand { Name = "chn", Value = 14 },
    new HeatLand { Name = "bra", Value = 13 },
    new HeatLand { Name = "are", Value = 13 },
    new HeatLand { Name = "deu", Value = 13 },
    new HeatLand { Name = "ind", Value = 12 },
    new HeatLand { Name = "zaf", Value = 12 },
    new HeatLand { Name = "rus", Value = 11 },
    new HeatLand { Name = "mex", Value = 10 },
    new HeatLand { Name = "kor", Value = 10 },
    new HeatLand { Name = "can", Value = 8 },
    new HeatLand { Name = "fra", Value = 8 },
    new HeatLand { Name = "esp", Value = 7 }
};</code></pre>
{{~ end ~}}

{{~ if xaml ~}}
<pre><code>&lt;lvc:GeoMap Shapes="{Binding Shapes}">&lt;/lvc:GeoMap></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;lvc:GeoMap Shapes="Shapes">&lt;/lvc:GeoMap></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>geoMap1.Shapes = new HeatLand[]
{
    new HeatLand { Name= "jpn", Value = 15 },
    new HeatLand { Name = "usa", Value = 15 },
    new HeatLand { Name = "chn", Value = 14 },
    new HeatLand { Name = "bra", Value = 13 },
    new HeatLand { Name = "are", Value = 13 },
    new HeatLand { Name = "deu", Value = 13 },
    new HeatLand { Name = "ind", Value = 12 },
    new HeatLand { Name = "zaf", Value = 12 },
    new HeatLand { Name = "rus", Value = 11 },
    new HeatLand { Name = "mex", Value = 10 },
    new HeatLand { Name = "kor", Value = 10 },
    new HeatLand { Name = "can", Value = 8 },
    new HeatLand { Name = "fra", Value = 8 },
    new HeatLand { Name = "esp", Value = 7 }
};</code></pre>
{{~ end ~}}

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/geomaphs.png)

