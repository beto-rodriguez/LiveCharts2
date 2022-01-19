using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Axes.ColorsAndPosition;

namespace EtoFormsSample.Axes.ColorsAndPosition;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(100, 100);

        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            XAxes = viewModel.XAxes,
            YAxes = viewModel.YAxes,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 50),
            Size = new Eto.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

        var b1 = new Button { Text = "toggle position", Location = new Eto.Drawing.Point(0, 0) };
        b1.Click += (object sender, System.EventArgs e) => viewModel.TogglePosition();
        Controls.Add(b1);

        var b2 = new Button { Text = "new color", Location = new Eto.Drawing.Point(80, 0) };
        b2.Click += (object sender, System.EventArgs e) => viewModel.SetNewColor();
        Controls.Add(b2);
    }
}
