using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EasyPad
{
    public partial class Form1 : Form
    {
        private string currentFile = "";
        private string currentLang = "ru";

        private ToolStripStatusLabel posLabel;
        private ToolStripStatusLabel linesLabel;
        private ToolStripStatusLabel charsLabel;
        private ToolStripStatusLabel selLabel;

        private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>()
        {
            {
                "ru", new Dictionary<string, string>()
                {
                    { "about", "О программе" },
                    { "aboutText", "EasyPad v1.0\nПростой текстовый редактор" },
                    { "settings", "Настройки" },
                    { "language", "Язык" },
                    { "file", "Файл" },
                    { "edit", "Правка" },
                    { "new", "Создать" },
                    { "open", "Открыть..." },
                    { "save", "Сохранить..." },
                    { "exit", "Выход" },
                    { "undo", "Отменить" },
                    { "redo", "Повторить" },
                    { "cut", "Вырезать" },
                    { "copy", "Копировать" },
                    { "paste", "Вставить" },
                    { "selectAll", "Выделить всё" },
                    { "newFile", "Новый файл" },
                    { "textFiles", "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*" },
                    { "ok", "OK" },
                    { "cancel", "Отмена" },
                    { "settingsTitle", "Настройки" },
                    { "pos", "Строка" },
                    { "lines", "Строк" },
                    { "chars", "Символов" },
                    { "sel", "Выделено" }
                }
            },
            {
                "en", new Dictionary<string, string>()
                {
                    { "about", "About" },
                    { "aboutText", "EasyPad v1.0\nSimple text editor" },
                    { "settings", "Settings" },
                    { "language", "Language" },
                    { "file", "File" },
                    { "edit", "Edit" },
                    { "new", "New" },
                    { "open", "Open..." },
                    { "save", "Save..." },
                    { "exit", "Exit" },
                    { "undo", "Undo" },
                    { "redo", "Redo" },
                    { "cut", "Cut" },
                    { "copy", "Copy" },
                    { "paste", "Paste" },
                    { "selectAll", "Select All" },
                    { "newFile", "New File" },
                    { "textFiles", "Text files (*.txt)|*.txt|All files (*.*)|*.*" },
                    { "ok", "OK" },
                    { "cancel", "Cancel" },
                    { "settingsTitle", "Settings" },
                    { "pos", "Line" },
                    { "lines", "Lines" },
                    { "chars", "Chars" },
                    { "sel", "Selected" }
                }
            },
            {
                "de", new Dictionary<string, string>()
                {
                    { "about", "Über" },
                    { "aboutText", "EasyPad v1.0\nEinfacher Texteditor" },
                    { "settings", "Einstellungen" },
                    { "language", "Sprache" },
                    { "file", "Datei" },
                    { "edit", "Bearbeiten" },
                    { "new", "Neu" },
                    { "open", "Öffnen..." },
                    { "save", "Speichern..." },
                    { "exit", "Beenden" },
                    { "undo", "Rückgängig" },
                    { "redo", "Wiederholen" },
                    { "cut", "Ausschneiden" },
                    { "copy", "Kopieren" },
                    { "paste", "Einfügen" },
                    { "selectAll", "Alles auswählen" },
                    { "newFile", "Neue Datei" },
                    { "textFiles", "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*" },
                    { "ok", "OK" },
                    { "cancel", "Abbrechen" },
                    { "settingsTitle", "Einstellungen" },
                    { "pos", "Zeile" },
                    { "lines", "Zeilen" },
                    { "chars", "Zeichen" },
                    { "sel", "Ausgewählt" }
                }
            }
        };

        private string T(string key)
        {
            if (translations.ContainsKey(currentLang) && translations[currentLang].ContainsKey(key))
                return translations[currentLang][key];
            return key;
        }

        public Form1() : this(null)
        {
        }

        public Form1(string filePath)
        {
            InitializeComponent();

            CreateStatusLabels();
            BuildMenus();
            ApplyLanguage();

            richTextBox1.TextChanged += richTextBox1_TextChanged;
            richTextBox1.SelectionChanged += richTextBox1_SelectionChanged;
            richTextBox1.KeyUp += richTextBox1_KeyUp;
            richTextBox1.MouseUp += richTextBox1_MouseUp;

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    richTextBox1.Text = File.ReadAllText(filePath);
                    currentFile = filePath;
                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "EasyPad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CreateStatusLabels()
        {
            posLabel = new ToolStripStatusLabel();
            posLabel.BorderSides = ToolStripStatusLabelBorderSides.Right;
            posLabel.AutoSize = true;

            linesLabel = new ToolStripStatusLabel();
            linesLabel.BorderSides = ToolStripStatusLabelBorderSides.Right;
            linesLabel.AutoSize = true;

            charsLabel = new ToolStripStatusLabel();
            charsLabel.BorderSides = ToolStripStatusLabelBorderSides.Right;
            charsLabel.AutoSize = true;

            selLabel = new ToolStripStatusLabel();
            selLabel.AutoSize = true;

            toolStrip2.Items.Add(posLabel);
            toolStrip2.Items.Add(linesLabel);
            toolStrip2.Items.Add(charsLabel);
            toolStrip2.Items.Add(selLabel);
        }

        private void BuildMenus()
        {
            toolStripSplitButton1.DropDownItems.Clear();

            ToolStripMenuItem aboutItem = new ToolStripMenuItem();
            aboutItem.Click += (s, ev) =>
            {
                MessageBox.Show(T("aboutText"), T("about"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            aboutItem.Tag = "about";
            toolStripSplitButton1.DropDownItems.Add(aboutItem);

            ToolStripMenuItem settingsItem = new ToolStripMenuItem();
            settingsItem.Click += (s, ev) => ShowSettings();
            settingsItem.Tag = "settings";
            toolStripSplitButton1.DropDownItems.Add(settingsItem);

            editLabel.DropDownItems.Clear();

            ToolStripMenuItem undoItem = new ToolStripMenuItem();
            undoItem.Click += (s, ev) => richTextBox1.Undo();
            undoItem.Tag = "undo";
            editLabel.DropDownItems.Add(undoItem);

            ToolStripMenuItem redoItem = new ToolStripMenuItem();
            redoItem.Click += (s, ev) => richTextBox1.Redo();
            redoItem.Tag = "redo";
            editLabel.DropDownItems.Add(redoItem);

            editLabel.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem cutItem = new ToolStripMenuItem();
            cutItem.Click += (s, ev) => richTextBox1.Cut();
            cutItem.Tag = "cut";
            editLabel.DropDownItems.Add(cutItem);

            ToolStripMenuItem copyItem = new ToolStripMenuItem();
            copyItem.Click += (s, ev) => richTextBox1.Copy();
            copyItem.Tag = "copy";
            editLabel.DropDownItems.Add(copyItem);

            ToolStripMenuItem pasteItem = new ToolStripMenuItem();
            pasteItem.Click += (s, ev) => richTextBox1.Paste();
            pasteItem.Tag = "paste";
            editLabel.DropDownItems.Add(pasteItem);

            editLabel.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem selectAllItem = new ToolStripMenuItem();
            selectAllItem.Click += (s, ev) => richTextBox1.SelectAll();
            selectAllItem.Tag = "selectAll";
            editLabel.DropDownItems.Add(selectAllItem);

            fileLabel.DropDownItems.Clear();

            ToolStripMenuItem newItem = new ToolStripMenuItem();
            newItem.Click += (s, ev) =>
            {
                richTextBox1.Clear();
                currentFile = "";
                UpdateTitle();
            };
            newItem.Tag = "new";
            fileLabel.DropDownItems.Add(newItem);

            ToolStripMenuItem openItem = new ToolStripMenuItem();
            openItem.Click += (s, ev) =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = T("textFiles");
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        richTextBox1.Text = File.ReadAllText(ofd.FileName);
                        currentFile = ofd.FileName;
                        UpdateTitle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "EasyPad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            openItem.Tag = "open";
            fileLabel.DropDownItems.Add(openItem);

            ToolStripMenuItem saveItem = new ToolStripMenuItem();
            saveItem.Click += (s, ev) =>
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = T("textFiles");
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(sfd.FileName, richTextBox1.Text);
                        currentFile = sfd.FileName;
                        UpdateTitle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "EasyPad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            saveItem.Tag = "save";
            fileLabel.DropDownItems.Add(saveItem);

            fileLabel.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem exitItem = new ToolStripMenuItem();
            exitItem.Click += (s, ev) => Application.Exit();
            exitItem.Tag = "exit";
            fileLabel.DropDownItems.Add(exitItem);
        }

        private void ApplyLanguage()
        {
            fileLabel.Text = T("file");
            editLabel.Text = T("edit");
            toolStripSplitButton1.Text = T("about");

            foreach (ToolStripItem item in toolStripSplitButton1.DropDownItems)
                if (item.Tag != null) item.Text = T(item.Tag.ToString());

            foreach (ToolStripItem item in editLabel.DropDownItems)
                if (item.Tag != null) item.Text = T(item.Tag.ToString());

            foreach (ToolStripItem item in fileLabel.DropDownItems)
                if (item.Tag != null) item.Text = T(item.Tag.ToString());

            UpdateTitle();
            UpdateStatus();
        }

        private void UpdateTitle()
        {
            if (string.IsNullOrEmpty(currentFile))
                this.Text = "EasyPad - " + T("newFile");
            else
                this.Text = "EasyPad - " + currentFile;
        }

        private void UpdateStatus()
        {
            int line = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart) + 1;
            int col = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexOfCurrentLine() + 1;
            int lines = richTextBox1.Lines.Length;
            int chars = richTextBox1.TextLength;
            int sel = richTextBox1.SelectionLength;

            posLabel.Text = T("pos") + ": " + line + ":" + col;
            linesLabel.Text = T("lines") + ": " + lines;
            charsLabel.Text = T("chars") + ": " + chars;

            if (sel > 0)
                selLabel.Text = T("sel") + ": " + sel;
            else
                selLabel.Text = "";
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) => UpdateStatus();
        private void richTextBox1_SelectionChanged(object sender, EventArgs e) => UpdateStatus();
        private void richTextBox1_KeyUp(object sender, KeyEventArgs e) => UpdateStatus();
        private void richTextBox1_MouseUp(object sender, MouseEventArgs e) => UpdateStatus();

        private void ShowSettings()
        {
            Form settingsForm = new Form();
            settingsForm.Text = T("settingsTitle");
            settingsForm.Size = new Size(300, 180);
            settingsForm.StartPosition = FormStartPosition.CenterParent;
            settingsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            settingsForm.MaximizeBox = false;
            settingsForm.MinimizeBox = false;

            Label langLabel = new Label();
            langLabel.Text = T("language") + ":";
            langLabel.Location = new Point(20, 25);
            langLabel.Size = new Size(100, 25);
            settingsForm.Controls.Add(langLabel);

            ComboBox langBox = new ComboBox();
            langBox.DropDownStyle = ComboBoxStyle.DropDownList;
            langBox.Location = new Point(130, 22);
            langBox.Size = new Size(130, 25);
            langBox.Items.Add("Русский");
            langBox.Items.Add("English");
            langBox.Items.Add("Deutsch");
            if (currentLang == "ru") langBox.SelectedIndex = 0;
            else if (currentLang == "en") langBox.SelectedIndex = 1;
            else if (currentLang == "de") langBox.SelectedIndex = 2;
            settingsForm.Controls.Add(langBox);

            Button okBtn = new Button();
            okBtn.Text = T("ok");
            okBtn.Location = new Point(60, 90);
            okBtn.Size = new Size(75, 30);
            okBtn.DialogResult = DialogResult.OK;
            settingsForm.Controls.Add(okBtn);

            Button cancelBtn = new Button();
            cancelBtn.Text = T("cancel");
            cancelBtn.Location = new Point(150, 90);
            cancelBtn.Size = new Size(75, 30);
            cancelBtn.DialogResult = DialogResult.Cancel;
            settingsForm.Controls.Add(cancelBtn);

            settingsForm.AcceptButton = okBtn;
            settingsForm.CancelButton = cancelBtn;

            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                if (langBox.SelectedIndex == 0) currentLang = "ru";
                else if (langBox.SelectedIndex == 1) currentLang = "en";
                else if (langBox.SelectedIndex == 2) currentLang = "de";
                ApplyLanguage();
            }
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton1.ShowDropDown();
        }

        private void editLabel_ButtonClick(object sender, EventArgs e)
        {
            editLabel.ShowDropDown();
        }

        private void fileLabel_ButtonClick(object sender, EventArgs e)
        {
            fileLabel.ShowDropDown();
        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }
    }
}