using JLS___Library.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Data;
using System.Threading;

namespace JLS___Library
{
    /// <summary>
    /// Selenium을 이용한 크롤링등을 담당하는 클래스입니다.
    /// </summary>
    public class WebControl
    {
        public static string usr_agent, lang;
        public static bool fake_plugin, use_win, gpu_acc;
        public string usr_agent_, lang_;
        public bool fake_plugin_, use_win_, gpu_acc_;
        public ChromeDriverService driverSvc;
        public ChromeOptions options;
        public ChromeDriver driver;
        public void makeLog(string msg)
        {
            var dat = DateTime.Now.ToString("[yyyy-MM-dd, HH:mm:ss.ffff] : ");
            Console.WriteLine(dat + msg);
        }
        public WebControl(string agent, bool gpu, bool fake, bool use_win, string lang)
        {
            makeLog("Headless Browser Initializing");
            driverSvc = ChromeDriverService.CreateDefaultService();
            options = new ChromeOptions();
            usr_agent_ = agent;
            gpu_acc_ = gpu;
            fake_plugin_ = fake;
            use_win_ = use_win;
            lang_ = lang;
            makeLog("Setting options");
            driverSvc.HideCommandPromptWindow = true;
            options.AddArgument("user_agent=" + agent);
            options.AddArgument("lang=" + lang);
            //options.AddArgument("window-size=1000x800");
            if (gpu)
            {
                options.AddArgument("disable-gpu");
            }
            if(!use_win)
            {
                options.AddArgument("--window-position=-32000,-32000");
                options.AddArgument("headless");
            }
            makeLog("Creating new chrome driver");
            driver = new ChromeDriver(driverSvc, options);
            makeLog("Done");
        }
        public string load()
        {
            makeLog("Loading Howework");
            if (Secure.Propile == null)
            {
                makeLog("Failure: No login data found");
                return "login required";
            }
            makeLog("Navigating to target URL");
            driver.Navigate().GoToUrl("https://www.gojls.com/branch/myjls/homework");
            makeLog("Finding elements");
            var id = driver.FindElementById("userId");
            var pwd = driver.FindElementById("passWd");
            var go = driver.FindElementByClassName("log");
            makeLog("Sending keys");
            id.SendKeys(Secure.Propile.Id);
            pwd.SendKeys(Secure.Propile.Pwd);
            go.Click();
            makeLog("Logged in");
            makeLog("Checking status");
            Thread.Sleep(1000);
            if(driver.Url.Equals("https://www.gojls.com/login?preURL=/branch/myjls/homework"))
            {
                makeLog("Login failure");
                return "login failure";
            }
            makeLog("Finding homework");
            var ok = driver.FindElementByClassName("swal2-confirm");
            ok.Click();
            var executor = driver as IJavaScriptExecutor;
            var CodeOfToday = DateTime.Now.ToString("yyyyMMdd");
            executor.ExecuteScript("studyDate(\'"+CodeOfToday+"\')");

            var hwa = driver.FindElementByClassName("oldarea");
            Console.WriteLine(hwa.Text);
            makeLog("Done.");
            return null;
        }
    }
}
