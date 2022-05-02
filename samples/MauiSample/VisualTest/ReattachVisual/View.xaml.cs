namespace MauiSample.VisualTest.ReattachVisual;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    private bool _isInVisualTree = true;

    public View()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
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
