using System;
using System.Linq;
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
        private WebControl web = null;
        public MainWindow()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if(args.Length>1) //첫번째 인수는 프로세스 시작 위치니까 믿고 거른다.
            {
                char[] ss = args[1].ToCharArray();
                string[] sq = new string[1024];
                int par = 0, start = 0, len = 0, abs = 0;
                foreach(char pae in ss)
                {
                    if(pae=='-')
                    {
                        start = abs;
                    }
                    else if(pae==',')
                    {
                        sq[par] = args[1].Substring(start, len);
                        par++;
                        len = -1;
                    }
                    len++;
                    abs++;
                }
                foreach(string cmd in sq)
                {
                    if (cmd == null) break;
                    if (cmd.Equals("-RESET_REQUEST"))
                    {
                        db.suicide();
                        db = null;
                        db = new Database();
                    }
                    else if (cmd.Equals("-debug"))
                    {
                        Console.WriteLine("YOU ARE USING DEBUG MODE\n========================================================");
                    }
                    else if(cmd.Equals("-no_window"))
                    {
                        Hide();
                    }
                }
            }
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
            Task.Run(async () =>//시간 새로고침
            {
                while (true)
                {
                    string regen = DateTime.Now.ToString("yyyy년 MM월 dd일  HH시 mm분 ss초");
                    await time.Dispatcher.InvokeAsync(() => time.Content = regen);
                    Thread.Sleep(1000);
                }
            });
            web = new WebControl(WebControl.usr_agent, WebControl.gpu_acc, WebControl.fake_plugin, WebControl.use_win, WebControl.lang);
            Task.Run( () =>//시작할때 눈치껏 브라우저 초기화 & 접속 & 숙제 로드
            {
                string hw = web.load();
                html_stream.Text = hw;
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
                r2.Visibility = Visibility.Hidden;
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
                r2.Visibility = Visibility.Visible;
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
                Close();
            }
        }
    }
}
