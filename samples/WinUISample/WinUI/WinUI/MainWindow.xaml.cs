using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUISample
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Samples = ViewModelsSamples.Index.Samples;
            grid.DataContext = this;
        }

        public string[] Samples { get; set; }

        private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is not string ctx) throw new Exception("Sample not found");

            content.Content = Activator.CreateInstance(null, $"WinUISample.{ctx.Replace('/', '.')}.View").Unwrap();
        }
    }
}
