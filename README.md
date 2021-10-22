# LiveCharts2

LiveCharts2 (v2) is the evolution of [LiveCharts](https://github.com/Live-Charts/Live-Charts) (v0), it fixes the main design issues of its predecessor, it's focused to run everywhere, improves flexibility without losing what we already had in v0.

### Extremely flexible data visualization library

The following image is a preview, `v2.0` is beta now, you can install it from NuGet: https://github.com/beto-rodriguez/LiveCharts2/issues/35, it support WPF, WinForms, Avalonia, WinUI, UWP, Xamarin and Blazor Wasm for now but MAUI ([#161](https://github.com/beto-rodriguez/LiveCharts2/pull/161)) will also be supported.

here is a preview (1.4MB gif, wait for it to load if you see a blank space bellow this text...):

![lv2](https://user-images.githubusercontent.com/10853349/124399763-41873900-dce3-11eb-937a-947d66d42597.gif)

### Install and try it from NuGet

In Visual Studio go to the `Solution Explorer` then right click on `Dependencies` then `Manage NuGet Packages`, enable the "Include prerelease" checkbox and in the browse tab search for the view of your target platform:

LiveChartsCore.SkiaSharpView.WPF<br/>
LiveChartsCore.SkiaSharpView.WinForms<br/>
LiveChartsCore.SkiaSharpView.Avalonia<br/>
LiveChartsCore.SkiaSharpView.XamarinForms<br/>
LiveChartsCore.SkiaSharpView.UWP (see [#145](https://github.com/beto-rodriguez/LiveCharts2/issues/145))<br/>
LiveChartsCore.SkiaSharpView.WinUI<br/>
LiveChartsCore.SkiaSharpView.Blazor<br/>

You can also use LiveCharts 2 on a console app or on the server side installing only the core packages, and using the `ISkiaSharpChart` objects (see [this sample](https://github.com/beto-rodriguez/LiveCharts2/blob/master/samples/WPFSample/General/ChartToImage/View.xaml.cs#L28)).

LiveChartsCore.SkiaSharpView

### Samples

The new web site will be publish soon with a collection of samples using `v2`, at this moment you can browse the [samples folder](https://github.com/beto-rodriguez/LiveCharts2/tree/master/samples) in this repo.

### The Errors of v0

V0 is built on top of WPF, this has many problems, WPF is not designed for the purposes of the library, it is always tricky to find a solution for the problems of the library.

### How Flexible is v2?

When we were on v0 and tried to take the library to UWP, we noticed it required a huge effort with the architecture the library had in v0.
V2 is designed to work on multiple platforms, it requires minimal effort to take the library to a new platform.

### Then LiveCharts2 requires SkiaSharp?

Not necessarily, The SkiaAPI makes it much easier to take the library everywhere, but that does not means that LiveCharts2 requires it to work we could easily move to any other drawing engine.

