using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text;

namespace JLS___Library.Data
{
    /// <summary>
    /// 프로필 관리 및 보안 유지 등
    /// 보안과 관련된 작업을 진행합니다.
    /// </summary>
    public class Secure
    {
        private static Profile propile;
        //로딩된 프로필 갯수
        private static int nop;
        public static int Nop
        {
            get
            {
                return nop;
            }
            set
            {
                nop = value;
            }
        }
        public static Profile Propile
        {
            get
            {
                return propile;
            }
        }
        public static void setProfile(string name, string id, string pwd)
        {
            propile = new Profile(name, id, pwd);
            nop++;
        }
    }
}
