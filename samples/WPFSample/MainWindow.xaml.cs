using System;
using System.Windows;
using System.Windows.Input;

namespace WPFSample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Samples = ViewModelsSamples.Index.Samples;
        DataContext = this;
    }

    public string[] Samples { get; set; }

    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if ((sender as FrameworkElement).DataContext is not string ctx) throw new Exception("Sample not found");
        content.Content = Activator.CreateInstance(null, $"WPFSample.{ctx.Replace('/', '.')}.View").Unwrap();
    }
}
