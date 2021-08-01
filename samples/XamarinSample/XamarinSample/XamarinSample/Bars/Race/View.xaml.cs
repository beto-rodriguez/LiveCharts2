using System.Threading.Tasks;
using ViewModelsSamples.Bars.Race;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinSample.Bars.Race
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