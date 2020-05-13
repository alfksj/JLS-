using System;
using System.Collections.Generic;
using System.Text;

namespace JLS___Library.Data
{
    public class Profile
    {
        private string name, id, pwd;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        public string Pwd
        {
            get
            {
                return pwd;
            }
            set
            {
                Pwd = value;
            }
        }
        public Profile(string name, string id, string pwd)
        {
            if(name.Equals("") || id.Equals("") || pwd.Equals(""))//셋 중 하나라도 입력되지 않았으면 저장 안함
            {
                return;
            }
            this.name = name;
            this.id = id;
            this.pwd = pwd;
        }
    }
}
