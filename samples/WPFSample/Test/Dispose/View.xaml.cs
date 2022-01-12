using System.Windows.Controls;

namespace WPFSample.Test.Dispose;

/// <summary>
/// Interaction logic for View.xaml
/// </summary>
public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var content = (ContentControl)FindName("content");
        content.Content = new UserControl1();
    }
}
