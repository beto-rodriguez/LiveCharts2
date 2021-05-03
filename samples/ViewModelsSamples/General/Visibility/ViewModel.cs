using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ViewModelsSamples.General.Visibility
{
    public class ViewModel
    {
        public List<ISeries> Series { get; set; } = new List<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 2, 5, 4, 3 },
                IsVisible = true
            },
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 6, 3, 2, 8},
                IsVisible = true
            },
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 4, 2, 8, 7 },
                IsVisible = true
            }
        };

        public void ToogleSeries0()
        {
            Series[0].IsVisible = !Series[0].IsVisible;
        }

        public void ToogleSeries1()
        {
            Series[1].IsVisible = !Series[1].IsVisible;
        }

        public void ToogleSeries2()
        {
            Series[2].IsVisible = !Series[2].IsVisible;
        }

        // The next commands are only to enable XAML bindings
        // they are not used in the WinForms sample
        public ICommand ToggleSeries0Command => new Command(o => ToogleSeries0());

        public ICommand ToggleSeries1Command => new Command(o => ToogleSeries1());

        public ICommand ToggleSeries2Command => new Command(o => ToogleSeries2());
    }
}
