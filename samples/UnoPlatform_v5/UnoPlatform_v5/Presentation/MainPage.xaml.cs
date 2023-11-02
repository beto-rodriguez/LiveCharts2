using Microsoft.UI.Xaml.Controls;

namespace UnoPlatform_v5.Presentation
{
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            grid.ColumnDefinitions[0].Width = new GridLength(0);

            var ctx = (string)((FrameworkElement)sender).DataContext;
            LoadSample(ctx);
        }
    }
}
