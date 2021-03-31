using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;

namespace ViewModelsSamples.Axes.ColorsAndPosition
{
    public class ViewModel
    {
        private AxisPosition selectedPosition;
        private int selectedColor = 0;
        private Color[] colors = ColorPacks.FluentDesign;

        public ViewModel()
        {
            Series = new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Values = new ObservableCollection<double> { 2, 5, 4, -2, 4, -3, 5 },
                    Stroke = null,
                    Fill = new SolidColorPaintTask { Color = SKColors.DarkOliveGreen }
                }
            };

            // Places the axis to the right (or top for X axes)
            selectedPosition = AxisPosition.End;

            XAxes = new List<Axis>
            {
                new Axis
                {
                    TextSize = 20,

                    // TextBrush = null will not draw the axis labels.
                    TextBrush = new SolidColorPaintTask{ Color = SKColors.CornflowerBlue },

                    // SeparatorsBrush = null will not draw the separator lines
                    SeparatorsBrush = new SolidColorPaintTask { Color = SKColors.LightBlue, StrokeThickness = 3 },

                    Position = selectedPosition
                }
            };

            YAxes = new List<Axis>
            {
                new Axis
                {
                    TextSize = 20,
                    TextBrush = new SolidColorPaintTask { Color = SKColors.Red },
                    SeparatorsBrush = new SolidColorPaintTask { Color = SKColors.LightPink, StrokeThickness = 3 },
                    Position = selectedPosition
                }
            };
        }

        public IEnumerable<ISeries> Series { get; set; }

        public List<Axis> XAxes { get; set; }

        public List<Axis> YAxes { get; set; }

        public void SetNewColor()
        {
            var nextColor = colors[selectedColor++ % colors.Length];
            XAxes[0].TextBrush = new SolidColorPaintTask(new SKColor(nextColor.R, nextColor.G, nextColor.B));
            XAxes[0].SeparatorsBrush = new SolidColorPaintTask(new SKColor(nextColor.R, nextColor.G, nextColor.B), 3);
        }

        public void TogglePosition()
        {
            selectedPosition = selectedPosition == AxisPosition.End ? AxisPosition.Start : AxisPosition.End;
            XAxes[0].Position = selectedPosition;
        }

        public ICommand SetNewColorCommand => new Command(o => SetNewColor());
        public ICommand TogglePositionCommand => new Command(o => TogglePosition());
    }
}
