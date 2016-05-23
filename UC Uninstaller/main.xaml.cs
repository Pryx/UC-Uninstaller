using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;
using Core;
using Language;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Uninstaller
{
    /// <summary>
    /// Interaction logic for main.xaml
    /// </summary>
    public partial class main : Page
    {
        ManualResetEvent wait = new ManualResetEvent(false); 

        public main()
        {
            InitializeComponent();
            core.keypress += Page_KeyDown;            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (core.uninstalllist.Where(x => x.uninstall == true).Count() > 0)
            {
                core.uninstalllist = core.uninstalllist.Where(x => x.uninstall == true).ToList();
                core.uninstalllist.Sort((a, b) => String.Compare(a.name, b.name));
                if (!Properties.Settings.Default.remember)
                {
                    Window browsers = new browsers();
                    browsers.Owner = Application.Current.MainWindow;
                    browsers.ShowInTaskbar = false;
                    browsers.ShowDialog();
                }

                Window uninst = new uninstall();
                uninst.Owner = Application.Current.MainWindow;
                uninst.ShowInTaskbar = false;
                uninst.ShowDialog();
                search.Text = null;
                list();
            }
        }



        public void list()
        {
            advancedbutton.IsEnabled = false;
            var worker = new BackgroundWorker();
            if (!System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + @"\uninstaller.udb"))
            { core.databaseupdate(); core.dbconnect(); list(); return; }
            else
            {
                worker.DoWork += (sender, e) =>
                {
                    bool advanced = false;
                    if ((Properties.Settings.Default.advanced) | (Properties.Settings.Default.autostart & !Properties.Settings.Default.is_initialized)) { advanced = true; }
                    e.Result = core.search(advanced);
                };
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    core.uninstalllist = e.Result as List<core.program>;
                    count();
                    render(false, core.uninstalllist.ToList());
                    if ((core.uninstalllist.Where(x => x.invalid == true).Count() > 0) && (!Properties.Settings.Default.dremember) || Properties.Settings.Default.delete)
                    {
                        Window invalid = new invalid();
                        invalid.Owner = Application.Current.MainWindow;
                        invalid.ShowInTaskbar = false;
                        bool? dr = invalid.ShowDialog();
                        if (dr.HasValue && dr.Value)
                        {
                            list();
                            return;
                        }
                    }
                    advancedbutton.IsEnabled = true;
                    Properties.Settings.Default.is_initialized = true;
                    Properties.Settings.Default.Save();
                    worker.Dispose();
                };
                worker.RunWorkerAsync();
            }
        }

        public void reset() 
        {
            switch (selectedlabel.Content.ToString())
            {
                case "▼":
                    {
                        selectedlabel.Content="•";
                        return;
                    }
                case "▲":
                    {
                        selectedlabel.Content = "•";
                        return;
                    }
            }
        }


        public void render(bool rerendering, List<core.program> torender)
        {
            wait.Reset();
            core.no_shutdown = true;
            namelabel.IsEnabled = false;
            selectedlabel.IsEnabled = false;
            scrollbar.ScrollToTop();
            results.Children.Clear();
            results.RowDefinitions.Clear();

            if (torender.Count() > 0)
            {
                foreach (core.program program in torender)
                {
                    RowDefinition r = new RowDefinition();
                    r.Height = new GridLength(30, GridUnitType.Pixel);
                    results.RowDefinitions.Add(r);
                }

                double x = 0.1;
                int i = -1;
                checkall.IsEnabled = false;
                foreach (core.program program in torender)
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
                        uninst.IsChecked = program.uninstall;

                        if (program.uninstall)
                        {
                            SolidColorBrush solidcolor = new SolidColorBrush();
                            solidcolor.Color = Colors.White;
                            ColorAnimation animation = new ColorAnimation();
                            animation.To = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E3E3E3");
                            animation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                            solidcolor.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                            gr.Background = solidcolor;
                        }
                        uninst.Name = "name" + program.id;
                        Grid.SetColumn(uninst, 3);
                        Grid.SetRow(uninst, 0);
                        uninst.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        uninst.Checked += (sender, e) =>
                        {
                            core.uninstalllist.Find(a => a.id == Convert.ToInt32(uninst.Name.Replace("name", ""))).uninstall = true;
                            reset();
                            count();
                        };
                        uninst.Unchecked += (sender, e) =>
                        {
                            core.uninstalllist.Find(a => a.id == Convert.ToInt32(uninst.Name.Replace("name", ""))).uninstall = false;
                            reset();
                            count();
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
                                Icon ico = Icon.ExtractAssociatedIcon(array[0]);
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

                        results.Children.Add(gr);
                        if (!rerendering)
                        {
                            if (x < 0.6) { x += 0.012; }
                            DoubleAnimation da = new DoubleAnimation();
                            da.From = 0;
                            da.To = 1;
                            da.Duration = new Duration(TimeSpan.FromSeconds(x));
                            gr.BeginAnimation(OpacityProperty, da);
                        }
                    }));
                }
            }
            else if (torender.Count()==0 && core.uninstalllist.Count()==0)
            {
                RowDefinition r = new RowDefinition();
                r.Height = new GridLength(40, GridUnitType.Pixel);
                results.RowDefinitions.Add(r);
                r = new RowDefinition();
                r.Height = new GridLength(70, GridUnitType.Pixel);
                results.RowDefinitions.Add(r);
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                Grid.SetRow(img, 1);
                img.Source = new BitmapImage(new Uri("images/smile.png", UriKind.Relative));
                results.Children.Add(img);
                r = new RowDefinition();
                r.Height = new GridLength(40, GridUnitType.Pixel);
                results.RowDefinitions.Add(r);
                Label lab = new Label();
                lab.Content = languages.controls.clean;
                lab.Foreground = new SolidColorBrush(Colors.Green);
                lab.FontFamily = new System.Windows.Media.FontFamily(new Uri("pack://application:,,,/fonts/"), "./#Open Sans Bold");
                lab.FontSize = 14;
                lab.HorizontalAlignment = HorizontalAlignment.Center;
                lab.Margin = new Thickness(0,10,0,0);
                Grid.SetRow(lab, 2);
                results.Children.Add(lab);
            }
                count();
                namelabel.IsEnabled = true;
                selectedlabel.IsEnabled = true;
                wait.Set();
                core.no_shutdown = false;
                if (core.shutdown) { Application.Current.MainWindow.Close(); }
        }

        public void count()
        {
            if (core.uninstalllist.Count() > 0)
            {

                checkall.IsEnabled = true;
                if (core.uninstalllist.Where(x => x.uninstall == true).Count() > 0)
                {
                    uninstall.IsEnabled = true;
                    feedback.IsEnabled = true;
                }
                else
                {
                    uninstall.IsEnabled = false;
                    feedback.IsEnabled = false;
                }

                if (core.uninstalllist.Where(x => x.uninstall == true).Count() > 0 & core.uninstalllist.Where(x => x.uninstall == true).Count() < core.uninstalllist.Count())
                {
                    checkall.Tag = "semichecked";
                    CheckMark.StrokeThickness = 1;
                    CheckMark.Fill = new SolidColorBrush(Colors.Black);
                    CheckMark.Data = Geometry.Parse("M 1,0 1,7 7,7 7,0.5 1,0.5");
                }
                else if (core.uninstalllist.Where(x => x.uninstall == true).Count() == core.uninstalllist.Count())
                {
                    CheckMark.Fill = null;
                    CheckMark.StrokeThickness = 2;
                    CheckMark.Data = Geometry.Parse("M 0,0 12,12 M 12,0 0,12");
                    checkall.Tag = "checked";
                }
                else
                {
                    CheckMark.StrokeThickness = 1;
                    CheckMark.Fill = new SolidColorBrush(Colors.Black);
                    checkall.Tag = "unchecked";
                    CheckMark.Data = Geometry.Parse(@"M 12.4227,0.00012207C 12.4867,0.126587 12.5333,0.274536 
                12.6787,0.321411C 9.49199,3.24792 6.704,6.57336 
                4.69865,10.6827C 4.04399,11.08 3.47066,11.5573 2.83199,
                11.9706C 2.09467,10.2198 1.692,8.13196 3.8147e-006,
                7.33606C 0.500004,6.79871 1.31733,6.05994 1.93067,6.2428C 
                2.85999,6.51868 3.14,7.9054 3.60399,8.81604C 5.80133,
                5.5387 8.53734,2.19202 12.4227,0.00012207 Z ");
                }
            }
            else { checkall.IsEnabled = false; }
            
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new settings());
        }

        private void info_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new about());
        }

        private void advanced_Click(object sender, RoutedEventArgs e)
        {
            if (advancedmode.Text == languages.controls.basic)
            {
                Properties.Settings.Default.advanced = false;
                Properties.Settings.Default.Save();
                advancedmode.Text = languages.controls.advanced;
                feedback.Visibility = Visibility.Hidden;
            }
            else
            {
                advancedmode.Text = languages.controls.basic;
                Properties.Settings.Default.advanced = true;
                Properties.Settings.Default.Save();
                feedback.Visibility = Visibility.Visible;
            }
            list();
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            namelabel.Content = languages.controls.name;
            advancedmode.Text = languages.controls.advanced;
            feedback.Content = languages.controls.send_feedback;
            uninstall.Content =  languages.controls.uninstall;
            if (Properties.Settings.Default.advanced)
            {
                advancedmode.Text = languages.controls.basic;
                feedback.Visibility = Visibility.Visible;
            }
            list();

            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 1;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            this.BeginAnimation(OpacityProperty, da);
        }

        private void feedback_Click(object sender, RoutedEventArgs e)
        {
                MessageBoxResult result = MessageBox.Show(languages.messages.feedback_selected.text, languages.messages.feedback_selected.title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    core.sendtoserver(core.uninstalllist.Where(x => x.uninstall == true).ToList());
                    System.Windows.Forms.MessageBox.Show(languages.messages.feedback_sent.text, languages.messages.feedback_sent.title);
                }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (checkall.Tag.ToString() == "unchecked") 
            {
                foreach (Grid grd in results.Children) 
                {
                    foreach (var cnt in grd.Children)
                    {
                        if (cnt is CheckBox){
                            var check = (CheckBox)cnt;
                            check.IsChecked = true;
                        }
                    }
                }
            }
            else if (checkall.Tag.ToString() == "checked")
            {
                foreach (Grid grd in results.Children)
                {
                    foreach (var cnt in grd.Children)
                    {
                        if (cnt is CheckBox)
                        {
                            var check = (CheckBox)cnt;
                            check.IsChecked = false;
                        }
                    }
                }
            }
            else 
            {
                foreach (Grid grd in results.Children)
                {
                    foreach (var cnt in grd.Children)
                    {
                        if (cnt is CheckBox)
                        {
                            var check = (CheckBox)cnt;
                            check.IsChecked = false;
                        }
                    }
                }
                CheckMark.StrokeThickness = 1;
                CheckMark.Fill = new SolidColorBrush(Colors.Black);
                checkall.Tag = "unchecked";
                CheckMark.Data = Geometry.Parse(@"M 12.4227,0.00012207C 12.4867,0.126587 12.5333,0.274536 
                12.6787,0.321411C 9.49199,3.24792 6.704,6.57336 
                4.69865,10.6827C 4.04399,11.08 3.47066,11.5573 2.83199,
                11.9706C 2.09467,10.2198 1.692,8.13196 3.8147e-006,
                7.33606C 0.500004,6.79871 1.31733,6.05994 1.93067,6.2428C 
                2.85999,6.51868 3.14,7.9054 3.60399,8.81604C 5.80133,
                5.5387 8.53734,2.19202 12.4227,0.00012207 Z ");
            }
        }

        private void namelabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selectedlabel.Content = "•";
            switch (namelabel.Content.ToString().Replace(languages.controls.name, ""))
            {
                case " ▼":
                    {
                        namelabel.Content = languages.controls.name + " ▲";
                        core.uninstalllist.Sort((a, b) => String.Compare(b.name, a.name));
                        render(true,core.uninstalllist.ToList());
                        return;
                    }
                case " ▲":
                    {
                        namelabel.Content = languages.controls.name + " ▼";
                        core.uninstalllist.Sort((a, b) => String.Compare(a.name, b.name));
                        render(true, core.uninstalllist.ToList());
                        return;
                    }
                default:
                    {
                        namelabel.Content = languages.controls.name + " ▼";
                        core.uninstalllist.Sort((a, b) => String.Compare(a.name, b.name));
                        render(false, core.uninstalllist.ToList());
                        return;
                    }
            }
        }

        private void selectedlabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            namelabel.Content = languages.controls.name;
            switch (selectedlabel.Content.ToString())
            {
                case "•":
                    {
                        selectedlabel.Content = "▼";
                        core.uninstalllist = core.uninstalllist.OrderByDescending(x => x.uninstall).ToList();
                        render(true, core.uninstalllist.ToList());
                        return;
                    }
                case "▼":
                    {
                        selectedlabel.Content = "▲";
                        core.uninstalllist = core.uninstalllist.OrderBy(x => x.uninstall).ToList();
                        render(true, core.uninstalllist.ToList());
                        return;
                    }
                case "▲":
                    {
                        selectedlabel.Content = "•";
                        core.uninstalllist.Sort((a, b) => String.Compare(a.name, b.name));
                        render(true, core.uninstalllist.ToList());
                        return;
                    }
            }
        }

        private void scrollbar_LayoutUpdated(object sender, EventArgs e)
        {
            if (scrollbar.ComputedVerticalScrollBarVisibility == Visibility.Visible) { helper.Width = new GridLength(18); }
            else { helper.Width = new GridLength(0); }
        }


        public void Page_KeyDown(object sender, EventArgs e)
        {
            KeyEventArgs k = (KeyEventArgs)e;
            if (k.Key.ToString().Length == 1)
            {
                search.Visibility = Visibility.Visible;
                search.Focus();
            }
        }

        bool running = false;
        bool waiting = false;

        private async void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (running&&!waiting)
            {
                waiting = true;
                await Task.Run(() =>
                {
                    wait.WaitOne();
                }
               );
                waiting = false;
                search_TextChanged(null,null);
                return;
            }
            else if (waiting) { return; }
            running = true;
            await Task.Run(() =>
            {
                wait.WaitOne();
            }
            );

            if (search.Text.Length == 0) 
            { 
                search.Visibility = Visibility.Collapsed;
                search.Focus();
                render(true, core.uninstalllist.ToList()); 
            }
            else 
            {
               render(true, core.uninstalllist.Where(x => x.name.ToLower().Contains(search.Text.ToLower())).ToList());
            }
            running = false;
            
        }


    }
}
