using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ViewModelsSamples.Bars.Race;

namespace WPFSample.Bars.Race
{
    /// <summary>
    /// Interaction logic for View.xaml
    /// </summary>
    public partial class View : UserControl
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
                Application.Current.Dispatcher.Invoke(vm.RandomIncrement);
                await Task.Delay(1500);
            }
        }
    }
}
