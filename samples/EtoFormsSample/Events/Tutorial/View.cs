using System.Collections.Generic;
using Eto.Forms;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Events.Tutorial;

namespace EtoFormsSample.Events.Tutorial;

public class View : Panel
{
    private ViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        _viewModel = new ViewModel();

        var cartesianChart = new CartesianChart
        {
            Series = _viewModel.SeriesCollection,
            FindingStrategy = _viewModel.Strategy
        };

        cartesianChart.DataPointerDown += CartesianChart_DataPointerDown;

        Content = cartesianChart;
    }

    private void CartesianChart_DataPointerDown(IChartView chart, IEnumerable<ChartPoint> points) =>
        _viewModel.OnDataDown(points);
}
