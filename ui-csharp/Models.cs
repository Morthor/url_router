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

    public static ActionCfg FromJson(JsonElement json)
    {
        return new ActionCfg
        {
            Type = json.TryGetProperty("type", out var type) ? type.GetString() ?? "app" : "app",
            Target = json.TryGetProperty("target", out var target) ? target.GetString() ?? "" : "",
            Browser = json.TryGetProperty("browser", out var browser) ? browser.GetString() : null,
            Args = json.TryGetProperty("args", out var args) && args.ValueKind == JsonValueKind.Array
                ? args.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToArray()
                : Array.Empty<string>()
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

    public static UrlRouterConfig FromJson(JsonElement json)
    {
        return new UrlRouterConfig
        {
            Version = json.TryGetProperty("version", out var version) ? version.GetInt32() : 1,
            Default = json.TryGetProperty("default", out var def) ? ActionCfg.FromJson(def) : new ActionCfg(),
            Rules = json.TryGetProperty("rules", out var rules) && rules.ValueKind == JsonValueKind.Array
                ? rules.EnumerateArray().Select(RuleCfg.FromJson).ToList()
                : new List<RuleCfg>()
        };
    }

    public JsonObject ToJson()
    {
        return new JsonObject
        {
            ["version"] = Version,
            ["default"] = Default.ToJson(),
            ["rules"] = new JsonArray(Rules.Select(x => x.ToJson()).ToArray())
        };
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
            Rules = new List<RuleCfg>()
        };
    }
}
