using Core;
using Language;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



namespace Uninstaller
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class invalid : Window
    {
        public invalid()
        {
            InitializeComponent();
            yes.Content=languages.controls.yes;
            no.Content = languages.controls.no;
            this.Title = languages.messages.delete.title;
            closebrowsers.Text = languages.messages.delete.text;
            remember.Content = languages.controls.remember;
            todelete = core.uninstalllist.Where(x => x.invalid == true).ToList();
            render();
        }

        public void render()
        {
            scrollbar.ScrollToTop();
                foreach (core.program program in todelete)
                {
                    RowDefinition r = new RowDefinition();
                    r.Height = new GridLength(30, GridUnitType.Pixel);
                    deletelist.RowDefinitions.Add(r);
                }

                double x = 0.1;
                int i = -1;
                foreach (core.program program in todelete)
                {
                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate
                    {
                        i++;
                        Grid gr = new Grid();
                        ColumnDefinition def1 = new ColumnDefinition();
                        ColumnDefinition def2 = new ColumnDefinition();
                        ColumnDefinition def3 = new ColumnDefinition();
                        ColumnDefinition def4 = new ColumnDefinition();
                        def1.Width = new GridLength(40);
                        def3.Width = new GridLength(30);
                        def4.Width = new GridLength(20);
                        gr.ColumnDefinitions.Add(def1);
                        gr.ColumnDefinitions.Add(def2);
                        gr.ColumnDefinitions.Add(def3);
                        gr.ColumnDefinitions.Add(def4);
                        Grid.SetRow(gr, i);

                        CheckBox uninst = new CheckBox();
                        uninst.IsChecked = true;

                        SolidColorBrush solidcolorb = new SolidColorBrush();
                        solidcolorb.Color = Colors.White;
                        ColorAnimation animationa = new ColorAnimation();
                        animationa.To = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E3E3E3");
                        animationa.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                        solidcolorb.BeginAnimation(SolidColorBrush.ColorProperty, animationa);
                        gr.Background = solidcolorb;
                        
                        uninst.Name = "name" + program.id;
                        Grid.SetColumn(uninst, 3);
                        Grid.SetRow(uninst, 0);
                        uninst.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        uninst.Checked += (sender, e) =>
                        {
                            todelete.Find(a => a.id == Convert.ToInt32(uninst.Name.Replace("name", ""))).invalid = true;
                        };
                        uninst.Unchecked += (sender, e) =>
                        {
                            todelete.Find(a => a.id == Convert.ToInt32(uninst.Name.Replace("name", ""))).invalid = false;
                        };
                        uninst.VerticalAlignment = VerticalAlignment.Center;
                        gr.Children.Add(uninst);
                        Label progname = new Label();
                        Style style = Application.Current.FindResource("dynamictext") as Style;
                        progname.Style = style;
                        progname.Content = program.name;
                        progname.VerticalAlignment = VerticalAlignment.Center;
                        progname.MouseDown += (sender, e) => { uninst.IsChecked = !uninst.IsChecked; };
                        progname.Foreground = new SolidColorBrush(Colors.Black);
                        progname.FontWeight = FontWeights.Bold;
                        Grid.SetColumn(progname, 1);
                        Grid.SetRow(progname, 0);
                        gr.Children.Add(progname);
                        if (program.path != "" & program.path != null)
                        {
                            Button button = new Button();
                            button.Height = 20;
                            button.Width = 20;
                            button.Content = new System.Windows.Controls.Image
                            {
                                Source = new BitmapImage(new Uri("images/folder.png", UriKind.Relative)),
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            style = Application.Current.FindResource("open") as Style;
                            button.Style = style;
                            button.VerticalAlignment = VerticalAlignment.Center;
                            Grid.SetColumn(button, 2);
                            Grid.SetRow(button, 0);
                            button.Name = "button" + program.id;
                            button.ToolTip = languages.controls.openfolder;
                            button.Click += (sender, e) => { System.Diagnostics.Process.Start("explorer.exe", program.path); };
                            gr.Children.Add(button);
                        }
                        System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                        if (program.icon != null && program.icon != "")
                        {
                            string[] array = program.icon.Replace("\"", "").Split(new char[] { ',' }, 2);
                            if (System.IO.File.Exists(array[0]))
                            {
                                Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(array[0]);
                                Bitmap bmp = ico.ToBitmap();
                                img.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(100, 100));
                            }
                            else { img.Source = new BitmapImage(new Uri("images/noicon.png", UriKind.Relative)); }
                        }
                        else { img.Source = new BitmapImage(new Uri("images/noicon.png", UriKind.Relative)); }
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.Height = 20;
                        img.MouseDown += (sender, e) => { uninst.IsChecked = !uninst.IsChecked; };
                        Grid.SetColumn(img, 0);
                        Grid.SetRow(img, 0);
                        gr.Children.Add(img);

                        gr.MouseEnter += (sender, e) =>
                        {
                            ColorAnimation animation = new ColorAnimation();
                            SolidColorBrush solidcolor = new SolidColorBrush();
                            if ((bool)!uninst.IsChecked)
                            {
                                solidcolor.Color = Colors.White;
                                animation.To = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#C9E9F1");
                            }
                            else
                            {
                                solidcolor.Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E3E3E3");
                                animation.To = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#C9E9F1");
                            }
                            animation.Duration = new Duration(TimeSpan.FromMilliseconds(100));
                            solidcolor.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                            gr.Background = solidcolor;
                        };
                        gr.MouseLeave += (sender, e) =>
                        {
                            ColorAnimation animation = new ColorAnimation();
                            SolidColorBrush solidcolor = new SolidColorBrush();
                            if ((bool)!uninst.IsChecked)
                            {
                                solidcolor.Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E3E3E3");
                                animation.To = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFFFFF");
                            }
                            else
                            {
                                solidcolor.Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CADBDB");
                                animation.To = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E3E3E3");
                            }
                            animation.Duration = new Duration(TimeSpan.FromMilliseconds(100));
                            solidcolor.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                            gr.Background = solidcolor;
                        };

                        deletelist.Children.Add(gr);
                        if (x < 0.6) { x += 0.012; }
                        DoubleAnimation da = new DoubleAnimation();
                        da.From = 0;
                        da.To = 1;
                        da.Duration = new Duration(TimeSpan.FromSeconds(x));
                        gr.BeginAnimation(OpacityProperty, da);
                        
                    }));
                }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.dremember)
            {
                Properties.Settings.Default.delete = true;
                Properties.Settings.Default.Save();
            }

            delete();
            this.DialogResult = true;
            this.Close();
        }

        public static List<core.program> todelete = new List<core.program>();
        public static void delete()
        {
            RegistryKey[] keys = new RegistryKey[] { RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true), RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true), RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true) };
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\UC Uninstaller")) { Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\UC Uninstaller"); }
            foreach (core.program p in todelete.Where(a => a.invalid == true).ToList())
            {
                Process process = new Process();
                if (core.is64bit) { process.StartInfo.FileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "sysnative", "regedt32.exe"); }
                else { process.StartInfo.FileName = "regedit.exe"; }
                process.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\UC Uninstaller";// +@"\UC Uninstaller\";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Verb = "runas";
                process.StartInfo.Arguments = "/e \"" + p.name + ".reg\" \"" + ((p.keyid==3)?keys[p.keyid - 1].Name.ToString():@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall") + @"\"+p.regkey+"\"";
                process.Start();
                process.WaitForExit();
                keys[p.keyid-1].DeleteSubKey(p.regkey);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.dremember)
            {
                Properties.Settings.Default.delete = false;
                Properties.Settings.Default.Save();
            }
            this.DialogResult = false;
            this.Close();
        }

        private void remember_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.dremember = true;
            Properties.Settings.Default.Save();
        }

        private void remember_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.remember = false;
            Properties.Settings.Default.Save();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            core.write_ignored(todelete.Where(a => a.invalid == false).ToList());
        }
    }
}
