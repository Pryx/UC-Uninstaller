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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Language;

namespace Uninstaller
{
    /// <summary>
    /// Interaction logic for settings.xaml
    /// </summary>
    public partial class settings : Page
    {
        private bool initialized = false;
        public settings()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new main());
        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 1;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            this.BeginAnimation(OpacityProperty, da);

            rremember.IsChecked = Properties.Settings.Default.dremember;
            if (Properties.Settings.Default.delete == true) { rclean.IsChecked = true; }
            else { dontrclean.IsChecked = true; }
            remember.IsChecked = Properties.Settings.Default.remember;
            if (Properties.Settings.Default.close == true) { bclose.IsChecked = true; }
            else { bdontclose.IsChecked = true; }
            update.IsChecked = Properties.Settings.Default.update;
            autostart.IsChecked = Properties.Settings.Default.autostart;
            update.Content = languages.controls.autoupdate;
            lang.Content = languages.controls.lang;
            autostart.Content = languages.controls.autoadvmode;
            langselect.SelectedValue = languages.getlanguage();
            disable_silent.Content = languages.controls.disable_silent;
            disable_silent.IsChecked = Properties.Settings.Default.disable_silent;
            adv.Content = languages.controls.adv_settings;
            basic.Content = languages.controls.basic_settings;
            remember.Content = languages.controls.remember;
            bclose.Content = languages.controls.autoclose;
            bdontclose.Content = languages.controls.noautoclose;
            bautoclose.Content = languages.controls.bautoclose;
            rremember.Content = languages.controls.remember;
            registryclean.Content = languages.controls.rclean;
            dontrclean.Content = languages.controls.noautoclose;
            rclean.Content = languages.controls.autoclose;
            initialized = true;
        }


        private void settings_updated(object sender, RoutedEventArgs e)
        {
          if (initialized)
            {
            Properties.Settings.Default.update = update.IsChecked.Value;
            Properties.Settings.Default.autostart = autostart.IsChecked.Value;
            Properties.Settings.Default.disable_silent = disable_silent.IsChecked.Value;
            Properties.Settings.Default.remember = remember.IsChecked.Value;
            if (remember.IsChecked.Value==true)
            {
                if (bclose.IsChecked == true) { Properties.Settings.Default.close = true; }
                else { Properties.Settings.Default.close = false; }
            }

            Properties.Settings.Default.dremember = rremember.IsChecked.Value;
            if (rremember.IsChecked.Value == true)
            {
                if (rclean.IsChecked == true) { Properties.Settings.Default.delete = true; }
                else { Properties.Settings.Default.delete = false; }
            }

            Properties.Settings.Default.Save();
            }
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            if (initialized)
            {
                languages.setlanguage("cs");
                this.NavigationService.Navigate(new settings());
            }
        }

        private void ComboBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {
            if (initialized)
            {
                languages.setlanguage("en");
                this.NavigationService.Navigate(new settings());
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bclose.IsEnabled = true;
            bdontclose.IsEnabled = true;
            settings_updated(null,null);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            bclose.IsEnabled = false;
            bdontclose.IsEnabled = false;
            settings_updated(null, null);
        }

        private void CheckBox_Checked1(object sender, RoutedEventArgs e)
        {
            rclean.IsEnabled = true;
            dontrclean.IsEnabled = true;
            settings_updated(null, null);
        }

        private void CheckBox_Unchecked1(object sender, RoutedEventArgs e)
        {
            rclean.IsEnabled = false;
            dontrclean.IsEnabled = false;
            settings_updated(null, null);
        }

    }
}
