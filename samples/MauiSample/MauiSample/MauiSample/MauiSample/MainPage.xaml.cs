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
                new PieSeries<int>
                {
                    Values = new int[] { 4 }
                },
                new PieSeries<int>
                {
                    Values = new int[] { 4 }
                },
                new PieSeries<int>
                {
                    Values = new int[] { 4 }
                },
                new PieSeries<int>
                {
                    Values = new int[] { 4 }
                }
            };

            BindingContext = this;
		}

        public ISeries[] Series { get; set; }
    }
}
