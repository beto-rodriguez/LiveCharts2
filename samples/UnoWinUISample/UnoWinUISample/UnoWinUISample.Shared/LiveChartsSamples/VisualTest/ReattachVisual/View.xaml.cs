using Microsoft.UI.Xaml.Controls;

namespace UnoWinUISample.VisualTest.ReattachVisual;

public sealed partial class View : UserControl
{
    private bool _isInVisualTree = true;

    public View()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_isInVisualTree)
        {
            _ = parent.Children.Remove(chart);
            _ = parent.Children.Remove(pieChart);
            _ = parent.Children.Remove(polarChart);
            _isInVisualTree = false;
            return;
        }

        parent.Children.Add(chart);
        parent.Children.Add(pieChart);
        parent.Children.Add(polarChart);
        _isInVisualTree = true;
    }
}
