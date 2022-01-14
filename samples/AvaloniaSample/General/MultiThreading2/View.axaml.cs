using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ViewModelsSamples.General.MultiThreading2;

namespace AvaloniaSample.General.MultiThreading2;

public class View : UserControl
{
    public View()
    {
        InitializeComponent();

        var viewModel = new ViewModel(action =>
        {
            Dispatcher.UIThread.Post(action);
        });

        DataContext = viewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
