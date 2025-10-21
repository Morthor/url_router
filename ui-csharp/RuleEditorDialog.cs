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
        }
    }
}
