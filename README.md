# URL Router

A Windows application that routes HTTP/HTTPS URLs to different applications based on configurable rules.

## Architecture

- **C# Router**: Lightweight console app that handles HTTP/HTTPS protocol calls
- **C# UI**: Windows Forms desktop application for managing routing rules
- **JSON Config**: Shared configuration file in `%APPDATA%\UrlRouter\config.json`

## Installation

### Easy Installation (Recommended)

1. **Download and run the installer**:
   - Download `UrlRouter-Setup-1.2.exe` from [releases](https://github.com/Morthor/url_router/releases/tag/1.2)
   - Run the installer (no admin privileges required)
   - Follow the installation wizard

2. **Set as default browser**:
   - Go to Settings → Apps → Default apps → Choose defaults by link type
   - Select "URL Router" for HTTP and HTTPS links

3. **Configure routing rules**:
   - Run "URL Router" from the Start Menu
   - Create routing rules (e.g., "youtube.com → Chrome", "teams.microsoft.com → Edge")

### Building from Source

1. **Build the application**:
   ```powershell
   .\build.ps1
   ```

2. **Create installer** (requires Inno Setup):
   ```powershell
   .\build-installer.ps1
   ```

## Usage

1. **Configure routing rules**:
   - Run "URL Router" from the Start Menu
   - Create routing rules (e.g., "youtube.com → Chrome", "teams.microsoft.com → Edge")
   - Set a default browser for unmatched URLs

2. **Test the routing**:
   - Click any HTTP/HTTPS link to test the routing
   - URLs will be automatically routed based on your rules

## Uninstallation

To uninstall URL Router:
1. Go to Settings → Apps → Installed apps
2. Find "URL Router" and click "Uninstall"
3. Or use the uninstaller from the Start Menu

## Project Structure

```
URLrouter/
├── router/           # C# console application
├── ui-csharp/        # C# Windows Forms UI
├── install/          # Registry files
└── docs/             # Documentation and samples
```

---

© 2025 João Soares.  
All rights reserved.  
This repository is published for demonstration and educational purposes only.  
You may not copy, modify, or distribute this code without explicit permission.