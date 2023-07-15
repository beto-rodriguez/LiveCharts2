using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSample.General.Scrollable;

public class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
