& $PSScriptRoot\pack.ps1 Release

& dotnet nuget push **/*.nupkg --api-key oy2o6gainuded3gc42sq2gddywqhhhiu3cxtns4lokf5hu --source https://api.nuget.org/v3/index.json --skip-duplicate

## this should only be ran once?
## dotnet nuget add source --username beto-rodriguez --password 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 --store-password-in-clear-text --name github "https://nuget.pkg.github.com/beto-rodriguez/index.json"
& dotnet nuget push **/*.nupkg --api-key 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 --source "github" --skip-duplicate