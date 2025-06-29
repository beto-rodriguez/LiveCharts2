using System;
using System.Windows.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.WinForms;

namespace WinFormsSample.Pies.AngularGauge;

public partial class View : UserControl
{
    private readonly PieChart pieChart;
    private readonly Random _random = new();

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var sectionsOuter = 130;
        var sectionsWidth = 20;

        var needle = new NeedleVisual
        {
            Value = 45
        };

        pieChart = new PieChart
        {
            Series = GaugeGenerator.BuildAngularGaugeSections(
                new GaugeItem(60, s => SetStyle(sectionsOuter, sectionsWidth, s)),
                new GaugeItem(30, s => SetStyle(sectionsOuter, sectionsWidth, s)),
                new GaugeItem(10, s => SetStyle(sectionsOuter, sectionsWidth, s))),
            VisualElements = [
                new AngularTicksVisual
                {
                    Labeler = value => value.ToString("N1"),
                    LabelsSize = 16,
                    LabelsOuterOffset = 15,
                    OuterOffset = 65,
                    TicksLength = 20
                },
                needle
            ],
            InitialRotation = -225,
            MaxAngle = 270,
            MinValue = 0,
            MaxValue = 100,
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(pieChart);

        var b1 = new Button { Text = "Update", Location = new System.Drawing.Point(0, 0) };
        b1.Click += (sender, e) => needle.Value = _random.Next(0, 100);
        Controls.Add(b1);
        b1.BringToFront();
    }

    private static void SetStyle(
        double sectionsOuter, double sectionsWidth, PieSeries<ObservableValue> series)
    {
        series.OuterRadiusOffset = sectionsOuter;
        series.MaxRadialColumnWidth = sectionsWidth;
        series.CornerRadius = 0;
    }
}
