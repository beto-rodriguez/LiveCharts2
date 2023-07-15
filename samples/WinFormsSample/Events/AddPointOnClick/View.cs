// NOTE: // mark
// BECAUSE THIS VIEWMODEL IS SHARED WITH OTHER VIEWS // mark
// THE _viewModel.ChartUpdated, _viewModel.PointerDown and _viewModel.PointerUp METHODS // mark
// are repeated in WindowsForms, WindowsForms do not support Command binding, please // mark
// ignore the viewmodel RelayCommands and use the events instead. // mark

using System.Collections.ObjectModel;
using System.Windows.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Events.AddPointOnClick;

namespace WinFormsSample.Events.AddPointOnClick;

public partial class View : UserControl
{
    private readonly ObservableCollection<ObservablePoint> _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        _data = (ObservableCollection<ObservablePoint>)viewModel.SeriesCollection[0].Values;

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.SeriesCollection,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        cartesianChart.MouseDown += CartesianChart_Click;

        Controls.Add(cartesianChart);
    }

    private void CartesianChart_Click(object sender, MouseEventArgs e)
    {
        var chart = (CartesianChart)sender;

        // scales the UI coordinates to the corresponding data in the chart.
        var dataCoordinates = chart.ScalePixelsToData(new LvcPointD(e.Location.X, e.Location.Y));

        // finally add the new point to the data in our chart.
        _data.Add(new ObservablePoint(dataCoordinates.X, dataCoordinates.Y));

        // You can also get all the points or visual elements in a given location.
        var points = chart.GetPointsAt(new LvcPoint(e.Location.X, e.Location.Y));
        var visuals = chart.GetVisualsAt(new LvcPoint(e.Location.X, e.Location.Y));
    }
}
