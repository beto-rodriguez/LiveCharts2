<div id="edit-this-article-source">
    {{ edit_source | replace_local_to_server}}
</div>

# About MVVM and this site examples

The examples in this site use MVVM, **it is not necessary to follow the MVVM pattern to use LiveCharts**, but MVVM is really helpful, in the case
of this set of examples it is useful because we need to share the same data for all the supported platforms and we also need automatic updates. 

The repository contains the [ViewModelsSamples.csproj](https://github.com/beto-rodriguez/LiveCharts2/tree/master/samples/ViewModelsSamples)
project, there we can find all the view models of the plots we are building for this site, then each view (WinForms, WPF, Xamarin, Uno, Maui... etc) consumes the 
same view models to render the plot on different frameworks, it means that we are reusing the same data (view model) to render a chart in mobile, desktop or the web.
Because LiveCharts follows this architecture, it is easy to run LiveCharts on cross platform frameworks such as Maui, Uno Platform or Avalonia.

For example take a look at the [basic line view model](https://github.com/beto-rodriguez/LiveCharts2/blob/master/samples/ViewModelsSamples/Lines/Basic/ViewModel.cs),
then lets compare the [WPF](https://github.com/beto-rodriguez/LiveCharts2/blob/master/samples/WPFSample/Lines/Basic/View.xaml) and the 
[Xamarin](https://github.com/beto-rodriguez/LiveCharts2/blob/master/samples/XamarinSample/XamarinSample/XamarinSample/Lines/Basic/View.xaml) views:

**WPF:**

<pre><code>&lt;UserControl x:Class="WPFSample.Lines.Basic.View"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
             xmlns:lvc="clr-namespace:LiveChartsCore.SkiaSharpView.WPF;assembly=LiveChartsCore.SkiaSharpView.WPF"
             xmlns:vms="clr-namespace:ViewModelsSamples.Lines.Basic;assembly=ViewModelsSamples">
    &lt;UserControl.DataContext>
        &lt;vms:ViewModel/>
    &lt;/UserControl.DataContext>
    &lt;lvc:CartesianChart Series="{Binding Series}"/>
&lt;/UserControl></code></pre>

**Xamarin:**

<pre><code>&lt;ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="XamarinSample.Lines.Basic.View"
             xmlns:lvc="clr-namespace:LiveChartsCore.SkiaSharpView.Xamarin.Forms;assembly=LiveChartsCore.SkiaSharpView.XamarinForms"
             xmlns:vms="clr-namespace:ViewModelsSamples.Lines.Basic;assembly=ViewModelsSamples">
    &lt;ContentPage.BindingContext>
        &lt;vms:ViewModel/>
    &lt;/ContentPage.BindingContext>
    &lt;lvc:CartesianChart Series="{Binding Series}"/>
&lt;/ContentPage></code></pre>

As you can see both are basically identical, they set the data source to our view model, then they add a Cartesian chart control and bind the `Series` 
property to the `Series` defined in our view model. This was a really useful pattern in the 10's specially it maximizes code reusability and makes
it easy to build cross platform apps, now we have better options, we have cross platform frameworks that handle this for us 
(Maui, Uno Platform or Avalonia for example). In conclusion the library uses MVVM to re-use the same view model for all the supported 
frameworks by the library and to implement `INotifyPropertyChanged` to handle automatic updates.

## This site uses CommunityToolkit.Mvvm package

Probably the less lovely thing about MVVM is how verbose it was, now with source generators, the dotnet foundation is maintaining the 
[CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm/), this is a game changer IMO, it makes MVVM supper easy,
basically you only need to make your view models `partial` classes, then add the `ObservableObject` attribute to the class and 
the `ObservableProperty` attribute a field inside your class.

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

[ObservableObject]
public partial class ViewModel
{
    [ObservableProperty]
    public ISeries[] _series;
}</code></pre>

Notice the `ObservableProperty` attribute was added to the *_series*  field. By convention and with the magic of source generators the property
`Series` now exists in our class, source generators are adding all this boring and repetitive code for us, this is just a quick guide
to get started with the docs in this site but you can learn more about the toolkit [here](https://www.youtube.com/watch?v=aCxl0z04BN8).
