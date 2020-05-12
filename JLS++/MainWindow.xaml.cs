using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using JLS___Library;
using JLS___Library.Data;

namespace JLS__
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database db = new Database();
        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs();
            if(args.Length!=0)
            {
                if(args[1].Equals("-RESET_REQUEST"))
                {
                    db.suicide();
                    db = null;
                    db = new Database();
                }
                if(args[1].Equals("-debug"))
                {
                    Console.WriteLine("YOU ARE USING DEBUG MODE");
                }
            }
            InitializeComponent();
            db.loadAll();
            if (Secure.Propile != null)
            {
                setName.Text = Secure.Propile.Name;
                setID.Text = Secure.Propile.Id;
                setPwd.Password = Secure.Propile.Pwd;
                name.Content = Secure.Propile.Name;
            }
            lang.Text = WebControl.lang;
            showWin.IsChecked = WebControl.use_win;
            gpuac.IsChecked = WebControl.gpu_acc;
            fake_plugin.IsChecked = WebControl.fake_plugin;
            usragent.Text = WebControl.usr_agent;
            savePath.Text = System.Environment.GetEnvironmentVariable("appdata") + "\\.JLS++\\data.db";
            Task.Run(async () =>
            {
                while (true)
                {
                    string regen = DateTime.Now.ToString("yyyy년 MM월 dd일  HH시 mm분 ss초");
                    await time.Dispatcher.InvokeAsync(() => time.Content = regen);
                    Thread.Sleep(1000);
                }
            });
        }

        public void UpdateWindow()
        {
            if (Secure.Propile != null)
            {
                setName.Text = Secure.Propile.Name;
                setID.Text = Secure.Propile.Id;
                setPwd.Password = Secure.Propile.Pwd;
                name.Content = Secure.Propile.Name;
            }
            r2.Visibility = Visibility.Hidden;
            lang.Text = WebControl.lang;
            showWin.IsChecked = WebControl.use_win;
            gpuac.IsChecked = WebControl.gpu_acc;
            fake_plugin.IsChecked = WebControl.fake_plugin;
            usragent.Text = WebControl.usr_agent;
        }

        private void setting_Click(object sender, RoutedEventArgs e)
        {
            if(settingPage.Visibility == Visibility.Hidden)
            {
                db.loadAll();
                UpdateWindow();
                settingPage.Visibility = Visibility.Visible;
            }
            else
            {
                settingPage.Visibility = Visibility.Hidden;
                r2.Visibility = Visibility.Visible;
                WebControl.lang = lang.Text;
                WebControl.usr_agent = usragent.Text;
                WebControl.use_win = (bool)showWin.IsChecked;
                WebControl.gpu_acc = (bool)gpuac.IsChecked;
                WebControl.fake_plugin = (bool)fake_plugin.IsChecked;
                db.saveAll(new Profile(setName.Text, setID.Text, setPwd.Password));
                db.loadAll();
                UpdateWindow();
                Whatsetting.Content = "JLS++에 온걸 환영합니다!";
                greeting_setting.Visibility = Visibility.Visible;
                profileset.Visibility = Visibility.Hidden;
                browserset.Visibility = Visibility.Hidden;
            }
        }

        private void account_Click(object sender, RoutedEventArgs e)
        {
            if(Secure.Nop==0)
            {
                Whatsetting.Content = "프로필 설정";
            }
            else
            {
                Whatsetting.Content = Secure.Propile.Name+"에 대한 프로필 설정";
            }
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Visible;
            browserset.Visibility = Visibility.Hidden;
            fileset.Visibility = Visibility.Hidden;
        }

        private void browser_Click(object sender, RoutedEventArgs e)
        {
            Whatsetting.Content = "브라우저 설정";
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Hidden;
            browserset.Visibility = Visibility.Visible;
            fileset.Visibility = Visibility.Hidden;
        }

        private void file_Click(object sender, RoutedEventArgs e)
        {
            Whatsetting.Content = "스토리지 설정";
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Hidden;
            browserset.Visibility = Visibility.Hidden;
            fileset.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult ret = MessageBox.Show("프로필을 포함한 모든 설정이 초기화됩니다.\n확실합니까?", "JLS++", MessageBoxButton.YesNo);
            if(ret == MessageBoxResult.Yes)
            {
                last show = new last("설정을 초기화하기 위해 프로그램을 다시 시작합니다.\nSQLite: Database Reset.", "-RESET_REQUEST");
                show.Show();
                this.Close();
            }
        }
    }
}
