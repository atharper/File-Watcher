using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_Watcher
{
  public partial class Form1 : Form
  {
    private FileSystemWatcher _watcher;
    private string _lastRanVersion;

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      _watcher = new FileSystemWatcher
      {
        Path = Path.GetDirectoryName(fileTextBox.Text),
        Filter = Path.GetFileName(fileTextBox.Text),
        NotifyFilter = NotifyFilters.LastWrite,
        EnableRaisingEvents = true
      };

      _watcher.Changed += Changed;
    }

    private bool _running;
    private void Changed(object sender, FileSystemEventArgs e)
    {
      if (_running) return;
      var version = VersionNumber();
      if (_lastRanVersion == version) return;
      _lastRanVersion = version;

      _running = true;
      logTextBox.Invoke(new Action(() => logTextBox.AppendText(LogText())));
      if (runTextBox.Text.Length > 0 && File.Exists(runTextBox.Text))
      {
        var p = Process.Start(runTextBox.Text);
        p.WaitForExit();
      }

      _running = false;
    }

    private string LogText()
    {
      return string.Format("File Changed at {0} Version Number: {2} {1}", DateTime.Now, Environment.NewLine,_lastRanVersion);
    }

    private static string VersionNumber()
    {
      const string copybotDir = @"\\CASELLE3\Apps3\CopyBot2";
      var newestDirname = File.ReadAllLines(Path.Combine(copybotDir, "Development_roadsign.txt"))[0];
      var newestDir = Path.Combine(copybotDir, newestDirname);
      var fileName = Path.Combine(newestDir, "Caselle.exe");
      if (!File.Exists(fileName)) return string.Empty;
      return FileVersionInfo.GetVersionInfo(fileName).ProductVersion;
    }

    private void fileTextBox_TextChanged(object sender, EventArgs e)
    {
      _watcher.Path = Path.GetDirectoryName(fileTextBox.Text);
      _watcher.Filter = Path.GetFileName(fileTextBox.Text);
    }
  }
}
