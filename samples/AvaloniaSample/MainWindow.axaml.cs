using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;

namespace AvaloniaSample
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Samples = ViewModelsSamples.Index.Samples;
            DataContext = this;
        }

        private void OnPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            var content = this.FindControl<ContentControl>("content");
            var ctx = (sender as Border)?.DataContext as string;
            if (ctx == null) throw new Exception("Sample not found");
            content.Content = Activator.CreateInstance(null, $"AvaloniaSample.{ctx.Replace('/', '.')}.View")?.Unwrap();
        }

        public string[] Samples { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
