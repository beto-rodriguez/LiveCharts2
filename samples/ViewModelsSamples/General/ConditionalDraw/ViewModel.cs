using System;
using System.Collections.ObjectModel;
using LiveChartsCore.Defaults;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Kernel;
using System.Threading.Tasks;

namespace ViewModelsSamples.General.ConditionalDraw;

public partial class ViewModel
{
    public ViewModel()
    {
        Randomize();
    }

    public ObservableCollection<ObservableValue> Values { get; set; } =
        [
            new(2),
            new(3),
            new(4)
        ];

    [RelayCommand]
    private void OnPointMeasured(ChartPoint point)
    {
        if (point.Context.DataSource is not ObservableValue observable) return;

        if (observable.Value > 5)
        {
            point.SetState("Danger");
        }
        else
        {
            point.ClearState("Danger");
        }
    }

    private async void Randomize()
    {
        var r = new Random();

        while (true)
        {
            await Task.Delay(3000);

            foreach (var item in Values)
            {
                item.Value = r.Next(1, 10);
            }
        }
    }
}
