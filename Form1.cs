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
using System.Timers;
using Timer = System.Timers.Timer;

namespace WinFormsApp3
{
    /* To do
     * 1. Save last source and detination folders.
     * 2. Use the last saved folders when frorm loads.
     * 3. Future: delete files except last X days - have option to choose number of files to leave
     * 4. Show countdown timer
     */
    public partial class File_Copier : Form
    {
        private FileInfo[] _tempList = new FileInfo[500];
        private List<string> _tempListFileNames = new List<string>();
        private List<string> _tempListSorted = new List<string>();
        private FileInfo[] _compareList = new FileInfo[500];
        private List<string> _compareListSorted = new List<string>();
        private string fileName;
        private double _time;
        private static System.Timers.Timer aTimer;
        // Initialising...
        public File_Copier()
        {
            InitializeComponent();
            FileKeyword.Focus();
            SourcePath.ReadOnly = true;
            TargetPath.ReadOnly = true;
            ResetForm();
        }
        // Source browse button
        private void button1_Click(object sender, EventArgs e)    
        {
            SourcePath.Text = "";
            FolderBrowserDialog _folderBrowser = new FolderBrowserDialog();
            _folderBrowser.Description = "Source Path";
            if (_folderBrowser.ShowDialog() == DialogResult.OK)
            {
                SourcePath.Text = _folderBrowser.SelectedPath.ToString();
            }
        }
        // Target browse button
        private void button2_Click(object sender, EventArgs e)    
        {
            TargetPath.Text = "";
            FolderBrowserDialog _folderBrowser = new FolderBrowserDialog();
            _folderBrowser.Description = "Target Path";
            if (_folderBrowser.ShowDialog() == DialogResult.OK)
            {
                TargetPath.Text = _folderBrowser.SelectedPath.ToString();
            }
        }
        //  Copy File button
        private void button3_Click(Object source, EventArgs e)  
        {
            if (SourcePath.Text == "" || SourcePath.Text == "Choose Source Path" || TargetPath.Text == "" || TargetPath.Text == "Choose Source Path")
            {
                MessageBox.Show("Missing Info: Path(s)", "Info Needed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (FileKeyword.Text == "" || FileKeyword.Text == "Enter a keyword of the file")
            {
                MessageBox.Show("Missing Info: File Keyword", "Info Needed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else 
            {
                try
                {
                    _time = double.Parse(TimeBox.Text);
                    _time *= 1000 * 360 * 24;
                    RunTimer(_time);
                    CopyingInProgress();
                }
                catch
                {
                    MessageBox.Show("Wrong or missing Info: Time Interval", "Info Needed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    TimeBox.Text = "";
                    TimeBox.Focus();
                }
            }
        }
        // Timer 
        private void RunTimer(double _t)
        {
            aTimer = new System.Timers.Timer(_t);
            aTimer.Elapsed += (sender, e) => CopyFilesToFolder(sender, e, FileKeyword.Text, SourcePath.Text, TargetPath.Text);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Start();
            NextLabel.Text = DateTime.Now.AddDays(double.Parse(TimeBox.Text)).ToString();
        }
        private void CopyFilesToFolder(Object sender, ElapsedEventArgs e, string _fk, string _sp, string _tp) 
        {
            _tempListSorted.Clear();
            _tempListFileNames.Clear();

            DirectoryInfo sourceLines = new DirectoryInfo(_sp);
            _tempList = sourceLines.GetFiles();

            if (_tempList.Length > 0)
            {
                for (int i = 0; i < _tempList.Length; i++)
                {
                    string s = _tempList[i].Name.ToString().ToLower();
                    for (int j = 0; j < (s.Length - FileKeyword.Text.Length); j++)
                    {
                        string y = s.Substring(j, FileKeyword.Text.Length);
                        if (y == FileKeyword.Text.ToLower())
                        {
                            _tempListSorted.Add(_tempList[i].ToString());
                            _tempListFileNames.Add(_tempList[i].Name.ToString());
                        }
                    }
                }
            }
            for (int i = 0; i < _tempListSorted.Count; i++)
            {
                fileName = _tempListFileNames[i].ToString();
                string sourcePath = SourcePath.Text.Trim();
                string targetPath = TargetPath.Text.Trim();

                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileName);     
                try
                {
                    if (System.IO.File.Exists(sourceFile))
                    {
                        System.IO.File.Copy(sourceFile, destFile, true);

                    }

                }
                catch (Exception e1)
                {
                    MessageBox.Show($"Error : {e1}");
                }
            }
            //NextLabel.Text = DateTime.Now.AddDays(double.Parse(TimeBox.Text)).ToString();
        }
        private void StopButton_Click(object sender, EventArgs e)
        {
            aTimer.Stop();
            ResetForm();
        }
        private void CopyingInProgress()
        {
            StopButton.Enabled = true;
            button3.Enabled = false;
            FileKeyword.Enabled = false;
            TimeBox.Enabled = false;
            StopButton.Focus();
            button1.Enabled = false;
            button2.Enabled = false;
            FilesToLeaveBox.Enabled = false;
        }
        private void ResetForm()
        {
            FileKeyword.Enabled = true;
            button3.Enabled = true;
            StopButton.Enabled = false;
            TimeBox.Enabled = true;
            FileKeyword.Focus();
            button1.Enabled = true;
            button2.Enabled = true;
            FilesToLeaveBox.Enabled = true;
            NextLabel.Text = "Date/Time";
        }

    }
}
