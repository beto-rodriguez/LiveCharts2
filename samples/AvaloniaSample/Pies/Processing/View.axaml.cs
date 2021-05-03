using System;
using System.Collections;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using LiveChartsCore.SkiaSharpView.Painting;

namespace AvaloniaSample.Pies.Processing
{
    public class View : UserControl
    {
        public View()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class PaintTaskToBrushConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable enumerable) return null;

            var enumerator = enumerable.GetEnumerator();
            enumerator.MoveNext();

            var firstPaintTask = enumerator.Current;
            if (firstPaintTask is not SolidColorPaintTask solidPaintTask) return null;

            return new SolidColorBrush(
                new Color(
                    solidPaintTask.Color.Alpha,
                    solidPaintTask.Color.Red,
                    solidPaintTask.Color.Green,
                    solidPaintTask.Color.Blue));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
