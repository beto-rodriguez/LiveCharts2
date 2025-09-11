using System;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;
using ViewModelsSamples.General.DrawOnCanvas;

namespace WinFormsSample.General.DrawOnCanvas;

public partial class View : UserControl
{
    private readonly MotionGeometry _geometry;
    private bool _isBig;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(400, 400);

        var cartesianChart = new CartesianChart
        {
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(400, 400),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        _geometry = new MotionGeometry
        {
            Diameter = 20,
            Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(100)),
            Stroke = new SolidColorPaint(SKColors.Blue, strokeWidth: 3)
        };

        cartesianChart.UpdateStarted += chart =>
        {
            // Place the geometry at (5, 5) in chart value scale
            var locationInChartValues = new LvcPointD(5, 5);
            var locationInPixels = cartesianChart.ScaleDataToPixels(locationInChartValues);
            _geometry.X = (float)locationInPixels.X;
            _geometry.Y = (float)locationInPixels.Y;
            _geometry.Initialize(new(chart));
        };

        cartesianChart.MouseDown += (s, e) =>
        {
            _geometry.Diameter = _isBig ? 20 : 70;
            _isBig = !_isBig;
            cartesianChart.Invalidate();
        };

        Controls.Add(cartesianChart);
    }
}
