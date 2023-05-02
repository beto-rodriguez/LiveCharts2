using System.Threading.Tasks;
using ViewModelsSamples.Pies.AutoUpdate;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace UnoWinUISample.Pies.AutoUpdate;

public sealed partial class View : UserControl
{
    private bool? _isStreaming = false;

    public View()
    {
        InitializeComponent();
    }

    private async void Button_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var vm = (ViewModel)DataContext;
        _isStreaming = _isStreaming is null ? true : !_isStreaming;

        while (_isStreaming.Value)
        {
            vm.RemoveSeries();
            vm.AddSeries();
            await Task.Delay(1000);
        }
    }
}
