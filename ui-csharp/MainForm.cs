using System.ComponentModel;

namespace UrlRouterUI;

public partial class MainForm : Form
{
    private readonly ConfigService _configService = new();
    private UrlRouterConfig _config;
    private List<BrowserApp> _browsers = new();

    public MainForm()
    {
        InitializeComponent();
        LoadConfig();
        LoadBrowsers();
        SetupDataBinding();
    }

    private void LoadConfig()
    {
        try
        {
            _config = _configService.Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            _config = UrlRouterConfig.Defaults();
        }
    }

    private void LoadBrowsers()
    {
        try
        {
            _browsers = BrowserDetection.DetectInstalledBrowsers();
        }
        catch
        {
            _browsers = new List<BrowserApp>();
        }
    }

    private void SetupDataBinding()
    {
        // Default browser
        txtDefaultTarget.Text = _config.Default.Target;
        txtDefaultArgs.Text = string.Join(" ", _config.Default.Args);
        
        // Populate browser dropdown
        cmbDefaultBrowser.Items.Clear();
        cmbDefaultBrowser.Items.AddRange(_browsers.ToArray());
        
        // Rules
        RefreshRulesList();
    }

    private void RefreshRulesList()
    {
        lstRules.Items.Clear();
        foreach (var rule in _config.Rules)
        {
            var item = new ListViewItem(rule.Name);
            item.SubItems.Add(string.Join(", ", rule.When.Host));
            item.SubItems.Add(rule.Action.Browser ?? "Default");
            item.SubItems.Add(rule.Action.Target);
            item.Tag = rule;
            
            // Grey out disabled rules
            if (!rule.Enabled)
            {
                item.BackColor = Color.LightGray;
                item.ForeColor = Color.DarkGray;
            }
            
            lstRules.Items.Add(item);
        }
    }

    private void SaveConfig()
    {
        try
        {
            _configService.Save(_config);
            MessageBox.Show("Configuration saved successfully!", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        // Update default action
        _config.Default.Target = txtDefaultTarget.Text.Trim();
        _config.Default.Args = txtDefaultArgs.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        SaveConfig();
    }

    private void btnAddRule_Click(object sender, EventArgs e)
    {
        using var dialog = new RuleEditorDialog(_browsers);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _config.Rules.Add(dialog.Rule);
            RefreshRulesList();
        }
    }

    private void btnEditRule_Click(object sender, EventArgs e)
    {
        if (lstRules.SelectedItems.Count == 0) return;
        
        var rule = (RuleCfg)lstRules.SelectedItems[0].Tag;
        using var dialog = new RuleEditorDialog(_browsers, rule);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var index = _config.Rules.IndexOf(rule);
            if (index >= 0)
            {
                _config.Rules[index] = dialog.Rule;
                RefreshRulesList();
            }
        }
    }

    private void btnDeleteRule_Click(object sender, EventArgs e)
    {
        if (lstRules.SelectedItems.Count == 0) return;
        
        var rule = (RuleCfg)lstRules.SelectedItems[0].Tag;
        var result = MessageBox.Show($"Delete rule '{rule.Name}'?", "Confirm Delete", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            _config.Rules.Remove(rule);
            RefreshRulesList();
        }
    }

    private void btnMoveUp_Click(object sender, EventArgs e)
    {
        if (lstRules.SelectedItems.Count == 0) return;
        
        var selectedIndex = lstRules.SelectedIndices[0];
        if (selectedIndex > 0)
        {
            var rule = _config.Rules[selectedIndex];
            _config.Rules.RemoveAt(selectedIndex);
            _config.Rules.Insert(selectedIndex - 1, rule);
            RefreshRulesList();
            lstRules.Items[selectedIndex - 1].Selected = true;
        }
    }

    private void btnMoveDown_Click(object sender, EventArgs e)
    {
        if (lstRules.SelectedItems.Count == 0) return;
        
        var selectedIndex = lstRules.SelectedIndices[0];
        if (selectedIndex < _config.Rules.Count - 1)
        {
            var rule = _config.Rules[selectedIndex];
            _config.Rules.RemoveAt(selectedIndex);
            _config.Rules.Insert(selectedIndex + 1, rule);
            RefreshRulesList();
            lstRules.Items[selectedIndex + 1].Selected = true;
        }
    }

    private void lstRules_DoubleClick(object sender, EventArgs e)
    {
        if (lstRules.SelectedItems.Count > 0)
        {
            // Double-clicked on a rule - edit it
            btnEditRule_Click(sender, e);
        }
        else
        {
            // Double-clicked on empty space - create new rule
            btnAddRule_Click(sender, e);
        }
    }

    private void btnTestUrl_Click(object sender, EventArgs e)
    {
        var url = txtTestUrl.Text.Trim();
        if (string.IsNullOrEmpty(url))
        {
            MessageBox.Show("Please enter a URL to test.", "Test URL", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || !uri.Scheme.StartsWith("http"))
        {
            MessageBox.Show("Please enter a valid HTTP/HTTPS URL.", "Invalid URL", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Find matching rule
        foreach (var rule in _config.Rules)
        {
            if (MatchesRule(rule, uri, url))
            {
                var args = string.Join(" ", rule.Action.Args);
                MessageBox.Show($"✅ Matches: {rule.Name}\n🚀 Would launch: {rule.Action.Target} {args} \"{url}\"", 
                    "Test Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        // No rule matched, use default
        var defaultArgs = string.Join(" ", _config.Default.Args);
        MessageBox.Show($"⚙️ No rule matched. Would launch default: {_config.Default.Target} {defaultArgs} \"{url}\"", 
            "Test Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private bool MatchesRule(RuleCfg rule, Uri uri, string rawUrl)
    {
        // Check host patterns
        if (rule.When.Host.Length > 0)
        {
            var hostMatches = false;
            foreach (var pattern in rule.When.Host)
            {
                if (MatchesHostPattern(pattern, uri.Host))
                {
                    hostMatches = true;
                    break;
                }
            }
            if (!hostMatches) return false;
        }

        // Check path contains
        if (rule.When.PathContains.Length > 0)
        {
            foreach (var pattern in rule.When.PathContains)
            {
                if (!uri.AbsolutePath.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
        }

        // Check regex
        if (!string.IsNullOrEmpty(rule.When.UrlRegex))
        {
            try
            {
                var regex = new System.Text.RegularExpressions.Regex(rule.When.UrlRegex, 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (!regex.IsMatch(rawUrl)) return false;
            }
            catch
            {
                // Invalid regex, skip
            }
        }

        return true;
    }

    private bool MatchesHostPattern(string pattern, string host)
    {
        host = host.ToLowerInvariant();
        pattern = pattern.ToLowerInvariant();
        
        if (pattern.StartsWith("*."))
        {
            var suffix = pattern.Substring(1); // .example.com
            return host.EndsWith(suffix);
        }
        
        return host == pattern;
    }

    private void cmbDefaultBrowser_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbDefaultBrowser.SelectedItem is BrowserApp browser)
        {
            txtDefaultTarget.Text = browser.ExePath;
        }
    }
}
