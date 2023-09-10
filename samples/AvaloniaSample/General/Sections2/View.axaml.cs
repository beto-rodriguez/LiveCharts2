using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSample.General.Sections2;

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
