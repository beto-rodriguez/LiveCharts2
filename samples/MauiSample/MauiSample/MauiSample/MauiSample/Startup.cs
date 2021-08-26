using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Controls.Xaml;
using SkiaSharp.Views.Maui.Controls.Compatibility;
using SkiaSharp.Views.Maui.Controls;

[assembly: XamlCompilationAttribute(XamlCompilationOptions.Compile)]

namespace MauiSample
{
	public class Startup : IStartup
	{
		public void Configure(IAppHostBuilder appBuilder)
		{
			appBuilder
                .UseSkiaSharpCompatibilityRenderers()
                .UseSkiaSharpHandlers()
                .UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});
		}
	}
}
