using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using ViewModelsSamples.Bars.Race;

namespace MauiSample.Bars.Race
{
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
                Device.BeginInvokeOnMainThread(vm.RandomIncrement);
                await Task.Delay(1500);
            }
        }
    }
}
