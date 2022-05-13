using System.Threading.Tasks;
using System.Windows.Controls;
using ViewModelsSamples.Lines.AutoUpdate;

namespace WPFSample.Lines.AutoUpdate;

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
            vm.RemoveItem();
            vm.AddItem();
            await Task.Delay(1000);
        }
    }
}
