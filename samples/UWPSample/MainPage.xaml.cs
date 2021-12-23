using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            Samples = ViewModelsSamples.Index.Samples;
            grid.DataContext = this;
        }

        public string[] Samples { get; set; }

        private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var ctx = (string)((FrameworkElement)sender).DataContext;

            var t = Type.GetType($"UWPSample.{ctx.Replace('/', '.')}.View");
            content.Content = Activator.CreateInstance(t);
        }
    }
}
