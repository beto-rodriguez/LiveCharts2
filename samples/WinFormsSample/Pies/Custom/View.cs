using System;
using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;

namespace WinFormsSample.Pies.Custom;

public partial class View : UserControl
{
    private readonly PieChart pieChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var outer = 0;
        var data = new[] { 6, 5, 4, 3 };

        // you can convert any array, list or IEnumerable<T> to a pie series collection:
        var seriesCollection = data.AsPieSeries((value, series) =>
        {
            // this method is called once per element in the array, so:

            // for the series with the value 6, we set the OuterRadiusOffset to 0
            // for the series with the value 5, the OuterRadiusOffset is 50
            // for the series with the value 4, the OuterRadiusOffset is 100
            // for the series with the value 3, the OuterRadiusOffset is 150

            series.OuterRadiusOffset = outer;
            outer += 50;

            series.DataLabelsPaint = new SolidColorPaint(SKColors.White)
            {
                SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            };

            series.ToolTipLabelFormatter =
                point =>
                {
                    var pv = point.Coordinate.PrimaryValue;
                    var sv = point.StackedValue!;

                    var a = $"{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                    return a;
                };

            series.DataLabelsFormatter =
                point =>
                {
                    var pv = point.Coordinate.PrimaryValue;
                    var sv = point.StackedValue!;

                    var a = $"{pv}/{sv.Total}{Environment.NewLine}{sv.Share:P2}";
                    return a;
                };
        });

        pieChart = new PieChart
        {
            Series = seriesCollection,
            InitialRotation = -90,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(pieChart);
    }
}
