param([string]$configuration = "Release")

dotnet workload install macos
dotnet workload install ios
dotnet workload install maccatalyst 
dotnet workload install android

dotnet build ./src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/LiveChartsCore.SkiaSharpView.Avalonia.csproj -c $configuration
dotnet build ./src/skiasharp/LiveChartsCore.SkiaSharp.WinForms/LiveChartsCore.SkiaSharpView.WinForms.csproj -c $configuration
dotnet build ./src/skiasharp/LiveChartsCore.SkiaSharp.Wpf/LiveChartsCore.SkiaSharpView.Wpf.csproj -c $configuration
dotnet build ./src/skiasharp/LiveChartsCore.SkiaSharp.Xamarin.Forms/LiveChartsCore.SkiaSharpView.Xamarin.Forms.csproj -c $configuration
dotnet build ./src/skiasharp/LiveChartsCore.SkiaSharpView.Blazor/LiveChartsCore.SkiaSharpView.Blazor.csproj -c $configuration
dotnet build ./src/skiasharp/LiveChartsCore.SkiaSharpView.Eto/LiveChartsCore.SkiaSharpView.Eto.csproj -c $configuration
dotnet build ./src/skiasharp/LiveChartsCore.SkiaSharpView.Maui/LiveChartsCore.SkiaSharpView.Maui.csproj -c $configuration

# DISABLE DOTNET BUILD FOR NOW
# LiveCharts requires VisualStudio 17.8
# but it seems that the current image installed is 17.6 (UPDATE NOT SURE WHICH VERSION IS INSTALLED AS TODAY)
# FOR NOW... DISABLE MSBUILD

# $msbuild = &"${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe

# & $msbuild `
#     ./src/skiasharp/LiveChartsCore.SkiaSharpView.WinUI/LiveChartsCore.SkiaSharpView.WinUI.csproj `
#     /p:configuration=$configuration `
#     /restore

# & $msbuild `
#     ./src/skiasharp/LiveChartsCore.SkiaSharpView.Uno.WinUI/LiveChartsCore.SkiaSharpView.Uno.WinUI.csproj `
#     /p:configuration=$configuration `
#     /restore