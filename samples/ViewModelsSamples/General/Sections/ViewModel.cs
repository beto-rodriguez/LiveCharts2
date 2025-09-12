using LiveChartsCore.Defaults;

namespace ViewModelsSamples.General.Sections;

public partial class ViewModel
{
    public ObservablePoint[] Values { get; set; } = [
        new ObservablePoint(2.2, 5.4),
        new ObservablePoint(4.5, 2.5),
        new ObservablePoint(4.2, 7.4),
        new ObservablePoint(6.4, 9.9),
        new ObservablePoint(8.9, 3.9),
        new ObservablePoint(9.9, 5.2)
    ];
}
