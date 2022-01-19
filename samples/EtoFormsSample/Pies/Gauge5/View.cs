using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Pies.Gauge5;

namespace EtoFormsSample.Pies.Gauge5;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        pieChart = new PieChart
        {
            Series = viewModel.Series,
            InitialRotation = -90,
            MaxAngle = 270,
            Total = 100,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(pieChart);

        var b1 = new Button { Text = "Update", Location = new Eto.Drawing.Point(0, 0) };
        b1.Click += (object sender, System.EventArgs e) => viewModel.DoRandomChange();
        Controls.Add(b1);
        b1.BringToFront();
    }
}
