using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace UrlRouterUI;

public class ActionCfg
{
    public string Type { get; set; } = "app";
    public string Target { get; set; } = "";
    public string? Browser { get; set; }
    public string[] Args { get; set; } = Array.Empty<string>();
    public bool RemoveTrackingParams { get; set; } = false;
    public string[]? TrackingParamsToRemove { get; set; }

    public static ActionCfg FromJson(JsonElement json)
    {
        return new ActionCfg
        {
            Type = json.TryGetProperty("type", out var type) ? type.GetString() ?? "app" : "app",
            Target = json.TryGetProperty("target", out var target) ? target.GetString() ?? "" : "",
            Browser = json.TryGetProperty("browser", out var browser) ? browser.GetString() : null,
            Args = json.TryGetProperty("args", out var args) && args.ValueKind == JsonValueKind.Array
                ? args.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToArray()
                : Array.Empty<string>(),
            RemoveTrackingParams = json.TryGetProperty("removeTrackingParams", out var removeTracking) && removeTracking.GetBoolean(),
            TrackingParamsToRemove = json.TryGetProperty("trackingParamsToRemove", out var trackingParams) && trackingParams.ValueKind == JsonValueKind.Array
                ? trackingParams.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToArray()
                : null
        };
    }

    public JsonObject ToJson()
    {
        var obj = new JsonObject
        {
            ["type"] = Type,
            ["target"] = Target,
            ["args"] = new JsonArray(Args.Select(x => JsonValue.Create(x)).ToArray())
        };
        if (!string.IsNullOrEmpty(Browser))
            obj["browser"] = Browser;
        if (RemoveTrackingParams)
            obj["removeTrackingParams"] = RemoveTrackingParams;
        if (TrackingParamsToRemove != null && TrackingParamsToRemove.Length > 0)
            obj["trackingParamsToRemove"] = new JsonArray(TrackingParamsToRemove.Select(x => JsonValue.Create(x)).ToArray());
        return obj;
    }
}

public class WhenCfg
{
    public string[] Host { get; set; } = Array.Empty<string>();
    public string[] PathContains { get; set; } = Array.Empty<string>();
    public string? UrlRegex { get; set; }

    public static WhenCfg FromJson(JsonElement json)
    {
        return new WhenCfg
        {
            Host = json.TryGetProperty("host", out var host) && host.ValueKind == JsonValueKind.Array
                ? host.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToArray()
                : Array.Empty<string>(),
            PathContains = json.TryGetProperty("pathContains", out var path) && path.ValueKind == JsonValueKind.Array
                ? path.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToArray()
                : Array.Empty<string>(),
            UrlRegex = json.TryGetProperty("urlRegex", out var regex) ? regex.GetString() : null
        };
    }

    public JsonObject ToJson()
    {
        var obj = new JsonObject
        {
            ["host"] = new JsonArray(Host.Select(x => JsonValue.Create(x)).ToArray()),
            ["pathContains"] = new JsonArray(PathContains.Select(x => JsonValue.Create(x)).ToArray())
        };
        if (!string.IsNullOrEmpty(UrlRegex))
            obj["urlRegex"] = UrlRegex;
        return obj;
    }
}

public class RuleCfg
{
    public string Name { get; set; } = "";
    public WhenCfg When { get; set; } = new();
    public ActionCfg Action { get; set; } = new();
    public bool Enabled { get; set; } = true;

    public static RuleCfg FromJson(JsonElement json)
    {
        return new RuleCfg
        {
            Name = json.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "",
            When = json.TryGetProperty("when", out var when) ? WhenCfg.FromJson(when) : new WhenCfg(),
            Action = json.TryGetProperty("action", out var action) ? ActionCfg.FromJson(action) : new ActionCfg(),
            Enabled = json.TryGetProperty("enabled", out var enabled) ? enabled.GetBoolean() : true
        };
    }

    public JsonObject ToJson()
    {
        return new JsonObject
        {
            ["name"] = Name,
            ["when"] = When.ToJson(),
            ["action"] = Action.ToJson(),
            ["enabled"] = Enabled
        };
    }
}

public class UrlRouterConfig
{
    public int Version { get; set; } = 1;
    public ActionCfg Default { get; set; } = new();
    public List<RuleCfg> Rules { get; set; } = new();
    public bool RemoveTrackingParamsGlobal { get; set; } = false;
    public string[] TrackingParamsToRemoveGlobal { get; set; } = GetDefaultTrackingParams();

    public static string[] GetDefaultTrackingParams()
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

    public static UrlRouterConfig FromJson(JsonElement json)
    {
        return new UrlRouterConfig
        {
            Version = json.TryGetProperty("version", out var version) ? version.GetInt32() : 1,
            Default = json.TryGetProperty("default", out var def) ? ActionCfg.FromJson(def) : new ActionCfg(),
            Rules = json.TryGetProperty("rules", out var rules) && rules.ValueKind == JsonValueKind.Array
                ? rules.EnumerateArray().Select(RuleCfg.FromJson).ToList()
                : new List<RuleCfg>(),
            RemoveTrackingParamsGlobal = json.TryGetProperty("removeTrackingParamsGlobal", out var removeTracking) && removeTracking.GetBoolean(),
            TrackingParamsToRemoveGlobal = json.TryGetProperty("trackingParamsToRemoveGlobal", out var trackingParams) && trackingParams.ValueKind == JsonValueKind.Array
                ? trackingParams.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToArray()
                : GetDefaultTrackingParams()
        };
    }

    public JsonObject ToJson()
    {
        var obj = new JsonObject
        {
            ["version"] = Version,
            ["default"] = Default.ToJson(),
            ["rules"] = new JsonArray(Rules.Select(x => x.ToJson()).ToArray())
        };
        if (RemoveTrackingParamsGlobal)
            obj["removeTrackingParamsGlobal"] = RemoveTrackingParamsGlobal;
        if (TrackingParamsToRemoveGlobal != null && TrackingParamsToRemoveGlobal.Length > 0)
            obj["trackingParamsToRemoveGlobal"] = new JsonArray(TrackingParamsToRemoveGlobal.Select(x => JsonValue.Create(x)).ToArray());
        return obj;
    }

    public static UrlRouterConfig Defaults()
    {
        return new UrlRouterConfig
        {
            Version = 1,
            Default = new ActionCfg
            {
                Type = "app",
                Target = @"C:\Program Files\Mozilla Firefox\firefox.exe",
                Args = Array.Empty<string>()
            },
            Rules = new List<RuleCfg>(),
            RemoveTrackingParamsGlobal = false,
            TrackingParamsToRemoveGlobal = GetDefaultTrackingParams()
        };
    }
}
