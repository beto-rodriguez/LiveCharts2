using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSample.Test.Dispose;

public partial class UserControl1 : UserControl
{
    public UserControl1()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
