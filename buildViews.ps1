param([string]$configuration = "Release")

[Project[]]$projects = @(
    [Project]::new("./src/LiveChartsCore/LiveChartsCore.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharp/LiveChartsCore.SkiaSharpView.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharp.Avalonia/LiveChartsCore.SkiaSharpView.Avalonia.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharp.WinForms/LiveChartsCore.SkiaSharpView.WinForms.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharp.Wpf/LiveChartsCore.SkiaSharpView.Wpf.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharp.Xamarin.Forms/LiveChartsCore.SkiaSharpView.Xamarin.Forms.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharpView.Blazor/LiveChartsCore.SkiaSharpView.Blazor.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharpView.Eto/LiveChartsCore.SkiaSharpView.Eto.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharpView.Maui/LiveChartsCore.SkiaSharpView.Maui.csproj")
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharpView.Uno/LiveChartsCore.SkiaSharpView.Uno.csproj", $true)
    [Project]::new("./src/skiasharp/LiveChartsCore.SkiaSharpView.Uno.WinUI/LiveChartsCore.SkiaSharpView.Uno.WinUI.csproj", $true)
    [Project]::new(
        "./src/skiasharp/LiveChartsCore.SkiaSharpView.WinUI/LiveChartsCore.SkiaSharpView.WinUI.csproj", 
        $true,
        "nuget",
        "./src/skiasharp/LiveChartsCore.SkiaSharpView.WinUI/LiveChartsCore.SkiaSharpView.WinUI.nuspec"
    )
)

$msbuild = &"${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe

class Project {
    [string]$src
    [bool]$useMsbuild
    [string]$packingMethod ## dotnet | msbuild | nuget
    [string]$nuspecFile

    Project(
        [string]$src
    ) {
        $this.src = $src
        $this.useMsbuild = $false
        $this.packingMethod = "dotnet"
        $this.nuspecFile = $null
    }

    Project(
        [string]$src,
        [bool]$useMsbuild
    ) {
        $this.src = $src
        $this.useMsbuild = $useMsbuild
        if ($useMsbuild) {
            $this.packingMethod = "msbuild"
            $this.nuspecFile = ""
        }
    }

    Project(
        [string]$src,
        [bool]$useMsbuild,
        [string]$packingMethod,
        [string]$nuspecFile
    ) {
        $this.src = $src
        $this.useMsbuild = $useMsbuild
        $this.packingMethod = $packingMethod
        $this.nuspecFile = $nuspecFile
    }

    [string]GetFolder() {
        return $this.src.SubString(0, $this.src.LastIndexOf("/"));
    }
}

function Add-Build {
    param (
        [Project] $project
    )

    $folder = $project.GetFolder()

    if (Test-Path $($folder + "/bin")) {
        Remove-Item $($folder + "/bin") -Force -Recurse
    }

    if ($project.useMsbuild) {
        & $msbuild $project.src /p:configuration=$configuration /restore
    }
    else {
        dotnet build $project.src -c $configuration
    }

    if ($LastExitCode -ne 0) {
        throw $("failed at '" + $project.src + "' with exit code " + $LastExitCode);
    }    
}

function Write-ColorOutput($foregroundColor)
{
    # save the current color
    $fc = $host.UI.RawUI.ForegroundColor

    # set the new color
    $host.UI.RawUI.ForegroundColor = $foregroundColor

    # output
    if ($args) {
        Write-Output $args
    }
    else {
        $input | Write-Output
    }

    # restore the original color
    $host.UI.RawUI.ForegroundColor = $fc
}

foreach ($p in $projects) {
    Add-Build $p
}

Write-ColorOutput green $("`n`nSuccessfully built " + $projects.length + " projects using '" + $configuration + "' config.`n`n")
