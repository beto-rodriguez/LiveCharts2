using System.Threading.Tasks;
using System.Windows.Controls;
using ViewModelsSamples.Pies.AutoUpdate;

namespace WPFSample.Pies.AutoUpdate;

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
        isStreaming = isStreaming is null ? true : !isStreaming;

        while (isStreaming.Value)
        {
            vm.RemoveSeries();
            vm.AddSeries();
            await Task.Delay(1000);
        }
    }
}
