using Microsoft.Win32;

namespace UrlRouterUI;

public class BrowserApp
{
    public string Name { get; set; } = "";
    public string ExePath { get; set; } = "";

    public override string ToString() => $"{Name} ({ExePath})";
}

public static class BrowserDetection
{
    public static List<BrowserApp> DetectInstalledBrowsers()
    {
        var found = new List<BrowserApp>();
        
        // Registry-based discovery
        try
        {
            var regBrowsers = DetectViaRegistry();
            found.AddRange(regBrowsers);
        }
        catch
        {
            // Best effort
        }

        // Path-based discovery
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var candidates = new Dictionary<string, List<string>>
        {
            ["Google Chrome"] = new()
            {
                Path.Combine(programFiles, @"Google\Chrome\Application\chrome.exe"),
                Path.Combine(programFilesX86, @"Google\Chrome\Application\chrome.exe"),
            },
            ["Microsoft Edge"] = new()
            {
                Path.Combine(programFiles, @"Microsoft\Edge\Application\msedge.exe"),
                Path.Combine(programFilesX86, @"Microsoft\Edge\Application\msedge.exe"),
            },
            ["Mozilla Firefox"] = new()
            {
                Path.Combine(programFiles, @"Mozilla Firefox\firefox.exe"),
                Path.Combine(programFilesX86, @"Mozilla Firefox\firefox.exe"),
            },
            ["Brave"] = new()
            {
                Path.Combine(programFiles, @"BraveSoftware\Brave-Browser\Application\brave.exe"),
                Path.Combine(programFilesX86, @"BraveSoftware\Brave-Browser\Application\brave.exe"),
            },
            ["Vivaldi"] = new()
            {
                Path.Combine(programFiles, @"Vivaldi\Application\vivaldi.exe"),
                Path.Combine(programFilesX86, @"Vivaldi\Application\vivaldi.exe"),
                Path.Combine(localAppData, @"Vivaldi\Application\vivaldi.exe"),
            },
            ["Opera"] = new()
            {
                Path.Combine(localAppData, @"Programs\Opera\opera.exe"),
                Path.Combine(userProfile, @"AppData\Local\Programs\Opera\opera.exe"),
            },
            ["Chromium"] = new()
            {
                Path.Combine(programFiles, @"Chromium\Application\chromium.exe"),
                Path.Combine(programFilesX86, @"Chromium\Application\chromium.exe"),
            },
        };

        foreach (var entry in candidates)
        {
            foreach (var path in entry.Value)
            {
                if (string.IsNullOrEmpty(path)) continue;
                if (File.Exists(path))
                {
                    found.Add(new BrowserApp { Name = entry.Key, ExePath = path });
                    break; // Take first hit per browser
                }
            }
        }

        // De-duplicate and sort
        var byPath = new Dictionary<string, BrowserApp>();
        foreach (var browser in found)
        {
            byPath[browser.ExePath.ToLowerInvariant()] = browser;
        }

        return byPath.Values.OrderBy(b => b.Name).ToList();
    }

    private static List<BrowserApp> DetectViaRegistry()
    {
        var results = new List<BrowserApp>();
        
        foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
                using var clientsKey = baseKey.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
                if (clientsKey == null) continue;

                var subKeys = clientsKey.GetSubKeyNames();
                foreach (var subKeyName in subKeys)
                {
                    try
                    {
                        using var subKey = clientsKey.OpenSubKey(subKeyName);
                        if (subKey == null) continue;

                        using var commandKey = subKey.OpenSubKey(@"shell\open\command");
                        if (commandKey == null) continue;

                        var command = commandKey.GetValue("")?.ToString();
                        if (string.IsNullOrEmpty(command)) continue;

                        var exePath = ExtractExeFromCommand(command);
                        if (string.IsNullOrEmpty(exePath)) continue;

                        var name = subKeyName;
                        var friendlyName = subKey.GetValue("")?.ToString();
                        if (!string.IsNullOrEmpty(friendlyName))
                        {
                            name = friendlyName;
                        }

                        results.Add(new BrowserApp { Name = name, ExePath = exePath });
                    }
                    catch
                    {
                        // Skip invalid entries
                    }
                }
            }
            catch
            {
                // Skip inaccessible hives
            }
        }

        return results;
    }

    private static string? ExtractExeFromCommand(string command)
    {
        var trimmed = command.Trim();
        if (string.IsNullOrEmpty(trimmed)) return null;

        var candidate = trimmed;
        if (candidate.StartsWith("\""))
        {
            var end = candidate.IndexOf("\"", 1);
            if (end > 1)
            {
                candidate = candidate.Substring(1, end - 1);
            }
        }

        var exeIndex = candidate.ToLowerInvariant().IndexOf(".exe");
        if (exeIndex != -1)
        {
            candidate = candidate.Substring(0, exeIndex + 4);
        }

        return candidate.ToLowerInvariant().EndsWith(".exe") ? candidate : null;
    }
}
