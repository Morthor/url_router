# URL Router Build Script
# This script builds both the router and UI applications

Write-Host "URL Router Build Script" -ForegroundColor Green
Write-Host "=====================" -ForegroundColor Green
Write-Host ""

# Check if dotnet is available
if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Error: .NET SDK not found. Please install .NET 8.0 SDK." -ForegroundColor Red
    Write-Host "Download from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    exit 1
}

Write-Host "Building Router..." -ForegroundColor Yellow
Push-Location "router"
try {
    dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Router built successfully" -ForegroundColor Green
    } else {
        Write-Host "Failed to build router" -ForegroundColor Red
        exit 1
    }
} finally {
    Pop-Location
}

Write-Host "Building UI..." -ForegroundColor Yellow
Push-Location "ui-csharp"
try {
    dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false
    if ($LASTEXITCODE -eq 0) {
        # Copy icon to publish directory
        $publishDir = "bin\Release\net8.0-windows\win-x64\publish"
        if (Test-Path "icon.ico") {
            Copy-Item "icon.ico" $publishDir -Force
            Write-Host "Icon copied to publish directory" -ForegroundColor Green
        }
        Write-Host "UI built successfully" -ForegroundColor Green
    } else {
        Write-Host "Failed to build UI" -ForegroundColor Red
        exit 1
    }
} finally {
    Pop-Location
}

Write-Host ""
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Ready for installer creation." -ForegroundColor Cyan
