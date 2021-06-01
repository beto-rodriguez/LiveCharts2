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

using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    /// <inheritdoc cref="IChartLegend{TDrawingContext}" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultLegend : ContentView, IChartLegend<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate _defaultTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLegend"/> class.
        /// </summary>
        public DefaultLegend()
        {
            InitializeComponent();
            _defaultTemplate = (DataTemplate)Resources["defaultTemplate"];
        }

        /// <summary>
        /// Gets or sets the legend template.
        /// </summary>
        /// <value>
        /// The legend template.
        /// </value>
        public DataTemplate? LegendTemplate { get; set; }

        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        public IEnumerable<ISeries> Series { get; set; } = Enumerable.Empty<ISeries>();

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string? LegendFontFamily { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public double LegendFontSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        public Color LegendTextColor { get; set; }

        /// <summary>
        /// Gets or sets the font attributes.
        /// </summary>
        /// <value>
        /// The font attributes.
        /// </value>
        public FontAttributes LegendFontAttributes { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public StackOrientation LegendOrientation { get; set; }

        // <summary>
        /// Gets or sets the color of the tooltip background.
        /// </summary>
        /// <value>
        /// The color of the tooltip background.
        /// </value>
        public Color LegendBackgroundColor { get; set; }

        void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
        {
            var mobileChart = (IMobileChart)chart.View;
            var series = chart.DrawableSeries;
            var legendOrientation = chart.LegendOrientation;
            var legendPosition = chart.LegendPosition;
            Series = series;

            switch (legendPosition)
            {
                case LegendPosition.Hidden:
                    IsVisible = false;
                    break;
                case LegendPosition.Top:
                    IsVisible = true;
                    if (legendOrientation == LiveChartsCore.Measure.LegendOrientation.Auto) LegendOrientation = StackOrientation.Horizontal;
                    mobileChart.LayoutGrid.ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Star }
                    };
                    mobileChart.LayoutGrid.RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Star }
                    };
                    Grid.SetRow(mobileChart.Legend, 0);
                    Grid.SetRow(mobileChart.Canvas, 1);
                    Grid.SetColumn(mobileChart.Legend, 0);
                    Grid.SetColumn(mobileChart.Canvas, 0);
                    break;
                case LegendPosition.Left:
                    IsVisible = true;
                    if (legendOrientation == LiveChartsCore.Measure.LegendOrientation.Auto) LegendOrientation = StackOrientation.Vertical;
                    mobileChart.LayoutGrid.RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = GridLength.Star }
                    };
                    mobileChart.LayoutGrid.ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Star }
                    };
                    Grid.SetColumn(mobileChart.Legend, 0);
                    Grid.SetColumn(mobileChart.Canvas, 1);
                    Grid.SetRow(mobileChart.Legend, 0);
                    Grid.SetRow(mobileChart.Canvas, 0);
                    break;
                case LegendPosition.Right:
                    IsVisible = true;
                    if (legendOrientation == LiveChartsCore.Measure.LegendOrientation.Auto) LegendOrientation = StackOrientation.Vertical;
                    mobileChart.LayoutGrid.RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = GridLength.Star }
                    };
                    mobileChart.LayoutGrid.ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto }
                    };
                    Grid.SetColumn(mobileChart.Canvas, 0);
                    Grid.SetColumn(mobileChart.Legend, 1);
                    Grid.SetRow(mobileChart.Legend, 0);
                    Grid.SetRow(mobileChart.Canvas, 0);
                    break;
                case LegendPosition.Bottom:
                    IsVisible = true;
                    if (legendOrientation == LiveChartsCore.Measure.LegendOrientation.Auto) LegendOrientation = StackOrientation.Horizontal;
                    mobileChart.LayoutGrid.ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Star }
                    };
                    mobileChart.LayoutGrid.RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = GridLength.Star },
                        new RowDefinition { Height = GridLength.Auto }
                    };
                    Grid.SetRow(mobileChart.Canvas, 0);
                    Grid.SetRow(mobileChart.Legend, 1);
                    Grid.SetColumn(mobileChart.Legend, 0);
                    Grid.SetColumn(mobileChart.Canvas, 0);
                    break;
                default:
                    break;
            }

            if (legendOrientation != LiveChartsCore.Measure.LegendOrientation.Auto)
                LegendOrientation = legendOrientation == LiveChartsCore.Measure.LegendOrientation.Horizontal
                    ? StackOrientation.Horizontal
                    : StackOrientation.Vertical;

            LegendTemplate = mobileChart.LegendTemplate;
            LegendBackgroundColor = mobileChart.TooltipBackground;
            LegendFontFamily = mobileChart.LegendFontFamily;
            LegendFontSize = mobileChart.LegendFontSize;
            LegendTextColor = mobileChart.LegendTextColor;
            LegendFontAttributes = mobileChart.LegendFontAttributes;

            BuildContent();
        }

        /// <summary>
        /// Builds the content.
        /// </summary>
        protected void BuildContent()
        {
            var template = LegendTemplate ?? _defaultTemplate;
            if (template.CreateContent() is not View view) return;

            view.BindingContext = new LegendBindingContext
            {
                Series = Series,
                FontFamily = LegendFontFamily,
                FontSize = LegendFontSize,
                TextColor = LegendTextColor,
                FontAttributes = LegendFontAttributes,
                Orientation = LegendOrientation,
                BackgroundColor = LegendBackgroundColor
            };

            Content = view;
        }
    }
}
