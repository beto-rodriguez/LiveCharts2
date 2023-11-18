using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.General.ConditionalDraw;

public partial class City(double population) : ObservableObject
{
    [ObservableProperty]
    private double _population = population;
}
