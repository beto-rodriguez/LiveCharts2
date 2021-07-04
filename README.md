# LiveCharts2

LiveCharts2 (v2) is the evolution of [LiveCharts](https://github.com/Live-Charts/Live-Charts) (v0), it fixes the main design issues of its predecessor, it's focused to run everywhere, improves flexibility without losing what we already had in v0.

### Extremely flexible data visualization library

The following image is a preview, `v2.0` is beta now, you can install it from nugget: https://github.com/beto-rodriguez/LiveCharts2/issues/35, it support WPF, WinForms, Avalonia and Xamarin for now, but it will also support WinUI and Uno.

here is a preview (1.4MB gif, wait for it to load if you see a blank space bellow this text...):

![lv2](https://user-images.githubusercontent.com/10853349/124399763-41873900-dce3-11eb-937a-947d66d42597.gif)

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

