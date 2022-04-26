using System;
using Windows.UI.Xaml.Controls;
using ViewModelsSamples.General.MultiThreading2;

namespace UnoSample.General.MultiThreading2;

public sealed partial class View : UserControl
{
    public View()
    {
        InitializeComponent();

        var vm = new ViewModel(InvokeOnUIThread);
        DataContext = vm;
    }

    // this method takes another function as an argument.
    // the idea is that we are invoking the passed action in the UI thread
    // but the UI framework will let the view model how to do this.
    // we will pass the InvokeOnUIThread method to our view model so the view model knows how
    // to invoke an action in the UI thred.
    private void InvokeOnUIThread(Action action)
    {
        // the InvokeOnUIThread method provided by livecharts is a simple helper class
        // that handles the invoke in the multiple platforms Uno supports.
        LiveChartsCore.SkiaSharpView.Uno.Helpers.UnoPlatformHelpers.InvokeOnUIThread(action);
    }
}
