using System;
using Microsoft.UI.Xaml.Controls;
using ViewModelsSamples.General.MultiThreading2;

namespace WinUISample.General.MultiThreading2;

public sealed partial class View : UserControl
{
    public View()
    {
        InitializeComponent();

        var mv = new ViewModel(InvokeOnUIThread);
        DataContext = mv;
    }

    // this method takes another function as an argument.
    // the idea is that we are invoking the passed action in the UI thread
    // but the UI framework will let the view model how to do this.
    // we will pass the InvokeOnUIThread method to our view model so the view model knows how
    // to invoke an action in the UI thred.
    private void InvokeOnUIThread(Action action)
    {
        _ = DispatcherQueue.TryEnqueue(
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
            () => action());
    }
}
