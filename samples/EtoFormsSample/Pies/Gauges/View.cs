using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
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
            Total = viewModel.GaugeTotal1,
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series2,
            Total = viewModel.GaugeTotal2,
            InitialRotation = viewModel.InitialRotation2
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series3,
            Total = viewModel.GaugeTotal3,
            InitialRotation = viewModel.InitialRotation3
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series4,
            Total = viewModel.GaugeTotal4,
            InitialRotation = viewModel.InitialRotation4
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series5,
            Total = viewModel.GaugeTotal5,
            InitialRotation = viewModel.InitialRotation5
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series6,
            Total = viewModel.GaugeTotal6,
            InitialRotation = viewModel.InitialRotation6
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series7,
            Total = viewModel.GaugeTotal7,
            InitialRotation = viewModel.InitialRotation7
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series8,
            Total = viewModel.GaugeTotal8,
            InitialRotation = viewModel.InitialRotation8,
            MaxAngle = viewModel.MaxAngle8
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series9,
            Total = viewModel.GaugeTotal9,
            InitialRotation = viewModel.InitialRotation9,
            MaxAngle = viewModel.MaxAngle9
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series10,
            Total = viewModel.GaugeTotal10,
            InitialRotation = viewModel.InitialRotation10,
            MaxAngle = viewModel.MaxAngle10
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series11,
            Total = viewModel.GaugeTotal11,
            InitialRotation = viewModel.InitialRotation11,
            MaxAngle = viewModel.MaxAngle11
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series12,
            Total = viewModel.GaugeTotal12,
            InitialRotation = viewModel.InitialRotation12,
            MaxAngle = viewModel.MaxAngle12
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series13,
            Total = viewModel.GaugeTotal3,
            InitialRotation = viewModel.InitialRotation13,
            MaxAngle = viewModel.MaxAngle13
        });
        flowLayoutPanel1.Items.Add(new PieChart
        {
            Height = 400,
            Width = 400,
            Series = viewModel.Series14,
            Total = viewModel.GaugeTotal14,
            InitialRotation = viewModel.InitialRotation14,
            MaxAngle = viewModel.MaxAngle14
        });
    }
}
