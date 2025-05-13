using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ViewModelsSamples.General.Visibility;

public partial class ViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool Visibility1 { get; set; } = true;

    [ObservableProperty]
    public partial bool Visibility2 { get; set; } = true;

    [ObservableProperty]
    public partial bool Visibility3 { get; set; } = true;

    [RelayCommand]
    public void ToggleSeries1() => Visibility1 = !Visibility1;

    [RelayCommand]
    public void ToggleSeries2() => Visibility2 = !Visibility2;

    [RelayCommand]
    public void ToggleSeries3() => Visibility3 = !Visibility3;
}
