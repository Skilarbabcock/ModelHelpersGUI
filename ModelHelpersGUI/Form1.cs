﻿using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ModelHelpersGUI
{
    public partial class Form1 : Form
    {
        // Global Session Preferences
        Settings sessionSettings = new Settings();
        public Form1()
        {
            InitializeComponent();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sessionSettings.ShowDialog();
        }

        private void unpackSourceButton_Click(object sender, EventArgs e)
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = Properties.Settings.Default["unpackDir"].ToString();
                proc.StartInfo.Arguments = $"\"{unpackSourceTextBox.Text}\"";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();

                fauxConsoleTextBox.Text = $"Executing: {Path.GetFileName(proc.StartInfo.FileName)} {proc.StartInfo.Arguments}...\r\n\r\n";
                StreamReader read = proc.StandardOutput;
                fauxConsoleTextBox.AppendText($"From {Path.GetFileName(proc.StartInfo.FileName)}:\r\n");
                fauxConsoleTextBox.AppendText(read.ReadToEnd());

                proc.WaitForExit();
            }
        }

        private void unpackSourceBrowseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.InitialDirectory = Environment.CurrentDirectory;
                dlg.Filter = "Archives (*.arc)|*.arc|All Files (*.*)|*.*";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    unpackSourceTextBox.Text = Path.GetFullPath(dlg.FileName);
                    Properties.Settings.Default["unpackDir"] = unpackSourceTextBox.Text;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load Saved Settings
            unpackSourceTextBox.Text = Properties.Settings.Default["unpackDir"].ToString();
            cleanFolderTextBox.Text = Properties.Settings.Default["packCleanDir"].ToString();
            customFolderTextBox.Text = Properties.Settings.Default["packCustomDir"].ToString();
            rarcnameTextBox.Text = Properties.Settings.Default["packRarcName"].ToString();

            fauxConsoleTextBox.Size = new Size(649, 320);
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Console
            if (consoleToolStripMenuItem.Checked == true)
            {
                this.Size = new Size(990, 416);
                fauxConsoleTextBox.Visible = true;
            }
            else
            {
                this.Size = new Size(336, 416);
                fauxConsoleTextBox.Visible = false;
            }

            // Controls
            if (unpackSourceTextBox.Text == "")
                unpackSourceButton.Enabled = false;
            else
                unpackSourceButton.Enabled = true;

            if (cleanFolderTextBox.Text != "" && customFolderTextBox.Text != "" && rarcnameTextBox.Text != "")
            {
                packButton.Enabled = true;
                moveButton.Enabled = true;
            }
            else
            {
                packButton.Enabled = false;
                moveButton.Enabled = false;
            }
        }

        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            consoleToolStripMenuItem.Checked = !consoleToolStripMenuItem.Checked;
        }

        private void cleanFolderBrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    cleanFolderTextBox.Text = Path.GetFullPath(dlg.SelectedPath);
                    Properties.Settings.Default["packCleanDir"] = cleanFolderTextBox.Text;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void customFolderBrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    customFolderTextBox.Text = Path.GetFullPath(dlg.SelectedPath);
                    Properties.Settings.Default["packCustomDir"] = customFolderTextBox.Text;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void rarcnameTextBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["packRarcName"] = rarcnameTextBox.Text;
            Properties.Settings.Default.Save();
        }

        private void packButton_Click(object sender, EventArgs e)
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = Properties.Settings.Default["packDir"].ToString();
                proc.StartInfo.Arguments = $"-clean \"{cleanFolderTextBox.Text}\" -custom \"{customFolderTextBox.Text}\" -rarcname \"{rarcnameTextBox.Text}.arc\"";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();

                fauxConsoleTextBox.Text = $"Executing: {Path.GetFileName(proc.StartInfo.FileName)} {proc.StartInfo.Arguments}...\r\n\r\n";
                StreamReader read = proc.StandardOutput;
                fauxConsoleTextBox.AppendText($"From {Path.GetFileName(proc.StartInfo.FileName)}:\r\n");
                fauxConsoleTextBox.AppendText(read.ReadToEnd());

                proc.WaitForExit();
            }
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            // This is the directory plus Filename of the output file.
            string finalRarc = $"{customFolderTextBox.Text}\\{rarcnameTextBox.Text}.arc";

            // This is the directory plus Filename of the output file after being moved to the Dolphin Folder.
            string outDir = $"{Properties.Settings.Default["dolphinDir"].ToString()}\\{rarcnameTextBox.Text}.arc";

            // If the output file already exists, delete it.
            if (File.Exists(outDir))
            {
                File.Delete(outDir);
            }

            // Move the final file to the output directory. This doesn't copy it, so the old one should not remain.
            if (File.Exists(finalRarc))
            {
                File.Move(finalRarc, outDir);
                // This should prompt you, telling you that the move was successful.
                MessageBox.Show($"{rarcnameTextBox.Text}.arc copied to {Properties.Settings.Default["dolphinDir"].ToString()}");
            }
            else
            {
                MessageBox.Show($"{rarcnameTextBox.Text}.arc has either been deleted or already moved and no longer exists. Did you mean to reback instead?");
            }
        }

        private void fauxConsoleTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void unpackGroupBox_Enter(object sender, EventArgs e)
        {

        }
    }
}
