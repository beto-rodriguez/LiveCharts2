using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSample.General.MapPoints;

public partial class View : UserControl
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
