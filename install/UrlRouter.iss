; URL Router Inno Setup Script
; This script creates a Windows installer for the URL Router application

#define MyAppName "URL Router"
#define MyAppVersion "1.2"
#define MyAppPublisher "João Soares"
#define MyAppURL "https://github.com/yourusername/urlrouter"
#define MyAppExeName "UrlRouterUI.exe"
#define MyRouterExeName "UrlRouter.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={{8B5F3C2A-1D4E-4F7A-9B8C-3E6D5F2A1B9C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=
OutputDir=..\dist
OutputBaseFilename=UrlRouter-Setup-{#MyAppVersion}
SetupIconFile=icon.ico
UninstallDisplayIcon={app}\UI\icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Router executable
Source: "..\router\bin\Release\net8.0\win-x64\publish\{#MyRouterExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\router\bin\Release\net8.0\win-x64\publish\*.pdb"; DestDir: "{app}"; Flags: ignoreversion; Check: IsDebugMode

; UI executable
Source: "..\ui-csharp\bin\Release\net8.0-windows\win-x64\publish\{#MyAppExeName}"; DestDir: "{app}\UI"; Flags: ignoreversion
Source: "..\ui-csharp\bin\Release\net8.0-windows\win-x64\publish\*.pdb"; DestDir: "{app}\UI"; Flags: ignoreversion; Check: IsDebugMode
Source: "..\ui-csharp\bin\Release\net8.0-windows\win-x64\publish\icon.ico"; DestDir: "{app}\UI"; Flags: ignoreversion

; Documentation
Source: "..\docs\config.sample.json"; DestDir: "{app}\docs"; Flags: ignoreversion
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\UI\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\UI\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\UI\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\UI\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
// Custom functions for registry operations
function IsDebugMode(): Boolean;
begin
  Result := False; // Always include PDB files for now
end;
procedure RegisterUrlRouter();
var
  RegistryPath: String;
begin
  // Register as a browser application (user-level)
  RegistryPath := 'SOFTWARE\Clients\StartMenuInternet\UrlRouter';
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, '', 'URL Router') then
    Log('Registered browser application');
  
  RegistryPath := 'SOFTWARE\UrlRouter\Capabilities';
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, 'ApplicationName', 'URL Router') then
    Log('Registered application name');
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, 'ApplicationDescription', 'Routes http/https to configured apps') then
    Log('Registered application description');
  
  // Register URL associations
  RegistryPath := 'SOFTWARE\UrlRouter\Capabilities\URLAssociations';
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, 'http', 'UrlRouterHTTP') then
    Log('Registered HTTP association');
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, 'https', 'UrlRouterHTTPS') then
    Log('Registered HTTPS association');
  
  // Register protocol handlers
  RegistryPath := 'SOFTWARE\Classes\UrlRouterHTTP';
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, '', 'URL Router HTTP') then
    Log('Registered HTTP protocol handler');
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, 'URL Protocol', '') then
    Log('Set URL Protocol flag for HTTP');
  
  RegistryPath := 'SOFTWARE\Classes\UrlRouterHTTP\DefaultIcon';
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, '', ExpandConstant('{app}\UI\{#MyAppExeName},0')) then
    Log('Set HTTP icon');
  
  RegistryPath := 'SOFTWARE\Classes\UrlRouterHTTP\shell\open\command';
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, '', ExpandConstant('"{app}\{#MyRouterExeName}" "%1"')) then
    Log('Set HTTP command');
  
  RegistryPath := 'SOFTWARE\Classes\UrlRouterHTTPS';
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, '', 'URL Router HTTPS') then
    Log('Registered HTTPS protocol handler');
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, 'URL Protocol', '') then
    Log('Set URL Protocol flag for HTTPS');
  
  RegistryPath := 'SOFTWARE\Classes\UrlRouterHTTPS\DefaultIcon';
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, '', ExpandConstant('{app}\UI\{#MyAppExeName},0')) then
    Log('Set HTTPS icon');
  
  RegistryPath := 'SOFTWARE\Classes\UrlRouterHTTPS\shell\open\command';
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, '', ExpandConstant('"{app}\{#MyRouterExeName}" "%1"')) then
    Log('Set HTTPS command');
  
  // Register as a browser
  RegistryPath := 'SOFTWARE\RegisteredApplications';
  if RegWriteStringValue(HKEY_CURRENT_USER, RegistryPath, 'UrlRouter', 'SOFTWARE\UrlRouter\Capabilities') then
    Log('Registered as browser application');
end;

procedure UnregisterUrlRouter();
var
  RegistryPath: String;
begin
  // Remove registry entries
  RegistryPath := 'SOFTWARE\Clients\StartMenuInternet\UrlRouter';
  if RegDeleteKeyIncludingSubkeys(HKEY_CURRENT_USER, RegistryPath) then
    Log('Removed browser application registration');
  
  RegistryPath := 'SOFTWARE\UrlRouter';
  if RegDeleteKeyIncludingSubkeys(HKEY_CURRENT_USER, RegistryPath) then
    Log('Removed UrlRouter registry entries');
  
  RegistryPath := 'SOFTWARE\Classes\UrlRouterHTTP';
  if RegDeleteKeyIncludingSubkeys(HKEY_CURRENT_USER, RegistryPath) then
    Log('Removed HTTP protocol handler');
  
  RegistryPath := 'SOFTWARE\Classes\UrlRouterHTTPS';
  if RegDeleteKeyIncludingSubkeys(HKEY_CURRENT_USER, RegistryPath) then
    Log('Removed HTTPS protocol handler');
  
  RegistryPath := 'SOFTWARE\RegisteredApplications';
  if RegDeleteValue(HKEY_CURRENT_USER, RegistryPath, 'UrlRouter') then
    Log('Removed browser registration');
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
  // Note: .NET 8.0 Runtime is required but we'll let the application handle the error
  // if it's not installed, as the detection is complex and varies by installation method
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    RegisterUrlRouter();
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    UnregisterUrlRouter();
  end;
end;
