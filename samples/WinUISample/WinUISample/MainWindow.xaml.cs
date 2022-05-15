using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace WinUISample;

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
