using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JLS___Library
{
    /// <summary>
    /// Selenium을 이용한 크롤링등을 담당하는 클래스입니다.
    /// </summary>
    public class WebControl
    {
        public static string usr_agent, lang;
        public static bool fake_plugin, use_win, gpu_acc;
        public IWebDriver chromeDriver = new ChromeDriver();
        public int login()
        {
            return 0;
        }
    }
}
