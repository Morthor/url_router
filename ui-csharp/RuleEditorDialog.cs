namespace UrlRouterUI;

public partial class RuleEditorDialog : Form
{
    public RuleCfg Rule { get; private set; }

    private readonly List<BrowserApp> _browsers;
    private readonly string[] _globalTrackingParams;

    public RuleEditorDialog(List<BrowserApp> browsers, RuleCfg? existingRule = null, string[]? globalTrackingParams = null)
    {
        _browsers = browsers;
        Rule = existingRule ?? new RuleCfg();
        _globalTrackingParams = globalTrackingParams ?? UrlRouterConfig.GetDefaultTrackingParams();
        InitializeComponent();
        LoadRule();
    }

    private void LoadRule()
    {
        txtName.Text = Rule.Name;
        txtHosts.Text = string.Join(", ", Rule.When.Host);
        txtPathContains.Text = string.Join(", ", Rule.When.PathContains);
        txtUrlRegex.Text = Rule.When.UrlRegex ?? "";
        txtTarget.Text = Rule.Action.Target;
        txtArgs.Text = string.Join(" ", Rule.Action.Args);
        chkEnabled.Checked = Rule.Enabled;
        
        // Load tracking removal settings
        chkRemoveTracking.Checked = Rule.Action.RemoveTrackingParams;
        if (Rule.Action.TrackingParamsToRemove != null && Rule.Action.TrackingParamsToRemove.Length > 0)
        {
            radUseCustomList.Checked = true;
            txtTrackingParams.Text = string.Join(", ", Rule.Action.TrackingParamsToRemove);
        }
        else
        {
            radUseGlobalList.Checked = true;
            txtTrackingParams.Text = "";
        }
        
        UpdateTrackingControlsVisibility();

        // Populate browser dropdown
        cmbBrowsers.Items.Clear();
        cmbBrowsers.Items.AddRange(_browsers.ToArray());
        
        // Restore browser selection if Browser property is set
        if (!string.IsNullOrEmpty(Rule.Action.Browser))
        {
            var browser = _browsers.FirstOrDefault(b => b.Name == Rule.Action.Browser);
            if (browser != null)
            {
                cmbBrowsers.SelectedItem = browser;
            }
        }
        // If Browser not set but Target matches a known browser, select it
        else if (!string.IsNullOrEmpty(Rule.Action.Target))
        {
            var targetPath = Rule.Action.Target.ToLowerInvariant();
            var browser = _browsers.FirstOrDefault(b => 
                b.ExePath.ToLowerInvariant() == targetPath);
            if (browser != null)
            {
                cmbBrowsers.SelectedItem = browser;
                Rule.Action.Browser = browser.Name; // Set it for future saves
            }
        }
    }

    private void SaveRule()
    {
        Rule.Name = txtName.Text.Trim();
        Rule.When.Host = txtHosts.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim()).ToArray();
        Rule.When.PathContains = txtPathContains.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim()).ToArray();
        Rule.When.UrlRegex = string.IsNullOrWhiteSpace(txtUrlRegex.Text) ? null : txtUrlRegex.Text.Trim();
        Rule.Action.Target = txtTarget.Text.Trim();
        Rule.Action.Args = txtArgs.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Rule.Enabled = chkEnabled.Checked;
        
        // Save tracking removal settings
        Rule.Action.RemoveTrackingParams = chkRemoveTracking.Checked;
        if (chkRemoveTracking.Checked && radUseCustomList.Checked && !string.IsNullOrWhiteSpace(txtTrackingParams.Text))
        {
            Rule.Action.TrackingParamsToRemove = txtTrackingParams.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();
        }
        else
        {
            Rule.Action.TrackingParamsToRemove = null;
        }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Please enter a rule name.", "Validation Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtTarget.Text))
        {
            MessageBox.Show("Please enter a target application path.", "Validation Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SaveRule();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void cmbBrowsers_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbBrowsers.SelectedItem is BrowserApp browser)
        {
            txtTarget.Text = browser.ExePath;
            Rule.Action.Browser = browser.Name; // Set Browser property for display
        }
    }

    private void chkRemoveTracking_CheckedChanged(object sender, EventArgs e)
    {
        UpdateTrackingControlsVisibility();
    }

    private void radTrackingList_CheckedChanged(object sender, EventArgs e)
    {
        UpdateTrackingControlsVisibility();
    }

    private void UpdateTrackingControlsVisibility()
    {
        var trackingEnabled = chkRemoveTracking.Checked;
        radUseGlobalList.Enabled = trackingEnabled;
        radUseCustomList.Enabled = trackingEnabled;
        btnViewGlobalList.Enabled = trackingEnabled;
        
        var showCustomList = trackingEnabled && radUseCustomList.Checked;
        lblTrackingParams.Visible = showCustomList;
        txtTrackingParams.Visible = showCustomList;
    }

    private void btnViewGlobalList_Click(object sender, EventArgs e)
    {
        // Format parameters as comma-separated string for easy copying
        var paramList = string.Join(", ", _globalTrackingParams);
        
        // Create a dialog with a text box that's easy to copy from
        using var dialog = new Form
        {
            Text = "Global Tracking Parameters",
            Size = new Size(700, 200),
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false
        };
        
        var label = new Label
        {
            Text = "Global Tracking Parameters (comma-separated):",
            Dock = DockStyle.Top,
            Height = 30,
            Padding = new Padding(10, 10, 10, 5)
        };
        
        // Create a text box that's selectable and copyable
        var textBox = new TextBox
        {
            Multiline = true,
            ReadOnly = false, // Allow selection even if read-only, but make it editable for easier copying
            ScrollBars = ScrollBars.Horizontal,
            Text = paramList,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 9F),
            Padding = new Padding(5),
            Margin = new Padding(10)
        };
        
        // Select all text by default for easy copying
        textBox.SelectAll();
        textBox.Focus();
        
        var labelCount = new Label
        {
            Text = $"Total: {_globalTrackingParams.Length} parameters",
            Dock = DockStyle.Bottom,
            Height = 30,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(5)
        };
        
        var btnClose = new Button
        {
            Text = "Close",
            DialogResult = DialogResult.OK,
            Dock = DockStyle.Bottom,
            Height = 35
        };
        
        dialog.Controls.Add(textBox);
        dialog.Controls.Add(label);
        dialog.Controls.Add(labelCount);
        dialog.Controls.Add(btnClose);
        dialog.AcceptButton = btnClose;
        
        dialog.ShowDialog(this);
    }
}
