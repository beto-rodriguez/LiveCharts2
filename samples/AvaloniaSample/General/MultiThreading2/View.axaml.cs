using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ViewModelsSamples.General.MultiThreading2;

namespace AvaloniaSample.General.MultiThreading2;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();

        var viewModel = new ViewModel(InvokeOnUIThread);
        DataContext = viewModel;
    }

    // this method takes another function as an argument.
    // the idea is that we are invoking the passed action in the UI thread
    // but the UI framework will let the view model how to do this.
    // we will pass the InvokeOnUIThread method to our view model so the view model knows how
    // to invoke an action in the UI thred.
    private void InvokeOnUIThread(Action action)
    {
        Dispatcher.UIThread.Post(action);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
