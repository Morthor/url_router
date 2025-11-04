# URL Router Installer Build Script
# This script builds the application and creates the installer

Write-Host "URL Router Installer Build Script" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green
Write-Host ""

# Check if Inno Setup is available
$InnoSetupPath = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
if (!(Test-Path $InnoSetupPath)) {
    $InnoSetupPath = "${env:ProgramFiles}\Inno Setup 6\ISCC.exe"
    if (!(Test-Path $InnoSetupPath)) {
        Write-Host "Error: Inno Setup not found. Please install Inno Setup 6." -ForegroundColor Red
        Write-Host "Download from: https://jrsoftware.org/isinfo.php" -ForegroundColor Yellow
        exit 1
    }
}

# Build the application first
Write-Host "Building application..." -ForegroundColor Yellow
& ".\build.ps1"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Aborting installer creation." -ForegroundColor Red
    exit 1
}

# Create dist directory
if (!(Test-Path "dist")) {
    New-Item -ItemType Directory -Path "dist" | Out-Null
}

# Build the installer
Write-Host "Creating installer..." -ForegroundColor Yellow
& $InnoSetupPath "install\UrlRouter.iss"

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Installer created successfully!" -ForegroundColor Green
    Write-Host "Location: dist\UrlRouter-Setup-1.2.exe" -ForegroundColor Cyan
} else {
    Write-Host "Failed to create installer" -ForegroundColor Red
    exit 1
}
