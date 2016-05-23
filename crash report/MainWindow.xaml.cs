using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using Language;

namespace crash_report
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        private static string _PostURI = "http://uninstaller.pryx.net/crash.php";
        private string _ErrorFile;

        public MainWindow()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, this.OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, this.OnMaximizeWindow, this.OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, this.OnMinimizeWindow, this.OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, this.OnRestoreWindow, this.OnCanResizeWindow));
            InitializeComponent();
            languages.setlanguage(null);

            _ErrorFile = Environment.GetCommandLineArgs()[1].ToString();
            using (StreamReader reader = File.OpenText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UC Uninstaller", "Errors", _ErrorFile)))
            {
                errortext.Text = reader.ReadToEnd();
            }
            Cancel.Content = languages.controls.cancel;
            Send.Content = languages.controls.send;
            describe.Content = languages.controls.describe;
            crashed.Content = languages.controls.crashed;
            email_label.Content = languages.controls.email;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpPost(_PostURI);

            System.Windows.Forms.MessageBox.Show(languages.messages.crash_sent.text, languages.messages.crash_sent.title, MessageBoxButtons.OK, MessageBoxIcon.Information);
           this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void HttpPost(string uri)
        {
            WebClient webClient = new WebClient();
            string json = new JavaScriptSerializer().Serialize(new
            {
                usertext = usertext.Text,
                log = errortext.Text,
                mail= email.Text
            });
            NameValueCollection post = new NameValueCollection();
            post["json"] = json;
            webClient.UploadValues(uri, "POST", post);
            webClient.Dispose();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.IO.File.Delete(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UC Uninstaller", "Errors", _ErrorFile));
        }
    }
}

