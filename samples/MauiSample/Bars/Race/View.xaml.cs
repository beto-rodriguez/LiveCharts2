using ViewModelsSamples.Bars.Race;

namespace MauiSample.Bars.Race;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    public View()
    {
        InitializeComponent();
        Update();
    }

    public async void Update()
    {
        var vm = (ViewModel)BindingContext;
        while (true)
        {
            _ = Dispatcher.Dispatch(vm.RandomIncrement);
            await Task.Delay(100);
        }
    }
}
