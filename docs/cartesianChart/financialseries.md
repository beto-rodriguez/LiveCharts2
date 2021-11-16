<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# {{ name | to_title_case }}

{{ render this "~/shared/series.md" }}

{{ render this "~/shared/datalabels.md" }}

## UpStroke property

Defines the stroke to use when the `Open` is greater than the `Close`.

## UpFill property

Defines the fill to use when the `Open` is greater than the `Close`.

## DownStroke property

Defines the stroke to use when the `Close` is greater than the `Open`.

## DownFill property

Defines the stroke to use when the `Close` is greater than the `Open`.

## Paints example

The following sample illustrates the use of the previous properties.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/financialpaints.png)

<pre><code>XAxes = new[]
{
    new Axis
    {
        LabelsRotation = 15,
        Labeler = value => new DateTime((long)value).ToString("yyyy MMM dd"),
        // set the unit width of the axis to "days"
        // since our X axis is of type date time and 
        // the interval between our points is in days
        UnitWidth = TimeSpan.FromDays(1).Ticks
    }
};

Series = new ISeries[]
{
    new CandlesticksSeries&lt;FinancialPoint>
    {
        UpFill = new SolidColorPaintTask(SKColors.Blue), // mark
        UpStroke = new SolidColorPaintTask(SKColors.CornflowerBlue) { StrokeThickness = 5 }, // mark
        DownFill = new SolidColorPaintTask(SKColors.Red), // mark
        DownStroke = new SolidColorPaintTask(SKColors.Orange) { StrokeThickness = 5 }, // mark
        Values = new ObservableCollection&lt;FinancialPoint>
        {
            //                             date,        high, open, close, low
            new FinancialPoint(new DateTime(2021, 1, 1), 523, 500, 450, 400),
            new FinancialPoint(new DateTime(2021, 1, 2), 500, 450, 425, 400),
            new FinancialPoint(new DateTime(2021, 1, 3), 490, 425, 400, 380),
            new FinancialPoint(new DateTime(2021, 1, 4), 420, 400, 420, 380),
            new FinancialPoint(new DateTime(2021, 1, 5), 520, 420, 490, 400),
            new FinancialPoint(new DateTime(2021, 1, 6), 580, 490, 560, 440),
            new FinancialPoint(new DateTime(2021, 1, 7), 570, 560, 350, 340),
            new FinancialPoint(new DateTime(2021, 1, 8), 380, 350, 380, 330),
            new FinancialPoint(new DateTime(2021, 1, 9), 440, 380, 420, 350),
            new FinancialPoint(new DateTime(2021, 1, 10), 490, 420, 460, 400),
            new FinancialPoint(new DateTime(2021, 1, 11), 520, 460, 510, 460),
            new FinancialPoint(new DateTime(2021, 1, 12), 580, 510, 560, 500),
            new FinancialPoint(new DateTime(2021, 1, 13), 600, 560, 540, 510),
            new FinancialPoint(new DateTime(2021, 1, 14), 580, 540, 520, 500),
            new FinancialPoint(new DateTime(2021, 1, 15), 580, 520, 560, 520),
            new FinancialPoint(new DateTime(2021, 1, 16), 590, 560, 580, 520),
            new FinancialPoint(new DateTime(2021, 1, 17), 650, 580, 630, 550),
            new FinancialPoint(new DateTime(2021, 1, 18), 680, 630, 650, 600),
            new FinancialPoint(new DateTime(2021, 1, 19), 670, 650, 600, 570),
            new FinancialPoint(new DateTime(2021, 1, 20), 640, 600, 610, 560),
            new FinancialPoint(new DateTime(2021, 1, 21), 630, 610, 630, 590),
        }
    }
};</code></pre>

## MaxBarWidth property

Specifies the maximum width a column can take, take a look at the following sample, where the max width is `10`.

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/financialw10.png)

<pre><code>XAxes = new[]
{
    new Axis
    {
        LabelsRotation = 15,
        Labeler = value => new DateTime((long)value).ToString("yyyy MMM dd"),
        // set the unit width of the axis to "days"
        // since our X axis is of type date time and 
        // the interval between our points is in days
        UnitWidth = TimeSpan.FromDays(1).Ticks // mark
    }
};

