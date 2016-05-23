using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core;
using Language;

namespace Uninstaller
{
    /// <summary>
    /// Interaction logic for about.xaml
    /// </summary>
    public partial class about : Page
    {
        public about()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new main());
        }

        private void hyperlink_navigate(object sender, RequestNavigateEventArgs e)
        {
            var url = (Hyperlink)sender;
            Core.core.openwebpage( url.NavigateUri.ToString());
            e.Handled=true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 1;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            this.BeginAnimation(OpacityProperty, da);

            ver.Content = languages.controls.version;
            databasever.Content = languages.controls.database;
            applicationver.Content = languages.controls.application;
            programmer.Content = languages.controls.programmer;
            testing.Content = languages.controls.testing;
            musal.ToolTip = languages.controls.musal;
            kantova.ToolTip = languages.controls.kantova;
            planansky.ToolTip = languages.controls.planansky;
            updatecheck.Content = languages.controls.updatecheck;
            thanks.Content = languages.controls.thanks;
            try { dbver.Content = Core.core.database_ver[Core.core.database_ver.Length - 3] + "." + Core.core.database_ver[Core.core.database_ver.Length - 2] + "." + Core.core.database_ver[Core.core.database_ver.Length-1]; }
            catch { Core.core.dbconnect(); dbver.Content = Core.core.database_ver[Core.core.database_ver.Length - 3] + "." + Core.core.database_ver[Core.core.database_ver.Length - 2] + "." + Core.core.database_ver[Core.core.database_ver.Length - 1]; core.sqlite.Close(); }
            appver.Content = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(3);
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Core.core.openwebpage("http://www.gnu.org/licenses/gpl-3.0-standalone.html");
        }

        private async void updatecheck_Click(object sender, RoutedEventArgs e)
        {
            updatecheck.IsEnabled = false;
                await core.updatecheck(true);
                if (core.updatedata.server_status == "up" & core.updatedata.update == true)
                {
                    if (core.updatedata.updatetype == "program") { Application.Current.MainWindow.Hide(); Window updatedialog = new updater(); updatedialog.ShowDialog(); Application.Current.MainWindow.Show(); }
                    else if (core.updatedata.updatetype == "database") { core.databaseupdate(); MessageBox.Show(languages.messages.dbupdated.text, languages.messages.dbupdated.title, MessageBoxButton.OK, MessageBoxImage.Information); core.dbconnect(); core.sqlite.Close(); dbver.Content = Core.core.database_ver[Core.core.database_ver.Length - 3] + "." + Core.core.database_ver[Core.core.database_ver.Length - 2] + "." + Core.core.database_ver[Core.core.database_ver.Length - 1]; }
                    else if (core.updatedata.updatetype == "both") { core.databaseupdate(); Application.Current.MainWindow.Hide(); Window updatedialog = new updater(); updatedialog.Owner = Application.Current.MainWindow; updatedialog.ShowDialog(); Application.Current.MainWindow.Show(); }
                }
                else if (core.updatedata.update == false)
                {
                    MessageBox.Show(languages.messages.no_update.text, languages.messages.no_update.title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else { MessageBox.Show(languages.messages.server_down.text, languages.messages.server_down.title, MessageBoxButton.OK, MessageBoxImage.Warning); }
                updatecheck.IsEnabled = true;
            }
    }
}
