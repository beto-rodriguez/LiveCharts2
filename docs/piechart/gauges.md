<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

## Gauges

You can also create gauges with the `PieChart` control, the library provides the `GaugeBuilder` class, 
it is a helper class that build a collection of `PieSeries<ObservableValue>` based on the properties
we specify, take a look at the following sample.

{{~ if xaml || blazor ~}}
<pre><code>GaugeTotal = 100; // 100 is the max value for this gauge.
Series = new GaugeBuilder()
    .WithLabelsSize(50)
    .WithInnerRadius(50)
    .WithBackgroundInnerRadius(50)
    .AddValue(30)
    .BuildSeries();</code></pre>
{{~ end ~}}

{{~ if xaml ~}}
<pre><code>&lt;lvc:PieChart
    Series="{Binding Series}"
    Total="{Binding GaugeTotal}">
&lt;/lvc:PieChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;PieChart
    Series="Series"
    Total="GaugeTotal">
&lt;/PieChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>pieChart1.GaugeTotal = 100; // 100 is the max value for this gauge.
pieChart1.Series = new GaugeBuilder()
    .WithLabelsSize(50)
    .WithInnerRadius(50)
    .WithBackgroundInnerRadius(50)
    .AddValue(30)
    .BuildSeries();</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/basicgauge.png)

## InitialRotation property

You could also use the `PieChart.InitialRotation` property to customize the angle where the gauge starts.

{{~ if xaml || blazor ~}}
<pre><code>InitialRotation = -90; // -90 degrees for the starting angle // mark
GaugeTotal = 100;
Series = new GaugeBuilder()
    .WithLabelsSize(50)
    .WithInnerRadius(50)
    .WithBackgroundInnerRadius(50)
    .AddValue(30)
    .BuildSeries();</code></pre>
{{~ end ~}}

{{~ if xaml ~}}
<pre><code>&lt;lvc:PieChart
    Series="{Binding Series}"
    Total="{Binding GaugeTotal}"
    InitialRotation="{Binding InitialRotation}"> &lt;!-- mark -->
&lt;/lvc:PieChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;PieChart
    Series="Series"
    Total="GaugeTotal"
    InitialRotation="InitialRotation"> &lt;!-- mark -->
&lt;/PieChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>pieChart1.InitialRotation = -90; // mark
pieChart1.GaugeTotal = 100;
pieChart1.Series = new GaugeBuilder()
    .WithLabelsSize(50)
    .WithInnerRadius(50)
    .WithBackgroundInnerRadius(50)
    .AddValue(30)
    .BuildSeries();</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/basicgauge-90.png)

## OffsetRadius property

Now trying the `GaugeBuilder.OffsetRadius`, this property defines an offset in pixels from the `InnerRadius` of the 
pie slice shape to the actual start of the shape:

{{~ if xaml || blazor ~}}
<pre><code>InitialRotation = -90;
GaugeTotal = 100;
Series = new GaugeBuilder()
    .WithLabelsSize(50)
    .WithInnerRadius(50)
    .WithBackgroundInnerRadius(50)
    .WithOffsetRadius(10) // mark
    .AddValue(30)
    .BuildSeries();</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>pieChart1.InitialRotation = -90;
pieChart1.GaugeTotal = 100;
pieChart1.Series = new GaugeBuilder()
    .WithLabelsSize(50)
    .WithInnerRadius(50)
    .WithBackgroundInnerRadius(50)
    .WithOffsetRadius(10) // mark
    .AddValue(30)
    .BuildSeries();</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/basicgaugeor.png)

Notice that there is a similar property the `GaugeBuilder.BackgroundOffsetRadius` will do the same effect
but in the background slice (gray one).

## Multiple series

You can also compare multiple series in the same gauge, the following sample combines adds 2 more series and
uses the `LabelFormatter` property to customize the labels.

{{~ if xaml || blazor ~}}
<pre><code>InitialRotation = 45;
GaugeTotal = 100;
Series = new GaugeBuilder()
    .WithLabelsPosition(PolarLabelsPosition.Start)
    .WithLabelFormatter(point => point.PrimaryValue + " " + point.Context.Series.Name)
    .WithLabelsSize(20)
    .WithInnerRadius(20)
    .WithOffsetRadius(8)
    .WithBackgroundInnerRadius(20)
    .AddValue(30, "Vannesa") // mark
    .AddValue(50, "Charles") // mark
    .AddValue(90, "Ana")  // mark
    .BuildSeries();</code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>pieChart1.InitialRotation = 45;
pieChart1.GaugeTotal = 100;
pieChart1.Series = new GaugeBuilder()
    .WithLabelsPosition(PolarLabelsPosition.Start)
    .WithLabelFormatter(point => point.PrimaryValue + " " + point.Context.Series.Name)
    .WithLabelsSize(20)
    .WithInnerRadius(20)
    .WithOffsetRadius(8)
    .WithBackgroundInnerRadius(20)
    .AddValue(30, "Vannesa") // mark
    .AddValue(50, "Charles") // mark
    .AddValue(90, "Ana")  // mark
    .BuildSeries();</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/basicgaugemultiple.png)

## MaxAngle property

The max angle property defines the maximum angle the gauge can take, the range goes from 0 to 360 degrees (full circle),
by default the value is 360, we can avoid the overlap of the green series in the previous sample if we reserve a space for 
the labels, in this case we will set the `MaxAngle` to 270 degrees, this way we will have space for the labels to render
in the last 90 degrees of our circumference.

{{~ if xaml || blazor ~}}
<pre><code>InitialRotation = 45;
GaugeTotal = 100;
Series = new GaugeBuilder()
    .WithLabelsPosition(PolarLabelsPosition.Start)
    .WithLabelFormatter(point => point.PrimaryValue + " " + point.Context.Series.Name)
    .WithLabelsSize(20)
    .WithInnerRadius(20)
    .WithOffsetRadius(8)
    .WithBackgroundInnerRadius(20)
    .AddValue(30, "Vannesa")
    .AddValue(50, "Charles")
    .AddValue(90, "Ana")
    .BuildSeries();</code></pre>
{{~ end ~}}

{{~ if xaml ~}}
<pre><code>&lt;lvc:PieChart
    Series="{Binding Series}"
    Total="{Binding GaugeTotal}"
    InitialRotation="{Binding InitialRotation}"
    MaxAngle="270"> &lt;!-- mark -->
&lt;/lvc:PieChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;PieChart
    Series="Series"
    Total="GaugeTotal"
    InitialRotation="InitialRotation"
    MaxAngle="270"> &lt;!-- mark -->
&lt;/PieChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>pieChart1.MaxAngle = 270; // mark
pieChart1.InitialRotation = 45;
pieChart1.GaugeTotal = 100;
pieChart1.Series = new GaugeBuilder()
    .WithLabelsPosition(PolarLabelsPosition.Start)
    .WithLabelFormatter(point => point.PrimaryValue + " " + point.Context.Series.Name)
    .WithLabelsSize(20)
    .WithInnerRadius(20)
    .WithOffsetRadius(8)
    .WithBackgroundInnerRadius(20)
    .AddValue(30, "Vannesa")
    .AddValue(50, "Charles")
    .AddValue(90, "Ana")
    .BuildSeries();</code></pre>
{{~ end ~}}

![image]({{ assets_url }}/docs/_assets/basicgaugemultiple270.png)
