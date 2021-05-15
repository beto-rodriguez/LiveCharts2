using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;

namespace ViewModelsSamples.Pies.Gauges
{
    public class ViewModel
    {
        public ViewModel()
        {
            GaugeTotal1 = 100;
            Series1 = new GaugeBuilder()
            {
                LabelsSize = 80,
                Background = new RadialGradientPaintTask(new SKColor(167, 192, 205, 0), new SKColor(167, 192, 205))
            }
            .AddValue(new ObservableValue(30))
            .BuildSeries();

            GaugeTotal2 = 100;
            InitialRotation2 = -90;
            Series2 = new GaugeBuilder()
            {
                LabelsSize = 80,
                Background = new RadialGradientPaintTask(new SKColor(167, 192, 205, 0), new SKColor(167, 192, 205))
            }
            .AddValue(new ObservableValue(30))
            .BuildSeries();

            GaugeTotal3 = 100;
            InitialRotation3 = -90;
            Series3 = new GaugeBuilder() { LabelsSize = 60, InnerRadius = 50 }
               .AddValue(new ObservableValue(30))
               .BuildSeries();

            GaugeTotal4 = 100;
            InitialRotation4 = -90;
            Series4 = new GaugeBuilder()
            {
                LabelsSize = 50,
                InnerRadius = 50,
                BackgroundInnerRadius = 50,
                BackgroundOffsetRadius = 10
            }
            .AddValue(new ObservableValue(30))
            .BuildSeries();

            GaugeTotal5 = 100;
            InitialRotation5 = -90;
            Series5 = new GaugeBuilder() { LabelsSize = 50, InnerRadius = 50, OffsetRadius = 10, BackgroundInnerRadius = 50 }
               .AddValue(new ObservableValue(30))
               .BuildSeries();

            GaugeTotal6 = 100;
            InitialRotation6 = -90;
            Series6 = new GaugeBuilder()
            {
                LabelsSize = 50,
                InnerRadius = 50,
                BackgroundInnerRadius = 50,
                Background = new SolidColorPaintTask(new SKColor(100, 181, 246))
            }
            .AddValue(new ObservableValue(30), null, new SolidColorPaintTask(new SKColor(21, 101, 192)))
            .BuildSeries();

            GaugeTotal7 = 100;
            InitialRotation7 = -90;
            Series7 = new GaugeBuilder()
            {
                LabelsSize = 50,
                InnerRadius = 75,
                BackgroundInnerRadius = 50,
                Background = new SolidColorPaintTask(new SKColor(100, 181, 246, 90))
            }
            .AddValue(new ObservableValue(30), null, new SolidColorPaintTask(new SKColor(21, 101, 192)))
            .BuildSeries();

            GaugeTotal8 = 100;
            InitialRotation8 = -225;
            MaxAngle8 = 270;
            Series8 = new GaugeBuilder()
            {
                LabelsSize = 50,
                InnerRadius = 75,
                BackgroundInnerRadius = 50,
                Background = new SolidColorPaintTask(new SKColor(100, 181, 246, 90))
            }
            .AddValue(new ObservableValue(30), null, new SolidColorPaintTask(new SKColor(21, 101, 192)))
            .BuildSeries();

            GaugeTotal9 = 100;
            InitialRotation9 = 315;
            MaxAngle9 = 270;
            Series9 = new GaugeBuilder()
            {
                LabelsSize = 50,
                InnerRadius = 75,
                BackgroundInnerRadius = 50,
                Background = new SolidColorPaintTask(new SKColor(100, 181, 246, 90))
            }
            .AddValue(new ObservableValue(30), null, new SolidColorPaintTask(new SKColor(21, 101, 192)))
            .BuildSeries();

            GaugeTotal10 = 100;
            InitialRotation10 = -200;
            MaxAngle10 = 220;
            Series10 = new GaugeBuilder()
            {
                LabelsSize = 30,
                InnerRadius = 75,
                BackgroundInnerRadius = 50,
                Background = new LinearGradientPaintTask(new SKColor(250, 243, 224), new SKColor(182, 137, 115))
            }
            .AddValue(new ObservableValue(30), null, new SolidColorPaintTask(new SKColor(30, 33, 45)), new SolidColorPaintTask(new SKColor(30, 33, 45)))
            .BuildSeries();

            GaugeTotal11 = 100;
            InitialRotation11 = -90;
            MaxAngle11 = 270;
            Series11 = new GaugeBuilder()
            {
                LabelsPosition = PolarLabelsPosition.Start,
                LabelFormatter = point => point.Context.Series.Name + " " + point.PrimaryValue,
                LabelsSize = 20,
                InnerRadius = 20,
                BackgroundInnerRadius = 20
            }
            .AddValue(new ObservableValue(30), "Vanessa")
            .AddValue(new ObservableValue(50), "Charles")
            .AddValue(new ObservableValue(70), "Ana")
            .BuildSeries();

            GaugeTotal12 = 100;
            InitialRotation12 = 45;
            MaxAngle12 = 270;
            Series12 = new GaugeBuilder()
            {
                LabelsPosition = PolarLabelsPosition.Start,
                LabelFormatter = point => point.PrimaryValue + " " + point.Context.Series.Name,
                LabelsSize = 20,
                InnerRadius = 20,
                OffsetRadius = 8,
                BackgroundInnerRadius = 20
            }
            .AddValue(new ObservableValue(30), "Vanessa")
            .AddValue(new ObservableValue(50), "Charles")
            .AddValue(new ObservableValue(70), "Ana")
            .BuildSeries();

            GaugeTotal13 = 100;
            InitialRotation13 = 90;
            MaxAngle13 = 270;
            Series13 = new GaugeBuilder()
            {
                LabelsPosition = PolarLabelsPosition.Start,
                LabelFormatter = point => point.PrimaryValue + " " + point.Context.Series.Name,
                LabelsSize = 20,
                InnerRadius = 20,
                OffsetRadius = 4,
                BackgroundInnerRadius = 20,
                BackgroundOffsetRadius = 10
            }
            .AddValue(new ObservableValue(30), "Vanessa")
            .AddValue(new ObservableValue(50), "Charles")
            .AddValue(new ObservableValue(70), "Ana")
            .BuildSeries();

            GaugeTotal14 = 100;
            InitialRotation14 = -90;
            MaxAngle14 = 350;
            Series14 = new GaugeBuilder()
            {
                LabelsPosition = PolarLabelsPosition.End,
                LabelFormatter = point => point.PrimaryValue.ToString(),
                LabelsSize = 20,
                InnerRadius = 20,
                MaxRadialColumnWidth = 5,
                Background = null
            }
            .AddValue(new ObservableValue(50), "Vanessa")
            .AddValue(new ObservableValue(80), "Charles")
            .AddValue(new ObservableValue(95), "Ana")
            .BuildSeries();
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
}
