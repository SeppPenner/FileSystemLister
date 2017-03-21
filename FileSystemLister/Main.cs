using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Languages.Implementation;
using Languages.Interfaces;
using UiThreadInvoke;

public partial class Main : Form
{
    private readonly List<string> _files = new List<string>();

    private readonly ILanguageManager _lm = new LanguageManager();
    private readonly BackgroundWorker _worker = new BackgroundWorker();
    private Language _lang;

    public Main()
    {
        InitializeComponent();
        Initialize();
    }

    private void InitializeLanguageManager()
    {
        _lm.SetCurrentLanguage("de-DE");
        _lm.OnLanguageChanged += OnLanguageChanged;
    }

    private void LoadLanguagesToCombo()
    {
        foreach (var lang in _lm.GetLanguages())
            comboBoxLanguage.Items.Add(lang.Name);
        comboBoxLanguage.SelectedIndex = 0;
    }

    private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (comboBoxLanguage.SelectedItem.ToString())
        {
            case "Deutsch":
                _lm.SetCurrentLanguage("de-DE");
                break;
            case "English (US)":
                _lm.SetCurrentLanguage("en-US");
                break;
        }
    }

    private void OnLanguageChanged(object sender, EventArgs eventArgs)
    {
        _lang = _lm.GetCurrentLanguage();
        buttonSelectFolder.Text = _lang.GetWord("SelectFolder");
        buttonSaveFile.Text = _lang.GetWord("SaveFile");
        buttonStart.Text = _lang.GetWord("Start");
        checkBoxBulletinCode.Text = _lang.GetWord("UseBulletinCode");
    }

    private void Initialize()
    {
        InitializeCaption();
        InitializeLanguageManager();
        LoadLanguagesToCombo();
        InitializeBackgroundWorker();
    }

    private void InitializeCaption()
    {
        Text = Application.ProductName + @" " + Application.ProductVersion;
    }

    private void InitializeBackgroundWorker()
    {
        _worker.DoWork += SearchDirectoryBackground;
        _worker.RunWorkerCompleted += EvaluateResult;
    }

    private void SearchDirectoryBackground(object sender, DoWorkEventArgs e)
    {
        var directory = "";
        var bbCode = false;
        this.UiThreadInvoke(() =>
        {
            directory = richTextBoxFolder.Text;
            bbCode = checkBoxBulletinCode.Checked;
        });
        SearchAndWriteResult(directory, bbCode);
    }

    private void SearchAndWriteResult(string directory, bool bbCode)
    {
        SearchDirectory(directory, bbCode);
        WriteResultFile();
    }

    private void SearchDirectory(string directory, bool bbCode)
    {
        try
        {
            SearchFilesAndDirectories(directory, bbCode);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message + ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SearchFilesAndDirectories(string directory, bool bbCode)
    {
        SaveFileNames(directory, bbCode);
        SearchDirectories(directory, bbCode);
    }

    private void SaveFileNames(string directory, bool bbCode)
    {
        foreach (var file in Directory.EnumerateFiles(directory))
            try
            {
                SaveFileName(file, bbCode);
            }
            catch (Exception)
            {
                // ignored
            }
    }

    private void SearchDirectories(string directory, bool bbCode)
    {
        foreach (var dir in Directory.EnumerateDirectories(directory))
            try
            {
                SearchDirectory(dir, bbCode);
            }
            catch (Exception)
            {
                // ignored
            }
    }

    private void SaveFileName(string file, bool bbCode)
    {
        if (bbCode)
            _files.Add("[*]" + Path.GetFileName(file));
        else
            _files.Add(Path.GetFileName(file));
    }

    private void WriteResultFile()
    {
        try
        {
            CheckBbCodeAndWriteLines(GetResultDirectory());
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message + ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private string GetResultDirectory()
    {
        var resultDirectory = "";
        this.UiThreadInvoke(() => { resultDirectory = richTextBoxSaveFile.Text; });
        return resultDirectory;
    }

    private void CheckBbCodeAndWriteLines(string resultDirectory)
    {
        CheckBbCodeForFile();
        File.WriteAllLines(resultDirectory, _files);
    }

    private void CheckBbCodeForFile()
    {
        if (!checkBoxBulletinCode.Checked) return;
        _files.Insert(0, "[list]");
        _files.Add("[/list]");
    }

    private void EvaluateResult(object sender, RunWorkerCompletedEventArgs e)
    {
        MessageBox.Show(_lang.GetWord("SearchCompletedText"), _lang.GetWord("SearchCompletedCaption"),
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        LockGui(false);
    }

    private void buttonSelectFolder_Click(object sender, EventArgs e)
    {
        var dialog = new FolderBrowserDialog();
        var result = dialog.ShowDialog();
        if (result == DialogResult.OK)
            richTextBoxFolder.Text = dialog.SelectedPath;
    }

    private void buttonStart_Click(object sender, EventArgs e)
    {
        try
        {
            StartBackgroundScan();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message + ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StartBackgroundScan()
    {
        if (!CheckFoldersSelected()) return;
        _files.Clear();
        LockGui(true);
        _worker.RunWorkerAsync();
    }

    private void buttonSaveFile_Click(object sender, EventArgs e)
    {
        var dialog = GetSaveFileDialog();
        var result = dialog.ShowDialog();
        if (result == DialogResult.OK)
            richTextBoxSaveFile.Text = dialog.FileName;
    }

    private SaveFileDialog GetSaveFileDialog()
    {
        return new SaveFileDialog
        {
            Filter = _lang.GetWord("Filter")
        };
    }

    private bool CheckFoldersSelected()
    {
        return FolderSelected() && FileSelected();
    }

    private bool FolderSelected()
    {
        if (!string.IsNullOrWhiteSpace(richTextBoxFolder.Text)) return true;
        MessageBox.Show(_lang.GetWord("NoFolderSelectedText"), _lang.GetWord("NoFolderSelectedCaption"),
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }

    private bool FileSelected()
    {
        if (!string.IsNullOrWhiteSpace(richTextBoxSaveFile.Text)) return true;
        MessageBox.Show(_lang.GetWord("NoFileSelectedText"), _lang.GetWord("NoFileSelectedCaption"),
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }

    private void LockGui(bool locked)
    {
        buttonSelectFolder.Enabled = !locked;
        buttonSaveFile.Enabled = !locked;
        buttonStart.Enabled = !locked;
        checkBoxBulletinCode.Enabled = !locked;
    }
}