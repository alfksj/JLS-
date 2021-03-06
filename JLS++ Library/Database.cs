﻿using System.IO;
using System.Data.SQLite;
using System;
using System.Collections;
using System.Text;

namespace JLS___Library.Data
{
    // TODO: 프로필 정보를 수정하고 저장하는 경우 PRIMARY KEY가 1씩 늘어남(???)
    public class Database
    {
        private string DB_ROOT = Environment.GetEnvironmentVariable("appdata") + "/.JLS++/data.db";
        private string HISTORY_ROOT = Environment.GetEnvironmentVariable("appdata") + "/.JLS++/homeworks.db";
        private SQLiteConnection con, hw;
        /// <summary>
        /// 데이터베이스를 초기화합니다.
        /// </summary>
        public Database()
        {
            if (!File.Exists(DB_ROOT) || !File.Exists(HISTORY_ROOT))
            {
                Directory.CreateDirectory(Environment.GetEnvironmentVariable("appdata") + "/.JLS++");
                DirectoryInfo rootDir = new DirectoryInfo(Environment.GetEnvironmentVariable("appdata") + "/.JLS++");
                if ((rootDir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    rootDir.Attributes = rootDir.Attributes | FileAttributes.Hidden;
                }
                SQLiteConnection.CreateFile(DB_ROOT);
                con = new SQLiteConnection("Data Source = " + DB_ROOT + "; Version = 3;");
                SQLiteConnection.CreateFile(HISTORY_ROOT);
                hw = new SQLiteConnection("Data Source = " + HISTORY_ROOT + "; Version = 3;");
                mkTable("profile", "key INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                    "name TEXT," +
                    "id TEXT," +
                    "password TEXT," +
                    "usCache BOOL," +
                    "callCache BOOL");
                mkTable("browser", "key INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                    "usr_agent TEXT," +
                    "fake_plugin BOOL," +
                    "use_window BOOL," +
                    "gpu_acc BOOL," +
                    "lang TEXT");
                mkTableOfHw("hw", "key INTEGER PRIMARY KEY NOT NULL," +
                    "content TEXT NOT NULL," +
                    "get INTEGER NOT NULL");
                exeCommand("insert into browser (usr_agent, fake_plugin, use_window, gpu_acc, lang) values " +
                    "(\'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/60.0.3112.50 Safari/537.36\'," +
                    "false, false, true, \'ko-KR\')");
                File.WriteAllText(System.Environment.GetEnvironmentVariable("appdata") + "/.JLS++/setting.json", "{\n" +
                    "\"rather_cache\":true,\n" +
                    "\"loadWhenStart\":true\n" +
                    "}");
            }
            else
            {
                con = new SQLiteConnection("Data Source = " + DB_ROOT + "; Version = 3;");
                hw = new SQLiteConnection("Data Source = " + HISTORY_ROOT + "; Version=3;");
            }
            con.Close();
            hw.Close();
        }
        /// <summary>
        /// SQLite에서 NonQuery 명령을 실행합니다.
        /// </summary>
        /// <param name="con">SQLite 연결 객체입니다.</param>
        /// <param name="command">실행할 명령입니다.</param>
        /// <returns>실행 결과를 반환합니다.</returns>
        private int exeCommand(string command)
        {
            // TODO: 어디선가 열곤 안닫음(처음 DB생성 부분)
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand(command, con);
            int returm = cmd.ExecuteNonQuery();
            con.Close();
            return returm;
        }
        /// <summary>
        /// HW db에 대한 명령 수행을 담당합니다.
        /// ...ㅋ
        /// </summary>
        /// <param name="command"></param>
        /// <returns>실행 결과를 반환합니다.</returns>
        private int exeCommandOfHw(string command)
        {
            hw.Open();
            SQLiteCommand cmd = new SQLiteCommand(command, hw);
            int returm = cmd.ExecuteNonQuery();
            hw.Close();
            return returm;
        }
        /// <summary>
        /// SQLite에서 NonQuery 명령을 실행합니다.
        /// 이때 SQLite를 열지 않습니다.
        /// </summary>
        /// <param name="con">SQLite 연결 객체입니다.</param>
        /// <param name="command">실행할 명령입니다.</param>
        /// <returns>실행 결과를 반환합니다.</returns>
        private int exeCommandWithoutOpen(string command)
        {
            SQLiteCommand cmd = new SQLiteCommand(command, con);
            int returm = cmd.ExecuteNonQuery();
            return returm;
        }
        /// <summary>
        /// SQLite에서 NonQuery 명령을 실행합니다.
        /// 이때 SQLite를 열지 않습니다.
        /// 업계포상 ㅗㅜㅑ
        /// </summary>
        /// <param name="con">SQLite 연결 객체입니다.</param>
        /// <param name="command">실행할 명령입니다.</param>
        /// <returns>실행 결과를 반환합니다.</returns>
        private int exeCommandWithoutOpenOfHw(string command)
        {
            SQLiteCommand cmd = new SQLiteCommand(command, hw);
            int returm = cmd.ExecuteNonQuery();
            return returm;
        }
        /// <summary>
        /// 테이블을 생성합니다.
        /// </summary>
        /// <param name="name">테이블의 이름입니다.</param>
        /// <param name="config">테이블의 속성(컬럼의 정보)입니다.</param>
        /// <returns>실행 결과를 반환합니다.</returns>
        public int mkTable(string name, string config)
        {
            return exeCommand("create table " + name + " (" + config + ")");
        }
        public int mkTableOfHw(string name, string config)
        {
            return exeCommandOfHw("create table " + name + " (" + config + ")");
        }
        /// <summary>
        /// Database를 모두 로딩해서 적용합니다.
        /// </summary>
        public void loadAll()
        {
            Secure.Nop = 0;
            con.Open();
            string cmds = "select * from profile";
            SQLiteCommand cmd = new SQLiteCommand(cmds, con);
            SQLiteDataReader dat = cmd.ExecuteReader();
            while (dat.Read())
            {
                Secure.setProfile(dat["name"].ToString(), Secure.AES256Decrypt(dat["id"].ToString()), Secure.AES256Decrypt(dat["password"].ToString()));
            }
            Setting.load();
            dat.Close();
            cmds = "select * from browser";
            cmd = new SQLiteCommand(cmds, con);
            SQLiteDataReader dat2 = cmd.ExecuteReader();
            while(dat2.Read())
            {
                WebControl.usr_agent = dat2["usr_agent"].ToString();
                WebControl.fake_plugin = (bool)dat2["fake_plugin"];
                WebControl.gpu_acc = (bool)dat2["gpu_acc"];
                WebControl.lang = dat2["lang"].ToString();
                WebControl.use_win = (bool)dat2["use_window"];
            }
            dat2.Close();
            con.Close();
        }
        /// <summary>
        /// Database를 모두 저장합니다.
        /// </summary>
        public void saveAll(Profile prof)
        {
            con.Open();
            if (!(prof.Name == null || prof.Id == null || prof.Pwd == null))
            {
                string cmds = "select * from profile";
                bool flag = true;
                SQLiteCommand cmd = new SQLiteCommand(cmds, con);
                SQLiteDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    exeCommandWithoutOpen("update profile set name=\'" + prof.Name + "\', id=\'" + Secure.AES256Encrypt(prof.Id)
                        + "\', password=\'" + Secure.AES256Encrypt(prof.Pwd) + "\' where key="+read["key"]);
                    exeCommandWithoutOpen("update browser set usr_agent=\'" + WebControl.usr_agent + "\', fake_plugin=" + WebControl.fake_plugin
                        + ", use_window=" + WebControl.use_win + ", gpu_acc=" + WebControl.gpu_acc + ", lang=\'" + WebControl.lang
                        + "\' where key="+read["key"]);
                    flag = false;
                }
                read.Close();
                if (flag)
                {
                    exeCommandWithoutOpen("insert into profile (name, id, password) values (\'" + prof.Name + "\', \'" + Secure.AES256Encrypt(prof.Id)
                        + "\', \'" + Secure.AES256Encrypt(prof.Pwd) + "\')");
                    exeCommandWithoutOpen("update browser set usr_agent=\'" + WebControl.usr_agent + "\', fake_plugin=" + WebControl.fake_plugin
                        + ", use_window=" + WebControl.use_win + ", gpu_acc=" + WebControl.gpu_acc + ", lang=\'" + WebControl.lang
                        + "\' where key=1");
                }
            }
            con.Close();
            Setting.save();
        }
        public void addHw(int date, string content)
        {
            int today = Int32.Parse(DateTime.Now.ToString("yyyyMMdd"));
            hw.Open();
            string cmds = "select key from hw";
            SQLiteCommand cmd = new SQLiteCommand(cmds, hw);
            SQLiteDataReader read = cmd.ExecuteReader();
            while (read.Read())
            {
                if((Int64)read["key"] == date)
                {
                    exeCommandWithoutOpenOfHw("update hw set content=\'" + preProcess(content) + "\', get=" + today + ", key=" + date + " where key=" + date) ;
                    hw.Close();
                    return;
                }
            }
            read.Close();
            exeCommandWithoutOpenOfHw("insert into hw (key, content, get) values (" + date + ", \'" + preProcess(content) + "\', " + today + ")");
            hw.Close();
        }
        public string getHw(int date)
        {
            hw.Open();
            string cmds = "select * from hw";
            SQLiteCommand cmd = new SQLiteCommand(cmds, hw);
            SQLiteDataReader read = cmd.ExecuteReader();
            currentDate = date.ToString();
            while (read.Read())
            {
                if((Int64)read["key"] == date)
                {
                    string s = read["content"].ToString();
                    read.Close();
                    hw.Close();
                    return s;
                }
            }
            read.Close();
            hw.Close();
            return "NO CACHE DATA FOUND";
        }
        public string getLatestHw()
        {
            debug.makeLog("Checking cache");
            hw.Open();
            string cmds = "select * from hw";
            SQLiteCommand cmd = new SQLiteCommand(cmds, hw);
            SQLiteDataReader readx = cmd.ExecuteReader();
            long DateOfLast = 10000000;
            string ContentOfLast = "NO CACHE DATA FOUND";
            while (readx.Read())
            {
                if((Int64)readx["key"] > DateOfLast)
                {
                    ContentOfLast = readx["content"].ToString();
                    DateOfLast = (Int64)readx["key"];
                }
            }
            currentDate = DateOfLast.ToString();
            readx.Close();
            hw.Close();
            debug.makeLog("Cache returned");
            return ContentOfLast;
        }
        public string langCode;
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
                if (langCode.Equals("ko-KR"))
                {
                    CurrentDate = "이 과제는 " + yyyy + "년 " + mm + "월 " + dd + "일의 과제입니다.";
                }
                else if(langCode.Equals("en-US"))
                {
                    CurrentDate = "This homework is on " + yyyy + "/" + mm + "/" + dd + ".";
                }
            }
        }
        public string preProcess(string content)
        {
            StringBuilder hw = new StringBuilder();
            char[] s = content.ToCharArray();
            foreach(char c in s)
            {
                switch(c)
                {
                    case '\'':
                        hw.Append("\'\'");
                        break;
                    default:
                        hw.Append(c);
                        break;
                }
            }
            return hw.ToString();
        }
        /// <summary>
        /// db를 완전히 지웁니다.
        /// </summary>
        public void suicide()
        {
            exeCommand("delete from profile");
            exeCommand("delete from browser");
            exeCommand("insert into browser (usr_agent, fake_plugin, use_window, gpu_acc, lang) values " +
                    "(\'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/60.0.3112.50 Safari/537.36\'," +
                    "false, false, true, \'ko-KR\')");
            suicideOfHw();
        }
        public void suicideOfHw()
        {
            exeCommandOfHw("delete from hw");
        }
    }
}
