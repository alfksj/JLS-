using System.IO;
using System.Windows;

namespace JLS__
{
    /// <summary>
    /// About.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            //html.Text = File.ReadAllText("asset/hello.html");
        }
    }
}
