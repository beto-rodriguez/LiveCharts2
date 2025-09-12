using Microsoft.UI.Xaml.Controls;

namespace WinUISample.Test.Dispose;

public sealed partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        content.Content = new UserControl1();
    }
}
