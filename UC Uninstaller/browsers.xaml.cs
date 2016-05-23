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
    public partial class browsers : Window
    {
        public browsers()
        {
            InitializeComponent();
            yes.Content=languages.controls.yes;
            no.Content = languages.controls.no;
            this.Title = languages.messages.browsers.title;
            closebrowsers.Text = languages.messages.browsers.text;
            remember.Content = languages.controls.remember;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.close = true;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.close = false;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void remember_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.remember = true;
            Properties.Settings.Default.Save();
        }

        private void remember_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.remember = false;
            Properties.Settings.Default.Save();
        }
    }
}
