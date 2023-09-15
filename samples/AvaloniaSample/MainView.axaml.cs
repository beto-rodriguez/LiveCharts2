// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using Avalonia.Controls;

namespace AvaloniaSample;
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
        LoadContent("Design.LinearGradients");
    }

    private void OnPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if ((sender as Border)?.DataContext is not string ctx) throw new Exception("Sample not found");
        LoadContent(ctx.Replace('/', '.'));
    }

    private void LoadContent(string view)
    {
        var content = this.FindControl<ContentControl>("content")!;
        content.Content = Activator.CreateInstance(null!, $"AvaloniaSample.{view}.View")?.Unwrap();
        if (DataContext is not MainWindowViewModel dc) throw new Exception();
    }
}
