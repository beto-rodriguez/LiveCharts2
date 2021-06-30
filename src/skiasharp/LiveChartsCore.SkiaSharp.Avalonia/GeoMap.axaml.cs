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

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments;
using LiveChartsCore.SkiaSharpView.Painting;

namespace LiveChartsCore.SkiaSharpView.Avalonia
{
    /// <summary>
    /// Defines a geographic map.
    /// </summary>
    public partial class GeoMap : UserControl
    {
        private static GeoJsonFile? s_map = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMap"/> class.
        /// </summary>
        public GeoMap()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The projection property.
        /// </summary>
        public static readonly AvaloniaProperty<Projection> ProjectionProperty =
           AvaloniaProperty.Register<CartesianChart, Projection>(nameof(Projection), Projection.Default, inherits: true);

        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        public Projection Projection
        {
            get => (Projection)GetValue(ProjectionProperty);
            set => SetValue(ProjectionProperty, value);
        }

        /// <inheritdoc cref="OnPropertyChanged{T}(AvaloniaPropertyChangedEventArgs{T})"/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(Bounds)) load();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void load()
        {
            var canvas = this.FindControl<MotionCanvas>("canvas");

            var paint = new SolidColorPaintTask
            {
                Color = SkiaSharp.SKColors.DarkGray,
                IsStroke = true,
                IsFill = false,
                StrokeThickness = 1
            };

            var worldMap = s_map ??= Maps.GetWorldMap();

            var d = new double[0][][][];
            var paths = new List<PathGeometry>();
            var projector = Maps.BuildProjector(Projection, new[] { (float)Bounds.Width, (float)Bounds.Height });

            foreach (var feature in worldMap.Features ?? new GeoJsonFeature[0])
            {
                foreach (var geometry in feature.Geometry?.Coordinates ?? d)
                {
                    foreach (var segment in geometry)
                    {
                        var path = new PathGeometry();
                        var isFirst = true;
                        foreach (var point in segment)
                        {
                            var p = projector.ToMap(point);

                            if (isFirst)
                            {
                                isFirst = false;
                                path.AddCommand(new MoveToPathCommand { X = p[0], Y = p[1] });
                                continue;
                            }

                            path.AddCommand(new LineSegment { X = p[0], Y = p[1] });
                        }
                        paths.Add(path);
                    }
                }
            }

            canvas.PaintTasks = new List<PaintSchedule<SkiaSharpDrawingContext>>
            {
                 new PaintSchedule<SkiaSharpDrawingContext>(
                    paint,
                    new HashSet<IDrawable<SkiaSharpDrawingContext>>(paths))
            };
        }
    }
}
