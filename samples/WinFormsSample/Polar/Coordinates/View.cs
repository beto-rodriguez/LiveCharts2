using System;
using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Defaults;

namespace WinFormsSample.Polar.Coordinates;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var values = new ObservablePolarPoint[]
        {
            new(0, 10),
            new(45, 15),
            new(90, 20),
            new(135, 25),
            new(180, 30),
            new(225, 35),
            new(270, 40),
            new(315, 45),
            new(360, 50)
        };

        var series = new ISeries[]
        {
            new PolarLineSeries<ObservablePolarPoint>
            {
                Values = values,
                IsClosed = true,
                Fill = null
            }
        };

        var angleAxes = new PolarAxis[]
        {
            new() {
                MinLimit = 0,
                MaxLimit = 360,
                Labeler = angle => $"{angle}°",
                ForceStepToMin = true,
                MinStep = 30
            }
        };

        var polarChart = new PolarChart
        {
            Series = series,
            AngleAxes = angleAxes,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(polarChart);
    }
}
