using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ViewModelsSamples.Bars.Race;

namespace AvaloniaSample.Bars.Race;

public class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Update();
    }

    public async void Update()
    {
        var vm = (ViewModel?)DataContext;
        if (vm is null) return;
        while (true)
        {
            await Dispatcher.UIThread.InvokeAsync(vm.RandomIncrement, DispatcherPriority.Background);
            await Task.Delay(1500);
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
