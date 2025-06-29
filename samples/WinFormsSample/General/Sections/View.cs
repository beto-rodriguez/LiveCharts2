using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using SkiaSharp;

namespace WinFormsSample.General.Sections;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        // Hard-coded values from WinUI sample
        var values = new ObservablePoint[]
        {
            new(2.2, 5.4),
            new(4.5, 2.5),
            new(4.2, 7.4),
            new(6.4, 9.9),
            new(8.9, 3.9),
            new(9.9, 5.2)
        };

        var series = new ISeries[]
        {
            new ScatterSeries<ObservablePoint>
            {
                Values = values
            }
        };

        var sections = new RectangularSection[]
        {
            // Section from 3 to 4 in X axis
            new() {
                Xi = 3,
                Xj = 4,
                Fill = new SolidColorPaint(SKColor.Parse("#FFCDD2"))
            },
            // Section from 5 to 6 in X axis and 2 to 8 in Y axis
            new() {
                Xi = 5,
                Xj = 6,
                Yi = 2,
                Yj = 8,
                Fill = new SolidColorPaint(SKColor.Parse("#BBDEFB"))
            },
            // Section from 8 to end in X axis
            new() {
                Xi = 8,
                Label = "A section here!",
                LabelSize = 14,
                LabelPaint = new SolidColorPaint(SKColor.Parse("#FF6F00")),
                Fill = new SolidColorPaint(SKColor.Parse("#F9FBE7"))
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            Sections = sections,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
