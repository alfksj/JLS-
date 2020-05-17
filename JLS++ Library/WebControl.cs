using JLS___Library.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
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
        public Database sav;
        public ChromeDriverService driverSvc;
        public ChromeOptions options;
        public ChromeDriver driver;
        private bool loaded = false;
        public WebControl(string agent, bool gpu, bool fake, bool use_win, string lang, Database sav)
        {
            debug.makeLog("Headless Browser Initializing");
            driverSvc = ChromeDriverService.CreateDefaultService();
            options = new ChromeOptions();
            usr_agent_ = agent;
            gpu_acc_ = gpu;
            fake_plugin_ = fake;
            use_win_ = use_win;
            lang_ = lang;
            this.sav = sav;
            debug.makeLog("Setting options");
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
            debug.makeLog("Creating new chrome driver");
            driver = new ChromeDriver(driverSvc, options);
            debug.makeLog("Done");
            loaded = false;
        }
        public string load()
        {
            try
            {
                if (!loaded)
                {
                    debug.makeLog("Loading Howework");
                    if (Secure.Propile == null)
                    {
                        debug.makeLog("Failure: No login data found");
                        return "failure:1";
                    }
                    debug.makeLog("Navigating to target URL");
                    driver.Navigate().GoToUrl("https://www.gojls.com/branch/myjls/homework");
                    debug.makeLog("Finding elements");
                    var id = driver.FindElementById("userId");
                    var pwd = driver.FindElementById("passWd");
                    var go = driver.FindElementByClassName("log");
                    debug.makeLog("Sending keys");
                    id.SendKeys(Secure.Propile.Id);
                    pwd.SendKeys(Secure.Propile.Pwd);
                    go.Click();
                    debug.makeLog("Logged in");
                    debug.makeLog("Checking status");
                    Thread.Sleep(1000);
                    if (driver.Url.Equals("https://www.gojls.com/login?preURL=/branch/myjls/homework"))
                    {
                        debug.makeLog("Login failure");
                        return "failure:2";
                    }
                    debug.makeLog("Getting rid of garbage alert");
                    var ok = driver.FindElementByClassName("swal2-confirm");
                    ok.Click();
                    if (!Setting.LoadDatAtSet)
                    {
                        debug.makeLog("No update data");
                        return "No Data Load When Program Initialize";
                    }
                }
                loaded = true;
                //최신 숙제 받아오기
                return justGet();
            }
            catch(Exception e)
            {
                debug.makeLog("Exception: " + e.Message + "\n" + e.StackTrace);
                return "failure:3";
            }
        }
        public string load(int date)
        {
            try
            {
                debug.makeLog("Loading Howework");
                if(!loaded)
                {
                    if (Secure.Propile == null)
                    {
                        debug.makeLog("Failure: No login data found");
                        return "failure:1";
                    }
                    debug.makeLog("Navigating to target URL");
                    driver.Navigate().GoToUrl("https://www.gojls.com/branch/myjls/homework");
                    debug.makeLog("Finding elements");
                    var id = driver.FindElementById("userId");
                    var pwd = driver.FindElementById("passWd");
                    var go = driver.FindElementByClassName("log");
                    debug.makeLog("Sending keys");
                    id.SendKeys(Secure.Propile.Id);
                    pwd.SendKeys(Secure.Propile.Pwd);
                    go.Click();
                    debug.makeLog("Logged in");
                    debug.makeLog("Checking status");
                    Thread.Sleep(1000);
                    if (driver.Url.Equals("https://www.gojls.com/login?preURL=/branch/myjls/homework"))
                    {
                        debug.makeLog("Login failure");
                        return "failure:2";
                    }
                    debug.makeLog("Getting rid of garbage alert");
                    var ok = driver.FindElementByClassName("swal2-confirm");
                    ok.Click();
                    if (!Setting.LoadDatAtSet)
                    {
                        debug.makeLog("No update data");
                        return "No Data Load When Program Initialize";
                    }
                }
                loaded = true;
                return justGet(date);
            }
            catch(Exception e)
            {
                debug.makeLog("Exception: " + e.Message + "\n" + e.StackTrace);
                return "failure:3";
            }
        }
        public string justGet()
        {
            try {
                debug.makeLog("Getting homework");
                var ne = driver.FindElementByClassName("new");
                string cmdTo = ne.GetAttribute("id");
                string cmdToExe = "studyDate(\'" + cmdTo.Substring(4, 8) + "\')";
                IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
                executor.ExecuteScript(cmdToExe);
                var tst = driver.FindElementById("day_" + cmdTo.Substring(4, 8));
                if(!tst.GetAttribute("class").Contains("on"))
                {
                    return "failure:4";
                }
                var hwPane = driver.FindElementByClassName("oldarea");
                string real = hwPane.GetAttribute("innerHTML");
                debug.makeLog("Saving to DB");
                sav.addHw(Int32.Parse(cmdTo.Substring(4, 8)), real);
                debug.makeLog("All set!");
                currentDate = cmdTo.Substring(4, 8);
                return real;
            }
            catch (Exception e)
            {
                debug.makeLog(e.Message);
                if (e.Message.Equals("javascript error: studyDate is not defined"))
                {
                    return "failure:5";
                }
                else
                {
                    debug.makeLog("Exception: " + e.Message + "\n" + e.StackTrace);
                    return "failiure:3";
                }
            }
        }
        public string justGet(int date)
        {
            try
            {
                debug.makeLog("Getting homework of " + date);
                string cmdToExe = "studyDate(\'" + date + "\')";
                IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
                executor.ExecuteScript(cmdToExe);
                var tst = driver.FindElementById("day_" + date);
                if (!tst.GetAttribute("class").Contains("on"))
                {
                    return "failure:4";
                }
                var hwPane = driver.FindElementByClassName("oldarea");
                string real = hwPane.GetAttribute("innerHTML");
                debug.makeLog("Saving to DB");
                sav.addHw(date, real);
                debug.makeLog("All set!");
                currentDate = date.ToString();
                return real;
            }
            catch(Exception e)
            {
                debug.makeLog(e.Message);
                if (e.Message.Equals("javascript error: studyDate is not defined"))
                {
                    return "failure:5";
                }
                else
                {
                    return "failure:3";
                }
            }
        }
        public string CurrentDate;
        public string langCode;
        public string currentDate
        {
            get
            {
                return CurrentDate;
            }
            set
            {
                string yyyy = value.Substring(0, 4), mm = value.Substring(4, 2), dd = value.Substring(6, 2);
                if (langCode.Equals("ko-KR"))
                {
                    CurrentDate = "이 과제는 " + yyyy + "년 " + mm + "월 " + dd + "일의 과제입니다.";
                }
                else if (langCode.Equals("en-US"))
                {
                    CurrentDate = "This homework is on " + yyyy + "/" + mm + "/" + dd + ".";
                }
            }
        }
        public void close()
        {
            driver.Close();
        }
        public void openOnGui(int date)
        {
            // TODO: 보이는 JLS 과제
        }
    }
}
