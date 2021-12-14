## Data labels 

Data labels are labels for every point in a series, there are multiple properties to customize them, take a look at the 
following sample:

<pre><code>new {{ name  | to_title_case_no_spaces }}&lt;double>
{
    DataLabelsSize = 20,
    DataLabelsPaint = new SolidColorPaint(SKColors.Blue),
    // all the available positions at:
    // {{ website_url }}/api/{{ version }}/LiveChartsCore.Measure.DataLabelsPosition
    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
    // The DataLabelsFormatter is a function 
    // that takes the current point as parameter
    // and returns a string.
    // in this case we returned the PrimaryValue property as currency
    DataLabelsFormatter = (point) => point.PrimaryValue.ToString("C2"),
    Values = new ObservableCollection&lt;double> { 2, 1, 3, 5, 3, 4, 6 },
    Fill = null
}</code></pre>

The previous series will result in the following chart:

![image]({{ assets_url }}/docs/_assets/1.8.datalabels.png)
