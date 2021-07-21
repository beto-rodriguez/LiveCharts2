using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;

namespace ViewModelsSamples.Axes.ColorsAndPosition
{
    public class ViewModel
    {
        private AxisPosition _selectedPosition;
        private int _selectedColor = 0;
        private readonly Color[] _colors = ColorPalletes.FluentDesign;

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
            _selectedPosition = AxisPosition.End;

            XAxes = new List<Axis>
            {
                new Axis
                {
                    Name = "X axis",
                    TextSize = 20,

                    // LabelsPaint = null will not draw the axis labels.
                    LabelsPaint = new SolidColorPaintTask{ Color = SKColors.CornflowerBlue },

                    // SeparatorsPaint = null will not draw the separator lines
                    SeparatorsPaint = new SolidColorPaintTask { Color = SKColors.LightBlue, StrokeThickness = 3 },

                    Position = _selectedPosition
                }
            };

            YAxes = new List<Axis>
            {
                new Axis
                {
                    Name = "Y axis",
                    TextSize = 20,
                    LabelsPaint = new SolidColorPaintTask { Color = SKColors.Red },
                    SeparatorsPaint = new SolidColorPaintTask { Color = SKColors.LightPink, StrokeThickness = 3 },
                    Position = _selectedPosition
                }
            };
        }

        public IEnumerable<ISeries> Series { get; set; }

        public List<Axis> XAxes { get; set; }

        public List<Axis> YAxes { get; set; }

        public void SetNewColor()
        {
            var nextColor = _colors[_selectedColor++ % _colors.Length];
            XAxes[0].LabelsPaint = new SolidColorPaintTask(new SKColor(nextColor.R, nextColor.G, nextColor.B));
            XAxes[0].SeparatorsPaint = new SolidColorPaintTask(new SKColor(nextColor.R, nextColor.G, nextColor.B), 3);
        }

        public void TogglePosition()
        {
            _selectedPosition = _selectedPosition == AxisPosition.End ? AxisPosition.Start : AxisPosition.End;
            XAxes[0].Position = _selectedPosition;
            YAxes[0].Position = _selectedPosition;
        }

        // The next commands are only to enable XAML bindings
        // they are not used in the WinForms sample
        public ICommand SetNewColorCommand => new Command(o => SetNewColor());
        public ICommand TogglePositionCommand => new Command(o => TogglePosition());
    }
}
