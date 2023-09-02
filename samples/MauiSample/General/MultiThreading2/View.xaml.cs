using ViewModelsSamples.General.MultiThreading2;

namespace MauiSample.General.MultiThreading2;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    public View()
    {
        InitializeComponent();
        var vm = new ViewModel(InvokeOnUIThread);
        BindingContext = vm;
    }

    // this method takes another function as an argument.
    // the idea is that we are invoking the passed action in the UI thread
    // but the UI framework will let the view model how to do this.
    // we will pass the InvokeOnUIThread method to our view model so the view model knows how
    // to invoke an action in the UI thred.
    private void InvokeOnUIThread(Action action)
    {
        MainThread.BeginInvokeOnMainThread(action);
    }
}
