using Avalonia.Media;
using LiveChartsCore.AvaloniaView;
using LiveChartsCore.AvaloniaView.Drawing;
using LiveChartsCore.AvaloniaView.Painting;
using LiveChartsCore.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaloniaSample
{
    public class CartesianViewModel
    {
        public ObservableCollection<ICartesianSeries<AvaloniaDrawingContext>> Series { get; set; }
        public List<IAxis<AvaloniaDrawingContext>> YAxes { get; set; }
        public List<IAxis<AvaloniaDrawingContext>> XAxes { get; set; }

        public CartesianViewModel()
        {

            var r = new Random();
            Series = new ObservableCollection<ICartesianSeries<AvaloniaDrawingContext>>();
            for (int i = 0; i < 1; i++)
            {
                var values = new ObservableCollection<int>();
                var t = 0;
                for (int j = 0; j < 10; j++)
                {
                    values.Add(t += r.Next(-10, 10));
                }

                Series.Add(new ColumnSeries<int> { Values = values, Fill = null });
            }

            YAxes = new List<IAxis<AvaloniaDrawingContext>>
            {
                new Axis
                {
                    TextBrush = new SolidColorPaintTask(new Color(255, 90,90,90)),
                    SeparatorsBrush = new SolidColorPaintTask(new Color(255, 180, 180, 180)),
                    //LabelsRotation = 10,
                    Labeler = (value, tick) => $"this {value}"
                }
            };

            XAxes = new List<IAxis<AvaloniaDrawingContext>>
            {
                new Axis
                {
                    TextBrush = new SolidColorPaintTask(new Color(255, 90,90,90)),
                    SeparatorsBrush = new SolidColorPaintTask(new Color(255, 180, 180, 180)),
                    //LabelsRotation = 80,
                    Labeler = (value, tick) => $"this {value}"
                }
            };
        }
    }
}
