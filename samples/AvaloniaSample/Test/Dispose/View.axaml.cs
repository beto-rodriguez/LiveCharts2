using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSample.Test.Dispose;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var content = this.Find<ContentControl>("content");
        content.Content = new UserControl1();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
