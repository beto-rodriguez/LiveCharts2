using ViewModelsSamples.Pies.AutoUpdate;

namespace MauiSample.Pies.AutoUpdate;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    private bool? isStreaming = false;

    public View()
    {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, System.EventArgs e)
    {
        var vm = (ViewModel)BindingContext;

        isStreaming = isStreaming is null ? true : !isStreaming;

        while (isStreaming.Value)
        {
            vm.RemoveSeries();
            vm.AddSeries();
            await Task.Delay(1000);
        }
    }
}
