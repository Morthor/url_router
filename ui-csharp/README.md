# URL Router UI (C#)

A lightweight Windows Forms application for configuring URL routing rules.

## Features

- **Rules Management**: Add, edit, delete, and reorder domain rules
- **Browser Detection**: Automatically detect installed browsers
- **Default Settings**: Configure default browser and arguments
- **Test Bench**: Test URLs to see which rule would match
- **Simple Interface**: Clean, native Windows UI

## Build

```powershell
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false
```

## Usage

1. Run the application
2. Use the Rules tab to manage routing rules
3. Use the Default tab to set the default browser
4. Use the Test tab to test URL routing
5. Changes are automatically saved to `%APPDATA%\UrlRouter\config.json`

## Configuration

Edits the same JSON config file that the C# router reads, ensuring consistency between the UI and router.
