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
                new LineSeries<int>
                {
                    Values = new int[] { 4, 2, 6, 2, 5 }
                }
            };

            BindingContext = this;
		}

        public ISeries[] Series { get; set; }
    }
}
