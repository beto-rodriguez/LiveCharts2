using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.General.ConditionalDraw;

[ObservableObject]
public partial class City
{
    public City(double population)
    {
        _population = population;
    }

    [ObservableProperty]
    private double _population;
}
