﻿using System;
using System.Collections;
using System.Globalization;
using LiveChartsCore.SkiaSharpView.Painting;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinSample.Pies.Processing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class View : ContentPage
    {
        public View()
        {
            InitializeComponent();
        }
    }

    public class PaintTaskToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable enumerable) return null;

            var enumerator = enumerable.GetEnumerator();
            return enumerator.MoveNext() && enumerator.Current is SolidColorPaintTask solidPaintTask
                ? Color.FromRgba(
                    solidPaintTask.Color.Red,
                    solidPaintTask.Color.Green,
                    solidPaintTask.Color.Blue,
                    solidPaintTask.Color.Alpha)
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
