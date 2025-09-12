using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using ViewModelsSamples.Lines.AutoUpdate;

namespace WinUISample.Lines.AutoUpdate;

public sealed partial class View : UserControl
{
    private bool? _isStreaming = false;

    public View()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
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
