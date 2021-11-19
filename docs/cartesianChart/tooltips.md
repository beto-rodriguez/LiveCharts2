<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# Tooltips

Tooltips are popups that help the user to read a chart as the pointer moves.

![tooltips](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/tooltips.gif)

## TooltipPosition property

You can place a tooltip at `Top`, `Bottom`, `Left`, `Right`, `Center` or `Hidden` positions, for now 
tooltips for the `PieChart` class only support the `Center` position, default value is `Top`.

Notice the `Hidden` position will disable tooltips in a chart.

{{~ if xaml ~}}
<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Top">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Bottom">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Left">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Right">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Center">&lt;!-- mark -->
&lt;/lvc:CartesianChart>
&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Hidden">&lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Top">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Bottom">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Left">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Right">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Center">&lt;!-- mark -->
&lt;/CartesianChart>
&lt;CartesianChart
    Series="series"
    TooltipPosition="LiveChartsCore.Measure.TooltipPosition.Hidden">&lt;!-- mark -->
&lt;/CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.Series = new ISeries[] { new LineSeries&lt;int> { Values = new[] { 2, 5, 4 } } };
cartesianChart1.TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Bottom;</code></pre>
{{~ end ~}}

## TooltipFindingStrategy property

The `TooltipFindingStrategy`property determines the method the chart will use to find the points to include in a 
tooltip, the options are:

**CompareAll**: Selects one point per series, the closer point to the pointer position.

**CompareOnlyX**: Selects one point per series, the closer point to the pointer position, but it only measures the horizontal distance.

**CompareOnlyY**: Selects one point per series, the closer point to the pointer position, but it only measures the vertical distance.

**Automatic**: Based on the series in the chart, LiveCharts will determine a finding strategy (`CompareAll`, `CompareOnlyX` or 
`CompareOnlyY`), all the series have a preferred finding strategy, normally vertical series prefer the `CompareOnlyX` strategy, 
horizontal series prefer `CompareOnlyY`, and scatter series prefers `CompareAll`, if all the series prefer the same strategy, then that
strategy will be selected for the chart, if any series differs then the `CompareAll` strategy will be used.

{{~ if xaml ~}}
<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipFindingStrategy="CompareOnlyX">&lt;!-- mark -->
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>&lt;CartesianChart
    Series="series"
    TooltipFindingStrategy="LiveChartsCore.Measure.TooltipFindingStrategy.CompareOnlyX">&lt;!-- mark -->
&lt;/CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
<pre><code>cartesianChart1.Series = new ISeries[] { new LineSeries&lt;int> { Values = new[] { 2, 5, 4 } } };
cartesianChart1.TooltipFindingStrategy = LiveChartsCore.Measure.TooltipFindingStrategy.CompareOnlyX;</code></pre>
{{~ end ~}}

## Tooltip point text

You can define the text the tooltip will display for a given point, using the `Series.TooltipLabelFormatter` property, this 
property is of type `Func<ChartPoint, string>` this means that is is a function, that takes a point as parameter
and returns a string, the point will be injected by LiveCharts in this function to get a string out of it when it
requires to build the text for a point in a tooltip, the injected point will be different as the user moves the pointer over the
user interface.

By default the library already defines a default `TooltipLabelFormatter` for every series, all the series have a different
formatter, but generally the default value uses the `Series.Name` and the `ChartPoint.PrimaryValue` properties, the following
code snippet illustrates how to build a custom tooltip formatter.

<pre><code>new LineSeries&lt;double>
{
    Name = "Sales",
    Values = new ObservableCollection&lt;double> { 200, 558, 458 },
    // for the following formatter
    // when the pointer is over the first point (200), the tooltip will display:
    // Sales: 200
    TooltipLabelFormatter =
        (chartPoint) => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue}"
},

new ColumnSeries&lt;double>
{
    Name = "Sales 2",
    Values = new ObservableCollection&lt;double> { 250, 350, 240 },
    // now it will use a currency formatter to display the primary value
    // result: Sales 2: $200.00
    TooltipLabelFormatter =
        (chartPoint) => $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:C2}"
},

