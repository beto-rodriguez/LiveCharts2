using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using ViewModelsSamples.Bars.Race;

namespace WinUISample.Bars.Race;

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
            _ = DispatcherQueue.TryEnqueue(vm.RandomIncrement);
            await Task.Delay(100);
        }
    }
}
