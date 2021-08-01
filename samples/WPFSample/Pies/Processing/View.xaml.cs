using System;
using System.Collections;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using LiveChartsCore.SkiaSharpView.Painting;

namespace WPFSample.Pies.Processing
{
    /// <summary>
    /// Interaction logic for View.xaml
    /// </summary>
    public partial class View : UserControl
    {
        public View()
        {
            InitializeComponent();
        }
    }

    public class PaintTaskToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable enumerable) return null;

            var enumerator = enumerable.GetEnumerator();
            return enumerator.MoveNext() && enumerator.Current is SolidColorPaintTask solidPaintTask
                ? new SolidColorBrush(Color.FromArgb(
                    solidPaintTask.Color.Alpha,
                    solidPaintTask.Color.Red,
                    solidPaintTask.Color.Green,
                    solidPaintTask.Color.Blue))
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
