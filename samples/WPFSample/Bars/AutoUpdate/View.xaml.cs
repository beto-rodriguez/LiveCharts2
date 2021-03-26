using System.Threading.Tasks;
using System.Windows.Controls;
using ViewModelsSamples.Bars.AutoUpdate;

namespace WPFSample.Bars.AutoUpdate
{
    /// <summary>
    /// Interaction logic for View.xaml
    /// </summary>
    public partial class View : UserControl
    {
        private bool? isStreaming = false;

        public View()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = (ViewModel)DataContext;
            if (isStreaming.Value) isStreaming = false;
            isStreaming = isStreaming == null ? true : !isStreaming;

            while(isStreaming.Value)
            {
                vm.RemoveFirstItem();

                vm.AddRandomItem();

                await Task.Delay(2000);
            }
        }
    }
}
