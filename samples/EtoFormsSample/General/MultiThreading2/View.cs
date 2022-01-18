using System;
using System.Linq;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.General.MultiThreading2;

namespace WinFormsSample.General.MultiThreading2;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        cartesianChart = new CartesianChart
        {
            Series = Enumerable.Empty<ISeries>(),

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

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
        _ = BeginInvoke(action);
    }
}
