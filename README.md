# LiveCharts2

[![CodeFactor](https://www.codefactor.io/repository/github/beto-rodriguez/livecharts2/badge)](https://www.codefactor.io/repository/github/beto-rodriguez/livecharts2)
![Unit tests](https://github.com/beto-rodriguez/LiveCharts2/actions/workflows/run-unit-tests.yml/badge.svg)
![SkiaSharp Views](https://github.com/beto-rodriguez/LiveCharts2/actions/workflows/compile-all-views.yml/badge.svg)

[Watch Blazor WASM demo](https://blazor-livecharts.controli.app/) (only designed for desktop devices for now)

LiveCharts2 (v2) is the evolution of [LiveCharts](https://github.com/Live-Charts/Live-Charts) (v0), it fixes the main design issues of its predecessor, it's focused to run everywhere, improves flexibility without losing what we already had in v0.

### Extremely flexible data visualization library

The following image is a preview, `v2.0` is beta now.

Here is a preview (1.4MB gif, wait for it to load if you see a blank space bellow this text...):

![lv2](https://user-images.githubusercontent.com/10853349/124399763-41873900-dce3-11eb-937a-947d66d42597.gif)

### Get started

Live charts is a cross platforms charting library for .Net, to get started go to https://livecharts.dev and take a look at the instalation guide of your target platform,
the web site contains all the samples provided in this repo, docs and more.

LiveCharts supports:

- Maui
- Uno Platform
- Wpf
- WinUI
- Xamarin.Forms
- WindowsForms
- BlazorWasm
- Avalonia
- Eto Forms
- Uwp

You can also use LiveCharts 2 on a console app or on the server side installing only the core packages, take a look at [this guide](https://lvcharts.com/docs/WPF/2.0.0-beta.330/samples.general.chartToImage#build-an-image-in-the-server-side-or-console-app).

### The Errors of v0

V0 is built on top of WPF, this has many problems, WPF is not designed for the purposes of the library, it is always tricky to find a solution for the problems of the library.

### How Flexible is v2?

When we were on v0 and tried to take the library to UWP, we noticed it required a huge effort with the architecture the library had in v0.
V2 is designed to work on multiple platforms, it requires minimal effort to take the library to a new platform.

### Then LiveCharts2 requires SkiaSharp?

Not necessarily, The SkiaAPI makes it much easier to take the library everywhere, but that does not means that LiveCharts2 requires it to work we could easily move to any other drawing engine.

