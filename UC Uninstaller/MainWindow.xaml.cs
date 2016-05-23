using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using Core;
using Language;

namespace Uninstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private RegistryKey regKeyCurrentUser;
        private RegistryKey regSubKeyCurrent;
        private string value;

        public sealed class ErrorLog
        {
            #region Properties

            private string _LogPath;
            public string LogPath
            {
                get
                {
                    return _LogPath;
                }
            }

            #endregion

            #region Constructors

            public ErrorLog()
            {
                _LogPath = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.Windows.Forms.Application.ProductName), "Errors");
                if (!Directory.Exists(_LogPath))
                    Directory.CreateDirectory(_LogPath);
            }

            public ErrorLog(string logPath)
            {
                _LogPath = logPath;
                if (!Directory.Exists(_LogPath))
                    Directory.CreateDirectory(_LogPath);
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Logs exception information to the assigned log file.
            /// </summary>
            /// <param name="exception">Exception to log.</param>
            public string LogError(Exception exception)
            {
                Assembly caller = Assembly.GetEntryAssembly();
                Process thisProcess = Process.GetCurrentProcess();

                string LogFile = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".txt";

                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(_LogPath, LogFile)))
                {
                    sw.WriteLine("==============================================================================");
                    sw.WriteLine(caller.FullName);
                    sw.WriteLine("------------------------------------------------------------------------------");
                    sw.WriteLine("Application Information");
                    sw.WriteLine("------------------------------------------------------------------------------");
                    sw.WriteLine("Time         : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    sw.WriteLine("OS           : " + Environment.OSVersion.ToString());
                    sw.WriteLine("Culture      : " + CultureInfo.CurrentCulture.Name);
                    sw.WriteLine("Framework    : " + Environment.Version);
                    sw.WriteLine("Computer ID  : " + Environment.MachineName +" - "+ Environment.UserName);
                    sw.WriteLine("------------------------------------------------------------------------------");
                    sw.WriteLine("Exception Information");
                    sw.WriteLine("------------------------------------------------------------------------------");
                    sw.WriteLine("Source       : " + exception.Source.ToString().Trim());
                    sw.WriteLine("Method       : " + exception.TargetSite.Name.ToString());
                    sw.WriteLine("Type         : " + exception.GetType().ToString());
                    sw.WriteLine("Error        : " + GetExceptionStack(exception));
                    String innerMessage = (exception.InnerException != null)
                      ? exception.InnerException.ToString().Trim()
                      : "none";
                    sw.WriteLine("Inner Ex.    : " + innerMessage);
                    sw.WriteLine("");
                    sw.WriteLine("==============================================================================");
                    sw.WriteLine("DETAILED INFO");
                    sw.WriteLine("==============================================================================");
                    sw.WriteLine("Installed software");
                    sw.WriteLine("------------------------------------------------------------------------------");
                    try
                    {
                        foreach (core.program program in core.search(true))
                        {
                            sw.WriteLine(program.name);
                        }
                    }
                    catch { sw.WriteLine("ERROR!"); }
                    sw.WriteLine("------------------------------------------------------------------------------");
                    sw.WriteLine("Stack Trace  : " + exception.StackTrace.ToString().Trim());
                    sw.WriteLine("------------------------------------------------------------------------------");
                    sw.WriteLine("Loaded Modules");
                    sw.WriteLine("------------------------------------------------------------------------------");
                    foreach (ProcessModule module in thisProcess.Modules)
                    {
                        try
                        {
                            sw.WriteLine(module.FileName + " | " + module.FileVersionInfo.FileVersion + " | " + module.ModuleMemorySize);
                        }
                        catch (FileNotFoundException)
                        {
                            sw.WriteLine("File Not Found: " + module.ToString());
                        }
                        catch (Exception)
                        {

                        }
                    }
                    sw.WriteLine("------------------------------------------------------------------------------");
                    sw.WriteLine(LogFile);
                    sw.WriteLine("==============================================================================");
                }

                return LogFile;
            }

            #endregion

            #region Private Methods

            private string GetExceptionStack(Exception e)
            {
                StringBuilder message = new StringBuilder();
                message.Append(e.Message);
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    message.Append(Environment.NewLine);
                    message.Append(e.Message);
                }

                return message.ToString();
            }

            #endregion
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                string LogFile = Logger.LogError(ex);

                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crashreport.exe");
                proc.StartInfo.Arguments = LogFile;
                proc.Start();
            }
            finally
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                string LogFile = Logger.LogError(e.Exception);

                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crashreport.exe");
                proc.StartInfo.Arguments = LogFile;
                proc.Start();
            }
            finally
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
        public static ErrorLog Logger;
        
        public MainWindow()
        {
            core.lastcheck.Tick+=new EventHandler(core.timerreset);
            languages.setlanguage(null);
            Application.Current.Resources["close"] = languages.controls.close;
            Application.Current.Resources["maximize"] = languages.controls.maximize;
            Application.Current.Resources["minimize"] = languages.controls.minimize;
            Application.Current.Resources["restore"] = languages.controls.restore; 

            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, this.OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, this.OnMaximizeWindow, this.OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, this.OnMinimizeWindow, this.OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, this.OnRestoreWindow, this.OnCanResizeWindow));


            // Initialize the error log
            Logger = new ErrorLog();

            // Handle unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            System.Windows.Forms.Application.ThreadException +=
                new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            regKeyCurrentUser = Registry.CurrentUser;
            regSubKeyCurrent = regKeyCurrentUser.OpenSubKey(@"AppEvents\Schemes\Apps\Explorer\Navigating\.Current", true);
            value = regSubKeyCurrent.GetValue("").ToString();
            regSubKeyCurrent.SetValue("", "");

            if (Properties.Settings.Default.autostart==false)
            {
                Properties.Settings.Default.advanced = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.advanced = true;
                Properties.Settings.Default.Save();
            }
            core.dbconnect();
            InitializeComponent();
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            this.WindowState = Properties.Settings.Default.WindowState;
         }


        private void NavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (core.no_shutdown) { core.shutdown = true; e.Cancel = true; }
            else
            {
                var regSubKeyDefault = regKeyCurrentUser.OpenSubKey(@"AppEvents\Schemes\Apps\Explorer\Navigating\.Default");
                regSubKeyCurrent.SetValue("", value);
            }

            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Height = this.Height;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.WindowState = this.WindowState;
            Properties.Settings.Default.Save();
        }

        async void startup_sequence()
        {
            #region Aktualizační a inicializační sekvence
            if (System.IO.Directory.Exists(System.Windows.Forms.Application.StartupPath + @"\temp")) { System.IO.Directory.Delete(System.Windows.Forms.Application.StartupPath + @"\temp", true); }

            if (Properties.Settings.Default.update)
            {
                await core.updatecheck(true);
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new System.Threading.ThreadStart(delegate
            {
                if (core.updatedata.server_status == "up" & core.updatedata.update == true)
                {
                    if (core.updatedata.updatetype == "program") { Application.Current.MainWindow.Hide(); Window updatedialog = new updater(); updatedialog.ShowDialog(); Application.Current.MainWindow.Show(); }
                    else if (core.updatedata.updatetype == "database") { core.databaseupdate(); }
                    else if (core.updatedata.updatetype == "both") { core.databaseupdate(); Window updatedialog = new updater(); updatedialog.Owner = Application.Current.MainWindow; updatedialog.ShowDialog(); Application.Current.MainWindow.Show(); }
                }
            }));
            }
                   
            #endregion
        }


        private void navwindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + @"\uninstaller.udb"))
            {
                startup_sequence();
            }
        }

        private void navwindow_KeyDown(object sender, KeyEventArgs e)
        {
            core.fire(e);
        }

        private void navwindow_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Refresh)
                e.Cancel = true;
        }

    }


}
