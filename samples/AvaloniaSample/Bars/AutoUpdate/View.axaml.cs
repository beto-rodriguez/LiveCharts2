using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using ViewModelsSamples.Bars.AutoUpdate;

namespace AvaloniaSample.Bars.AutoUpdate;

public class View : UserControl
{
    private bool? isStreaming = false;

    public View()
    {
        InitializeComponent();
    }

    private async void ButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = (ViewModel?)DataContext;
        if (vm is null) return;

        isStreaming = isStreaming is null ? true : !isStreaming;

        while (isStreaming.Value)
        {
            vm.RemoveFirstItem();
            vm.AddRandomItem();
            await Task.Delay(1000);
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
