:::info
This article do not include all the properties of the {{ name | to_title_case }} class, it only highlights some features, to
explore the full object checkout the [API explorer]({{ website_url }}/api/{{ version }}/LiveChartsCore.SkiaSharpView.{{ name  | to_title_case_no_spaces }}-3)
:::

## Name property

The name property is a string identifier that is normally used in tooltips and legends to display the data name,
if this property is not set, then the library will generate a name for the series that by default is called 
"Series 1" when it is the first series in the series collection, "Series 2" when it is the second series in the 
series collection, "Series 3" when it is the third series in the series collection, and so on a series *n* will be 
named "Series *n*".

<pre><code>SeriesCollection = new ISeries[]
{
    new {{ name  | to_title_case_no_spaces }}&lt;int>
    {
        Values = new []{ 2, 5, 4, 2, 6 },
        Name = "Income", // mark
        Stroke = null
    },
    new {{ name | to_title_case_no_spaces }}&lt;int>
    {
        Values = new []{ 3, 7, 2, 9, 4 },
        Name = "Outcome", // mark
        Stroke = null
    }
};
</code></pre>

## Values property

The `Values` property is of type `IEnumerable<T>`, this means that you can use any object that implements the `IEnumerable<T>` interface, 
such as `Array`, `List<T>` or `ObservableCollection<T>`, this property contains the data to plot, you can use any type as the
generic argument (`<T>`) as soon as you let the library how to handle it, the library already knows how to handle multiple types, 
but you can register any type and teach the library how to handle any object in a chart, for more information please see the 
[mappers article]({{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Mappers).

<pre><code>var series1 = new {{ name  | to_title_case_no_spaces }}&lt;int>
{
    Values = new List&lt;int> { 2, 1, 3 }
};

// == Update the chart when a value is added, removed or replaced  == // mark
// using ObservableCollections allows the chart to update
// every time you add a new element to the values collection
// (not needed in Blazor, it just... updates)
var series2 = new {{ name  | to_title_case_no_spaces }}&lt;double>
{
    Values = new ObservableCollection&lt;double> { 2, 1, 3 }
}
series2.add(4); // and the chart will animate the change!

// == Update the chart when a property in our collection changes  == // mark
// if the object implements INotifyPropertyChanged, then the chart will
// update automatically when a property changes, the library already provides
// many 'ready to go' objects such as the ObservableValue class.
var observableValue =  new ObservableValue(5);
var series3 = new {{ name  | to_title_case_no_spaces }}&lt;ObservableValue>
{
    Values = new ObservableCollection&lt;ObservableValue> { observableValue },
}
observableValue.Value = 9; // the chart will animate the change from 5 to 9!

// == Passing X and Y coordinates // mark 
// you can indicate both, X and Y using the Observable point class.
// or you could define your own object using mappers.
var series4 = new {{ name  | to_title_case_no_spaces }}&lt;ObservablePoint>
{
    Values = new ObservableCollection&lt;ObservablePoint> { new ObservablePoint(2, 6)}
}</code></pre>

<pre><code>// == Custom types and mappers == // mark
// finally you can also use your own object, take a look at the City class.
public class City 
{
    public string Name { get; set; }
    public double Population { get; set; }
}</code></pre>

<pre><code>// we must let the series know how to handle the city class.
// use the Mapping property to build a point from the city class
// you could also register the map globally.
// for more about global mappers info see:
// {{ website_url }}/docs/{{ platform }}/{{ version }}/Overview.Mappers
var citiesSeries = new {{ name  | to_title_case_no_spaces }}&lt;City>
{
    Values = new City[]
    { 
        new City { Name = "Tokio", Population = 9 },
        new City { Name = "New York", Population = 11 },
        new City { Name = "Mexico City", Population = 10 },
    },
    Mapping = (city, point) =>
    {
        // this function will be called for every city in our data collection
        // in this case Tokio, New York and Mexico city
        // it takes the city and the point in the chart liveCharts built for the given city
        // you must map the coordinates to the point

        // use the Population property as the primary value (normally Y)
        point.PrimaryValue = (float)city.Population;

        // use the index of the city in our data collection as the secondary value
        // (normally X)
        point.SecondaryValue = point.Context.Index;
    }
};</code></pre>

:::info
Automatic updates do not have a significant performance impact in most of the cases!
:::
