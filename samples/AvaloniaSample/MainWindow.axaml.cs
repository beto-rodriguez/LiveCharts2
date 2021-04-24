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

            DataContext = new MainWindowViewModel();
        }

        private void OnPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            var content = this.FindControl<ContentControl>("content");
            if ((sender as Border)?.DataContext is not string ctx) throw new Exception("Sample not found");
            content.Content = Activator.CreateInstance(null, $"AvaloniaSample.{ctx.Replace('/', '.')}.View")?.Unwrap();
            if (content.Content is not Home.View homeView) return;
            if (DataContext is not MainWindowViewModel dc) throw new Exception();
            homeView.MainWindowVM = dc;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
