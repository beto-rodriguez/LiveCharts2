using System;
using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.General.Animations;

namespace EtoFormsSample.General.Animations;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,

            // there are already many defined functions in the EasingFunctions static class
            EasingFunction = LiveChartsCore.EasingFunctions.BackOut,
            AnimationsSpeed = TimeSpan.FromMilliseconds(600),
        };

        Content = cartesianChart;
    }
}
