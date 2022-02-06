// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileSystemLister
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;

    using FileSystemLister.UiThreadInvoke;

    using Languages.Implementation;
    using Languages.Interfaces;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// The files.
        /// </summary>
        private readonly List<string> files = new List<string>();

        /// <summary>
        /// The language manager.
        /// </summary>
        private readonly ILanguageManager languageManager = new LanguageManager();

        /// <summary>
        /// The background worker.
        /// </summary>
        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

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
            this.languageManager.OnLanguageChanged += this.OnLanguageChanged;
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
            this.languageManager.SetCurrentLanguageFromName(this.comboBoxLanguage.SelectedItem.ToString());
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
            this.backgroundWorker.DoWork += this.SearchDirectoryBackground;
            this.backgroundWorker.RunWorkerCompleted += this.EvaluateResult;
        }

        /// <summary>
        /// Searches the directory in the background.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void SearchDirectoryBackground(object sender, DoWorkEventArgs e)
        {
            var directory = string.Empty;
            var bulletinCode = false;

            this.UiThreadInvoke(() =>
            {
                directory = this.richTextBoxFolder.Text;
                bulletinCode = this.checkBoxBulletinCode.Checked;
            });

            this.SearchAndWriteResult(directory, bulletinCode);
        }

        /// <summary>
        /// Searches the directory and writes the result file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="bulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
        private void SearchAndWriteResult(string directory, bool bulletinCode)
        {
            this.SearchDirectory(directory, bulletinCode);
            this.WriteResultFile();
        }

        /// <summary>
        /// Searches the directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="bulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
        private void SearchDirectory(string directory, bool bulletinCode)
        {
            try
            {
                this.SearchFilesAndDirectories(directory, bulletinCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Searches the files and the directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="bulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
        private void SearchFilesAndDirectories(string directory, bool bulletinCode)
        {
            this.SaveFileNames(directory, bulletinCode);
            this.SearchDirectories(directory, bulletinCode);
        }

        /// <summary>
        /// Saves the file names to the result file.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="bulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
        private void SaveFileNames(string directory, bool bulletinCode)
        {
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                try
                {
                    this.SaveFileName(file, bulletinCode);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Searches the directories.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="bulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
        private void SearchDirectories(string directory, bool bulletinCode)
        {
            foreach (var dir in Directory.EnumerateDirectories(directory))
            {
                try
                {
                    this.SearchDirectory(dir, bulletinCode);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Saves the file name.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="bulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
        private void SaveFileName(string file, bool bulletinCode)
        {
            if (bulletinCode)
            {
                this.files.Add("[*]" + Path.GetFileName(file));
            }
            else
            {
                this.files.Add(Path.GetFileName(file));
            }
        }

        /// <summary>
        /// Writes the result file.
        /// </summary>
        private void WriteResultFile()
        {
            try
            {
                this.CheckBbCodeAndWriteLines(this.GetResultDirectory());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gets the result directory.
        /// </summary>
        /// <returns>The result directory.</returns>
        private string GetResultDirectory()
        {
            var resultDirectory = string.Empty;
            this.UiThreadInvoke(() => { resultDirectory = this.richTextBoxSaveFile.Text; });
            return resultDirectory;
        }

        /// <summary>
        /// Checks the bulletin code and writes the lines to a file.
        /// </summary>
        /// <param name="resultDirectory">The result directory.</param>
        private void CheckBbCodeAndWriteLines(string resultDirectory)
        {
            this.CheckBbCodeForFile();
            File.WriteAllLines(resultDirectory, this.files);
        }

        /// <summary>
        /// Checks the bulletin code for a file.
        /// </summary>
        private void CheckBbCodeForFile()
        {
            if (!this.checkBoxBulletinCode.Checked)
            {
                return;
            }

            this.files.Insert(0, "[list]");
            this.files.Add("[/list]");
        }

        /// <summary>
        /// Evaluates the result.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void EvaluateResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.language is null)
            {
                return;
            }

            MessageBox.Show(this.language.GetWord("SearchCompletedText"), this.language.GetWord("SearchCompletedCaption"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.LockGui(false);
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
                MessageBox.Show(ex.Message + ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            this.files.Clear();
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
}