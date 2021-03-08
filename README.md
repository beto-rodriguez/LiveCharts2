# LiveCharts2

LiveCharts2 (v2) is the evolution of [LiveCharts](https://github.com/Live-Charts/Live-Charts) (v0), it fixes the main design issues of its predecessor, it's focused to run everywhere, improves flexibility without losing what we already had in v0.

### The Errors of v0

V0 is built on top of WPF, this has many problems, WPF is not designed for the purposes of the library, it is always tricky to find a solution for the problems of the library.

### How Flexible is v2?

When we were on v0 and tried to take the library to UWP, we noticed it required a huge effort with the arquitecture the library had in v0.
V2 is designed to work on multiple platforms, it requires minimal effort to take the library to a new platform.

### Where can LiveCharts2 run?

Thanks to [SkiaSharp](https://github.com/mono/SkiaSharp) practically everywhere, yes desktop, mobile and web**.

Yes desktop means Linux, MacOS and Windows, mobile IOS and Android.

The ** in web means that is has its problems, Is WASM production ready? Is Blazor WASM production ready? Is there a SkiaSharpView for Blazor? (I think no, [this](https://github.com/mattleibow/BlazorSkiaSharp) is the closest), but there is hope for web there is [Uno platform](https://github.com/unoplatform) also, and yes SkiaSharp has a view for Uno.

[Avalonia](https://avaloniaui.net/) is also an interesting project that has a [SkiaView](https://www.nuget.org/packages/Avalonia.Skia/).

If your application targets multiple platforms, you could also share the same chart model for your Xamarin App and your desktop App.

### Then LiveCharts2 requires SkiaSharp?

Not necessarily, The SkiaAPI makes it much easier to take the library everywhere, but that does not means that LiveCharts2 requires it to work we could easily move to any other drawing engine.

### I dont care about mobile, or web I just need it for my WinForms app, then what is new?

Compared with v0, v2 should be more solid, should fix many issues in the old project, you will have less problems here, also since v2 now takes care of practically everything, we can also customize practically everyhing.

We can now customize the easing curves, and how every property in our plot is animated, in the next image we have some [ColumnSeries](https://github.com/Live-Charts/LiveCharts2/blob/master/LiveChartsCore/ColumnSeries.cs), every column series draws a [RectangleGeometry](https://github.com/Live-Charts/LiveCharts2/blob/master/LiveChartsCore.SkiaSharp/Drawing/RectangleGeometry.cs) for every point in the chart, the rectangle geometry has X, Y, Height and Width properties, notice that the Y, and Heigth propeties [bounce](https://github.com/Live-Charts/LiveCharts2/blob/master/LiveChartsCore/Easing/BounceEasingFunction.cs), while the X and Width properties use a [lineal transition](https://github.com/Live-Charts/LiveCharts2/blob/master/LiveChartsCore/EasingFunctions.cs#L37).

![bounce](https://user-images.githubusercontent.com/10853349/107853263-71887b00-6dda-11eb-94ba-03aa518e86dc.gif)

In the next image we are also animating The StrokeDash array of our column series, and the Fill of our line series.

![](https://user-images.githubusercontent.com/10853349/107728642-39871800-6cb4-11eb-8373-422123e2e59e.gif)

### About this repo

Net 5 is required, latest Visual studio would help, Xamarin SDK might be required if you need to run the xamarin samples.

The project contains 3 folders:

src: contains the source code of the library

samples: contains samples targeting different platforms consumin src folder projects.

tests: contains units tests.
      
### Road map

The next sections describes the task we need to complete.

##### Core

The next topics are the base of the library.

- [x] ~~Get enough feedback from v0 and find a posible solution for the main issues~~
- [x] ~~Build a solid platform to help the drawing engine do its job~~ the framework is interesting... maybe it will be an independent package.

##### Platform specific

The following topics require a solution that will only work for an specific platform.

- [x] ~~Buid a WPF View~~
- [x] ~~Buid a WPF default tooltip~~
- [x] ~~Buid a WPF default legend~~
- [x] ~~Buid a Winforms View~~
- [ ] Buid a Winforms default tooltip
- [ ] Buid a Winforms default legend
- [x] ~~Buid a Xamarin Forms View~~
- [ ] Buid a Xamarin default tooltip
- [ ] Buid a Xamarin default legend
- [ ] Buid an UnoPlatform View
- [ ] Buid an UnoPlatform default tooltip
- [ ] Buid an UnoPlatform default legend
- [ ] Buid an Avalonia View
- [ ] Buid an Avalonia default tooltip
- [ ] Buid an Avalonia default legend
- [ ] Buid or at least try a view for WASM, Blazor... and the tooltip and legend

##### Shared tasks

The following topics will work on any platform.

- [x] ~~Create the Chart class, an object that coordinates every object inside the chart~~
- [ ] Allow zooming
- [ ] Allow panning
- [x] ~~Create the Axis class, an object that defines the data range in a plane (x or y) and scale a chart point to the screen based on the data range in the axis~~
- [x] ~~Draw Axes in the UI~~
- [x] ~~Multiple axes in the same chart~~
- [ ] Axis labels rotation
- [ ] Date scaled Axes
- [ ] Logaritmic scaled Axes
- [ ] Allow the chart to have external elements in the UI ([visual elements](https://lvcharts.net/App/examples/v1/wpf/Visual%20Elements))
- [x] ~~Create the Series class, an object that is able to draw a data set in the screen in diferent ways (defined by the class that inherits from this) a series must be flexible, practically anything was plotted, could be changed by the user, it must also animate when the data changes~~

- [x] ~~Negative stacked series~~

- [x] ~~LineSeries~~
  - [x] ~~Draw a basic Spline that allows to customize the curve smoothness~~
  - [x] ~~Allow the user to define a custom geometry for every point in the line~~
  - [x] ~~allow gaps in the line series~~
  - [x] ~~allow data labels and make them cutomizable~~
    
- [x] ~~ColumnSeries~~
  - [x] ~~Draw a basic column~~
  - [x] ~~Allow the user to define a custom geometry for every column in the series~~
  - [x] ~~Layered columns~~
  - [x] ~~allow vertical mode~~
  - [x] ~~allow data labels and make them cutomizable~~
  
- [x] ~~StackedColumnSeries~~
  - [x] ~~Draw a basic stacked column~~
  - [x] ~~Allow the user to define a custom geometry for every column in the series~~
  - [x] ~~allow vertical mode~~
  - [x] ~~allow data labels and make them cutomizable~~
  
- [x] ~~StackedAreaSeries~~
  - [x] ~~Draw a basic stacked area~~
  - [x] ~~allow data labels and make them cutomizable~~
  
- [x] ~~ScatteredSeries~~
  - [x] ~~Draw a basic scatered series~~
  - [x] ~~Allow the user to define a custom geometry for every point in the series~~
  - [x] ~~Allow points to have a "weight" so we can scale on "Z", see  [bubble chart](https://lvcharts.net/App/examples/v1/wpf/Bubble%20Chart)~~
  - [x] ~~allow data labels and make them cutomizable~~
  
- [x] ~~PieChart and Pie Series~~
  - [x] ~~Draw a basic pie slice geometry, where the corners could be rounded and you can define a "wedge" property so we can make a [doughnut](https://lvcharts.net/App/examples/v1/wpf/Doughnut%20Chart)~~
  - [x] ~~allow data labels and make them cutomizable~~
  
- [ ] Solid Gauges