new StepLineSeries&lt;ObservablePoint>
{
    Name = "Average",
    Values = new ObservableCollection&lt;ObservablePoint>
    {
        new ObservablePoint(10, 5),
        new ObservablePoint(5, 8)
    },
    // We can also display both coordinates (X and Y in a cartesian coordinate system)
    // result: Average: 10, 5
    TooltipLabelFormatter =
        (chartPoint) => $"{chartPoint.Context.Series.Name}: {chartPoint.SecondaryValue}, {chartPoint.PrimaryValue}"
},

new ColumnSeries&lt;ObservablePoint>
{
    Values = new ObservableCollection&lt;double> { 250, 350, 240 },
    // or anything...
    // result: Sales at this moment: $200.00
    TooltipLabelFormatter =
        (chartPoint) => $"Sales at this moment: {chartPoint.PrimaryValue:C2}"
}</code></pre>

## Styling tooltips

{{~ if xaml || blazor ~}}
A chart exposes many properties to quickly style a tooltip:

<pre><code>&lt;lvc:CartesianChart
    Series="{Binding Series}"
    TooltipPosition="Left"
    TooltipFontFamily="Courier New"
    TooltipFontSize="25"
    TooltipTextBrush="#f2f4c3"
    TooltipBackground="#480032">
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if winforms ~}}
A chart exposes many properties to quickly style a tooltip:

<pre><code>cartesianChart1.TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Left;
cartesianChart1.TooltipFont = new System.Drawing.Font("Courier New", 25);
cartesianChart1.TooltipTextColor = System.Drawing.Color.FromArgb(255, 242, 244, 195);
cartesianChart1.TooltipBackColor = System.Drawing.Color.FromArgb(255, 72, 0, 50);</code></pre>
{{~ end ~}}

{{~ if blazor ~}}
You can use css to override the style of the tooltip.

<pre><code>&lt;style>
	.lvc-tooltip {
		background-color: #480032 !important;
	}

	.lvc-tooltip-item {
		font-family: SFMono-Regular, Menlo, Monaco, Consolas !important;
		color: #F2F4C3 !important;
	}
&lt;/style></code></pre>
{{~ end ~}}

The code above would result in the following tooltip:

![zooming](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/tooltip-custom-style.png)

## Custom template

{{~ if xaml ~}}
If you need to customize more, you can also use the create your own template:
{{~ end ~}}

{{~ if avalonia ~}}

<pre><code>&lt;lvc:CartesianChart Series="{Binding Series}">
    &lt;lvc:CartesianChart.TooltipTemplate>
        &lt;DataTemplate>
            &lt;Border Background="Transparent" Padding="12">
            &lt;Border Background="#353535" CornerRadius="4"
                    BoxShadow="0 0 10 0 #40000000, 0 0 10 0 #40000000, 0 0 10 0 #40000000, 0 0 10 0 #40000000">
                &lt;ItemsControl Items="{Binding Points, RelativeSource={RelativeSource AncestorType=lvc:DefaultTooltip}}">
                &lt;ItemsControl.ItemsPanel>
                    &lt;ItemsPanelTemplate>
                    &lt;StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Orientation="Vertical" />
                    &lt;/ItemsPanelTemplate>
                &lt;/ItemsControl.ItemsPanel>
                &lt;ItemsControl.ItemTemplate>
                    &lt;DataTemplate DataType="{x:Type ctx:TooltipPoint}">
                    &lt;Border Padding="7 5">
                        &lt;StackPanel Orientation="Horizontal">
                        &lt;TextBlock
                            Foreground="#fafafa"
                            Text="{Binding Point.AsTooltipString}"
                            Margin="0 0 8 0"
                            VerticalAlignment="Center"/>
                        &lt;!-- LiveCharts uses the motion canvas control to display the series miniature -->
                        &lt;lvc:MotionCanvas
                            Margin="0 0 8 0"
                            PaintTasks="{Binding Series.CanvasSchedule.PaintSchedules}"
                            Width="{Binding Series.CanvasSchedule.Width}"
                            Height="{Binding Series.CanvasSchedule.Height}"
                            VerticalAlignment="Center"/>
                        &lt;/StackPanel>
                    &lt;/Border>
                    &lt;/DataTemplate>
                &lt;/ItemsControl.ItemTemplate>
                &lt;/ItemsControl>
            &lt;/Border>
            &lt;/Border>
        &lt;/DataTemplate>
    &lt;/lvc:CartesianChart.TooltipTemplate>
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if blazor ~}}
<pre><code>@page "/General/TemplatedTooltips"
@using LiveChartsCore.SkiaSharpView.Blazor
@using ViewModelsSamples.General.TemplatedTooltips

