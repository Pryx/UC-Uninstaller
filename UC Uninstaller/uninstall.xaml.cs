using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using Language;

namespace Uninstaller
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class uninstall : Window
    {
        public uninstall()
        {
            InitializeComponent();
            this.Title = languages.controls.uninstalling;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.AppStarting;
            run();
        }


        private void run()
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, e) =>
            {
                int progress = 0;
                int i = 0;

                foreach (Core.core.program program in Core.core.uninstalllist)
                {
                    if (Properties.Settings.Default.close)
                    {
                        string[] processes = new string[] { "iexplore", "firefox", "chrome", "opera", "safari" };
                        foreach (string browser in processes)
                        {
                            try
                            {
                                foreach (Process proc in Process.GetProcessesByName(browser))
                                {
                                    proc.Kill();
                                }
                            }
                            catch { }
                        }
                    }


                    i++;
                    worker.ReportProgress(progress + 1, program.name);

                    string[] splitter = { " \"", " /", " -" };
                    string oldstring = program.uninstallstring.ToString();
                    string[] array = program.uninstallstring.Split(splitter, 2, StringSplitOptions.None);
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    if (array.Count() != 1)
                    {
                        //Pokud msiexec, udělej vlastní uninstall parametr, jinak dostaň zpět / - apod.
                        if (array[0].ToLower() == "msiexec.exe")
                        {
                            array[1] = "/x " + program.regkey + " /qb";
                        }
                        else
                        {
                            array[1] = oldstring.Replace(array[0], "");
                        }
                    }
                    else
                    {
                        if (array.Count() == 0) { Array.Resize(ref array, 2); array[0] = program.uninstallstring; }
                        Array.Resize(ref array, 2);
                        array[1] = "-removeonly /S /SILENT /Q /QUIET /q /s";
                    }

                    if (!Properties.Settings.Default.disable_silent)
                    {
                        startInfo.Arguments = array[1];
                        startInfo.FileName = array[0].Replace("\"", "");
                        Process process = new Process();
                        process.StartInfo = startInfo;
                        process.Start();
                        process.WaitForExit();
                    }
                    else
                    {
                        Process process = new Process();
                        startInfo.FileName = array[0].Replace("\"", "");
                        process.StartInfo = startInfo;
                        process.Start();
                        process.WaitForExit();
                    }
                    progress = (int)(Decimal.Divide(i, Core.core.uninstalllist.Count()) * 100m);
                    worker.ReportProgress(progress, program.name);
                }
                Thread.Sleep(1000);
            };
            worker.ProgressChanged += (sender, e) =>
            {
                progress.Value = e.ProgressPercentage;
                progname.Content = e.UserState as String;
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                this.Cursor = Cursors.Arrow;
                this.Close();
            };
            worker.RunWorkerAsync();
        }
    }
}
