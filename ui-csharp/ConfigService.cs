using System.Text.Json;

namespace UrlRouterUI;

public class ConfigService
{
    private string? _cachedPath;

    public string GetConfigPath()
    {
        if (_cachedPath != null) return _cachedPath;
        
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dir = Path.Combine(appData, "UrlRouter");
        Directory.CreateDirectory(dir);
        _cachedPath = Path.Combine(dir, "config.json");
        return _cachedPath;
    }

    public UrlRouterConfig Load()
    {
        var path = GetConfigPath();
        if (!File.Exists(path))
        {
            return UrlRouterConfig.Defaults();
        }

        var json = File.ReadAllText(path);
        var doc = JsonDocument.Parse(json);
        return UrlRouterConfig.FromJson(doc.RootElement);
    }

    public void Save(UrlRouterConfig config)
    {
        var path = GetConfigPath();
        var tmp = path + ".tmp";
        
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(config.ToJson(), options);
        
        File.WriteAllText(tmp, json);
        File.Move(tmp, path, true);
        
        WriteBackup(config);
    }

    private void WriteBackup(UrlRouterConfig config)
    {
        var basePath = GetConfigPath();
        var backupsDir = Path.Combine(Path.GetDirectoryName(basePath)!, "backups");
        Directory.CreateDirectory(backupsDir);
        
        var stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var path = Path.Combine(backupsDir, $"config_{stamp}.json");
        
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(config.ToJson(), options);
        File.WriteAllText(path, json);
    }
}
