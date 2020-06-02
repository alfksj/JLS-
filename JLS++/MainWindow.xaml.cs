using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Resources;
using JLS___Library;
using JLS___Library.Data;
using System.Globalization;
using System.Windows.Threading;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxButton = System.Windows.Forms.MessageBoxButtons;
using MessageBoxResult = System.Windows.Forms.DialogResult;
using Application = System.Windows.Application;
using System.Text;
using System.Drawing;

namespace JLS__
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database db = new Database();
        private WebControl web = null;
        private Thread currentThread = Thread.CurrentThread;
        public MainWindow()
        {
            //Write in UTF-8
            //Console.OutputEncoding = new UTF8Encoding();
            HelloWorld hwx = new HelloWorld();
            //hwx.Show();
            //언어정보 전파!
            db.loadAll();
            Setting.load();
            var langCode = WebControl.lang;
            db.langCode = langCode;
            currentLang = langCode;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(langCode);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCode);
            debug.makeLog("Current Culture=" + CultureInfo.CurrentCulture.Name);
            InitializeComponent();
            /////TRAY SYSTEM///////
            NotifyIcon ni = new NotifyIcon();
            ContextMenu tray = new ContextMenu();
            MenuItem exit = new MenuItem();
            exit.Index = 0;
            exit.Text = rm.GetString("exit");
            exit.Click += delegate (object click, EventArgs e)
            {
                coffin(null, null);
            };
            MenuItem open = new MenuItem();
            open.Text = rm.GetString("open");
            open.Click += delegate (object click, EventArgs e)
            {
                Show();
            };
            open.Index = 0;
            tray.MenuItems.Add(open);
            tray.MenuItems.Add(exit);
            ni.Icon = new Icon("Resources/icon.ico");
            ni.Visible = true;
            ni.DoubleClick += delegate (object senders, EventArgs e)
            {
                Show();
            };
            ni.ContextMenu = tray;
            ni.Text = "JLS++";
            ///////////////////////
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1) //첫번째 인수는 프로세스 시작 위치니까 믿고 거른다.
            {
                string q = args[1] + "\n";
                char[] ss = q.ToCharArray();
                string[] sq = new string[1024];
                int par = 0, start = 0, len = 0, abs = 0;
                foreach (char pae in ss)
                {
                    if (pae == '-')
                    {
                        start = abs;
                    }
                    else if (pae == ',' || pae == '\n')
                    {
                        sq[par] = args[1].Substring(start, len);
                        par++;
                        len = -1;
                    }
                    len++;
                    abs++;
                }
                foreach (string cmd in sq)
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
                    if (cmd.Equals("-debug"))
                    {
                        Console.WriteLine("YOU ARE USING DEBUG MODE\n========================================================");
                    }
                    if (cmd.Equals("-no_window"))
                    {
                        {
                            Hide();
                        }
                    }
                }
            }
            UpdateWindow();
            savePath.Text = System.Environment.GetEnvironmentVariable("appdata") + "/.JLS++/data.db";
            Task.Run(async () =>//시간 새로고침
            {
                while (true)
                {
                    string regen = DateTime.Now.ToString("yyyy/MM/dd  HH:mm:ss");
                    await time.Dispatcher.InvokeAsync(() => time.Content = regen);
                    Thread.Sleep(1000);
                }
            });
            Task.Run(() =>//시작할때 눈치껏 브라우저 초기화 & 접속 & 숙제 로드
            {
                web = new WebControl(WebControl.usr_agent, WebControl.gpu_acc, WebControl.fake_plugin, WebControl.use_win, WebControl.lang, db);
                web.langCode = langCode;
                var hw = web.load();
                var rlm = localize(hw);
                if (hw.Contains("failure:") || Setting.LoadDatAtSet)
                {
                    html_stream.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        html_stream.Text = rlm;
                    }));
                    thisis.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        thisis.Content = web.currentDate;
                    }));
                }
                hwreg.Dispatcher.Invoke(() =>
                {
                    hwreg.IsEnabled = true;
                });
            });
            string cach = db.getLatestHw();
            if (!cach.Equals("NO CACHE DATA FOUND") && Setting.LoadCache)
            {
                debug.makeLog("Use cache date first");
                html_stream.Text = cach;
                thisis.Content = db.currentDate;
            }
            //hwx.Hide();
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
            //TODO: 기다리는게 싫은 우리 친구들을 위해 애니메이션으로 암/복호화 눈치 못채게 함
            if (settingPage.Visibility == Visibility.Hidden)
            {
                debug.makeLog("Open Setting");
                db.loadAll();
                UpdateWindow();
                debug.makeLog("Security Clear");
                r2.Visibility = Visibility.Hidden;
                settingPage.Visibility = Visibility.Visible;
            }
            else
            {
                debug.makeLog("Close");
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
                debug.makeLog("Security Clear");
                UpdateWindow();
                Whatsetting.Content = rm.GetString("setting");
                greeting_setting.Visibility = Visibility.Visible;
                profileset.Visibility = Visibility.Hidden;
                browserset.Visibility = Visibility.Hidden;
                crawl_set.Visibility = Visibility.Hidden;
                fileset.Visibility = Visibility.Hidden;
                r2.Visibility = Visibility.Visible;
            }
        }
        private ResourceManager rm = new ResourceManager("JLS__.Localization", typeof(MainWindow).Assembly);
        private string currentLang;

        private void account_Click(object sender, RoutedEventArgs e)
        {
            if(Secure.Nop==0)
            {
                Whatsetting.Content = rm.GetString("profSetting");
            }
            else
            {
                if(currentLang.Equals("ko-KR"))
                {
                    Whatsetting.Content = rm.GetString("settedProfile");
                }
                else
                {
                    Whatsetting.Content = rm.GetString("settedProfile") + Secure.Propile.Name + ".";
                }
            }
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Visible;
            browserset.Visibility = Visibility.Hidden;
            fileset.Visibility = Visibility.Hidden;
            crawl_set.Visibility = Visibility.Hidden;
        }

        private void browser_Click(object sender, RoutedEventArgs e)
        {
            Whatsetting.Content = rm.GetString("browserSetting");
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Hidden;
            browserset.Visibility = Visibility.Visible;
            fileset.Visibility = Visibility.Hidden;
            crawl_set.Visibility = Visibility.Hidden;
        }

        private void file_Click(object sender, RoutedEventArgs e)
        {
            Whatsetting.Content = rm.GetString("storageSetting");
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Hidden;
            browserset.Visibility = Visibility.Hidden;
            fileset.Visibility = Visibility.Visible;
            crawl_set.Visibility = Visibility.Hidden;
        }
        private void crawl_Click(object sender, RoutedEventArgs e)
        {
            Whatsetting.Content = rm.GetString("crawlSetting");
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Hidden;
            browserset.Visibility = Visibility.Hidden;
            fileset.Visibility = Visibility.Hidden;
            crawl_set.Visibility = Visibility.Visible;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult ret = MessageBox.Show(rm.GetString("resetAlert"), "JLS++", MessageBoxButton.YesNo);
            if(ret == MessageBoxResult.Yes)
            {
                last show = new last(rm.GetString("resProg")+"\nSQLite: Database Reset.", "-RESET_REQUEST,");
                show.Show();
                Visibility = Visibility.Hidden;
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
                        var sq = web.load(Int32.Parse(date.Text));
                        s = localize(sq);
                        thisis.Content = web.currentDate;
                    }
                }
                else
                {
                    var qs = web.load(Int32.Parse(date.Text));
                    s = localize(qs);
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
                        var qs = web.load();
                        s = localize(qs);
                        thisis.Content = web.currentDate;
                    }
                }
                else
                {
                    var qs = web.load();
                    s = localize(qs);
                    thisis.Content = web.currentDate;
                }
            }
            html_stream.Text = s;
        }

        private void del_cache_Click(object sender, RoutedEventArgs e)
        {
            last lts = new last("", "-RESET_CACHE");
            lts.Show();
            Visibility = Visibility.Hidden;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            web.close();
            //꼭 로딩중에 이상한거 눌러서 오류만드는 유저 대비용 근데 실효성 없음
            hwreg.IsEnabled = false;
            setting.IsEnabled = false;
            web = new WebControl(WebControl.usr_agent, WebControl.gpu_acc, WebControl.fake_plugin, WebControl.use_win, WebControl.lang, db);
            hwreg.IsEnabled = true;
            setting.IsEnabled = true;
        }
        public void coffin(object sender, CancelEventArgs e)
        {
            web.driver.Close();
            Application.Current.Shutdown();
        }
        public void resurrection(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        public void aboutp(object senderm, RoutedEventArgs e)
        {
            About a = new About();
            a.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // TODO: 일단은 Chrome에서 jls 사이트 열어줌. JS실행은 나중에 지원하자^^
            Process.Start("C:/Program Files (x86)/Google/Chrome/Application/chrome.exe", "https://www.gojls.com/branch/myjls/homework");

        }

        private string localize(string s)
        {
            if (s.Contains("failure:"))//실패코드
            {
                return rm.GetString(s);
            }
            else return s;
        }
    }
}
