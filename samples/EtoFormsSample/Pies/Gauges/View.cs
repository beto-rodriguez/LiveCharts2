using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Pies.Gauges;

namespace EtoFormsSample.Pies.Gauges;

public class View : Panel
{
    public View()
    {
        var viewModel = new ViewModel();

        var flowLayoutPanel1 = new StackLayout
        {
            Orientation= Orientation.Vertical
        };
        Content = new Scrollable { Content = flowLayoutPanel1 };

        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series1,
            MaxValue = viewModel.GaugeTotal1,
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series2,
            MaxValue = viewModel.GaugeTotal2,
            InitialRotation = viewModel.InitialRotation2
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series3,
            MaxValue = viewModel.GaugeTotal3,
            InitialRotation = viewModel.InitialRotation3
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series4,
            MaxValue = viewModel.GaugeTotal4,
            InitialRotation = viewModel.InitialRotation4
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series5,
            MaxValue = viewModel.GaugeTotal5,
            InitialRotation = viewModel.InitialRotation5
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series6,
            MaxValue = viewModel.GaugeTotal6,
            InitialRotation = viewModel.InitialRotation6
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series7,
            MaxValue = viewModel.GaugeTotal7,
            InitialRotation = viewModel.InitialRotation7
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series8,
            MaxValue = viewModel.GaugeTotal8,
            InitialRotation = viewModel.InitialRotation8,
            MaxAngle = viewModel.MaxAngle8
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series9,
            MaxValue = viewModel.GaugeTotal9,
            InitialRotation = viewModel.InitialRotation9,
            MaxAngle = viewModel.MaxAngle9
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series10,
            MaxValue = viewModel.GaugeTotal10,
            InitialRotation = viewModel.InitialRotation10,
            MaxAngle = viewModel.MaxAngle10
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series11,
            MaxValue = viewModel.GaugeTotal11,
            InitialRotation = viewModel.InitialRotation11,
            MaxAngle = viewModel.MaxAngle11
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series12,
            MaxValue = viewModel.GaugeTotal12,
            InitialRotation = viewModel.InitialRotation12,
            MaxAngle = viewModel.MaxAngle12
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series13,
            MaxValue = viewModel.GaugeTotal3,
            InitialRotation = viewModel.InitialRotation13,
            MaxAngle = viewModel.MaxAngle13
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series14,
            MaxValue = viewModel.GaugeTotal14,
            InitialRotation = viewModel.InitialRotation14,
            MaxAngle = viewModel.MaxAngle14
        });
    }
}
