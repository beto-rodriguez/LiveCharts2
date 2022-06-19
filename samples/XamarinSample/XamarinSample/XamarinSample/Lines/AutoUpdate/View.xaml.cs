using System.Threading.Tasks;
using ViewModelsSamples.Lines.AutoUpdate;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinSample.Lines.AutoUpdate;

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
            vm.RemoveItem();
            vm.AddItem();
            await Task.Delay(1000);
        }
    }
}
