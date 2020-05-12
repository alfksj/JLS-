using System.Windows;
using System.Diagnostics;

namespace JLS__
{
    /// <summary>
    /// last.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class last : Window
    {
        string arg;
        public last(string code, string arg)
        {
            InitializeComponent();
            code_set.Content = code;
            this.arg = arg;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Application.ResourceAssembly.Location, arg);
            Application.Current.Shutdown();
        }
    }
}
