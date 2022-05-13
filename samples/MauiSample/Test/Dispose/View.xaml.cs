namespace MauiSample.Test.Dispose;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    private bool _isInVisualTree = true;

    public View()
    {
        InitializeComponent();
    }
}
