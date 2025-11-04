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
            
            // Check if HTTP protocol is set to UrlRouterHTTP
            using var httpKey = baseKey.OpenSubKey(@"SOFTWARE\Classes\http");
            if (httpKey == null) return false;
            
            var httpDefault = httpKey.GetValue("")?.ToString();
            if (httpDefault != "UrlRouterHTTP") return false;

            // Check if HTTPS protocol is set to UrlRouterHTTPS
            using var httpsKey = baseKey.OpenSubKey(@"SOFTWARE\Classes\https");
            if (httpsKey == null) return false;
            
            var httpsDefault = httpsKey.GetValue("")?.ToString();
            if (httpsDefault != "UrlRouterHTTPS") return false;

            // Both are set correctly
            return true;
        }
        catch
        {
            // If we can't check, assume false
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

