using System.Threading.Tasks;
using System.Windows.Controls;
using ViewModelsSamples.Bars.AutoUpdate;

namespace WPFSample.Bars.AutoUpdate;

/// <summary>
/// Interaction logic for View.xaml
/// </summary>
public partial class View : UserControl
{
    private bool? _isStreaming = false;

    public View()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var vm = (ViewModel)DataContext;
        _isStreaming = _isStreaming is null ? true : !_isStreaming;

        while (_isStreaming.Value)
        {
            vm.RemoveItem();
            vm.AddItem();
            await Task.Delay(1000);
        }
    }
}
