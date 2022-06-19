﻿using System.Threading.Tasks;
using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Bars.Race;

namespace EtoFormsSample.Bars.Race;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;
    private readonly ViewModel viewModel;

    public View()
    {
        viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            XAxes = viewModel.XAxes,
            YAxes = viewModel.YAxes,
            TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Hidden
        };

        Content = cartesianChart;

        UpdateViewModel();

        UnLoad += (o, e) => Visible = false;
    }

    public async void UpdateViewModel()
    {
        while (Visible)
        {
            viewModel.RandomIncrement();
            cartesianChart.Series = viewModel.Series;
            await Task.Delay(100);
        }
    }
}
