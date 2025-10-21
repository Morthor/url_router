# URL Router Installer
# Run this script as Administrator to install URL Router

param(
    [string]$InstallPath = "$env:ProgramFiles\UrlRouter"
)

Write-Host "URL Router Installer" -ForegroundColor Green
Write-Host "===================" -ForegroundColor Green
Write-Host ""

# Check if running as administrator
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "This script requires Administrator privileges. Please run as Administrator." -ForegroundColor Red
    Write-Host "Right-click PowerShell and select 'Run as Administrator'" -ForegroundColor Yellow
    pause
    exit 1
}

Write-Host "Installing URL Router to: $InstallPath" -ForegroundColor Yellow

# Create installation directory
if (!(Test-Path $InstallPath)) {
    New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null
    Write-Host "Created installation directory: $InstallPath" -ForegroundColor Green
}

# Copy files
$SourcePath = Split-Path -Parent $PSScriptRoot
$RouterSource = Join-Path $SourcePath "router\bin\Release\net8.0\win-x64\publish"
$UISource = Join-Path $SourcePath "ui-csharp"

if (Test-Path $RouterSource) {
    Copy-Item -Path "$RouterSource\*" -Destination $InstallPath -Recurse -Force
    Write-Host "Copied router files" -ForegroundColor Green
} else {
    Write-Host "Router files not found. Please build the project first:" -ForegroundColor Red
    Write-Host "dotnet publish router -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false" -ForegroundColor Yellow
    pause
    exit 1
}

# Build and copy UI
Write-Host "Building UI..." -ForegroundColor Yellow
Push-Location $UISource
try {
    dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false -o "$InstallPath\UI"
    if ($LASTEXITCODE -eq 0) {
        Write-Host "UI built and copied successfully" -ForegroundColor Green
    } else {
        Write-Host "Failed to build UI" -ForegroundColor Red
        pause
        exit 1
    }
} finally {
    Pop-Location
}

# Create registry file with correct paths
$RegistryContent = @"
Windows Registry Editor Version 5.00

; Register as a browser application
[HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\UrlRouter]
@="URL Router"
"LocalizedString"="URL Router"

; Register browser capabilities
[HKEY_LOCAL_MACHINE\SOFTWARE\UrlRouter\Capabilities]
"ApplicationName"="URL Router"
"ApplicationDescription"="Routes http/https to configured apps"

; Register URL associations for HTTP and HTTPS
[HKEY_LOCAL_MACHINE\SOFTWARE\UrlRouter\Capabilities\URLAssociations]
"http"="UrlRouterHTTP"
"https"="UrlRouterHTTPS"

; Register the actual protocol handlers
[HKEY_CLASSES_ROOT\UrlRouterHTTP]
@="URL Router HTTP"
"URL Protocol"=""

[HKEY_CLASSES_ROOT\UrlRouterHTTP\DefaultIcon]
@="$($InstallPath.Replace('\', '\\'))\\UrlRouter.exe,0"

[HKEY_CLASSES_ROOT\UrlRouterHTTP\shell]
@="open"

[HKEY_CLASSES_ROOT\UrlRouterHTTP\shell\open]
@="Open with URL Router"

[HKEY_CLASSES_ROOT\UrlRouterHTTP\shell\open\command]
@="\"$($InstallPath.Replace('\', '\\'))\\UrlRouter.exe\" \"%1\""

[HKEY_CLASSES_ROOT\UrlRouterHTTPS]
@="URL Router HTTPS"
"URL Protocol"=""

[HKEY_CLASSES_ROOT\UrlRouterHTTPS\DefaultIcon]
@="$($InstallPath.Replace('\', '\\'))\\UrlRouter.exe,0"

[HKEY_CLASSES_ROOT\UrlRouterHTTPS\shell]
@="open"

[HKEY_CLASSES_ROOT\UrlRouterHTTPS\shell\open]
@="Open with URL Router"

