using System.Collections.Generic;
using Eto.Forms;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Events.Tutorial;

namespace EtoFormsSample.Events.Tutorial;

public class View : Panel
{
    private ViewModel _viewModel;
    private CartesianChart _cartesianChart;

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        _viewModel = new ViewModel();

        _cartesianChart = new CartesianChart
        {
            Series = _viewModel.SeriesCollection,
            FindingStrategy = _viewModel.Strategy
        };

        _cartesianChart.MouseDown += CartesianChart_MouseDown;

        Content = _cartesianChart;
    }

    private void CartesianChart_MouseDown(object sender, MouseEventArgs e) =>
        _viewModel.OnPressed(
            new PointerCommandArgs(
                _cartesianChart,
                new(e.Location.X, e.Location.Y),
                e));
}
