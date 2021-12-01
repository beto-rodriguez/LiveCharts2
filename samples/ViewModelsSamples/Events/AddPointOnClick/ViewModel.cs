using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Events.AddPointOnClick
{
    public class ViewModel
    {
        public ViewModel()
        {
            var data = new ObservableCollection<ObservablePoint>
            {
                new ObservablePoint(0, 5),
                new ObservablePoint(3, 8),
                new ObservablePoint(7, 9)
            };

            Data = data;

            SeriesCollection = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = data,
                    Fill = null,
                    DataPadding = new LiveChartsCore.Drawing.LvcPoint(5, 5)
                }
            };
        }

        public ObservableCollection<ObservablePoint> Data { get; set; }

        public ISeries[] SeriesCollection { get; set; }
    }
}
