# URL Router (C#)

A lightweight Windows application that routes HTTP/HTTPS URLs to different applications based on configurable rules.

## Build

```powershell
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false
```

Output: `bin\Release\net8.0\win-x64\publish\UrlRouter.exe`

## Configuration

Config path: `%APPDATA%\UrlRouter\config.json`

## Registration

To register as the default HTTP/HTTPS handler:
1. Run `install\urlrouter.reg` as administrator
2. Go to Settings → Apps → Default apps → Choose defaults by link type
3. Select "URL Router" for HTTP and HTTPS links


