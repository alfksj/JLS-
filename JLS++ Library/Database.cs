using System.IO;
using System.Data.SQLite;

namespace JLS___Library.Data
{
    public class Database
    {
        private string DB_ROOT = System.Environment.GetEnvironmentVariable("appdata") + "/.JLS++/data.db";
        private SQLiteConnection con;
        /// <summary>
        /// 데이터베이스를 초기화합니다.
        /// </summary>
        public Database()
        {
            if(!File.Exists(DB_ROOT))
            {
                Directory.CreateDirectory(System.Environment.GetEnvironmentVariable("appdata") + "/.JLS++");
                DirectoryInfo rootDir = new DirectoryInfo(System.Environment.GetEnvironmentVariable("appdata") + "/.JLS++");
                if((rootDir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    rootDir.Attributes = rootDir.Attributes | FileAttributes.Hidden;
                }
                SQLiteConnection.CreateFile(DB_ROOT);
                con = new SQLiteConnection("Data Source = " + DB_ROOT + "; Version = 3;");
                mkTable("profile", "key INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                    "name TEXT NOT NULL," +
                    "id TEXT NOT NULL," +
                    "password TEXT NOT NULL");
                mkTable("browser", "key INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                    "usr_agent TEXT NOT NULL," +
                    "fake_plugin BOOL NOT NULL," +
                    "use_window BOOL NOT NULL," +
                    "gpu_acc BOOL NOT NULL," +
                    "lang TEXT NOT NULL");
            }
            else
            {
                con = new SQLiteConnection("Data Source = "+DB_ROOT+"; Version = 3;");
            }
        }
        /// <summary>
        /// SQLite에서 NonQuery 명령을 실행합니다.
        /// </summary>
        /// <param name="con">SQLite 연결 객체입니다.</param>
        /// <param name="command">실행할 명령입니다.</param>
        /// <returns>실행 결과를 반환합니다.</returns>
        private int exeCommand(string command)
        {
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand(command, con);
            int returm = cmd.ExecuteNonQuery();
            con.Close();
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
        /// <summary>
        /// Database를 모두 로딩해서 적용합니다.
        /// </summary>
        public void loadAll()
        {
            con.Open();
            string cmds = "select * from profile";
            SQLiteCommand cmd = new SQLiteCommand(cmds, con);
            SQLiteDataReader dat = cmd.ExecuteReader();
            while (dat.Read())
            {
                Secure.setProfile(dat["name"].ToString(), dat["id"].ToString(), dat["pwd"].ToString());
            }
            dat.Close();
            con.Close();
        }
        /// <summary>
        /// Database를 모두 저장합니다.
        /// </summary>
        public void saveAll(Profile prof)
        {
            con.Open();
            string cmds = "select * from profile";
            SQLiteCommand cmd = new SQLiteCommand(cmds, con);
            SQLiteDataReader read = cmd.ExecuteReader();
            while (read.Read())
            {
                exeCommand("update profile set name=\'" + prof.Name + "\', id=\'" + prof.Id + "\', pwd=\'" + prof.Pwd + "\' where key=0");
                read.Close();
                return;
            }
            read.Close();
            con.Close();
            exeCommand("insert into profile (name, id, password) values (\'" + prof.Name + "\', \'" + prof.Id + "\', \'" + prof.Pwd + "\')");
        }
    }
}
