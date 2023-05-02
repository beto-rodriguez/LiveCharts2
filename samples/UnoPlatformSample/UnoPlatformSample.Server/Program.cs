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

using Uno.Wasm.Bootstrap.Server;
using UnoPlatformSample.DataContracts.Serialization;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure the JsonOptions to use the generated WeatherForecastContext
    builder.Services.Configure<JsonOptions>(options =>
        options.JsonSerializerOptions.AddContext<WeatherForecastContext>());
    // Configure the RouteOptions to use lowercase URLs
    builder.Services.Configure<RouteOptions>(options =>
        options.LowercaseUrls = true);

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        // Include XML comments for all included assemblies
        Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml")
            .Where(x => x.Contains("UnoPlatformSample")
                && File.Exists(Path.Combine(
                    AppContext.BaseDirectory,
                    $"{Path.GetFileNameWithoutExtension(x)}.dll")))
            .ToList()
            .ForEach(path => c.IncludeXmlComments(path));
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseUnoFrameworkFiles();
    app.MapFallbackToFile("index.html");

    app.MapWeatherApi();
    app.UseStaticFiles();

    await app.RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine("Application terminated unexpectedly");
    Console.Error.WriteLine(ex);
#if DEBUG
	if (System.Diagnostics.Debugger.IsAttached)
	{
		System.Diagnostics.Debugger.Break();
	}
#endif
}
