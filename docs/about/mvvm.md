<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# About MVVM and this site examples

The examples in this site use MVVM, **it is not necessary to follow the MVVM pattern to use LiveCharts**, but MVVM is helpful, in the case
because we need to share the same data for all the supported platforms and we also need automatic updates. 

The repository contains the [ViewModelsSamples.csproj](https://github.com/beto-rodriguez/LiveCharts2/tree/master/samples/ViewModelsSamples)
project, there we can find all the view models of the plots we are building for this site, then each view (WinForms, WPF, Uno, Maui... etc) consumes the 
same view models to render the plot on different frameworks, it means that we are reusing the same data (view model) to render a chart in mobile, desktop or the web.
Because LiveCharts follows this architecture, it is easy to run LiveCharts on cross platform frameworks such as Maui, Uno Platform or Avalonia.

## This site uses CommunityToolkit.Mvvm package

Probably the less lovely thing about MVVM is how verbose it was, now with source generators, the dotnet foundation is maintaining the 
[CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm/), it makes MVVM easier.

To define a series property that notifies the change to the UI, without any MVVM framework a view model would look like:

<pre><code>using LiveChartsCore;
using System.ComponentModel;

public class ViewModel2 : INotifyPropertyChanged
{
    private ISeries[] _series;

    public ISeries[] Series
    {
        get => _series;
        set
        {
            if (_series == value) return;
            _series = value;
            OnPropertyChanged(nameof(Series));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}</code></pre>

Too much code to declare a property isn't it? but now we can use the `CommunityToolkit.Mvvm` package and write a cleaner view model:

<pre><code>using LiveChartsCore;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class ViewModel : ObservableObject
{
    [ObservableProperty]
    public ISeries[] _series;
}</code></pre>

Now we inherited from `ObservableObject` and marked the *_series* field with the `ObservableProperty` attribute; by convention and with 
the magic of source generators, the property `Series` now exists in our class, source generators are adding all this boring and repetitive 
code for us, this is just a quick guide to get started with the docs in this site but you can learn more about the 
toolkit [here](https://www.youtube.com/watch?v=aCxl0z04BN8) or in the [official docs](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/).
