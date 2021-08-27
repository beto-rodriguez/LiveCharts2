using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Maui.Controls;

namespace MauiSample
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            Series = new ISeries[]
            {
                new PolarLineSeries<int>
                {
                    Values = new int[] { 15,14,13,12,11,10,9,8,7,6,5 }
                }
            };

            BindingContext = this;
		}

        public ISeries[] Series { get; set; }
    }
}