&lt;CartesianChart Series="ViewModel.Series">

	<!--
		Use TooltipTemplate property to pass your own template.

		GetSeriesMiniatureStyle():
		returns a css style that sets the width and height css properties
		based on the series properties.

		GetSeriesAsMiniaturePaints():
		returns the series as miniature shapes for the MotionCanvas class.
	-->

	&lt;TooltipTemplate>
		&lt;h5>This is a custom tooltip</h5>

		@foreach (var tooltipPoint in @context)
		{
			&lt;div class="d-flex">
				&lt;div>
					@tooltipPoint.Point.AsTooltipString
				&lt;/div>
				&lt;div class="lvc-miniature" style="@LiveChartsBlazor.GetSeriesMiniatureStyle(tooltipPoint.Series)">
					&lt;MotionCanvas PaintTasks="@LiveChartsBlazor.GetSeriesAsMiniaturePaints(tooltipPoint.Series)" />
				&lt;/div>
			&lt;/div>
		}
	&lt;/TooltipTemplate>

&lt;/CartesianChart></code></pre>
{{~ end ~}}

{{~ if uwp ~}}
missing sample...
{{~ end ~}}

{{~ if winforms ~}}
You can create your own tooltip control, the key is that your control must implement `IChartTooltip<SkiaSharpDrawingContext>` and then
you have to create a new instance of that control when your chart initializes.

Add a new form to your app named `CustomTooltip`, then change the code as follows:

<pre><code>using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsSample.General.TemplatedTooltips
{
    public partial class CustomTooltip : Form, IChartTooltip&lt;SkiaSharpDrawingContext>, IDisposable
    {
        public CustomTooltip()
        {
            InitializeComponent();
        }

        public void Show(IEnumerable&lt;TooltipPoint> tooltipPoints, Chart&lt;SkiaSharpDrawingContext> chart)
        {
            var wfChart = (Chart)chart.View;

            var size = DrawAndMesure(tooltipPoints, wfChart);
            LvcPoint? location = null;

            if (chart is CartesianChart&lt;SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new LvcSize((float)size.Width, (float)size.Height), chart.ControlSize);
            }
            if (chart is PieChart&lt;SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new LvcSize((float)size.Width, (float)size.Height));
            }

            BackColor = Color.FromArgb(255, 30, 30, 30);
            Height = (int)size.Height;
            Width = (int)size.Width;

            var l = wfChart.PointToScreen(Point.Empty);
            var x = l.X + (int)location.Value.X;
            var y = l.Y + (int)location.Value.Y;
            Location = new Point(x, y);
            Show();

            wfChart.CoreCanvas.Invalidate();
        }

        private SizeF DrawAndMesure(IEnumerable&lt;TooltipPoint> tooltipPoints, Chart chart)
        {
            SuspendLayout();
            Controls.Clear();

            var h = 0f;
            var w = 0f;
            foreach (var point in tooltipPoints)
            {
                using var g = CreateGraphics();
                var text = point.Point.AsTooltipString;
                var size = g.MeasureString(text, chart.TooltipFont);

                var drawableSeries = (IChartSeries&lt;SkiaSharpDrawingContext>)point.Series;

                Controls.Add(new MotionCanvas
                {
                    Location = new Point(6, (int)h + 6),
                    PaintTasks = drawableSeries.CanvasSchedule.PaintSchedules,
                    Width = (int)drawableSeries.CanvasSchedule.Width,
                    Height = (int)drawableSeries.CanvasSchedule.Height
                });
                Controls.Add(new Label
                {
                    Text = text,
                    ForeColor = Color.FromArgb(255, 250, 250, 250),
                    Font = chart.TooltipFont,
                    Location = new Point(6 + (int)drawableSeries.CanvasSchedule.Width + 6, (int)h + 6),
                    AutoSize = true
                });

                var thisW = size.Width + 18 + (int)drawableSeries.CanvasSchedule.Width;
                h += size.Height + 6;
                w = thisW > w ? thisW : w;
            }

            h += 6;

            ResumeLayout();
            return new SizeF(w, h);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}</code></pre>

