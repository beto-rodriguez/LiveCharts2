using System;
using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
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
        };

        // disabled for now
        //var binding1 = Binding.Delegate(((string, Func<float, float>) c) => c.Item1);
        //var b1 = new DropDown() { DataStore = viewModel.AvalaibaleCurves, ItemTextBinding = binding1 };
        //b1.SelectedValueChanged += (object sender, System.EventArgs e) =>
        //{
        //    viewModel.SelectedCurve = b1.SelectedValue as AvailableEasingCurve;
        //};
        //b1.SelectedIndex = 0;

        //var binding2 = Binding.Delegate((AvailableSpeed c) => c.Name);
        //var b2 = new DropDown() { DataStore = viewModel.AvailableSpeeds, ItemTextBinding = binding2 };
        //b2.SelectedValueChanged += (object sender, System.EventArgs e) =>
        //{
        //    viewModel.SelectedSpeed = b2.SelectedValue as AvailableSpeed;
        //};
        //b2.SelectedIndex = 2;

        //Content = new DynamicLayout(
        //    new DynamicRow(new DynamicControl() { Control = b1, XScale = true }),
        //    new DynamicRow(new DynamicControl() { Control = b2, XScale = true }),
        //    cartesianChart);
    }
}
