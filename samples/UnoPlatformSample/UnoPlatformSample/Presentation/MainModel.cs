using System.ComponentModel;
using Uno.Extensions;
using Uno.Extensions.Reactive;
using ViewModelsSamples;

namespace UnoPlatformSample.Presentation;

public partial record MainModel : INotifyPropertyChanged
{
    private INavigator _navigator;
    private string selectedSample = "...";

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator)
    {
        _navigator = navigator;
    }

    public string[] Samples { get; } = ViewModelsSamples.Index.Samples;
    public string SelectedSample 
    {
        get => selectedSample; 
        set { selectedSample = value; OnSelectedSampleChanged(value); } 
    }

    private void OnSelectedSampleChanged(string value)
    {
        _navigator.NavigateViewModelAsync<SecondModel>(this, data: new Entity(value));
    }
}
