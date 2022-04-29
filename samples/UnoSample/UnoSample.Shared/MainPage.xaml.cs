using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnoSample;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
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
        var ctx = (string)((FrameworkElement)sender).DataContext;
        LoadSample(ctx);
    }

    private void LoadSample(string route)
    {
        route = route.Replace('/', '.');
        var t = Type.GetType($"UnoSample.{route}.View");
        content.Content = Activator.CreateInstance(t);
    }
}
