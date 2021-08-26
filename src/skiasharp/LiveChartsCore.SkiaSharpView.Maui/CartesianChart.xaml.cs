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

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace LiveChartsCore.SkiaSharpView.Maui
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartesianChart : ContentView
    {
        private SKPoint? touchLocation;

        public CartesianChart()
        {
            InitializeComponent();
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            if (e.InContact)
                touchLocation = e.Location;
            else
                touchLocation = null;

            skiaView.InvalidateSurface();

            e.Handled = true;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            // the the canvas and properties
            var canvas = e.Surface.Canvas;

            // get the screen density for scaling
            var scale = (float)(e.Info.Width / skiaView.Width);

            // handle the device screen density
            canvas.Scale(scale);

            // make sure the canvas is blank
            canvas.Clear(SKColors.White);

            // decide what the text looks like
            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = 24
            };

            // adjust the location based on the pointer
            var coord = (touchLocation is SKPoint loc)
                ? new SKPoint(loc.X / scale, loc.Y / scale)
                : new SKPoint((float)skiaView.Width / 2, ((float)skiaView.Height + paint.TextSize) / 2);

            // draw some text
            canvas.DrawText("holaaa", coord, paint);
        }
    }
}
