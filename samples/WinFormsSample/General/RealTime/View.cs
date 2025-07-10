using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;

namespace WinFormsSample.General.RealTime;

public partial class View : UserControl
{
    private readonly ObservableCollection<DateTimePoint> _values;
    private readonly object _sync = new();
    private bool _isReading = true;
    private double[] _separators = [];
    private readonly CartesianChart _cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(600, 400);

        _values = [];

        var seriesColection = new ISeries[]
        {
            new LineSeries<DateTimePoint>
            {
                Values = _values,
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            }
        };

        var xAxis = new Axis
        {
            Labeler = value => Formatter(new DateTime((long)value)),
            AnimationsSpeed = TimeSpan.FromMilliseconds(0),
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray),
            CustomSeparators = _separators
        };

        _cartesianChart = new CartesianChart
        {
            Series = seriesColection,
            XAxes = [xAxis],
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(600, 400),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(_cartesianChart);
        _ = ReadData(xAxis);
    }

    private async Task ReadData(Axis xAxis)
    {
        var random = new Random();
        while (_isReading)
        {
            await Task.Delay(100);
            lock (_sync)
            {
                _values.Add(new DateTimePoint(DateTime.Now, random.Next(0, 10)));
                if (_values.Count > 250) _values.RemoveAt(0);
                _separators = GetSeparators();
                xAxis.CustomSeparators = _separators;
            }
        }
    }

    private static double[] GetSeparators()
    {
        var now = DateTime.Now;
        return [
            now.AddSeconds(-25).Ticks,
            now.AddSeconds(-20).Ticks,
            now.AddSeconds(-15).Ticks,
            now.AddSeconds(-10).Ticks,
            now.AddSeconds(-5).Ticks,
            now.Ticks
        ];
    }

    private static string Formatter(DateTime date)
    {
        var secsAgo = (DateTime.Now - date).TotalSeconds;
        return secsAgo < 1 ? "now" : $"{secsAgo:N0}s ago";
    }
}
