// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultLegend : ContentView, IChartLegend<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate defaultTemplate;

        public DefaultLegend()
        {
            InitializeComponent();
            defaultTemplate = (DataTemplate)Resources["defaultTemplate"];
        }

        public DataTemplate? LegendTemplate { get; set; }

        public IEnumerable<ISeries> Series { get; set; } = Enumerable.Empty<ISeries>();

        public string? FontFamily { get; set; }

        public double FontSize { get; set; }

        public Color TextColor { get; set; }

        public FontAttributes FontAttributes { get; set; }

        public StackOrientation Orientation { get; set; }

        public void Draw(Chart<SkiaSharpDrawingContext> chart)
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
                    if (legendOrientation == LegendOrientation.Auto) Orientation = StackOrientation.Horizontal;
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
                    if (legendOrientation == LegendOrientation.Auto) Orientation = StackOrientation.Vertical;
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
                    if (legendOrientation == LegendOrientation.Auto) Orientation = StackOrientation.Vertical;
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
                    if (legendOrientation == LegendOrientation.Auto) Orientation = StackOrientation.Horizontal;
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

            if (legendOrientation != LegendOrientation.Auto)
                Orientation = legendOrientation == LegendOrientation.Horizontal
                    ? StackOrientation.Horizontal
                    : StackOrientation.Vertical;

            LegendTemplate = mobileChart.LegendTemplate;
            FontFamily = mobileChart.LegendFontFamily;
            FontSize = mobileChart.LegendFontSize;
            TextColor = mobileChart.LegendTextColor;
            FontAttributes = mobileChart.LegendFontAttributes;

            BuildContent();
        }

        protected void BuildContent()
        {
            var template = LegendTemplate ?? defaultTemplate;
            if (template.CreateContent() is not View view) return;

            view.BindingContext = new LegendBindingContext
            {
                Series = Series,
                FontFamily = FontFamily,
                FontSize = FontSize,
                TextColor = TextColor,
                FontAttributes = FontAttributes,
                Orientation = Orientation
            };

            Content = view;
        }
    }
}