[HKEY_CLASSES_ROOT\UrlRouterHTTPS\shell\open\command]
@="\"$($InstallPath.Replace('\', '\\'))\\UrlRouter.exe\" \"%1\""

; This is the key part - register our app as a browser
[HKEY_LOCAL_MACHINE\SOFTWARE\RegisteredApplications]
"UrlRouter"="SOFTWARE\\UrlRouter\\Capabilities"
"@

# Write registry file
$RegistryFile = Join-Path $InstallPath "urlrouter.reg"
$RegistryContent | Out-File -FilePath $RegistryFile -Encoding ASCII

# Import registry settings
Write-Host "Registering URL Router with Windows..." -ForegroundColor Yellow
regedit /s $RegistryFile

if ($LASTEXITCODE -eq 0) {
    Write-Host "Registry settings imported successfully" -ForegroundColor Green
} else {
    Write-Host "Failed to import registry settings" -ForegroundColor Red
    pause
    exit 1
}

# Create Start Menu shortcut
$StartMenuPath = "$env:ProgramData\Microsoft\Windows\Start Menu\Programs"
$ShortcutPath = Join-Path $StartMenuPath "URL Router.lnk"
$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut($ShortcutPath)
$Shortcut.TargetPath = Join-Path $InstallPath "UI\UrlRouterUI.exe"
$Shortcut.WorkingDirectory = Join-Path $InstallPath "UI"
$Shortcut.Description = "URL Router - Route URLs to different applications"
$Shortcut.Save()

Write-Host "Created Start Menu shortcut" -ForegroundColor Green

# Create uninstaller
$UninstallerContent = @"
# URL Router Uninstaller
# Run this script as Administrator to uninstall URL Router

Write-Host "URL Router Uninstaller" -ForegroundColor Red
Write-Host "=====================" -ForegroundColor Red
Write-Host ""

# Check if running as administrator
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "This script requires Administrator privileges. Please run as Administrator." -ForegroundColor Red
    Write-Host "Right-click PowerShell and select 'Run as Administrator'" -ForegroundColor Yellow
    pause
    exit 1
}

Write-Host "Uninstalling URL Router..." -ForegroundColor Yellow

# Remove registry entries
Write-Host "Removing registry entries..." -ForegroundColor Yellow
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\UrlRouter" /f 2>$null
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\UrlRouter" /f 2>$null
reg delete "HKEY_CLASSES_ROOT\UrlRouterHTTP" /f 2>$null
reg delete "HKEY_CLASSES_ROOT\UrlRouterHTTPS" /f 2>$null
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\RegisteredApplications" /v "UrlRouter" /f 2>$null

# Remove Start Menu shortcut
`$StartMenuPath = "`$env:ProgramData\Microsoft\Windows\Start Menu\Programs"
`$ShortcutPath = Join-Path `$StartMenuPath "URL Router.lnk"
if (Test-Path `$ShortcutPath) {
    Remove-Item `$ShortcutPath -Force
    Write-Host "Removed Start Menu shortcut" -ForegroundColor Green
}

# Remove installation directory
if (Test-Path "$InstallPath") {
    Remove-Item -Path "$InstallPath" -Recurse -Force
    Write-Host "Removed installation directory: $InstallPath" -ForegroundColor Green
}

Write-Host "URL Router has been uninstalled successfully!" -ForegroundColor Green
Write-Host "Press any key to continue..."
pause
"@

$UninstallerFile = Join-Path $InstallPath "uninstall.ps1"
$UninstallerContent | Out-File -FilePath $UninstallerFile -Encoding UTF8

Write-Host ""
Write-Host "Installation completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Go to Settings → Apps → Default apps → Choose defaults by link type" -ForegroundColor White
Write-Host "2. Select 'URL Router' for HTTP and HTTPS links" -ForegroundColor White
Write-Host "3. Run 'URL Router' from the Start Menu to configure routing rules" -ForegroundColor White
Write-Host ""
Write-Host "To uninstall, run: $UninstallerFile" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to continue..."
pause
