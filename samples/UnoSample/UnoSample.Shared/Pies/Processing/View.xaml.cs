using System;
using LiveChartsCore.SkiaSharpView.Painting;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UWPSample.Pies.Processing
{
    public sealed partial class View : UserControl
    {
        public View()
        {
            InitializeComponent();
        }
    }

    public class PaintTaskToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is SolidColorPaint solidColor
                ? new SolidColorBrush(
                    Color.FromArgb(
                        solidColor.Color.Alpha,
                        solidColor.Color.Red,
                        solidColor.Color.Green,
                        solidColor.Color.Blue))
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
