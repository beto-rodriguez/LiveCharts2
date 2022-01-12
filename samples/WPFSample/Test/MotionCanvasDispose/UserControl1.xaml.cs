using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using LiveChartsCore.SkiaSharpView.WPF;

namespace WPFSample.Test.MotionCanvasDispose;
/// <summary>
/// Interaction logic for UserControl1.xaml
/// </summary>
public partial class UserControl1 : UserControl
{
    public UserControl1()
    {
        InitializeComponent();

        Loaded += UserControl1_Loaded;
    }

    private async void UserControl1_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        var canvas = (MotionCanvas)FindName("canvas");
        await Task.Delay(TimeSpan.FromMilliseconds(10)); // workaround to wait for the canvas to be ready...
        ViewModelsSamples.Test.MotionCanvasDispose.ViewModel.Generate(canvas.CanvasCore);
    }
}
