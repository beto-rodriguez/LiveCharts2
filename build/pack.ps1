param([string]$configuration = "Debug", [string]$nupkgOutputPath = "./nupkg")

[Project[]]$projects = @(
    [Project]::new("./src/LiveChartsCore/LiveChartsCore.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharp/LiveChartsCore.SkiaSharpView.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/LiveChartsCore.SkiaSharpView.Avalonia.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharp.WinForms/LiveChartsCore.SkiaSharpView.WinForms.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharp.Wpf/LiveChartsCore.SkiaSharpView.Wpf.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharpView.Blazor/LiveChartsCore.SkiaSharpView.Blazor.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharpView.Eto/LiveChartsCore.SkiaSharpView.Eto.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharpView.Maui/LiveChartsCore.SkiaSharpView.Maui.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharpView.Uno.WinUI/LiveChartsCore.SkiaSharpView.Uno.WinUI.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharpView.WinUI/LiveChartsCore.SkiaSharpView.WinUI.csproj")
)

class Project {
    [string]$src

    Project(
        [string]$src
    ) {
        $this.src = $src
    }

    [string]GetFolder() {
        return $this.src.SubString(0, $this.src.LastIndexOf("/"));
    }

    [string]GetName() {
        $end = $this.src.LastIndexOf("/")
        return $this.src.SubString($end + 1, $this.src.Length - $end - 1);
    }
}

if (Test-Path $nupkgOutputPath) {
    Get-ChildItem $nupkgOutputPath -Include *.* -File -Recurse | ForEach-Object { $_.Delete() }
} else {
    New-Item $nupkgOutputPath -ItemType "directory"
}

foreach ($p in $projects) {
    $folder = $p.GetFolder()
    $name = $p.GetName()

    if (Test-Path $($folder + "/bin")) {
        Remove-Item $($folder + "/bin") -Force -Recurse
    }

    $expression = "dotnet pack $($p.src) -o $nupkgOutputPath -c $configuration -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg -p:IsPacking=true"

    Write-Progress -Activity "$name" -Status "Packing..."
    $result = & dotnet pack $p.src -o $nupkgOutputPath -c $configuration -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg -p:IsPacking=true -p:UseNuGetForGenerator=true 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Output "✖ $name failed to pack."
        Write-Output $result
        break
    }else {
        Write-Output "✓ $name packed successfully."
    }
}
