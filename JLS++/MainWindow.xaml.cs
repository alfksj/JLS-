using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
            InitializeComponent();
            db.loadAll();
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

        private void setting_Click(object sender, RoutedEventArgs e)
        {
            if(settingPage.Visibility == Visibility.Hidden)
            {
                settingPage.Visibility = Visibility.Visible;
            }
            else
            {
                settingPage.Visibility = Visibility.Hidden;
                db.saveAll(new Profile(setName.Text, setID.Text, setPwd.Password));
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
        }

        private void browser_Click(object sender, RoutedEventArgs e)
        {
            Whatsetting.Content = "브라우저 설정";
            greeting_setting.Visibility = Visibility.Hidden;
            profileset.Visibility = Visibility.Hidden;
            browserset.Visibility = Visibility.Visible;
        }
    }
}
