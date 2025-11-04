namespace UrlRouterUI;

public partial class RuleEditorDialog : Form
{
    public RuleCfg Rule { get; private set; }

    private readonly List<BrowserApp> _browsers;

    public RuleEditorDialog(List<BrowserApp> browsers, RuleCfg? existingRule = null)
    {
        _browsers = browsers;
        Rule = existingRule ?? new RuleCfg();
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
}
