using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using ViewModelsSamples.Bars.Race;

namespace UWPSample.Bars.Race
{
    public sealed partial class View : UserControl
    {
        public View()
        {
            InitializeComponent();
            Update();
        }

        public async void Update()
        {
            var vm = (ViewModel)DataContext;
            while (true)
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => vm.RandomIncrement());
                await Task.Delay(1500);
            }
        }
    }
}
