using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Pies.Gauges;

namespace WinFormsSample.Pies.Gauges;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        var flowLayoutPanel1 = new FlowLayoutPanel
        {
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Dock = DockStyle.Fill,
            AutoScroll = true
        };
        Controls.Add(flowLayoutPanel1);

        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 0),
            Height = 400,
            Width = 400,
            Series = viewModel.Series1,
            MaxValue = viewModel.GaugeTotal1,
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 300),
            Height = 400,
            Width = 400,
            Series = viewModel.Series2,
            MaxValue = viewModel.GaugeTotal2,
            InitialRotation = viewModel.InitialRotation2
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 600),
            Height = 400,
            Width = 400,
            Series = viewModel.Series3,
            MaxValue = viewModel.GaugeTotal3,
            InitialRotation = viewModel.InitialRotation3
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 900),
            Height = 400,
            Width = 400,
            Series = viewModel.Series4,
            MaxValue = viewModel.GaugeTotal4,
            InitialRotation = viewModel.InitialRotation4
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 1200),
            Height = 400,
            Width = 400,
            Series = viewModel.Series5,
            MaxValue = viewModel.GaugeTotal5,
            InitialRotation = viewModel.InitialRotation5
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 1500),
            Height = 400,
            Width = 400,
            Series = viewModel.Series6,
            MaxValue = viewModel.GaugeTotal6,
            InitialRotation = viewModel.InitialRotation6
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 1800),
            Height = 400,
            Width = 400,
            Series = viewModel.Series7,
            MaxValue = viewModel.GaugeTotal7,
            InitialRotation = viewModel.InitialRotation7
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 2100),
            Height = 400,
            Width = 400,
            Series = viewModel.Series8,
            MaxValue = viewModel.GaugeTotal8,
            InitialRotation = viewModel.InitialRotation8,
            MaxAngle = viewModel.MaxAngle8
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 2400),
            Height = 400,
            Width = 400,
            Series = viewModel.Series9,
            MaxValue = viewModel.GaugeTotal9,
            InitialRotation = viewModel.InitialRotation9,
            MaxAngle = viewModel.MaxAngle9
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 2700),
            Height = 400,
            Width = 400,
            Series = viewModel.Series10,
            MaxValue = viewModel.GaugeTotal10,
            InitialRotation = viewModel.InitialRotation10,
            MaxAngle = viewModel.MaxAngle10
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 3000),
            Height = 400,
            Width = 400,
            Series = viewModel.Series11,
            MaxValue = viewModel.GaugeTotal11,
            InitialRotation = viewModel.InitialRotation11,
            MaxAngle = viewModel.MaxAngle11
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 3300),
            Height = 400,
            Width = 400,
            Series = viewModel.Series12,
            MaxValue = viewModel.GaugeTotal12,
            InitialRotation = viewModel.InitialRotation12,
            MaxAngle = viewModel.MaxAngle12
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 3600),
            Height = 400,
            Width = 400,
            Series = viewModel.Series13,
            MaxValue = viewModel.GaugeTotal3,
            InitialRotation = viewModel.InitialRotation13,
            MaxAngle = viewModel.MaxAngle13
        });
        flowLayoutPanel1.Controls.Add(new PieChart
        {
            Location = new System.Drawing.Point(0, 3900),
            Height = 400,
            Width = 400,
            Series = viewModel.Series14,
            MaxValue = viewModel.GaugeTotal14,
            InitialRotation = viewModel.InitialRotation14,
            MaxAngle = viewModel.MaxAngle14
        });
    }
}
