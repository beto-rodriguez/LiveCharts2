using System.Collections.ObjectModel;
using Eto.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Events.AddPointOnClick;

namespace EtoFormsSample.Events.AddPointOnClick;

public class View : Panel
{
    private readonly ObservableCollection<ObservablePoint> _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        var viewModel = new ViewModel();

        _data = viewModel.Data;

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.SeriesCollection,
        };

        cartesianChart.MouseDown += CartesianChart_Click;

        Content = cartesianChart;
    }

    private void CartesianChart_Click(object sender, MouseEventArgs e)
    {
        var chart = (CartesianChart)sender;

        // scales the UI coordinates to the corresponding data in the chart.
        // ScaleUIPoint returns an array of double
        var scaledPoint = chart.ScaleUIPoint(new LvcPoint(e.Location.X, e.Location.Y));

        // where the X coordinate is in the first position
        var x = scaledPoint[0];

        // and the Y coordinate in the second position
        var y = scaledPoint[1];

        // finally add the new point to the data in our chart.
        _data.Add(new ObservablePoint(x, y));
    }
}
