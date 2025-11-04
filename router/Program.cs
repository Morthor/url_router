using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UrlRouter;

internal record ActionCfg(string type, string target, string[]? args);
internal record When(string[]? host, string[]? pathContains, string? urlRegex);
internal record Rule(string name, When when, ActionCfg action, bool enabled = true);
internal record Config(int version, ActionCfg @default, Rule[] rules);

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0) return 0;

        var rawUrl = args[0];
        WriteLog($"URL Router called with: {rawUrl}");
        
        if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out var uri))
        {
            WriteLog($"Error: Invalid URL format: {rawUrl}");
            return 0;
        }

        // Extract real URL from Microsoft/Teams safelinks
        var realUrl = ExtractRealUrlFromSafelink(rawUrl, uri);
        var realUri = uri;
        if (realUrl != rawUrl)
        {
            WriteLog($"Detected safelink, extracted real URL: {realUrl}");
            if (Uri.TryCreate(realUrl, UriKind.Absolute, out var extractedUri))
            {
                realUri = extractedUri;
            }
        }

        var cfgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UrlRouter", "config.json");
        var config = LoadConfig(cfgPath);
        WriteLog($"Loaded config with {config.rules.Length} rules");

        foreach (var rule in config.rules)
        {
            // Skip disabled rules
            if (!rule.enabled)
            {
                WriteLog($"Skipping disabled rule: {rule.name}");
                continue;
            }
            
            // Use realUri for matching, but keep rawUrl for launching
            if (Matches(rule.when, realUri, realUrl))
            {
                WriteLog($"Rule matched: {rule.name} -> {rule.action.target}");
                // Launch with original URL so browser can handle safelink properly
                return Launch(rule.action, rawUrl);
            }
        }

        WriteLog($"No rule matched, using default: {config.@default.target}");
        return Launch(config.@default, rawUrl);
    }

    private static string ExtractRealUrlFromSafelink(string url, Uri uri)
    {
        // Check if this is a Microsoft/Teams safelink
        var host = uri.Host.ToLowerInvariant();
        if (!host.Contains("safelinks.protection.outlook.com") && 
            !host.Contains("statics.teams.cdn.office.net") &&
            !host.Contains("safelinks") &&
            !uri.AbsolutePath.Contains("atp-safelinks.html"))
        {
            return url; // Not a safelink
        }

        // Try to extract the 'url' parameter from query string
        var query = uri.Query;
        if (string.IsNullOrEmpty(query) || query.Length < 2) return url;

        // Remove leading '?'
        var queryString = query.Substring(1);
        
        // Parse query parameters
        var paramsArray = queryString.Split('&');
        foreach (var param in paramsArray)
        {
            var parts = param.Split(new[] { '=' }, 2);
            if (parts.Length == 2 && parts[0].Equals("url", StringComparison.OrdinalIgnoreCase))
            {
                // URL decode the value
                var realUrl = Uri.UnescapeDataString(parts[1]);
                WriteLog($"Extracted real URL from safelink: {realUrl}");
                return realUrl;
            }
        }

        return url;
    }

    private static Config LoadConfig(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return DefaultConfig();
            }

            var json = File.ReadAllText(path);
            var cfg = JsonSerializer.Deserialize<Config>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return cfg ?? DefaultConfig();
        }
        catch
        {
            return DefaultConfig();
        }
    }

    private static Config DefaultConfig()
    {
        return new Config(
            1,
            new ActionCfg("app", @"C:\\Program Files\\Mozilla Firefox\\firefox.exe", Array.Empty<string>()),
            Array.Empty<Rule>()
        );
    }

    private static bool Matches(When when, Uri uri, string rawUrl)
    {
        if (when.host is { Length: > 0 } && !AnyHostMatch(when.host, uri.Host)) return false;

        if (when.pathContains is { Length: > 0 })
        {
            foreach (var needle in when.pathContains)
            {
                if (!uri.AbsolutePath.Contains(needle, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(when.urlRegex))
        {
            if (!Regex.IsMatch(rawUrl, when.urlRegex!, RegexOptions.IgnoreCase)) return false;
        }

        return true;
    }

    private static bool AnyHostMatch(string[] patterns, string host)
    {
        host = host.ToLowerInvariant();
        foreach (var p in patterns)
        {
            var pat = p.ToLowerInvariant();
            if (pat.StartsWith("*."))
            {
                var suffix = pat[1..]; // ".example.com"
                if (host.EndsWith(suffix, StringComparison.Ordinal)) return true;
            }
            else if (host == pat)
            {
                return true;
            }
        }

        return false;
    }

    private static int Launch(ActionCfg action, string url)
    {
        // Validate file exists
        if (string.IsNullOrWhiteSpace(action.target) || !File.Exists(action.target))
        {
            WriteLog($"Error: Target file does not exist: {action.target}");
            return 1;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = action.target,
            UseShellExecute = true, // Use true for GUI applications like browsers
            WorkingDirectory = Path.GetDirectoryName(action.target) ?? ""
        };

        var argsPrefix = string.Empty;
        if (action.args is { Length: > 0 })
        {
            argsPrefix = string.Join(" ", action.args.Select(QuoteIfNeeded));
            if (argsPrefix.Length > 0) argsPrefix += " ";
        }

        startInfo.Arguments = argsPrefix + QuoteIfNeeded(url);

        try
        {
            WriteLog($"Launching: {action.target} {startInfo.Arguments}");
            var process = Process.Start(startInfo);
            if (process == null)
            {
                WriteLog($"Error: Process.Start returned null for {action.target}");
                return 1;
            }
            WriteLog($"Successfully launched process with ID: {process.Id}");
            return 0;
        }
        catch (Exception ex)
        {
            WriteLog($"Error launching {action.target}: {ex.Message}");
            return 1;
        }
    }

    private static void WriteLog(string message)
    {
        try
        {
            var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UrlRouter");
            Directory.CreateDirectory(logDir);
            var logFile = Path.Combine(logDir, "router.log");
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            File.AppendAllText(logFile, $"[{timestamp}] {message}\r\n");
        }
        catch
        {
            // Ignore logging errors
        }
    }

    private static string QuoteIfNeeded(string s)
    {
        if (string.IsNullOrEmpty(s)) return "\"\"";
        return s.Contains(' ') || s.Contains('"') ? $"\"{s.Replace("\"", "\\\"")}\"" : s;
    }
}