Your tooltip is ready to be used, now when you create a chart, we have to pass a new instance of the tooltip we just created.

<pre><code>var cartesianChart = new CartesianChart(tooltip: new CustomTooltip())
{
    Series = viewModel.Series
};</code></pre>

{{~ end ~}}

{{~ if winui ~}}
missing sample...
{{~ end ~}}

{{~ if wpf ~}}
<pre><code>&lt;lvc:CartesianChart Grid.Row="0" Series="{Binding Series}" TooltipPosition="Top" >
    &lt;lvc:CartesianChart.TooltipTemplate>
        &lt;DataTemplate>
            &lt;Border Background="#303030">
                &lt;ItemsControl ItemsSource="{Binding Points, RelativeSource={RelativeSource AncestorType=lvc:DefaultTooltip}}">
                    &lt;ItemsControl.ItemsPanel>
                        &lt;ItemsPanelTemplate>
                            &lt;StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Orientation="Vertical" />
                        &lt;/ItemsPanelTemplate>
                    &lt;/ItemsControl.ItemsPanel>
                    &lt;ItemsControl.ItemTemplate>
                        &lt;DataTemplate DataType="{x:Type ctx:TooltipPoint}">
                            &lt;Border Padding="7 5">
                                &lt;StackPanel Orientation="Horizontal">
                                    &lt;TextBlock Text="{Binding Point.AsTooltipString}" Margin="0 0 8 0" Foreground="AntiqueWhite" />
                                    &lt;lvc:MotionCanvas Margin="0 0 8 0" 
                                        PaintTasks="{Binding Series.CanvasSchedule.PaintSchedules}"
                                        Width="{Binding Series.CanvasSchedule.Width}"
                                        Height="{Binding Series.CanvasSchedule.Height}"
                                        VerticalAlignment="Center"/>
                                &lt;/StackPanel>
                            &lt;/Border>
                        &lt;/DataTemplate>
                    &lt;/ItemsControl.ItemTemplate>
                &lt;/ItemsControl>
            &lt;/Border>
        &lt;/DataTemplate>
    &lt;/lvc:CartesianChart.TooltipTemplate>
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

{{~ if xamarin ~}}
</code></pre>&lt;lvc:CartesianChart Grid.Row="0" Series="{Binding Series}">
    &lt;lvc:CartesianChart.TooltipTemplate>
        &lt;DataTemplate>
            &lt;Frame Background="#353535" CornerRadius="4" HasShadow="True" Padding="6">
                &lt;StackLayout BindableLayout.ItemsSource="{Binding Points, Source={RelativeSource AncestorType={x:Type lvc:TooltipBindingContext}}}">
                    &lt;BindableLayout.ItemTemplate>
                        &lt;DataTemplate>
                            &lt;StackLayout Orientation="Horizontal" VerticalOptions="Center">
                                &lt;Label Text="{Binding Point.AsTooltipString}"
                                    TextColor="#fafafa"/>
                                &lt;lvc:MotionCanvas 
                                    VerticalOptions="Center"
                                    Margin="5, 0, 0, 0"
                                    WidthRequest="{Binding Series, Converter={StaticResource wConverter}}"
                                    HeightRequest="{Binding Series, Converter={StaticResource hConverter}}"
                                    PaintTasks="{Binding Series, Converter={StaticResource paintTaskConverter}}"/>
                            &lt;/StackLayout>
                        &lt;/DataTemplate>
                    &lt;/BindableLayout.ItemTemplate>
                &lt;/StackLayout>
            &lt;/Frame>
        &lt;/DataTemplate>
    &lt;/lvc:CartesianChart.TooltipTemplate>
&lt;/lvc:CartesianChart></code></pre>
{{~ end ~}}

![custom tooltip](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/tooltip-custom-template.png)
