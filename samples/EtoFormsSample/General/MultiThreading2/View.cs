using System;
using System.Linq;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.MultiThreading2;

namespace EtoFormsSample.General.MultiThreading2;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        cartesianChart = new CartesianChart
        {
            Series = Enumerable.Empty<ISeries>(),
        };

        Content = cartesianChart;
    }
    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);

        var viewModel = new ViewModel(InvokeOnUIThread);
        cartesianChart.Series = viewModel.Series;
    }

    // this method takes another function as an argument.
    // the idea is that we are invoking the passed action in the UI thread
    // but the UI framework will let the view model how to do this.
    // we will pass the InvokeOnUIThread method to our view model so the view model knows how
    // to invoke an action in the UI thred.
    private void InvokeOnUIThread(Action action)
    {
        Application.Instance.InvokeAsync(action);
    }
}
