using System;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using Core;
using Language;


namespace Uninstaller
{
    /// <summary>
    /// Interaction logic for updater.xaml
    /// </summary>
    public partial class updater : Window
    {
        public updater()
        {
            InitializeComponent();
        }

        public BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        public bool completed = false;
        private void button1_Click(object sender, EventArgs e)
        {

            backgroundWorker1.CancelAsync();
            backgroundWorker1.Dispose();
            this.Close();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {


            if (Directory.Exists(System.Windows.Forms.Application.StartupPath + @"\Temp")) { Directory.Delete(System.Windows.Forms.Application.StartupPath + @"\Temp", true); }
            Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + @"\Temp");

            Uri url = new Uri(core.updatedata.download + "update.zip");

            const int BUFFER_SIZE = 16 * 1024;
            using (var outputFileStream = File.Create(System.Windows.Forms.Application.StartupPath + @"\Temp\update.zip", BUFFER_SIZE))
            {
                float Total = 0;
                float bytes = 0;
                var req = WebRequest.Create(url);
                using (var response = req.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        Total = response.ContentLength;
                        var buffer = new byte[BUFFER_SIZE];
                        int bytesRead;
                        do
                        {
                            bytesRead = responseStream.Read(buffer, 0, BUFFER_SIZE);
                            bytes += bytesRead;
                            outputFileStream.Write(buffer, 0, bytesRead);
                            backgroundWorker1.ReportProgress((int)((bytes / Total) * 100));
                            if ((bytes / Total) == 1) { completed = true; }
                        } while (bytesRead > 0 & !backgroundWorker1.CancellationPending);
                    }
                }
            }
        }


        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (completed)
            {
                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(System.Windows.Forms.Application.StartupPath + @"\Temp\update.zip"))
                    {
                        archive.ExtractToDirectory(System.Windows.Forms.Application.StartupPath + @"\Temp");
                    }

                    File.Delete(System.Windows.Forms.Application.StartupPath + @"\temp\update.zip");

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo.WorkingDirectory = System.Windows.Forms.Application.StartupPath;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C taskkill /f /im \"UC Uninstaller.exe\" &&  xcopy temp /Y /s && PING -n 5 127.0.0.1>nul && \"" + System.Windows.Forms.Application.ExecutablePath + "\" && pause";
                    process.StartInfo = startInfo;
                    process.Start();

                    System.Windows.Forms.Application.ExitThread();
                }
                catch
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show(languages.messages.update_apply_error.text, languages.messages.update_apply_error.title, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (result == System.Windows.Forms.DialogResult.Retry) { progressBar1.Value = 0; backgroundWorker1.RunWorkerAsync(); }
                    else { Environment.Exit(0); }
                }
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
