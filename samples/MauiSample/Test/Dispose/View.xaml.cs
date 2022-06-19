namespace MauiSample.Test.Dispose;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    private ContentView _currentView;

    public View()
    {
        InitializeComponent();
        _currentView = content;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        _ = container.Remove(_currentView);
        _currentView = null;
        _currentView = new NewPage1();
        Grid.SetRow(_currentView, 1);
        container.Add(_currentView);
    }
}
