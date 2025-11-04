namespace UrlRouterUI;

partial class MainForm
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
        this.pnlDefaultBrowserStatus = new Panel();
        this.lblDefaultBrowserStatus = new Label();
        this.btnSetAsDefault = new Button();
        this.tabControl = new TabControl();
        this.tabRules = new TabPage();
        this.tabDefault = new TabPage();
        this.tabTest = new TabPage();
        
        // Rules tab
        this.lstRules = new ListView();
        this.colName = new ColumnHeader();
        this.colHost = new ColumnHeader();
        this.colBrowser = new ColumnHeader();
        this.colTarget = new ColumnHeader();
        this.btnAddRule = new Button();
        this.btnEditRule = new Button();
        this.btnDeleteRule = new Button();
        this.btnMoveUp = new Button();
        this.btnMoveDown = new Button();
        this.btnSaveRules = new Button();
        
        // Default tab
        this.lblDefaultTarget = new Label();
        this.txtDefaultTarget = new TextBox();
        this.lblDefaultArgs = new Label();
        this.txtDefaultArgs = new TextBox();
        this.lblDefaultBrowser = new Label();
        this.cmbDefaultBrowser = new ComboBox();
        this.btnSave = new Button();
        
        // Test tab
        this.lblTestUrl = new Label();
        this.txtTestUrl = new TextBox();
        this.btnTestUrl = new Button();
        
        this.pnlDefaultBrowserStatus.SuspendLayout();
        this.tabControl.SuspendLayout();
        this.tabRules.SuspendLayout();
        this.tabDefault.SuspendLayout();
        this.tabTest.SuspendLayout();
        this.SuspendLayout();
        
        // 
        // pnlDefaultBrowserStatus
        // 
        this.pnlDefaultBrowserStatus.Controls.Add(this.lblDefaultBrowserStatus);
        this.pnlDefaultBrowserStatus.Controls.Add(this.btnSetAsDefault);
        this.pnlDefaultBrowserStatus.Dock = DockStyle.Top;
        this.pnlDefaultBrowserStatus.Location = new Point(0, 0);
        this.pnlDefaultBrowserStatus.Name = "pnlDefaultBrowserStatus";
        this.pnlDefaultBrowserStatus.Size = new Size(800, 50);
        this.pnlDefaultBrowserStatus.TabIndex = 0;
        this.pnlDefaultBrowserStatus.BackColor = Color.WhiteSmoke;
        this.pnlDefaultBrowserStatus.BorderStyle = BorderStyle.FixedSingle;
        
        // 
        // lblDefaultBrowserStatus
        // 
        this.lblDefaultBrowserStatus.AutoSize = true;
        this.lblDefaultBrowserStatus.Location = new Point(12, 17);
        this.lblDefaultBrowserStatus.Name = "lblDefaultBrowserStatus";
        this.lblDefaultBrowserStatus.Size = new Size(200, 15);
        this.lblDefaultBrowserStatus.TabIndex = 0;
        this.lblDefaultBrowserStatus.Text = "Checking default browser status...";
        
        // 
        // btnSetAsDefault
        // 
        this.btnSetAsDefault.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.btnSetAsDefault.Location = new Point(690, 12);
        this.btnSetAsDefault.Name = "btnSetAsDefault";
        this.btnSetAsDefault.Size = new Size(100, 26);
        this.btnSetAsDefault.TabIndex = 1;
        this.btnSetAsDefault.Text = "Set as Default";
        this.btnSetAsDefault.UseVisualStyleBackColor = true;
        this.btnSetAsDefault.Click += new EventHandler(this.btnSetAsDefault_Click);
        
        // 
        // tabControl
        // 
        this.tabControl.Controls.Add(this.tabRules);
        this.tabControl.Controls.Add(this.tabDefault);
        this.tabControl.Controls.Add(this.tabTest);
        this.tabControl.Dock = DockStyle.Fill;
        this.tabControl.Location = new Point(0, 50);
        this.tabControl.Name = "tabControl";
        this.tabControl.SelectedIndex = 0;
        this.tabControl.Size = new Size(800, 550);
        this.tabControl.TabIndex = 1;
        
        // 
        // tabRules
        // 
        this.tabRules.Controls.Add(this.lstRules);
        this.tabRules.Controls.Add(this.btnAddRule);
        this.tabRules.Controls.Add(this.btnEditRule);
        this.tabRules.Controls.Add(this.btnDeleteRule);
        this.tabRules.Controls.Add(this.btnMoveUp);
        this.tabRules.Controls.Add(this.btnMoveDown);
        this.tabRules.Controls.Add(this.btnSaveRules);
        this.tabRules.Location = new Point(4, 24);
        this.tabRules.Name = "tabRules";
        this.tabRules.Padding = new Padding(3);
        this.tabRules.Size = new Size(792, 572);
        this.tabRules.TabIndex = 0;
        this.tabRules.Text = "Rules";
        this.tabRules.UseVisualStyleBackColor = true;
        
        // 
        // lstRules
        // 
        this.lstRules.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.lstRules.Columns.AddRange(new ColumnHeader[] { this.colName, this.colHost, this.colBrowser, this.colTarget });
        this.lstRules.FullRowSelect = true;
        this.lstRules.GridLines = true;
        this.lstRules.Location = new Point(6, 6);
        this.lstRules.MultiSelect = false;
        this.lstRules.Name = "lstRules";
        this.lstRules.Size = new Size(780, 460);
        this.lstRules.TabIndex = 0;
        this.lstRules.UseCompatibleStateImageBehavior = false;
        this.lstRules.View = View.Details;
        this.lstRules.DoubleClick += new EventHandler(this.lstRules_DoubleClick);
        
        // 
        // colName
        // 
        this.colName.Text = "Name";
        this.colName.Width = 200;
        
        // 
        // colHost
        // 
        this.colHost.Text = "Hosts";
        this.colHost.Width = 300;
        
        // 
        // colBrowser
        // 
        this.colBrowser.Text = "Browser";
        this.colBrowser.Width = 120;
        
        // colTarget
        // 
        this.colTarget.Text = "Target";
        this.colTarget.Width = 260;
        
        // 
        // btnAddRule
        // 
        this.btnAddRule.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.btnAddRule.Location = new Point(6, 480);
        this.btnAddRule.Name = "btnAddRule";
        this.btnAddRule.Size = new Size(100, 35);
        this.btnAddRule.TabIndex = 1;
        this.btnAddRule.Text = "+ Add Rule";
        this.btnAddRule.UseVisualStyleBackColor = true;
        this.btnAddRule.Click += new EventHandler(this.btnAddRule_Click);
        
        // 
        // btnEditRule
        // 
        this.btnEditRule.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.btnEditRule.Location = new Point(112, 480);
        this.btnEditRule.Name = "btnEditRule";
        this.btnEditRule.Size = new Size(100, 35);
        this.btnEditRule.TabIndex = 2;
        this.btnEditRule.Text = "✏ Edit Rule";
        this.btnEditRule.UseVisualStyleBackColor = true;
        this.btnEditRule.Click += new EventHandler(this.btnEditRule_Click);
        
        // 
        // btnDeleteRule
        // 
        this.btnDeleteRule.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.btnDeleteRule.Location = new Point(218, 480);
        this.btnDeleteRule.Name = "btnDeleteRule";
        this.btnDeleteRule.Size = new Size(100, 35);
        this.btnDeleteRule.TabIndex = 3;
        this.btnDeleteRule.Text = "× Delete Rule";
        this.btnDeleteRule.UseVisualStyleBackColor = true;
        this.btnDeleteRule.Click += new EventHandler(this.btnDeleteRule_Click);
        
        // 
        // btnMoveUp
        // 
        this.btnMoveUp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.btnMoveUp.Location = new Point(570, 480);
        this.btnMoveUp.Name = "btnMoveUp";
        this.btnMoveUp.Size = new Size(100, 35);
        this.btnMoveUp.TabIndex = 4;
        this.btnMoveUp.Text = "↑ Move Up";
        this.btnMoveUp.UseVisualStyleBackColor = true;
        this.btnMoveUp.Click += new EventHandler(this.btnMoveUp_Click);
        
        // 
        // btnMoveDown
        // 
        this.btnMoveDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.btnMoveDown.Location = new Point(680, 480);
        this.btnMoveDown.Name = "btnMoveDown";
        this.btnMoveDown.Size = new Size(100, 35);
        this.btnMoveDown.TabIndex = 5;
        this.btnMoveDown.Text = "↓ Move Down";
        this.btnMoveDown.UseVisualStyleBackColor = true;
        this.btnMoveDown.Click += new EventHandler(this.btnMoveDown_Click);
        
        // 
        // btnSaveRules
        // 
        this.btnSaveRules.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.btnSaveRules.Location = new Point(324, 480);
        this.btnSaveRules.Name = "btnSaveRules";
        this.btnSaveRules.Size = new Size(100, 35);
        this.btnSaveRules.TabIndex = 6;
        this.btnSaveRules.Text = "💾 Save Rules";
        this.btnSaveRules.UseVisualStyleBackColor = true;
        this.btnSaveRules.Click += new EventHandler(this.btnSaveRules_Click);
        
        // 
        // tabDefault
        // 
        this.tabDefault.Controls.Add(this.lblDefaultTarget);
        this.tabDefault.Controls.Add(this.txtDefaultTarget);
        this.tabDefault.Controls.Add(this.lblDefaultArgs);
        this.tabDefault.Controls.Add(this.txtDefaultArgs);
        this.tabDefault.Controls.Add(this.lblDefaultBrowser);
        this.tabDefault.Controls.Add(this.cmbDefaultBrowser);
        this.tabDefault.Controls.Add(this.btnSave);
        this.tabDefault.Location = new Point(4, 24);
        this.tabDefault.Name = "tabDefault";
        this.tabDefault.Padding = new Padding(3);
        this.tabDefault.Size = new Size(792, 572);
        this.tabDefault.TabIndex = 1;
        this.tabDefault.Text = "Default";
        this.tabDefault.UseVisualStyleBackColor = true;
        
        // 
        // lblDefaultTarget
        // 
        this.lblDefaultTarget.AutoSize = true;
        this.lblDefaultTarget.Location = new Point(6, 20);
        this.lblDefaultTarget.Name = "lblDefaultTarget";
        this.lblDefaultTarget.Size = new Size(80, 15);
        this.lblDefaultTarget.TabIndex = 0;
        this.lblDefaultTarget.Text = "Target Path:";
        
        // 
        // txtDefaultTarget
        // 
        this.txtDefaultTarget.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.txtDefaultTarget.Location = new Point(6, 38);
        this.txtDefaultTarget.Name = "txtDefaultTarget";
        this.txtDefaultTarget.Size = new Size(780, 23);
        this.txtDefaultTarget.TabIndex = 1;
        
        // 
        // lblDefaultArgs
        // 
        this.lblDefaultArgs.AutoSize = true;
        this.lblDefaultArgs.Location = new Point(6, 80);
        this.lblDefaultArgs.Name = "lblDefaultArgs";
        this.lblDefaultArgs.Size = new Size(60, 15);
        this.lblDefaultArgs.TabIndex = 2;
        this.lblDefaultArgs.Text = "Arguments:";
        
        // 
        // txtDefaultArgs
        // 
        this.txtDefaultArgs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.txtDefaultArgs.Location = new Point(6, 98);
        this.txtDefaultArgs.Name = "txtDefaultArgs";
        this.txtDefaultArgs.Size = new Size(780, 23);
        this.txtDefaultArgs.TabIndex = 3;
        
        // 
        // lblDefaultBrowser
        // 
        this.lblDefaultBrowser.AutoSize = true;
        this.lblDefaultBrowser.Location = new Point(6, 140);
        this.lblDefaultBrowser.Name = "lblDefaultBrowser";
        this.lblDefaultBrowser.Size = new Size(100, 15);
        this.lblDefaultBrowser.TabIndex = 4;
        this.lblDefaultBrowser.Text = "Detected Browsers:";
        
        // 
        // cmbDefaultBrowser
        // 
        this.cmbDefaultBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.cmbDefaultBrowser.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbDefaultBrowser.FormattingEnabled = true;
        this.cmbDefaultBrowser.Location = new Point(6, 158);
        this.cmbDefaultBrowser.Name = "cmbDefaultBrowser";
        this.cmbDefaultBrowser.Size = new Size(780, 23);
        this.cmbDefaultBrowser.TabIndex = 5;
        this.cmbDefaultBrowser.SelectedIndexChanged += new EventHandler(this.cmbDefaultBrowser_SelectedIndexChanged);
        
        // 
        // btnSave
        // 
        this.btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.btnSave.Location = new Point(690, 200);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new Size(100, 35);
        this.btnSave.TabIndex = 6;
        this.btnSave.Text = "💾 Save";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += new EventHandler(this.btnSave_Click);
        
        // 
        // tabTest
        // 
        this.tabTest.Controls.Add(this.lblTestUrl);
        this.tabTest.Controls.Add(this.txtTestUrl);
        this.tabTest.Controls.Add(this.btnTestUrl);
        this.tabTest.Location = new Point(4, 24);
        this.tabTest.Name = "tabTest";
        this.tabTest.Padding = new Padding(3);
        this.tabTest.Size = new Size(792, 572);
        this.tabTest.TabIndex = 2;
        this.tabTest.Text = "Test";
        this.tabTest.UseVisualStyleBackColor = true;
        
        // 
        // lblTestUrl
        // 
        this.lblTestUrl.AutoSize = true;
        this.lblTestUrl.Location = new Point(6, 20);
        this.lblTestUrl.Name = "lblTestUrl";
        this.lblTestUrl.Size = new Size(60, 15);
        this.lblTestUrl.TabIndex = 0;
        this.lblTestUrl.Text = "Test URL:";
        
        // 
        // txtTestUrl
        // 
        this.txtTestUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.txtTestUrl.Location = new Point(6, 38);
        this.txtTestUrl.Name = "txtTestUrl";
        this.txtTestUrl.PlaceholderText = "https://www.example.com";
        this.txtTestUrl.Size = new Size(780, 23);
        this.txtTestUrl.TabIndex = 1;
        
        // 
        // btnTestUrl
        // 
        this.btnTestUrl.Location = new Point(6, 80);
        this.btnTestUrl.Name = "btnTestUrl";
        this.btnTestUrl.Size = new Size(100, 35);
        this.btnTestUrl.TabIndex = 2;
        this.btnTestUrl.Text = "🧪 Test URL";
        this.btnTestUrl.UseVisualStyleBackColor = true;
        this.btnTestUrl.Click += new EventHandler(this.btnTestUrl_Click);
        
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(800, 600);
        this.Controls.Add(this.tabControl);
        this.Controls.Add(this.pnlDefaultBrowserStatus);
        this.MinimumSize = new Size(600, 450);
        this.Name = "MainForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "URL Router Configuration";
        
        this.pnlDefaultBrowserStatus.ResumeLayout(false);
        this.pnlDefaultBrowserStatus.PerformLayout();
        this.tabControl.ResumeLayout(false);
        this.tabRules.ResumeLayout(false);
        this.tabDefault.ResumeLayout(false);
        this.tabDefault.PerformLayout();
        this.tabTest.ResumeLayout(false);
        this.tabTest.PerformLayout();
        this.ResumeLayout(false);
    }

    private Panel pnlDefaultBrowserStatus;
    private Label lblDefaultBrowserStatus;
    private Button btnSetAsDefault;
    private TabControl tabControl;
    private TabPage tabRules;
    private TabPage tabDefault;
    private TabPage tabTest;
    private ListView lstRules;
    private ColumnHeader colName;
    private ColumnHeader colHost;
    private ColumnHeader colBrowser;
    private ColumnHeader colTarget;
    private Button btnAddRule;
    private Button btnEditRule;
    private Button btnDeleteRule;
    private Button btnMoveUp;
    private Button btnMoveDown;
    private Button btnSaveRules;
    private Label lblDefaultTarget;
    private TextBox txtDefaultTarget;
    private Label lblDefaultArgs;
    private TextBox txtDefaultArgs;
    private Label lblDefaultBrowser;
    private ComboBox cmbDefaultBrowser;
    private Button btnSave;
    private Label lblTestUrl;
    private TextBox txtTestUrl;
    private Button btnTestUrl;
}
