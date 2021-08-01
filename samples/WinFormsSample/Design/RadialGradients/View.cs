﻿using LiveChartsCore.SkiaSharpView.WinForms;
using System.Windows.Forms;
using ViewModelsSamples.Design.RadialGradients;

namespace WinFormsSample.Design.RadialGradients
{
    public partial class View : UserControl
    {
        private readonly PieChart pieChart;

        public View()
        {
            InitializeComponent();
            Size = new System.Drawing.Size(50, 50);

            var viewModel = new ViewModel();

            pieChart = new PieChart
            {
                Series = viewModel.Series,
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,

                // out of livecharts properties...
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(50, 50),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            Controls.Add(pieChart);
        }
    }
}
