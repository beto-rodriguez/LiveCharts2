using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace AvaloniaSample.Test.MotionCanvasDispose;

public partial class UserControl1 : UserControl
{
    public UserControl1()
    {
        InitializeComponent();

        Initialized += UserControl1_Initialized;
    }

    private void UserControl1_Initialized(object? sender, EventArgs e)
    {
        var canvas = this.Find<MotionCanvas>("canvas")
            ?? throw new Exception();

        ViewModelsSamples.Test.MotionCanvasDispose.ViewModel.Generate(canvas.CanvasCore);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
