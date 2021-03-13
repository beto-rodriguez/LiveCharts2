using LiveChartsCore.Context;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples
{
    public class CartesianViewModel
    {
        public ObservableCollection<ICartesianSeries<SkiaSharpDrawingContext>> Series { get; set; }
        public List<IAxis<SkiaSharpDrawingContext>> YAxes { get; set; }
        public List<IAxis<SkiaSharpDrawingContext>> XAxes { get; set; }

        public CartesianViewModel()
        {

            var r = new Random();
            Series = new ObservableCollection<ICartesianSeries<SkiaSharpDrawingContext>>();
            for (int i = 0; i < 1; i++)
            {
                var values = new ObservableCollection<int>();
                var t = 0;
                for (int j = 0; j < 10; j++)
                {
                    values.Add(t += r.Next(-10, 10));
                }

                Series.Add(new LineSeries<int> { Values = values, Fill = null });
            }

            YAxes = new List<IAxis<SkiaSharpDrawingContext>>
            {
                new Axis
                {
                    TextBrush = new SolidColorPaintTask(new SKColor(255, 90,90,90)),
                    SeparatorsBrush = new SolidColorPaintTask(new SKColor(255, 180, 180, 180)),
                    //LabelsRotation = 10,
                    Labeler = (value, tick) => $"this {value}"
                }
            };

            XAxes = new List<IAxis<SkiaSharpDrawingContext>>
            {
                new Axis
                {
                    TextBrush = new SolidColorPaintTask(new SKColor(255, 90,90,90)),
                    SeparatorsBrush = new SolidColorPaintTask(new SKColor(255, 180, 180, 180)),
                    //LabelsRotation = 80,
                    Labeler = (value, tick) => $"this {value}"
                }
            };
        }
    }
}
