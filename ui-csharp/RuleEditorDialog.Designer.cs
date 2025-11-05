namespace UrlRouterUI;

partial class RuleEditorDialog
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.lblName = new Label();
        this.txtName = new TextBox();
        this.lblHosts = new Label();
        this.txtHosts = new TextBox();
        this.lblPathContains = new Label();
        this.txtPathContains = new TextBox();
        this.lblUrlRegex = new Label();
        this.txtUrlRegex = new TextBox();
        this.lblTarget = new Label();
        this.txtTarget = new TextBox();
        this.lblArgs = new Label();
        this.txtArgs = new TextBox();
        this.lblBrowsers = new Label();
        this.cmbBrowsers = new ComboBox();
        this.chkEnabled = new CheckBox();
        this.chkRemoveTracking = new CheckBox();
        this.radUseGlobalList = new RadioButton();
        this.radUseCustomList = new RadioButton();
        this.btnViewGlobalList = new Button();
        this.lblTrackingParams = new Label();
        this.txtTrackingParams = new TextBox();
        this.btnOK = new Button();
        this.btnCancel = new Button();
        this.SuspendLayout();
        
        // 
        // lblName
        // 
        this.lblName.AutoSize = true;
        this.lblName.Location = new Point(12, 15);
        this.lblName.Name = "lblName";
        this.lblName.Size = new Size(70, 15);
        this.lblName.TabIndex = 0;
        this.lblName.Text = "Rule Name:";
        
        // 
        // txtName
        // 
        this.txtName.Location = new Point(12, 33);
        this.txtName.Name = "txtName";
        this.txtName.Size = new Size(560, 23);
        this.txtName.TabIndex = 1;
        
        // 
        // lblHosts
        // 
        this.lblHosts.AutoSize = true;
        this.lblHosts.Location = new Point(12, 65);
        this.lblHosts.Name = "lblHosts";
        this.lblHosts.Size = new Size(40, 15);
        this.lblHosts.TabIndex = 2;
        this.lblHosts.Text = "Hosts:";
        
        // 
        // txtHosts
        // 
        this.txtHosts.Location = new Point(12, 83);
        this.txtHosts.Name = "txtHosts";
        this.txtHosts.PlaceholderText = "example.com, *.google.com";
        this.txtHosts.Size = new Size(560, 23);
        this.txtHosts.TabIndex = 3;
        
        // 
        // lblPathContains
        // 
        this.lblPathContains.AutoSize = true;
        this.lblPathContains.Location = new Point(12, 115);
        this.lblPathContains.Name = "lblPathContains";
        this.lblPathContains.Size = new Size(80, 15);
        this.lblPathContains.TabIndex = 4;
        this.lblPathContains.Text = "Path Contains:";
        
        // 
        // txtPathContains
        // 
        this.txtPathContains.Location = new Point(12, 133);
        this.txtPathContains.Name = "txtPathContains";
        this.txtPathContains.PlaceholderText = "admin, dashboard";
        this.txtPathContains.Size = new Size(560, 23);
        this.txtPathContains.TabIndex = 5;
        
        // 
        // lblUrlRegex
        // 
        this.lblUrlRegex.AutoSize = true;
        this.lblUrlRegex.Location = new Point(12, 165);
        this.lblUrlRegex.Name = "lblUrlRegex";
        this.lblUrlRegex.Size = new Size(90, 15);
        this.lblUrlRegex.TabIndex = 6;
        this.lblUrlRegex.Text = "URL Regex (Optional):";
        
        // 
        // txtUrlRegex
        // 
        this.txtUrlRegex.Location = new Point(12, 183);
        this.txtUrlRegex.Name = "txtUrlRegex";
        this.txtUrlRegex.PlaceholderText = "^https://.*\\.example\\.com/.*";
        this.txtUrlRegex.Size = new Size(560, 23);
        this.txtUrlRegex.TabIndex = 7;
        
        // 
        // lblTarget
        // 
        this.lblTarget.AutoSize = true;
        this.lblTarget.Location = new Point(12, 215);
        this.lblTarget.Name = "lblTarget";
        this.lblTarget.Size = new Size(100, 15);
        this.lblTarget.TabIndex = 8;
        this.lblTarget.Text = "Target Application:";
        
        // 
        // txtTarget
        // 
        this.txtTarget.Location = new Point(12, 233);
        this.txtTarget.Name = "txtTarget";
        this.txtTarget.Size = new Size(400, 23);
        this.txtTarget.TabIndex = 9;
        
        // 
        // lblBrowsers
        // 
        this.lblBrowsers.AutoSize = true;
        this.lblBrowsers.Location = new Point(418, 215);
        this.lblBrowsers.Name = "lblBrowsers";
        this.lblBrowsers.Size = new Size(100, 15);
        this.lblBrowsers.TabIndex = 10;
        this.lblBrowsers.Text = "Detected Browsers:";
        
        // 
        // cmbBrowsers
        // 
        this.cmbBrowsers.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbBrowsers.FormattingEnabled = true;
        this.cmbBrowsers.Location = new Point(418, 233);
        this.cmbBrowsers.Name = "cmbBrowsers";
        this.cmbBrowsers.Size = new Size(154, 23);
        this.cmbBrowsers.TabIndex = 11;
        this.cmbBrowsers.SelectedIndexChanged += new EventHandler(this.cmbBrowsers_SelectedIndexChanged);
        
        // 
        // lblArgs
        // 
        this.lblArgs.AutoSize = true;
        this.lblArgs.Location = new Point(12, 265);
        this.lblArgs.Name = "lblArgs";
        this.lblArgs.Size = new Size(60, 15);
        this.lblArgs.TabIndex = 12;
        this.lblArgs.Text = "Arguments:";
        
        // 
        // txtArgs
        // 
        this.txtArgs.Location = new Point(12, 283);
        this.txtArgs.Name = "txtArgs";
        this.txtArgs.PlaceholderText = "--profile-directory=Default";
        this.txtArgs.Size = new Size(560, 23);
        this.txtArgs.TabIndex = 13;
        
        // 
        // chkEnabled
        // 
        this.chkEnabled.AutoSize = true;
        this.chkEnabled.Checked = true;
        this.chkEnabled.CheckState = CheckState.Checked;
        this.chkEnabled.Location = new Point(12, 320);
        this.chkEnabled.Name = "chkEnabled";
        this.chkEnabled.Size = new Size(70, 19);
        this.chkEnabled.TabIndex = 14;
        this.chkEnabled.Text = "Enabled";
        this.chkEnabled.UseVisualStyleBackColor = true;
        
        // 
        // chkRemoveTracking
        // 
        this.chkRemoveTracking.AutoSize = true;
        this.chkRemoveTracking.Location = new Point(12, 350);
        this.chkRemoveTracking.Name = "chkRemoveTracking";
        this.chkRemoveTracking.Size = new Size(200, 19);
        this.chkRemoveTracking.TabIndex = 15;
        this.chkRemoveTracking.Text = "Remove tracking parameters";
        this.chkRemoveTracking.UseVisualStyleBackColor = true;
        this.chkRemoveTracking.CheckedChanged += new EventHandler(this.chkRemoveTracking_CheckedChanged);
        
        // 
        // radUseGlobalList
        // 
        this.radUseGlobalList.AutoSize = true;
        this.radUseGlobalList.Checked = true;
        this.radUseGlobalList.Location = new Point(30, 375);
        this.radUseGlobalList.Name = "radUseGlobalList";
        this.radUseGlobalList.Size = new Size(120, 19);
        this.radUseGlobalList.TabIndex = 16;
        this.radUseGlobalList.TabStop = true;
        this.radUseGlobalList.Text = "Use global list";
        this.radUseGlobalList.UseVisualStyleBackColor = true;
        this.radUseGlobalList.CheckedChanged += new EventHandler(this.radTrackingList_CheckedChanged);
        
        // 
        // btnViewGlobalList
        // 
        this.btnViewGlobalList.Location = new Point(160, 373);
        this.btnViewGlobalList.Name = "btnViewGlobalList";
        this.btnViewGlobalList.Size = new Size(120, 23);
        this.btnViewGlobalList.TabIndex = 17;
        this.btnViewGlobalList.Text = "View Global List";
        this.btnViewGlobalList.UseVisualStyleBackColor = true;
        this.btnViewGlobalList.Click += new EventHandler(this.btnViewGlobalList_Click);
        
        // 
        // radUseCustomList
        // 
        this.radUseCustomList.AutoSize = true;
        this.radUseCustomList.Location = new Point(30, 400);
        this.radUseCustomList.Name = "radUseCustomList";
        this.radUseCustomList.Size = new Size(125, 19);
        this.radUseCustomList.TabIndex = 18;
        this.radUseCustomList.Text = "Use custom list";
        this.radUseCustomList.UseVisualStyleBackColor = true;
        this.radUseCustomList.CheckedChanged += new EventHandler(this.radTrackingList_CheckedChanged);
        
        // 
        // lblTrackingParams
        // 
        this.lblTrackingParams.AutoSize = true;
        this.lblTrackingParams.Location = new Point(50, 425);
        this.lblTrackingParams.Name = "lblTrackingParams";
        this.lblTrackingParams.Size = new Size(200, 15);
        this.lblTrackingParams.TabIndex = 19;
        this.lblTrackingParams.Text = "Parameters (comma-separated):";
        this.lblTrackingParams.Visible = false;
        
        // 
        // txtTrackingParams
        // 
        this.txtTrackingParams.Location = new Point(50, 443);
        this.txtTrackingParams.Name = "txtTrackingParams";
        this.txtTrackingParams.PlaceholderText = "utm_source, utm_medium, gclid, fbclid";
        this.txtTrackingParams.Size = new Size(522, 23);
        this.txtTrackingParams.TabIndex = 20;
        this.txtTrackingParams.Visible = false;
        
        // 
        // btnOK
        // 
        this.btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.btnOK.Location = new Point(416, 480);
        this.btnOK.Name = "btnOK";
        this.btnOK.Size = new Size(75, 23);
        this.btnOK.TabIndex = 21;
        this.btnOK.Text = "OK";
        this.btnOK.UseVisualStyleBackColor = true;
        this.btnOK.Click += new EventHandler(this.btnOK_Click);
        
        // 
        // btnCancel
        // 
        this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.btnCancel.DialogResult = DialogResult.Cancel;
        this.btnCancel.Location = new Point(497, 480);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new Size(75, 23);
        this.btnCancel.TabIndex = 22;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
        
        // 
        // RuleEditorDialog
        // 
        this.AcceptButton = this.btnOK;
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new Size(584, 515);
        this.Controls.Add(this.txtTrackingParams);
        this.Controls.Add(this.lblTrackingParams);
        this.Controls.Add(this.btnViewGlobalList);
        this.Controls.Add(this.radUseCustomList);
        this.Controls.Add(this.radUseGlobalList);
        this.Controls.Add(this.chkRemoveTracking);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnOK);
        this.Controls.Add(this.chkEnabled);
        this.Controls.Add(this.txtArgs);
        this.Controls.Add(this.lblArgs);
        this.Controls.Add(this.cmbBrowsers);
        this.Controls.Add(this.lblBrowsers);
        this.Controls.Add(this.txtTarget);
        this.Controls.Add(this.lblTarget);
        this.Controls.Add(this.txtUrlRegex);
        this.Controls.Add(this.lblUrlRegex);
        this.Controls.Add(this.txtPathContains);
        this.Controls.Add(this.lblPathContains);
        this.Controls.Add(this.txtHosts);
        this.Controls.Add(this.lblHosts);
        this.Controls.Add(this.txtName);
        this.Controls.Add(this.lblName);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "RuleEditorDialog";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Edit Rule";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private Label lblName;
    private TextBox txtName;
    private Label lblHosts;
    private TextBox txtHosts;
    private Label lblPathContains;
    private TextBox txtPathContains;
    private Label lblUrlRegex;
    private TextBox txtUrlRegex;
    private Label lblTarget;
    private TextBox txtTarget;
    private Label lblArgs;
    private TextBox txtArgs;
    private Label lblBrowsers;
    private ComboBox cmbBrowsers;
    private CheckBox chkEnabled;
    private CheckBox chkRemoveTracking;
    private RadioButton radUseGlobalList;
    private Button btnViewGlobalList;
    private RadioButton radUseCustomList;
    private Label lblTrackingParams;
    private TextBox txtTrackingParams;
    private Button btnOK;
    private Button btnCancel;
}
