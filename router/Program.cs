using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UrlRouter;

internal record ActionCfg(string type, string target, string[]? args);
internal record When(string[]? host, string[]? pathContains, string? urlRegex);
internal record Rule(string name, When when, ActionCfg action);
internal record Config(int version, ActionCfg @default, Rule[] rules);

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0) return 0;

        var rawUrl = args[0];
        if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out var uri)) return 0;

        var cfgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UrlRouter", "config.json");
        var config = LoadConfig(cfgPath);

        foreach (var rule in config.rules)
        {
            if (Matches(rule.when, uri, rawUrl))
            {
                return Launch(rule.action, rawUrl);
            }
        }

        return Launch(config.@default, rawUrl);
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
        var startInfo = new ProcessStartInfo
        {
            FileName = action.target,
            UseShellExecute = false
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
            Process.Start(startInfo);
            return 0;
        }
        catch
        {
            return 1;
        }
    }

    private static string QuoteIfNeeded(string s)
    {
        if (string.IsNullOrEmpty(s)) return "\"\"";
        return s.Contains(' ') || s.Contains('"') ? $"\"{s.Replace("\"", "\\\"")}\"" : s;
    }
}


