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
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView
{
    /// <summary>
    /// Defines a visual frame in the draw margin of the chart.
    /// </summary>
    public class DrawMarginFrame : DrawMarginFrame<RectangleGeometry, SkiaSharpDrawingContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawMarginFrame"/> class.
        /// </summary>
        public DrawMarginFrame()
        {
            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();

            foreach (var rule in initializer.DrawMarginFrameBuilder) rule(this);
        }

        /// <summary>
        /// Measures the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public override void Measure(Chart<SkiaSharpDrawingContext> chart)
        {
            if (BackImage != null)
            {
                //make background image.
                var paintTask = BackImage as BackImagePaintTask;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if ((paintTask.BackImage is not null) && paintTask.BackImage.UseBackImage && (paintTask.BackImage.ImageBuffer != null))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                {
                    if (paintTask.BackImageBitmap == null)
                    {
                        var bitmap = new SKBitmap(new SKImageInfo(paintTask.BackImage.Width, paintTask.BackImage.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul));

                        unsafe
                        {
                            fixed (byte* ptr = paintTask.BackImage.ImageBuffer)
                            {
                                bitmap.SetPixels((IntPtr)ptr);
                            }
                        }
                        paintTask.BackImageBitmap = bitmap;
                    }
                    BackgroundImage.ImageRenderMode renderMode;
                    if (paintTask.BackImage.RenderMode == BackgroundImage.ImageRenderMode.MappingWithData)
                    {
                        //if chart type is not cartesianchart, render-mode is forcefully set to ImageRenderMode.Stretch.
                        renderMode = chart.GetType() == typeof(CartesianChart<SkiaSharpDrawingContext>)
                            ? BackgroundImage.ImageRenderMode.MappingWithData
                            : BackgroundImage.ImageRenderMode.Stretch;
                        var cartesianChart = (CartesianChart<SkiaSharpDrawingContext>)chart;
                        if (renderMode == BackgroundImage.ImageRenderMode.MappingWithData)
                        {
                            if ((cartesianChart.XAxes == null) || (cartesianChart.XAxes.Length <= 0) ||
                            (cartesianChart.YAxes == null) || (cartesianChart.YAxes.Length <= 0))
                            {
                                renderMode = BackgroundImage.ImageRenderMode.Stretch;
                            }
                        }
                        else
                        {
                            //do nothing
                        }
                    }
                    else
                    {
                        renderMode = paintTask.BackImage.RenderMode;
                    }

                    var width = (int)chart.DrawMarginSize.Width;
                    var height = (int)chart.DrawMarginSize.Height;
                    SKBitmap? result = null;
                    if (renderMode == BackgroundImage.ImageRenderMode.MappingWithData)
                    {
                        #region RenderMode-MappingWithData                        
                        var cartesianChart = (CartesianChart<SkiaSharpDrawingContext>)chart;
                        var secondaryAxis = cartesianChart.XAxes[0];
                        var primaryAxis = cartesianChart.YAxes[0];
                        var secondaryScale = new Scaler(cartesianChart.DrawMarginLocation, cartesianChart.DrawMarginSize, secondaryAxis);
                        var primaryScale = new Scaler(cartesianChart.DrawMarginLocation, cartesianChart.DrawMarginSize, primaryAxis);

                        var topLimit = primaryAxis.MaxLimit.HasValue ? primaryAxis.MaxLimit : primaryAxis.DataBounds.Max;
                        var bottomLimit = primaryAxis.MinLimit.HasValue ? primaryAxis.MinLimit : primaryAxis.DataBounds.Min;
                        var leftLimit = secondaryAxis.MinLimit.HasValue ? secondaryAxis.MinLimit : secondaryAxis.DataBounds.Min;
                        var rightLimit = secondaryAxis.MaxLimit.HasValue ? secondaryAxis.MaxLimit : secondaryAxis.DataBounds.Max;

                        var map = paintTask.BackImage.ImageMapping;

                        //valid range check
                        if ((leftLimit >= map.RightBottomMapping.X) ||
                            (rightLimit <= map.LefTopMapping.X) ||
                            (topLimit <= map.RightBottomMapping.Y) ||
                            (bottomLimit >= map.LefTopMapping.Y) ||
                            ((rightLimit - leftLimit) <= 0) ||
                            ((topLimit - bottomLimit) <= 0))
                        {
                            //do nothing
                        }
                        else
                        {
                            if ((leftLimit == map.LefTopMapping.X) &&
                                (topLimit == map.LefTopMapping.Y) &&
                                (rightLimit == map.RightBottomMapping.X) &&
                                (bottomLimit == map.RightBottomMapping.Y))
                            {
                                result = paintTask.BackImageBitmap.Resize(new SKImageInfo(width, height, SKColorType.Rgba8888), SKFilterQuality.High);
                            }
                            else
                            {
                                var backImageMappingWidth = map.RightBottomMapping.X - map.LefTopMapping.X;
                                var backImageMappingHeight = map.LefTopMapping.Y - map.RightBottomMapping.Y;


                                var cropLeft = (leftLimit <= map.LefTopMapping.X) ? map.LefTopMapping.X : leftLimit;
                                var cropRight = (rightLimit >= map.RightBottomMapping.X) ? map.RightBottomMapping.X : rightLimit;
                                var cropTop = (topLimit >= map.LefTopMapping.Y) ? map.LefTopMapping.Y : topLimit;
                                var cropBottom = (bottomLimit <= map.RightBottomMapping.Y) ? map.RightBottomMapping.Y : bottomLimit;


                                var cropLeftPixel = (int)((paintTask.BackImageBitmap.Width - 1) * ((cropLeft - map.LefTopMapping.X) / backImageMappingWidth));
                                var cropRightPixel = (int)((paintTask.BackImageBitmap.Width - 1) * ((cropRight - map.LefTopMapping.X) / backImageMappingWidth));
                                var cropTopPixel = (int)((paintTask.BackImageBitmap.Width - 1) * ((map.LefTopMapping.Y - cropTop) / backImageMappingHeight));
                                var cropBottomPixel = (int)((paintTask.BackImageBitmap.Width - 1) * ((map.LefTopMapping.Y - cropBottom) / backImageMappingWidth));
                                var roi = new SKRectI(cropLeftPixel, cropTopPixel, cropRightPixel, cropBottomPixel);

                                var croppedBitmap = new SKBitmap(new SKImageInfo(cropRightPixel - cropLeftPixel + 1, cropBottomPixel - cropTopPixel + 1, SKColorType.Rgba8888));
                                if (paintTask.BackImageBitmap.ExtractSubset(croppedBitmap, roi) == false)
                                {
                                    System.Diagnostics.Debug.Assert(false);
                                }
                                //resize the cropped image.
                                var xScale = (cropRight - cropLeft) / (rightLimit - leftLimit);
                                var yScale = (cropTop - cropBottom) / (topLimit - bottomLimit);
                                var resizeTargetWidth = (xScale != 1) ? width * xScale : width;
                                var resizeTargetHeight = (yScale != 1) ? height * yScale : height;
                                var croppedBitmapResized = new SKBitmap(new SKImageInfo((int)resizeTargetWidth, (int)resizeTargetHeight, SKColorType.Rgba8888));
                                //var croppedBitmapResized = croppedBitmap.Resize(new SKImageInfo((int)resizeTargetWidth, (int)resizeTargetHeight, SKColorType.Rgba8888), SKFilterQuality.High);
                                _ = croppedBitmap.ScalePixels(croppedBitmapResized, SKFilterQuality.High);
                                //create blank image
                                result = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888));
                                var canvas = new SKCanvas(result);
                                canvas.Clear(new SKColor(0, 0, 0, 0));

                                //mapping cropped image to blank image.
                                var offsetY = (topLimit - cropTop) / (topLimit - bottomLimit) * height;
                                var offsetX = (cropLeft - leftLimit) / (rightLimit - leftLimit) * width;
                                canvas.DrawBitmap(croppedBitmapResized, (float)offsetX, (float)offsetY);
                                croppedBitmapResized.Dispose();
                            }
                        }
                        #endregion
                    }
                    else if (renderMode == BackgroundImage.ImageRenderMode.KeepAspectRatio)
                    {

                        #region RenderMode-KeepAspectRatio
                        //if srcAspectRatio > 1, landscape.
                        var srcAspectRatio = (float)paintTask.BackImageBitmap.Width / paintTask.BackImageBitmap.Height;
                        var targetAspectRatio = (float)width / height;
                        if(srcAspectRatio == targetAspectRatio)
                        {
                            //Only resize
                            result = paintTask.BackImageBitmap.Resize(new SKImageInfo(width, height, SKColorType.Rgba8888), SKFilterQuality.High);
                        }
                        else
                        {
                            //adjust aspect ratio
                            result = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888));
                            var canvas = new SKCanvas(result);
                            canvas.Clear(new SKColor(0, 0, 0, 0));
                            if (srcAspectRatio > targetAspectRatio)
                            {
                                var resizedHeight = (int)(width / srcAspectRatio);
                                var resizedSrc = paintTask.BackImageBitmap.Resize(new SKImageInfo(width, resizedHeight, SKColorType.Rgba8888), SKFilterQuality.High);
                                var offsetX = 0f;
                                var offsetY = (height - resizedHeight) / 2f;
                                //Draw blank area on vertical spaces.
                                canvas.DrawBitmap(resizedSrc, offsetX, offsetY);
                            }
                            else
                            {
                                //Draw blank area on horizontal spaces.
                                var resizedWidth = (int)(height * srcAspectRatio);
                                var resizedSrc = paintTask.BackImageBitmap.Resize(new SKImageInfo(resizedWidth, height, SKColorType.Rgba8888), SKFilterQuality.High);
                                var offsetX = (width - resizedWidth) / 2f;
                                var offsetY = 0f;
                                //Draw blank area on vertical spaces.
                                canvas.DrawBitmap(resizedSrc, offsetX, offsetY);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        result = paintTask.BackImageBitmap.Resize(new SKImageInfo(width, height, SKColorType.Rgba8888), SKFilterQuality.High);
                    }
                    paintTask.BackImageBitmapForRendering = result;
                }
            }

            base.Measure(chart);
        }
    }
}
