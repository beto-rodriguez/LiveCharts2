namespace MauiSample.VisualTest.Tabs;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class View : ContentPage
{
    public View()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        TabbedPage.SelectedItem = null;
    }
}
