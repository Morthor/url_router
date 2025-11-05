using System.ComponentModel;

namespace UrlRouterUI;

public partial class MainForm : Form
{
    private readonly ConfigService _configService = new();
    private UrlRouterConfig _config;
    private List<BrowserApp> _browsers = new();
    private bool _hasUnsavedChanges = false;

    public MainForm()
    {
        // Set DPI awareness for this form
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        
        InitializeComponent();
        SetIcon();
        LoadConfig();
        LoadBrowsers();
        SetupDataBinding();
        CheckDefaultBrowserStatus();
        this.FormClosing += MainForm_FormClosing;
        this.Activated += MainForm_Activated; // Refresh status when form becomes active
    }

    private void SetIcon()
    {
        try
        {
            // Try to load the icon from the executable's resources
            var iconPath = Path.Combine(Application.StartupPath, "icon.ico");
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }
        }
        catch
        {
            // If icon loading fails, continue without setting an icon
        }
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
        
        // Global tracking removal settings
        chkRemoveTrackingGlobal.Checked = _config.RemoveTrackingParamsGlobal;
        RefreshTrackingParamsList();
        UpdateTrackingRemovalControls();
        
        // Rules
        RefreshRulesList();
    }

    private void RefreshTrackingParamsList()
    {
        lstTrackingParams.Items.Clear();
        foreach (var param in _config.TrackingParamsToRemoveGlobal)
        {
            lstTrackingParams.Items.Add(param);
        }
    }

    private void UpdateTrackingRemovalControls()
    {
        var enabled = chkRemoveTrackingGlobal.Checked;
        lblTrackingParamsList.Enabled = enabled;
        lstTrackingParams.Enabled = enabled;
        txtAddTrackingParam.Enabled = enabled;
        btnAddTrackingParam.Enabled = enabled;
        btnRemoveTrackingParam.Enabled = enabled;
        btnResetTrackingParams.Enabled = enabled;
    }

    private void RefreshRulesList()
    {
        lstRules.Items.Clear();
        foreach (var rule in _config.Rules)
        {
            var item = new ListViewItem(rule.Name);
            item.SubItems.Add(string.Join(", ", rule.When.Host));
            
            // Try to get browser name from Browser property, or match by target path
            string browserDisplay = "Default";
            if (!string.IsNullOrEmpty(rule.Action.Browser))
            {
                browserDisplay = rule.Action.Browser;
            }
            else if (!string.IsNullOrEmpty(rule.Action.Target))
            {
                var targetPath = rule.Action.Target.ToLowerInvariant();
                var browser = _browsers.FirstOrDefault(b => 
                    b.ExePath.ToLowerInvariant() == targetPath);
                if (browser != null)
                {
                    browserDisplay = browser.Name;
                    // Update the Browser property for future saves
                    rule.Action.Browser = browser.Name;
                }
            }
            
            // Add tracking removal indicator
            if (rule.Action.RemoveTrackingParams)
            {
                browserDisplay += " 🧹";
            }
            
            item.SubItems.Add(browserDisplay);
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
            _hasUnsavedChanges = false;
            UpdateWindowTitle();
            MessageBox.Show("Configuration saved successfully!", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MarkAsChanged()
    {
        _hasUnsavedChanges = true;
        UpdateWindowTitle();
    }

    private void UpdateWindowTitle()
    {
        var baseTitle = "URL Router Configuration";
        this.Text = _hasUnsavedChanges ? $"{baseTitle} *" : baseTitle;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        // Update default action
        _config.Default.Target = txtDefaultTarget.Text.Trim();
        _config.Default.Args = txtDefaultArgs.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        // Update global tracking removal settings
        _config.RemoveTrackingParamsGlobal = chkRemoveTrackingGlobal.Checked;
        _config.TrackingParamsToRemoveGlobal = lstTrackingParams.Items.Cast<string>().ToArray();
        
        SaveConfig();
    }

    private void btnSaveRules_Click(object sender, EventArgs e)
    {
        SaveConfig();
    }

    private void btnAddRule_Click(object sender, EventArgs e)
    {
        try
        {
            using var dialog = new RuleEditorDialog(_browsers, null, _config.TrackingParamsToRemoveGlobal);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _config.Rules.Add(dialog.Rule);
                RefreshRulesList();
                MarkAsChanged();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding rule: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnEditRule_Click(object sender, EventArgs e)
    {
        if (lstRules.SelectedItems.Count == 0) return;
        
        var rule = (RuleCfg)lstRules.SelectedItems[0].Tag;
        using var dialog = new RuleEditorDialog(_browsers, rule, _config.TrackingParamsToRemoveGlobal);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var index = _config.Rules.IndexOf(rule);
            if (index >= 0)
            {
                _config.Rules[index] = dialog.Rule;
                RefreshRulesList();
                MarkAsChanged();
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
            MarkAsChanged();
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
            MarkAsChanged();
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
            MarkAsChanged();
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

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_hasUnsavedChanges)
        {
            var result = MessageBox.Show(
                "You have unsaved changes. Do you want to save before closing?",
                "Unsaved Changes",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SaveConfig();
            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }

    private void CheckDefaultBrowserStatus()
    {
        var isDefault = DefaultBrowserHelper.IsUrlRouterSetAsDefault();
        if (isDefault)
        {
            lblDefaultBrowserStatus.Text = "✓ URL Router is set as your default browser for HTTP/HTTPS links";
            lblDefaultBrowserStatus.ForeColor = Color.Green;
            btnSetAsDefault.Visible = false;
        }
        else
        {
            lblDefaultBrowserStatus.Text = "⚠ URL Router is NOT set as your default browser. Click the button below to set it up.";
            lblDefaultBrowserStatus.ForeColor = Color.Orange;
            btnSetAsDefault.Visible = true;
        }
    }

    private void MainForm_Activated(object? sender, EventArgs e)
    {
        // Refresh status when form becomes active (user might have changed settings)
        CheckDefaultBrowserStatus();
    }

    private void btnSetAsDefault_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "This will open Windows Settings where you can set URL Router as the default browser for HTTP and HTTPS links.\n\n" +
            "In the settings:\n" +
            "1. Look for 'URL Router' in the app list\n" +
            "2. Click on 'URL Router'\n" +
            "3. Click 'Set default' or 'Manage' to set it as default for HTTP and HTTPS links\n\n" +
            "Open Settings now?",
            "Set as Default Browser",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

        if (result == DialogResult.Yes)
        {
            DefaultBrowserHelper.OpenDefaultAppsSettings();
            // Status will be refreshed when form becomes active again
        }
    }

    private void chkRemoveTrackingGlobal_CheckedChanged(object sender, EventArgs e)
    {
        UpdateTrackingRemovalControls();
        MarkAsChanged();
    }

    private void btnAddTrackingParam_Click(object sender, EventArgs e)
    {
        var param = txtAddTrackingParam.Text.Trim();
        if (string.IsNullOrWhiteSpace(param))
        {
            MessageBox.Show("Please enter a parameter name.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Check if already exists (case-insensitive)
        var exists = lstTrackingParams.Items.Cast<string>()
            .Any(p => p.Equals(param, StringComparison.OrdinalIgnoreCase));
        
        if (exists)
        {
            MessageBox.Show($"Parameter '{param}' is already in the list.", "Duplicate Parameter",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        lstTrackingParams.Items.Add(param);
        txtAddTrackingParam.Clear();
        MarkAsChanged();
    }

    private void btnRemoveTrackingParam_Click(object sender, EventArgs e)
    {
        if (lstTrackingParams.SelectedItem != null)
        {
            lstTrackingParams.Items.Remove(lstTrackingParams.SelectedItem);
            MarkAsChanged();
        }
        else
        {
            MessageBox.Show("Please select a parameter to remove.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void btnResetTrackingParams_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "This will replace your current list with the default tracking parameters. Continue?",
            "Reset to Defaults",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _config.TrackingParamsToRemoveGlobal = UrlRouterConfig.GetDefaultTrackingParams();
            RefreshTrackingParamsList();
            MarkAsChanged();
        }
    }
}
