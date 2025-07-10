using System;
using System.Windows.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace WinFormsSample.Pies.Gauge5;

public partial class View : UserControl
{
    private readonly PieChart pieChart;
    private readonly Random _random = new();

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var observableValue1 = new ObservableValue { Value = 50 };
        var observableValue2 = new ObservableValue { Value = 80 };

        pieChart = new PieChart
        {
            Series = GaugeGenerator.BuildSolidGauge(
                new GaugeItem(observableValue1, series =>
                {
                    series.Name = "North";
                    series.DataLabelsPosition = PolarLabelsPosition.Start;
                }),
                new GaugeItem(observableValue2, series =>
                {
                    series.Name = "South";
                    series.DataLabelsPosition = PolarLabelsPosition.Start;
                })),
            InitialRotation = -90,
            MaxAngle = 270,
            MinValue = 0,
            MaxValue = 100,
            LegendPosition = LegendPosition.Bottom,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(pieChart);

        var b1 = new Button { Text = "Update", Location = new System.Drawing.Point(0, 0) };
        b1.Click += (sender, e) =>
        {
            observableValue1.Value = _random.Next(0, 100);
            observableValue2.Value = _random.Next(0, 100);
        };
        Controls.Add(b1);
        b1.BringToFront();
    }
}
