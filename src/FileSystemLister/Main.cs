// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileSystemLister;

/// <summary>
/// The main form.
/// </summary>
public partial class Main : Form
{
    /// <summary>
    /// The file system lister service.
    /// </summary>
    private readonly IFileSystemListerService fileSystemListerService = new FileSystemListerService();

    /// <summary>
    /// The language manager.
    /// </summary>
    private readonly ILanguageManager languageManager = new LanguageManager();

    /// <summary>
    /// The background worker.
    /// </summary>
    private readonly BackgroundWorker backgroundWorker = new();

    /// <summary>
    /// The language.
    /// </summary>
    private ILanguage? language;

    /// <summary>
    /// Initializes a new instance of the <see cref="Main"/> class.
    /// </summary>
    public Main()
    {
        this.InitializeComponent();
        this.Initialize();
    }

    /// <summary>
    /// Initializes the language manager.
    /// </summary>
    private void InitializeLanguageManager()
    {
        this.languageManager.SetCurrentLanguage("de-DE");
        this.languageManager.OnLanguageChanged += this.OnLanguageChanged!;
    }

    /// <summary>
    /// Loads the languages to the combo box.
    /// </summary>
    private void LoadLanguagesToCombo()
    {
        foreach (var lang in this.languageManager.GetLanguages())
        {
            this.comboBoxLanguage.Items.Add(lang.Name);
        }

        this.comboBoxLanguage.SelectedIndex = 0;
    }

    /// <summary>
    /// Handles the event that the selected index is changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void ComboBoxLanguageSelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = this.comboBoxLanguage.SelectedItem?.ToString();

        if (string.IsNullOrWhiteSpace(selectedItem))
        {
            return;
        }

        this.languageManager.SetCurrentLanguageFromName(selectedItem);
    }

    /// <summary>
    /// Handles the language checked event args.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void OnLanguageChanged(object sender, EventArgs e)
    {
        this.language = this.languageManager.GetCurrentLanguage();
        this.buttonSelectFolder.Text = this.language.GetWord("SelectFolder");
        this.buttonSaveFile.Text = this.language.GetWord("SaveFile");
        this.buttonStart.Text = this.language.GetWord("Start");
        this.checkBoxBulletinCode.Text = this.language.GetWord("UseBulletinCode");
    }

    /// <summary>
    /// Initializes the data.
    /// </summary>
    private void Initialize()
    {
        this.InitializeCaption();
        this.InitializeLanguageManager();
        this.LoadLanguagesToCombo();
        this.InitializeBackgroundWorker();
    }

    /// <summary>
    /// Initializes the caption.
    /// </summary>
    private void InitializeCaption()
    {
        this.Text = Application.ProductName + @" " + Application.ProductVersion;
    }

    /// <summary>
    /// Initializes the background worker.
    /// </summary>
    private void InitializeBackgroundWorker()
    {
        this.backgroundWorker.DoWork += this.SearchDirectoryBackground!;
        this.backgroundWorker.RunWorkerCompleted += this.EvaluateResult!;
    }

    /// <summary>
    /// Searches the directory in the background.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void SearchDirectoryBackground(object sender, DoWorkEventArgs e)
    {
        var directory = string.Empty;
        var resultFile = string.Empty;
        var useBulletinCode = false;

        this.UiThreadInvoke(() =>
        {
            directory = this.richTextBoxFolder.Text;
            resultFile = this.richTextBoxSaveFile.Text;
            useBulletinCode = this.checkBoxBulletinCode.Checked;
        });

        var fileNames = this.fileSystemListerService.ListFileNames(directory, useBulletinCode);
        this.fileSystemListerService.WriteResultFile(resultFile, fileNames);
    }

    /// <summary>
    /// Evaluates the result.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void EvaluateResult(object sender, RunWorkerCompletedEventArgs e)
    {
        this.LockGui(false);

        if (e.Error is not null)
        {
            this.ShowError(e.Error);
            return;
        }

        MessageBox.Show(this.language?.GetWord("SearchCompletedText"), this.language?.GetWord("SearchCompletedCaption"), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Handles the select folder button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void SelectFolderClick(object sender, EventArgs e)
    {
        var dialog = new FolderBrowserDialog();
        var result = dialog.ShowDialog();

        if (result == DialogResult.OK)
        {
            this.richTextBoxFolder.Text = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// Handles the start button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void StartClick(object sender, EventArgs e)
    {
        try
        {
            this.StartBackgroundScan();
        }
        catch (Exception ex)
        {
            this.ShowError(ex);
        }
    }

    /// <summary>
    /// Starts the background scan.
    /// </summary>
    private void StartBackgroundScan()
    {
        if (!this.CheckFolderAndFilesSelected())
        {
            return;
        }

        this.LockGui(true);
        this.backgroundWorker.RunWorkerAsync();
    }

    /// <summary>
    /// Handles the save file button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void SaveFileClick(object sender, EventArgs e)
    {
        var dialog = this.GetSaveFileDialog();
        var result = dialog.ShowDialog();

        if (result == DialogResult.OK)
        {
            this.richTextBoxSaveFile.Text = dialog.FileName;
        }
    }

    /// <summary>
    /// Gets the save file dialog.
    /// </summary>
    /// <returns>The save file dialog.</returns>
    private SaveFileDialog GetSaveFileDialog()
    {
        if (this.language is null)
        {
            return new SaveFileDialog
            {
                Filter = string.Empty
            };
        }

        return new SaveFileDialog
        {
            Filter = this.language.GetWord("Filter")
        };
    }

    /// <summary>
    /// Checks whether folder and file are selected.
    /// </summary>
    /// <returns>A value indicating whether folder and file are selected or not.</returns>
    private bool CheckFolderAndFilesSelected()
    {
        return this.FolderSelected() && this.FileSelected();
    }

    /// <summary>
    /// Checks whether the folder is selected.
    /// </summary>
    /// <returns>A value indicating whether the folder is selected or not.</returns>
    private bool FolderSelected()
    {
        if (!string.IsNullOrWhiteSpace(this.richTextBoxFolder.Text))
        {
            return true;
        }

        if (this.language is null)
        {
            return false;
        }

        MessageBox.Show(this.language.GetWord("NoFolderSelectedText"), this.language.GetWord("NoFolderSelectedCaption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }

    /// <summary>
    /// Checks whether the file is selected.
    /// </summary>
    /// <returns>A value indicating whether the file is selected or not.</returns>
    private bool FileSelected()
    {
        if (!string.IsNullOrWhiteSpace(this.richTextBoxSaveFile.Text))
        {
            return true;
        }

        if (this.language is null)
        {
            return false;
        }

        MessageBox.Show(this.language.GetWord("NoFileSelectedText"), this.language.GetWord("NoFileSelectedCaption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }

    /// <summary>
    /// Shows an exception in an error message box.
    /// </summary>
    /// <param name="ex">The exception to show.</param>
    private void ShowError(Exception ex)
    {
        var title = this.language?.GetWord("ErrorTitle");
        var text = $"{ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.StackTrace}";
        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// Locks or unlocks the GUI.
    /// </summary>
    /// <param name="locked">A value indicating whether the GUI should be locked or not.</param>
    private void LockGui(bool locked)
    {
        this.buttonSelectFolder.Enabled = !locked;
        this.buttonSaveFile.Enabled = !locked;
        this.buttonStart.Enabled = !locked;
        this.checkBoxBulletinCode.Enabled = !locked;
    }
}
