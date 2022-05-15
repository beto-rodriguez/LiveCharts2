using Microsoft.UI.Xaml.Controls;

namespace WinUISample.Test.Dispose;

public sealed partial class View : UserControl
{
    private bool _isInVisualTree = true;

    public View()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        content.Content = new UserControl1();
    }
}
