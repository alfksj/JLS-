using System;
using System.ComponentModel;
using System.IO;
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
            HelloWorld hwx = new HelloWorld();
            hwx.Show();
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if(args.Length>1) //첫번째 인수는 프로세스 시작 위치니까 믿고 거른다.
            {
                string q = args[1] + "\n";
                char[] ss = q.ToCharArray();
                string[] sq = new string[1024];
                int par = 0, start = 0, len = 0, abs = 0;
                foreach(char pae in ss)
                {
                    if(pae=='-')
                    {
                        start = abs;
                    }
                    else if(pae==',' || pae=='\n')
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
                        debug.makeLog("Reset Data Reqested");
                        db.suicide();
                        db.suicideOfHw();
                    }
                    if (cmd.Equals("-RESET_CACHE"))
                    {
                        debug.makeLog("Reset Cache Data Reqested");
                        db.suicideOfHw();
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
            Setting.load();
            UpdateWindow();
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
            Task.Run(async () =>//시작할때 눈치껏 브라우저 초기화 & 접속 & 숙제 로드
            {
                web = new WebControl(WebControl.usr_agent, WebControl.gpu_acc, WebControl.fake_plugin, WebControl.use_win, WebControl.lang, db);
                string hw = web.load();
                await time.Dispatcher.InvokeAsync(() =>
                {
                    if(hw.Equals("<font size=\"4\" color=\"red\"><b>로그인할 수 없습니다.</b><br />ID와 비밀번호가 정확한지 확인해 주세요.<br />서버 지연이 너무 심하면 이런 오류가 뜰 수 도 있습니다.</font>") ||
                    hw.Equals("<font size=\"4\" color=\"red\"><b>숙제를 확인할 수 없습니다.</b><br />이 문제의 원인은 다양합니다. 일시적으로 JLS서버를 이용할 수 없는 것일 수 있고 지연시간이 너무 심한것 일 수 도 있으며 잘못된 날짜를 입력한 것 일 수 도 있습니다.</font>") ||
                    hw.Equals("NOT LOAD") ||
                    Setting.LoadDatAtSet) {
                        html_stream.Text = hw;
                        thisis.Content = web.currentDate;
                    }
                });
            });
            string cach = db.getLatestHw();
            if(!cach.Equals("NO CACHE DATA FOUND") && Setting.LoadCache)
            {
                debug.makeLog("Use cache date first");
                html_stream.Text = cach;
                thisis.Content = db.currentDate;
            }
            hwx.Hide();
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
            ratc.IsChecked = Setting.LoadCache;
            getAtSt.IsChecked = Setting.LoadDatAtSet;
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
                Setting.LoadCache = (bool)ratc.IsChecked;
                Setting.LoadDatAtSet = (bool)getAtSt.IsChecked;
                db.saveAll(new Profile(setName.Text, setID.Text, setPwd.Password));                
                db.loadAll();
                UpdateWindow();
                Whatsetting.Content = "JLS++에 온걸 환영합니다!";
                greeting_setting.Visibility = Visibility.Visible;
                profileset.Visibility = Visibility.Hidden;
                browserset.Visibility = Visibility.Hidden;
                crawl_set.Visibility = Visibility.Hidden;
                fileset.Visibility = Visibility.Hidden;
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
            crawl_set.Visibility = Visibility.Hidden;
        }

        private void browser_Click(object sender, RoutedEventArgs e)
        {
            Whatsetting.Content = "브라우저 설정";
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Hidden;
            browserset.Visibility = Visibility.Visible;
            fileset.Visibility = Visibility.Hidden;
            crawl_set.Visibility = Visibility.Hidden;
        }

        private void file_Click(object sender, RoutedEventArgs e)
        {
            Whatsetting.Content = "스토리지 설정";
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Hidden;
            browserset.Visibility = Visibility.Hidden;
            fileset.Visibility = Visibility.Visible;
            crawl_set.Visibility = Visibility.Hidden;
        }
        private void crawl_Click(object sender, RoutedEventArgs e)
        {
            Whatsetting.Content = "크롤링 설정";
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Hidden;
            browserset.Visibility = Visibility.Hidden;
            fileset.Visibility = Visibility.Hidden;
            crawl_set.Visibility = Visibility.Visible;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult ret = MessageBox.Show("프로필을 포함한 모든 설정이 초기화됩니다.\n확실합니까?", "JLS++", MessageBoxButton.YesNo);
            if(ret == MessageBoxResult.Yes)
            {
                last show = new last("설정을 초기화하기 위해 프로그램을 다시 시작합니다.\nSQLite: Database Reset.", "-RESET_REQUEST,");
                show.Show();
                Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string s;
            if (!date.Text.Equals(""))
            {
                if(Setting.LoadCache)
                {
                    s = db.getHw(Int32.Parse(date.Text));
                    thisis.Content = db.currentDate;
                    if (s.Equals("NO CACHE DATA FOUND"))
                    {
                        s = web.load(Int32.Parse(date.Text));
                        thisis.Content = web.currentDate;
                    }
                }
                else
                {
                    s = web.load(Int32.Parse(date.Text));
                    thisis.Content = web.currentDate;
                }
            }
            else
            {
                if(Setting.LoadCache)
                {
                    s = db.getLatestHw();
                    thisis.Content = db.currentDate;
                    if (s.Equals("NO CACHE DATA FOUND"))
                    {
                        s = web.justGet();
                        thisis.Content = web.currentDate;
                    }
                }
                else
                {
                    s = web.justGet();
                    thisis.Content = web.currentDate;
                }
            }
            html_stream.Text = s;
        }

        private void del_cache_Click(object sender, RoutedEventArgs e)
        {
            last lts = new last("", "-RESET_CACHE");
            Close();
            lts.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            web.close();
            //꼭 로딩중에 이상한거 눌러서 오류만드는 유저 대비용
            hwreg.IsEnabled = false;
            setting.IsEnabled = false;
            web = new WebControl(WebControl.usr_agent, WebControl.gpu_acc, WebControl.fake_plugin, WebControl.use_win, WebControl.lang, db);
            hwreg.IsEnabled = true;
            setting.IsEnabled = true;
        }
        public void coffin(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
        public void aboutp(object senderm, RoutedEventArgs e)
        {
            About a = new About();
            a.Show();
        }
    }
}
