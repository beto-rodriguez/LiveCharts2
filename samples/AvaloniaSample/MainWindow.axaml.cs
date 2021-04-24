using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

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
            if ((sender as Border)?.DataContext is not string ctx) throw new Exception("Sample not found");
            Active = ctx.Replace('/', '.');
            content.Content = Activator.CreateInstance(null, $"AvaloniaSample.{Active}.View")?.Unwrap();
        }

        public string Active { get; set; }
        public string[] Samples { get; set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
