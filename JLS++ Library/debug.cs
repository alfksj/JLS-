using System;
using System.Collections.Generic;
using System.Text;

namespace JLS___Library
{
    public class debug
    {
        public static void makeLog(string msg)
        {
            var dat = DateTime.Now.ToString("[yyyy-MM-dd, HH:mm:ss.ffff] : ");
            Console.WriteLine(dat + msg);
        }
    }
}
