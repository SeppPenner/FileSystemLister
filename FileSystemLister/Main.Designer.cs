partial class Main
{
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Vom Windows Form-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.buttonSelectFolder = new System.Windows.Forms.Button();
            this.richTextBoxFolder = new System.Windows.Forms.RichTextBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonSaveFile = new System.Windows.Forms.Button();
            this.richTextBoxSaveFile = new System.Windows.Forms.RichTextBox();
            this.checkBoxBulletinCode = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSelectFolder
            // 
            this.buttonSelectFolder.Location = new System.Drawing.Point(3, 33);
            this.buttonSelectFolder.Name = "buttonSelectFolder";
            this.buttonSelectFolder.Size = new System.Drawing.Size(109, 23);
            this.buttonSelectFolder.TabIndex = 0;
            this.buttonSelectFolder.Text = "Ordner auswählen";
            this.buttonSelectFolder.UseVisualStyleBackColor = true;
            this.buttonSelectFolder.Click += new System.EventHandler(this.buttonSelectFolder_Click);
            // 
            // richTextBoxFolder
            // 
            this.richTextBoxFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxFolder.Location = new System.Drawing.Point(3, 63);
            this.richTextBoxFolder.Name = "richTextBoxFolder";
            this.richTextBoxFolder.ReadOnly = true;
            this.richTextBoxFolder.Size = new System.Drawing.Size(279, 135);
            this.richTextBoxFolder.TabIndex = 1;
            this.richTextBoxFolder.Text = "";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(573, 33);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(137, 23);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "Ordnerscan starten";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonSaveFile
            // 
            this.buttonSaveFile.Location = new System.Drawing.Point(288, 33);
            this.buttonSaveFile.Name = "buttonSaveFile";
            this.buttonSaveFile.Size = new System.Drawing.Size(119, 23);
            this.buttonSaveFile.TabIndex = 3;
            this.buttonSaveFile.Text = "Datei speichern unter";
            this.buttonSaveFile.UseVisualStyleBackColor = true;
            this.buttonSaveFile.Click += new System.EventHandler(this.buttonSaveFile_Click);
            // 
            // richTextBoxSaveFile
            // 
            this.richTextBoxSaveFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxSaveFile.Location = new System.Drawing.Point(288, 63);
            this.richTextBoxSaveFile.Name = "richTextBoxSaveFile";
            this.richTextBoxSaveFile.ReadOnly = true;
            this.richTextBoxSaveFile.Size = new System.Drawing.Size(279, 135);
            this.richTextBoxSaveFile.TabIndex = 4;
            this.richTextBoxSaveFile.Text = "";
            // 
            // checkBoxBulletinCode
            // 
            this.checkBoxBulletinCode.AutoSize = true;
            this.checkBoxBulletinCode.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxBulletinCode.Location = new System.Drawing.Point(573, 63);
            this.checkBoxBulletinCode.Name = "checkBoxBulletinCode";
            this.checkBoxBulletinCode.Size = new System.Drawing.Size(137, 17);
            this.checkBoxBulletinCode.TabIndex = 5;
            this.checkBoxBulletinCode.Text = "BB-Code verwenden";
            this.checkBoxBulletinCode.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 3;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelMain.Controls.Add(this.richTextBoxFolder, 0, 2);
            this.tableLayoutPanelMain.Controls.Add(this.richTextBoxSaveFile, 1, 2);
            this.tableLayoutPanelMain.Controls.Add(this.checkBoxBulletinCode, 2, 2);
            this.tableLayoutPanelMain.Controls.Add(this.buttonStart, 2, 1);
            this.tableLayoutPanelMain.Controls.Add(this.buttonSaveFile, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.buttonSelectFolder, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.comboBoxLanguage, 2, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 3;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(713, 201);
            this.tableLayoutPanelMain.TabIndex = 6;
            // 
            // comboBoxLanguage
            // 
            this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLanguage.FormattingEnabled = true;
            this.comboBoxLanguage.Location = new System.Drawing.Point(573, 3);
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.Size = new System.Drawing.Size(137, 21);
            this.comboBoxLanguage.TabIndex = 6;
            this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxLanguage_SelectedIndexChanged);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 201);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(674, 240);
            this.Name = "Main";
            this.Text = "Main";
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button buttonSelectFolder;
    private System.Windows.Forms.RichTextBox richTextBoxFolder;
    private System.Windows.Forms.Button buttonStart;
    private System.Windows.Forms.Button buttonSaveFile;
    private System.Windows.Forms.RichTextBox richTextBoxSaveFile;
    private System.Windows.Forms.CheckBox checkBoxBulletinCode;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
    private System.Windows.Forms.ComboBox comboBoxLanguage;
}