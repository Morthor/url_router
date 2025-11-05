using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UrlRouter;

internal record ActionCfg(string type, string target, string[]? args, bool removeTrackingParams = false, string[]? trackingParamsToRemove = null);
internal record When(string[]? host, string[]? pathContains, string? urlRegex);
internal record Rule(string name, When when, ActionCfg action, bool enabled = true);
internal record Config(int version, ActionCfg @default, Rule[] rules, bool removeTrackingParamsGlobal = false, string[]? trackingParamsToRemoveGlobal = null);

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

        // Determine which URL to use for cleaning (use realUrl if safelink was extracted, otherwise rawUrl)
        var urlToClean = realUrl != rawUrl ? realUrl : rawUrl;
        var urlToLaunch = rawUrl;

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
                
                // Apply tracking removal if enabled
                if (rule.action.removeTrackingParams)
                {
                    var trackingParams = rule.action.trackingParamsToRemove ?? 
                                       config.trackingParamsToRemoveGlobal ?? 
                                       GetDefaultTrackingParams();
                    urlToLaunch = RemoveTrackingParameters(urlToClean, trackingParams);
                }
                else if (config.removeTrackingParamsGlobal)
                {
                    var trackingParams = config.trackingParamsToRemoveGlobal ?? GetDefaultTrackingParams();
                    urlToLaunch = RemoveTrackingParameters(urlToClean, trackingParams);
                }
                
                // Launch with cleaned URL (or original if tracking removal not enabled)
                return Launch(rule.action, urlToLaunch);
            }
        }

        WriteLog($"No rule matched, using default: {config.@default.target}");
        
        // Apply tracking removal for default action if enabled
        if (config.@default.removeTrackingParams)
        {
            var trackingParams = config.@default.trackingParamsToRemove ?? 
                               config.trackingParamsToRemoveGlobal ?? 
                               GetDefaultTrackingParams();
            urlToLaunch = RemoveTrackingParameters(urlToClean, trackingParams);
        }
        else if (config.removeTrackingParamsGlobal)
        {
            var trackingParams = config.trackingParamsToRemoveGlobal ?? GetDefaultTrackingParams();
            urlToLaunch = RemoveTrackingParameters(urlToClean, trackingParams);
        }
        
        return Launch(config.@default, urlToLaunch);
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
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var version = root.TryGetProperty("version", out var v) ? v.GetInt32() : 1;
            
            // Parse default action
            ActionCfg defaultAction;
            if (root.TryGetProperty("default", out var defaultElem))
            {
                defaultAction = ParseActionCfg(defaultElem);
            }
            else
            {
                defaultAction = new ActionCfg("app", @"C:\Program Files\Mozilla Firefox\firefox.exe", Array.Empty<string>());
            }

            // Parse rules
            var rules = new List<Rule>();
            if (root.TryGetProperty("rules", out var rulesElem) && rulesElem.ValueKind == JsonValueKind.Array)
            {
                foreach (var ruleElem in rulesElem.EnumerateArray())
                {
                    var rule = ParseRule(ruleElem);
                    if (rule != null)
                        rules.Add(rule);
                }
            }

            // Parse global tracking settings
            var removeTrackingParamsGlobal = root.TryGetProperty("removeTrackingParamsGlobal", out var rtp) && rtp.GetBoolean();
            string[]? trackingParamsToRemoveGlobal = null;
            if (root.TryGetProperty("trackingParamsToRemoveGlobal", out var tptg) && tptg.ValueKind == JsonValueKind.Array)
            {
                trackingParamsToRemoveGlobal = tptg.EnumerateArray()
                    .Select(x => x.GetString() ?? "")
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();
            }
            else if (removeTrackingParamsGlobal)
            {
                trackingParamsToRemoveGlobal = GetDefaultTrackingParams();
            }

            return new Config(version, defaultAction, rules.ToArray(), removeTrackingParamsGlobal, trackingParamsToRemoveGlobal);
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
            new ActionCfg("app", @"C:\Program Files\Mozilla Firefox\firefox.exe", Array.Empty<string>()),
            Array.Empty<Rule>(),
            false,
            null
        );
    }

    private static ActionCfg ParseActionCfg(JsonElement json)
    {
        var type = json.TryGetProperty("type", out var t) ? t.GetString() ?? "app" : "app";
        var target = json.TryGetProperty("target", out var tg) ? tg.GetString() ?? "" : "";
        string[]? args = null;
        if (json.TryGetProperty("args", out var a) && a.ValueKind == JsonValueKind.Array)
        {
            args = a.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }
        var removeTrackingParams = json.TryGetProperty("removeTrackingParams", out var rtp) && rtp.GetBoolean();
        string[]? trackingParamsToRemove = null;
        if (json.TryGetProperty("trackingParamsToRemove", out var tpt) && tpt.ValueKind == JsonValueKind.Array)
        {
            trackingParamsToRemove = tpt.EnumerateArray()
                .Select(x => x.GetString() ?? "")
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();
        }

        return new ActionCfg(type, target, args ?? Array.Empty<string>(), removeTrackingParams, trackingParamsToRemove);
    }

    private static Rule? ParseRule(JsonElement json)
    {
        var name = json.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
        if (string.IsNullOrEmpty(name))
            return null;

        When? when = null;
        if (json.TryGetProperty("when", out var w))
        {
            string[]? host = null;
            if (w.TryGetProperty("host", out var h) && h.ValueKind == JsonValueKind.Array)
            {
                host = h.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }

            string[]? pathContains = null;
            if (w.TryGetProperty("pathContains", out var pc) && pc.ValueKind == JsonValueKind.Array)
            {
                pathContains = pc.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }

            var urlRegex = w.TryGetProperty("urlRegex", out var ur) ? ur.GetString() : null;
            when = new When(host, pathContains, urlRegex);
        }

        ActionCfg? action = null;
        if (json.TryGetProperty("action", out var a))
        {
            action = ParseActionCfg(a);
        }

        if (when == null || action == null)
            return null;

        var enabled = json.TryGetProperty("enabled", out var e) ? e.GetBoolean() : true;

        return new Rule(name, when, action, enabled);
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

    private static string RemoveTrackingParameters(string url, string[] trackingParams)
    {
        if (trackingParams == null || trackingParams.Length == 0)
            return url;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return url; // Can't parse, return as-is

        var query = uri.Query;
        if (string.IsNullOrEmpty(query) || query.Length < 2)
            return url; // No query string, nothing to remove

        // Create a case-insensitive set of tracking parameters for fast lookup
        var trackingSet = new HashSet<string>(trackingParams, StringComparer.OrdinalIgnoreCase);

        // Remove leading '?'
        var queryString = query.Substring(1);
        
        // Parse query parameters
        var paramsArray = queryString.Split('&');
        var keptParams = new List<string>();

        foreach (var param in paramsArray)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var parts = param.Split(new[] { '=' }, 2);
            var paramName = parts[0];
            
            // Check if this is a tracking parameter (case-insensitive)
            if (!trackingSet.Contains(paramName))
            {
                // Keep this parameter
                keptParams.Add(param);
            }
        }

        // Reconstruct URL
        var builder = new System.Text.StringBuilder();
        builder.Append(uri.Scheme);
        builder.Append("://");
        builder.Append(uri.Authority);
        builder.Append(uri.AbsolutePath);

        if (keptParams.Count > 0)
        {
            builder.Append("?");
            builder.Append(string.Join("&", keptParams));
        }

        if (!string.IsNullOrEmpty(uri.Fragment))
        {
            builder.Append(uri.Fragment);
        }

        var cleanedUrl = builder.ToString();
        WriteLog($"Removed tracking parameters. Original: {url}, Cleaned: {cleanedUrl}");
        return cleanedUrl;
    }

    private static string[] GetDefaultTrackingParams()
    {
        return new[]
        {
            // UTM parameters
            "utm_source", "utm_medium", "utm_campaign", "utm_term", "utm_content", "utm_id", "utm_source_platform", "utm_creative_format", "utm_marketing_tactic",
            // Google
            "gclid", "dclid", "gbraid", "wbraid", "gclsrc", "_ga", "_gid", "_gac", "_gl",
            // Facebook
            "fbclid", "fb_action_ids", "fb_action_types", "fb_source", "fb_ref",
            // Twitter/X
            "twclid", "ref_src", "ref_url",
            // Instagram
            "igshid",
            // LinkedIn
            "li_fat_id", "trk", "trkInfo",
            // Microsoft/Bing
            "msclkid",
            // Yahoo
            "ysmwa", "yclid",
            // HubSpot
            "_hsenc", "_hssc", "_hssrc", "hsCtaTracking",
            // Mailchimp
            "mc_cid", "mc_eid",
            // Pinterest
            "epik",
            // TikTok
            "tt_medium", "tt_content",
            // Other common tracking
            "icid", "campaign_id", "ad_id", "clickid", "affiliate_id", "ncid", "ns_mchannel", "ns_source", "ns_linkname", "ns_fee"
        };
    }
}


