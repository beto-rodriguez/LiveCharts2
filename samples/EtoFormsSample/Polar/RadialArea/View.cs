using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Polar.RadialArea;

namespace EtoFormsSample.Polar.RadialArea;

public partial class View : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        var polarChart = new PolarChart
        {
            Series = viewModel.Series,
            AngleAxes = viewModel.AngleAxes,
            InitialRotation = -45,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(polarChart);
    }
}
