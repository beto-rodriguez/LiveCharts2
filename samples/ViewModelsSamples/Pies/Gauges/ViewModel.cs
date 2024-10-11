using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace ViewModelsSamples.Pies.Gauges;

public class ViewModel
{
    public ViewModel()
    {
        GaugeTotal1 = 100;
        Series1 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.DataLabelsSize = 80;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.Fill = new RadialGradientPaint(
                    new SKColor(167, 192, 205, 0),
                    new SKColor(167, 192, 205));
            }));

        GaugeTotal2 = 100;
        InitialRotation2 = -90;
        Series2 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.DataLabelsSize = 80;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.Fill = new RadialGradientPaint(
                    new SKColor(167, 192, 205, 0),
                    new SKColor(167, 192, 205));
            }));

        GaugeTotal3 = 100;
        InitialRotation3 = -90;
        Series3 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.DataLabelsSize = 60;
                series.InnerRadius = 50;
            }));

        GaugeTotal4 = 100;
        InitialRotation4 = -90;
        Series4 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.DataLabelsSize = 50;
                series.InnerRadius = 50;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 50;
                series.RelativeInnerRadius = 10;
                series.RelativeOuterRadius = 10;
            }));

        GaugeTotal5 = 100;
        InitialRotation5 = -90;
        Series5 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.DataLabelsSize = 50;
                series.InnerRadius = 50;
                series.RelativeOuterRadius = 10;
                series.RelativeInnerRadius = 10;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 50;
            }));

        GaugeTotal6 = 100;
        InitialRotation6 = -90;
        Series6 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.Fill = new SolidColorPaint(new SKColor(21, 101, 192));
                series.DataLabelsSize = 50;
                series.InnerRadius = 50;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 50;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246));
            }));

        GaugeTotal7 = 100;
        InitialRotation7 = -90;
        Series7 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.Fill = new SolidColorPaint(new SKColor(21, 101, 192));
                series.DataLabelsSize = 50;
                series.InnerRadius = 75;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 50;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));

        GaugeTotal8 = 100;
        InitialRotation8 = -225;
        MaxAngle8 = 270;
        Series8 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.Fill = new SolidColorPaint(new SKColor(21, 101, 192));
                series.DataLabelsSize = 50;
                series.InnerRadius = 75;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 50;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));

        GaugeTotal9 = 100;
        InitialRotation9 = 315;
        MaxAngle9 = 270;
        Series9 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.Fill = new SolidColorPaint(new SKColor(21, 101, 192));
                series.DataLabelsSize = 50;
                series.InnerRadius = 75;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 50;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));

        GaugeTotal10 = 100;
        InitialRotation10 = -200;
        MaxAngle10 = 220;
        Series10 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.Fill = new SolidColorPaint(new SKColor(30, 33, 45));
                series.DataLabelsSize = 50;
                series.InnerRadius = 75;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 50;
                series.Fill = new LinearGradientPaint(
                    new SKColor(250, 243, 224),
                    new SKColor(182, 137, 115));
            }));

        GaugeTotal11 = 100;
        InitialRotation11 = -90;
        MaxAngle11 = 270;

        void SetStyle11(string name, PieSeries<ObservableValue> series)
        {
            series.Name = name;
            series.DataLabelsPosition = PolarLabelsPosition.Start;
            series.DataLabelsFormatter =
                point => $"{point.Context.Series.Name} {point.Coordinate.PrimaryValue}";
            series.DataLabelsSize = 20;
            series.InnerRadius = 20;
        }

        Series11 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series => SetStyle11("Vanessa", series)),
            new GaugeItem(50, series => SetStyle11("Charles", series)),
            new GaugeItem(70, series => SetStyle11("Ana", series)),
            new GaugeItem(GaugeItem.Background, series => series.InnerRadius = 20));

        GaugeTotal12 = 100;
        InitialRotation12 = 45;
        MaxAngle12 = 270;

        void SetStyle12(string name, PieSeries<ObservableValue> series)
        {
            series.Name = name;
            series.DataLabelsPosition = PolarLabelsPosition.Start;
            series.DataLabelsFormatter =
                point => $"{point.Context.Series.Name} {point.Coordinate.PrimaryValue}";
            series.DataLabelsSize = 20;
            series.InnerRadius = 20;
            series.RelativeInnerRadius = 8;
            series.RelativeOuterRadius = 8;
        }

        Series12 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series => SetStyle12("Vanessa", series)),
            new GaugeItem(50, series => SetStyle12("Charles", series)),
            new GaugeItem(70, series => SetStyle12("Ana", series)),
            new GaugeItem(GaugeItem.Background, series => series.InnerRadius = 20));

        GaugeTotal13 = 100;
        InitialRotation13 = 90;
        MaxAngle13 = 270;

        void SetStyle13(string name, PieSeries<ObservableValue> series)
        {
            series.Name = name;
            series.DataLabelsPosition = PolarLabelsPosition.Start;
            series.DataLabelsFormatter =
                point => $"{point.Context.Series.Name} {point.Coordinate.PrimaryValue}";
            series.DataLabelsSize = 20;
            series.InnerRadius = 20;
            series.RelativeInnerRadius = 4;
            series.RelativeOuterRadius = 4;
        }

        Series13 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series => SetStyle13("Vanessa", series)),
            new GaugeItem(50, series => SetStyle13("Charles", series)),
            new GaugeItem(70, series => SetStyle13("Ana", series)),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 20;
                series.RelativeInnerRadius = 10;
                series.RelativeOuterRadius = 10;
            }));

        GaugeTotal14 = 100;
        InitialRotation14 = -90;
        MaxAngle14 = 350;

        void SetStyle14(string name, PieSeries<ObservableValue> series)
        {
            series.Name = name;
            series.DataLabelsPosition = PolarLabelsPosition.End;
            series.DataLabelsFormatter = point => point.Coordinate.PrimaryValue.ToString();
            series.DataLabelsSize = 20;
            series.InnerRadius = 20;
            series.MaxRadialColumnWidth = 5;
        }

        Series14 = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series => SetStyle14("Vanessa", series)),
            new GaugeItem(50, series => SetStyle14("Charles", series)),
            new GaugeItem(70, series => SetStyle14("Ana", series)),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.Fill = null;
            }));
    }

    public IEnumerable<ISeries> Series1 { get; set; }
    public double GaugeTotal1 { get; set; }

    public IEnumerable<ISeries> Series2 { get; set; }
    public double GaugeTotal2 { get; set; }
    public double InitialRotation2 { get; set; }

    public IEnumerable<ISeries> Series3 { get; set; }
    public double GaugeTotal3 { get; set; }
    public double InitialRotation3 { get; set; }

    public IEnumerable<ISeries> Series4 { get; set; }
    public double GaugeTotal4 { get; set; }
    public double InitialRotation4 { get; set; }

    public IEnumerable<ISeries> Series5 { get; set; }
    public double GaugeTotal5 { get; set; }
    public double InitialRotation5 { get; set; }

    public IEnumerable<ISeries> Series6 { get; set; }
    public double GaugeTotal6 { get; set; }
    public double InitialRotation6 { get; set; }

    public IEnumerable<ISeries> Series7 { get; set; }
    public double GaugeTotal7 { get; set; }
    public double InitialRotation7 { get; set; }

    public IEnumerable<ISeries> Series8 { get; set; }
    public double GaugeTotal8 { get; set; }
    public double InitialRotation8 { get; set; }
    public double MaxAngle8 { get; set; }

    public IEnumerable<ISeries> Series9 { get; set; }
    public double GaugeTotal9 { get; set; }
    public double InitialRotation9 { get; set; }
    public double MaxAngle9 { get; set; }

    public IEnumerable<ISeries> Series10 { get; set; }
    public double GaugeTotal10 { get; set; }
    public double InitialRotation10 { get; set; }
    public double MaxAngle10 { get; set; }

    public IEnumerable<ISeries> Series11 { get; set; }
    public double GaugeTotal11 { get; set; }
    public double InitialRotation11 { get; set; }
    public double MaxAngle11 { get; set; }

    public IEnumerable<ISeries> Series12 { get; set; }
    public double GaugeTotal12 { get; set; }
    public double InitialRotation12 { get; set; }
    public double MaxAngle12 { get; set; }

    public IEnumerable<ISeries> Series13 { get; set; }
    public double GaugeTotal13 { get; set; }
    public double InitialRotation13 { get; set; }
    public double MaxAngle13 { get; set; }

    public IEnumerable<ISeries> Series14 { get; set; }
    public double GaugeTotal14 { get; set; }
    public double InitialRotation14 { get; set; }
    public double MaxAngle14 { get; set; }
}
