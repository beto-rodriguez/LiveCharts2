using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Error.Basic;

public partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        // use the ErrorValue type to define the error in Y // mark
        var values0 = new ErrorValue[]
        {
            // (Y, Y+- error) // mark
            new(65, 6),
            // (Y, Y+ error, Y- error) // mark
            new(70, 15, 4),
            new(35, 4),
            new(70, 6),
            new(30, 5),
            new(60, 4, 16),
            new(65, 6)
        };

        // When you need to define the error in X and Y use the ErrorPoint type // mark
        var values1 = new ErrorPoint[]
        {
            // (X, Y, Y+- error, Y+- error) // mark
            new(0, 50, 0.2, 8),
            // (X, Y, X- error, X+ erorr, Y+ error, Y- error) // mark
            new(1, 45, 0.1, 0.3, 15, 4),
            new(2, 25, 0.3, 4),
            new(3, 30, 0.2, 6),
            new(4, 70, 0.2, 8),
            new(5, 30, 0.4, 4),
            new(6, 50, 0.3, 6)
        };

        Series0 = new ISeries[]
        {
            new ColumnSeries<ErrorValue>
            {
                Values = values0,
                ErrorPaint = new SolidColorPaint(SKColors.Black),
                Padding = 0
            },
            new ColumnSeries<ErrorPoint>
            {
                Values = values1,
                ErrorPaint = new SolidColorPaint(SKColors.Black),
                Padding = 0
            }
        };

        // LineSeries also supports error bars // mark
        Series1 = new ISeries[]
        {
            new LineSeries<ErrorValue, RectangleGeometry>
            {
                Values = values0,
                ErrorPaint = new SolidColorPaint(SKColors.Black),
                GeometrySize = 4,
                Fill = null
            }
        };

        // You can also use DateTime on the X axis // mark
        var now = DateTime.Now;

        var values2 = new ErrorDateTimePoint[]
        {
            // (X, Y, Y+- error, Y+- error) // mark
            new(now.AddDays(0), 50, 0.2, 8),
            // (X, Y, X- error, X+ erorr, Y+ error, Y- error) // mark
            new(now.AddDays(1), 45, 0.1, 0.3, 15, 4),
            new(now.AddDays(2), 25, 0.3, 4),
            new(now.AddDays(3), 30, 0.2, 6),
            new(now.AddDays(4), 70, 0.2, 8),
            new(now.AddDays(5), 30, 0.4, 4),
            new(now.AddDays(6), 50, 0.3, 6)
        };

        // ScatterSeries also supports error bars // mark
        Series2 = new ISeries[]
        {
            new ScatterSeries<ErrorDateTimePoint>
            {
                Values = values2,
                ErrorPaint = new SolidColorPaint(SKColors.Black),
                GeometrySize = 10
            }
        };
    }

    public ISeries[] Series0 { get; set; }

    public ISeries[] Series1 { get; set; }

    public ISeries[] Series2 { get; set; }

    // To get more help about DateTime axes see:
    // https://livecharts.dev/docs/{{ platform }}/{{ version }}/samples.axes.dateTimeScaled
    public Axis[] DateTimeAxis { get; set; } =
    {
        new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd"))
    };
}
