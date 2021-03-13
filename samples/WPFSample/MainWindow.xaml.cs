using SourceChord.FluentWPF;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace WPFSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AcrylicWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Samples = ViewModelsSamples.Index.Samples;
            DataContext = this;
        }

        public string[] Samples { get; set; }

        private void AcrylicWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var ctx = (sender as FrameworkElement).DataContext as string;
            if (ctx == null) throw new Exception("Sample not found");
            content.Content = Activator.CreateInstance(null, $"WPFSample.{ctx.Replace('/', '.')}.View").Unwrap();
        }
    }
}
