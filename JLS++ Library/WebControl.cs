using JLS___Library.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
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
        }
        public string load()
        {
            try
            {
                debug.makeLog("Loading Howework");
                if (Secure.Propile == null)
                {
                    debug.makeLog("Failure: No login data found");
                    return "<font size=\"4\" color=\"red\"><b>프로필이 설정되어 있지 않습니다.</b><br />프로필을 설정해주세요.</font>";
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
                    return "<font size=\"4\" color=\"red\"><b>로그인할 수 없습니다.</b><br />ID와 비밀번호가 정확한지 확인해 주세요.<br />서버 지연이 너무 심하면 이런 오류가 뜰 수 도 있습니다.</font>";
                }
                debug.makeLog("Getting rid of garbage alert");
                var ok = driver.FindElementByClassName("swal2-confirm");
                ok.Click();
                if (!Setting.LoadDatAtSet)
                {
                    debug.makeLog("No update data");
                    return "No Data Load When Program Initialize";
                }
                //최신 숙제 받아오기
                return justGet();
            }
            catch(Exception e)
            {
                debug.makeLog("Exception: " + e.Message);
                return "<font size=\"4\" color=\"red\"><b>숙제를 확인할 수 없습니다.</b><br />이 문제의 원인은 다양합니다. 일시적으로 JLS서버를 이용할 수 없는 것일 수 있고 지연시간이 너무 심한것 일 수 도 있으며 잘못된 날짜를 입력한 것 일 수 도 있습니다.</font>";
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
                    return "<font size=\"4\"><b>지정한 날짜에 과제가 없습니다.</b><br /> 날짜를 확인하세요</font>";
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
                    return "<font size=\"4\" color=\"red\"><b>숙제를 확인할 수 없습니다.</b><br />해당 날짜에 대한 과제를 찾을 수 없습니다.</font>";
                }
                else
                {
                    return "<font size=\"4\" color=\"red\"><b>숙제를 확인할 수 없습니다.</b><br />이 문제의 원인은 다양합니다. 일시적으로 JLS서버를 이용할 수 없는 것일 수 있고 지연시간이 너무 심한것 일 수 도 있으며 잘못된 날짜를 입력한 것 일 수 도 있습니다.</font>";
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
                    return "<font size=\"4\"><b>지정한 날짜에 과제가 없습니다.</b><br /> 날짜를 확인하세요</font>";
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
                    return "<font size=\"4\" color=\"red\"><b>숙제를 확인할 수 없습니다.</b><br />해당 날짜에 대한 과제를 찾을 수 없습니다.</font>";
                }
                else
                {
                    return "<font size=\"4\" color=\"red\"><b>숙제를 확인할 수 없습니다.</b><br />이 문제의 원인은 다양합니다. 일시적으로 JLS서버를 이용할 수 없는 것일 수 있고 지연시간이 너무 심한것 일 수 도 있으며 잘못된 날짜를 입력한 것 일 수 도 있습니다.</font>";
                }
            }
        }
        public string CurrentDate;
        public string currentDate
        {
            get
            {
                return CurrentDate;
            }
            set
            {
                string yyyy = value.Substring(0, 4), mm = value.Substring(4, 2), dd = value.Substring(6, 2);
                CurrentDate = "이 과제는 " + yyyy + "년 " + mm + "월 " + dd + "일의 과제입니다.";
            }
        }
        public void close()
        {
            driver.Close();
        }
    }
}
