using Microsoft.Win32;
using System.Diagnostics;

namespace UrlRouterUI;

public static class DefaultBrowserHelper
{
    public static bool IsUrlRouterSetAsDefault()
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            
            // Check HTTP protocol - verify the command points to our router
            var httpIsSet = VerifyDirectRouterCommand(baseKey, "http");
            if (!httpIsSet)
            {
                // Also check if it's set via our handler
                var httpHandler = GetProtocolHandler(baseKey, "http");
                if (httpHandler == "UrlRouterHTTP")
                {
                    httpIsSet = VerifyRouterCommand(baseKey, "UrlRouterHTTP");
                }
            }
            
            if (!httpIsSet) return false;

            // Check HTTPS protocol - verify the command points to our router
            var httpsIsSet = VerifyDirectRouterCommand(baseKey, "https");
            if (!httpsIsSet)
            {
                // Also check if it's set via our handler
                var httpsHandler = GetProtocolHandler(baseKey, "https");
                if (httpsHandler == "UrlRouterHTTPS")
                {
                    httpsIsSet = VerifyRouterCommand(baseKey, "UrlRouterHTTPS");
                }
            }
            
            return httpsIsSet;
        }
        catch
        {
            // If we can't check, assume false
            return false;
        }
    }

    private static string? GetProtocolHandler(RegistryKey baseKey, string protocol)
    {
        try
        {
            using var protocolKey = baseKey.OpenSubKey($@"SOFTWARE\Classes\{protocol}");
            if (protocolKey == null) return null;
            
            return protocolKey.GetValue("")?.ToString();
        }
        catch
        {
            return null;
        }
    }

    private static bool VerifyRouterCommand(RegistryKey baseKey, string handlerName)
    {
        try
        {
            using var handlerKey = baseKey.OpenSubKey($@"SOFTWARE\Classes\{handlerName}\shell\open\command");
            if (handlerKey == null) return false;
            
            var command = handlerKey.GetValue("")?.ToString();
            if (string.IsNullOrEmpty(command)) return false;
            
            // Check if command contains UrlRouter.exe
            return command.Contains("UrlRouter.exe", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static bool VerifyDirectRouterCommand(RegistryKey baseKey, string protocol)
    {
        try
        {
            using var protocolKey = baseKey.OpenSubKey($@"SOFTWARE\Classes\{protocol}\shell\open\command");
            if (protocolKey == null) return false;
            
            var command = protocolKey.GetValue("")?.ToString();
            if (string.IsNullOrEmpty(command)) return false;
            
            // Check if command contains UrlRouter.exe
            return command.Contains("UrlRouter.exe", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public static void OpenDefaultAppsSettings()
    {
        try
        {
            // Open Windows 11 default apps settings
            Process.Start(new ProcessStartInfo
            {
                FileName = "ms-settings:defaultapps",
                UseShellExecute = true
            });
        }
        catch
        {
            // Fallback to older method
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "ms-settings:defaultapps",
                    UseShellExecute = true
                });
            }
            catch
            {
                // Last resort: open general settings
                Process.Start(new ProcessStartInfo
                {
                    FileName = "ms-settings:",
                    UseShellExecute = true
                });
            }
        }
    }
}