Series = new ISeries[]
{
    new CandlesticksSeries&lt;FinancialPoint>
    {
        Values = new ObservableCollection&lt;FinancialPoint>
        {
            new FinancialPoint(new DateTime(2021, 1, 1), 523, 500, 450, 400),
            new FinancialPoint(new DateTime(2021, 1, 2), 500, 450, 425, 400),
            new FinancialPoint(new DateTime(2021, 1, 3), 490, 425, 400, 380),
            new FinancialPoint(new DateTime(2021, 1, 4), 420, 400, 420, 380),
            new FinancialPoint(new DateTime(2021, 1, 5), 520, 420, 490, 400),
            new FinancialPoint(new DateTime(2021, 1, 6), 580, 490, 560, 440),
            new FinancialPoint(new DateTime(2021, 1, 7), 570, 560, 350, 340),
            new FinancialPoint(new DateTime(2021, 1, 8), 380, 350, 380, 330),
            new FinancialPoint(new DateTime(2021, 1, 9), 440, 380, 420, 350),
            new FinancialPoint(new DateTime(2021, 1, 10), 490, 420, 460, 400)
        }
    }
};</code></pre>

## Plotting custom types

You can teach LiveCharts to plot anything, imagine the case where we have an array of the `Stock` class defined bellow:

<pre><code>public class Stock
{
    public DateTime Date { get; set; }
    public double Open { get; set; }
    public double Close { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
}</code></pre>

You can register this type **globally**, this means that every time LiveCharts finds a `Stock` instance in a chart
it will use the mapper we registered, global mappers are unique for a type, if you need to plot multiple
properties then you should use local mappers.

<pre><code>// Ideally you should call this when your application starts
// If you need help to decide where to add this code
// please see the installation guide in this docs.

// in this case we have an array of the Stock class

LiveCharts.Configure(config =>
    config
        .HasMap&lt;Stock>((stock, point) =>
        {
            // in this lambda function we take an instance of the City class (see city parameter)
            // and the point in the chart for that instance (see point parameter)
            // LiveCharts will call this method for every instance of our City class array,
            // now we need to populate the point coordinates from our City instance to our point

            point.SecondaryValue = stock.Date.Ticks; // use the date for the X axis (secondary)

            // now LiveCharts uses Primary (high), Tertiary (open)
            // Quaternary (close) and Quinary (low) planes to represent
            // a financial point:

            point.PrimaryValue = (float)stock.High;
            point.TertiaryValue = (float)stock.Open;
            point.QuaternaryValue = (float)stock.Close;
            point.QuinaryValue = (float)stock.Low;
        })
        .HasMap&lt;Foo>(...) // you can register more types here using our fluent syntax
        .HasMap&lt;Bar>(...)
    );</code></pre>

Now we are ready to plot stock all over our application:

<pre><code>var stockData = new[]
{
    new Stock
    {
        Date = new DateTime(2021, 1, 1),
        Open = 200f,
        Close = 280f,
        High = 290f,
        Low = 180f
    },
    new Stock
    {
        Date = new DateTime(2021, 1, 2),
        Open = 280f,
        Close = 220f,
        High = 320f,
        Low = 210f
    }
};

XAxes = new[]
{
    // set the UnitWidth to "days" to support date time scaled points.
    new Axis
    {
        UnitWidth = TimeSpan.FromDays(1).Ticks,
        LabelsRotation = 20,
        Labeler = p => new DateTime((long)p).ToShortDateString(),
        MinStep = TimeSpan.FromDays(1).Ticks
    }
};

Series = new[]
{
    new CandlesticksSeries&lt;Stock>
    {
        TooltipLabelFormatter =
            (p) => $"H: {p.PrimaryValue:N2}, O: {p.TertiaryValue:N2}, C: {p.QuaternaryValue:N2}, L: {p.QuinaryValue:N2}",
        Values = stockData
    }
};</code></pre>

![image](https://raw.githubusercontent.com/beto-rodriguez/LiveCharts2/master/docs/_assets/financialct.png)

Alternatively you could create a **local** mapper that will only work for a specific series, global mappers will be 
ignored when the series `Mapping` property is not null.

<pre><code>Series = new[]
{
    new CandlesticksSeries&lt;Stock>
    {
        Mapping = (stock, point) =>
        {
            point.SecondaryValue = stock.Date.Ticks;
            point.PrimaryValue = (float)stock.High;
            point.TertiaryValue = (float)stock.Open;
            point.QuaternaryValue = (float)stock.Close;
            point.QuinaryValue = (float)stock.Low;
        },
        Values = stockData
    }
};</code></pre>