using Microsoft.UI.Xaml.Input;

namespace UnoPlatformSample.Presentation;

public sealed partial class MainPage : Page
{
    private bool _isMenuOpen = false;

    public MainPage()
    {
        this.InitializeComponent();

        Samples = ViewModelsSamples.Index.Samples;
        grid.DataContext = this;

        LoadSample("Design/LinearGradients");

    }

    public string[] Samples { get; set; }

    private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        grid.ColumnDefinitions[0].Width = new GridLength(0);

        var ctx = (string)((FrameworkElement)sender).DataContext;
        LoadSample(ctx);
    }

    private void LoadSample(string route)
    {
        route = route.Replace('/', '.');
        var t = Type.GetType($"UnoWinUISample.{route}.View") ?? throw new FileNotFoundException($"{route} not found!");
        content.Content = Activator.CreateInstance(t);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        grid.ColumnDefinitions[0].Width = _isMenuOpen ? new GridLength(0) : new GridLength(250);
        _isMenuOpen = !_isMenuOpen;
    }
}
