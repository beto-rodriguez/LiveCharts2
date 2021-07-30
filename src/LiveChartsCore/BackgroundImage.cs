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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChartsCore
{
    /// <summary>
    /// Image information for chart background.
    /// </summary>
    public class BackgroundImage
    {
        /// <summary>
        /// Bacground image format
        /// </summary>
        public enum ImageFormat
        {
            /// <summary>
            /// Mono 8bit
            /// </summary>
            Mono8,
            /// <summary>
            /// RGB 24bit
            /// </summary>
            RGB8,
            /// <summary>
            /// RGBA 32bit
            /// </summary>
            RGBA8,
            /// <summary>
            /// BGR 24bit
            /// </summary>
            BGR8,
            /// <summary>
            /// BGRA 32bit
            /// </summary>
            BGRA8,

        }
        /// <summary>
        /// Image rendering mode
        /// </summary>
        public enum ImageRenderMode
        {
            /// <summary>
            /// stretch mode
            /// </summary>
            Stretch,
            /// <summary>
            /// keep aspect ratio
            /// </summary>
            KeepAspectRatio,
            /// <summary>
            /// sync with real data
            /// </summary>
            SyncWithData
        }
        /// <summary>
        /// Determine whether use background image.
        /// </summary>
        public bool UseBackImage { get; set; } = false;
        /// <summary>
        /// RawImage Buffer
        /// </summary>
        public byte[]? ImageBuffer { get; private set; } = null;
        /// <summary>
        /// Image width
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Image height
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Sync position between real data and image
        /// </summary>
        public ImageMapping ImageMapping { get; set; }
        /// <summary>
        /// Background image rendering mode
        /// </summary>
        public ImageRenderMode RenderMode { get; set; }
        /// <summary>
        /// Background image format
        /// </summary>
        public ImageFormat Format { get; private set; }

        /// <summary>
        /// Opacity of image. range: 0~100
        /// This property is available only 3-channel images.
        /// </summary>
        public int Opacity { get; private set; } = 100;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="imageFormat">imageformat</param>
        /// <param name="imageBuffer">imagebuffer</param>
        /// <param name="width">width of image</param>
        /// <param name="height">height of image</param>
        /// <param name="opacity">Opacity of image. range: 0~100</param>
        public BackgroundImage(ImageFormat imageFormat, byte[] imageBuffer, int width, int height, int opacity = 100)
        {
            UseBackImage = true;
            Format = imageFormat;
            Width = width;
            Height = height;
            opacity = Math.Min(100, opacity);
            opacity = Math.Max(0, opacity);
            Opacity = opacity;

            switch (Format)
            {
                case ImageFormat.RGB8:
                    if (imageBuffer.Length != width * height * 3) { return; }
                    ImageBuffer = RgbToRgba(imageBuffer, width, height);
                    break;
                case ImageFormat.RGBA8:
                    if (imageBuffer.Length != width * height * 4) { return; }
                    ImageBuffer = new byte[width * height * 4];
                    imageBuffer.CopyTo(ImageBuffer, 0);
                    break;
                case ImageFormat.BGR8:
                    if (imageBuffer.Length != width * height * 3) { return; }
                    ImageBuffer = BgrToRgba(imageBuffer, width, height);
                    break;
                case ImageFormat.BGRA8:
                    if (imageBuffer.Length != width * height * 4) { return; }
                    ImageBuffer = BgraToRgba(imageBuffer, width, height);
                    break;
                case ImageFormat.Mono8:
                    if (imageBuffer.Length != width * height) { return; }
                    ImageBuffer = GreyToRgba(imageBuffer, width, height);
                    break;
                default:
                    //not reached
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }
        /// <summary>
        /// returns deep copied instance
        /// </summary>
        /// <returns>cloned instance</returns>
        public BackgroundImage Clone()
        {
            var image = (BackgroundImage)MemberwiseClone();
            if (ImageBuffer != null)
            {
                image.ImageBuffer = new byte[ImageBuffer.Length];
                ImageBuffer.CopyTo(image.ImageBuffer, 0);
            }
            return image;
        }

        /// <summary>
        /// Convert RGB to RGBA
        /// </summary>
        /// <param name="rgb">RGB byte array</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <returns></returns>
        private byte[] RgbToRgba(byte[] rgb, int width, int height)
        {
            var byteOpacity = (byte)(0xFF * Opacity / 100f);
            var arrRgba = new byte[width * height * 4];
            for (var i = 0; i < rgb.Length; i++)
            {
                arrRgba[i] = rgb[i];
                arrRgba[++i] = rgb[i];
                arrRgba[++i] = rgb[i];
                arrRgba[i + 1] = byteOpacity;
            }
            return arrRgba;
        }

        /// <summary>
        /// Convert BGR to RGBA
        /// </summary>
        /// <param name="bgr">BGR byte array</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <returns></returns>
        private byte[] BgrToRgba(byte[] bgr, int width, int height)
        {
            var byteOpacity = (byte)(0xFF * Opacity / 100f);
            var arrRgba = new byte[width * height * 4];
            for (var i = 0; i < bgr.Length; i++)
            {
                arrRgba[i] = bgr[i + 2];
                arrRgba[++i] = bgr[i];
                arrRgba[++i] = bgr[i - 2];
                arrRgba[i + 1] = byteOpacity;
            }
            return arrRgba;
        }

        /// <summary>
        /// Convert Grey to RGBA
        /// </summary>
        /// <param name="grey"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private byte[] GreyToRgba(byte[] grey, int width, int height)
        {
            var byteOpacity = (byte)(0xFF * Opacity / 100f);
            var arrRgba = new byte[width * height * 4];
            for (var i = 0; i < grey.Length; i++)
            {
                var targetIdx = i * 4;
                arrRgba[targetIdx] = grey[i];
                arrRgba[targetIdx + 1] = grey[i];
                arrRgba[targetIdx + 2] = grey[i];
                arrRgba[targetIdx + 3] = byteOpacity;
            }
            return arrRgba;
        }

        /// <summary>
        /// Convert BGRA to RGBA
        /// </summary>
        /// <param name="bgra"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private byte[] BgraToRgba(byte[] bgra, int width, int height)
        {
            var arrRgba = new byte[width * height * 4];
            for (var i = 0; i < bgra.Length; i++)
            {
                arrRgba[i] = bgra[i + 2];
                arrRgba[++i] = bgra[i];
                arrRgba[++i] = bgra[i - 2];
                arrRgba[++i] = bgra[i];
            }
            return arrRgba;
        }
    }

    /// <summary>
    /// Sync position between real data and image
    /// </summary>
    public struct ImageMapping
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="leftTopMappingPoint">lefttop mapping point</param>
        /// <param name="rightBottomMappingPoint">rightbottom mapping point</param>
        public ImageMapping(PointF leftTopMappingPoint, PointF rightBottomMappingPoint)
        {
            LefTopMapping = leftTopMappingPoint;
            RightBottomMapping = rightBottomMappingPoint;
        }

        /// <summary>
        /// Sync position between real data and lefttop of image.
        /// </summary>
        public PointF LefTopMapping { get; set; }
        /// <summary>
        /// Sync position between real data and rightbottom of image.
        /// </summary>
        public PointF RightBottomMapping { get; set; }
    }
}
