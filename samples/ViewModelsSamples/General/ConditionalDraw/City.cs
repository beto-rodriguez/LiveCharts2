using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.General.ConditionalDraw;

public partial class City : ObservableObject
{
    public City(double population)
    {
        _population = population;
    }

    [ObservableProperty]
    private double _population;
}
