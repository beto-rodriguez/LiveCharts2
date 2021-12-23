using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSample.Home;

public class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }

    public MainWindowViewModel? MainWindowVM { get; set; }

    private void SetDark(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindowVM?.SetDark();
    }

    private void SetLight(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindowVM?.SetLight();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
