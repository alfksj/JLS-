using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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
        private static string PRIVATE_KEY = "2i8fqhgwq80hgwQHDW90whg90whgh08HSGwhgWGEDSweg42wjg0830fh008h3g";
        public static string Private_Key
        {
            get
            {
                return Hash(SHA256, PRIVATE_KEY);
            }
        }
        public static void setProfile(string name, string id, string pwd)
        {
            propile = new Profile(name, id, pwd);
            nop++;
        }
        public static string AES256Encrypt(string planeText)
        {
            string key = Private_Key;
            byte[] plane = Encoding.UTF8.GetBytes(planeText);
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            var salt = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(key.Length.ToString()));
            var PBKDF2Key = new Rfc2898DeriveBytes(key, salt, 50000);//50000번 돌리는 보일러
            var secretKey = PBKDF2Key.GetBytes(aes.KeySize / 8);
            var iv = PBKDF2Key.GetBytes(aes.BlockSize / 8);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(secretKey, iv), CryptoStreamMode.Write))
                {
                    cs.Write(plane, 0, plane.Length);
                }
                xBuff = ms.ToArray();
            }
            return Convert.ToBase64String(xBuff);
        }
        public static string AES256Decrypt(string encrypted)
        {
            string key = Private_Key;
            byte[] plane = Convert.FromBase64String(encrypted);
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            var salt = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(key.Length.ToString()));
            var PBKDF2Key = new Rfc2898DeriveBytes(key, salt, 50000);
            var secretKey = PBKDF2Key.GetBytes(aes.KeySize / 8);
            var iv = PBKDF2Key.GetBytes(aes.BlockSize / 8);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(secretKey, iv), CryptoStreamMode.Write))
                {
                    cs.Write(plane, 0, plane.Length);
                }
                xBuff = ms.ToArray();
            }
            return Encoding.UTF8.GetString(xBuff);
        }
        public const int SHA128 = -1, SHA256 = 0, SHA384 = 1, SHA512 = 2;
        public static string Hash(int algorithm, string planeText)
        {
            HashAlgorithm hash;
            switch(algorithm)
            {
                case SHA128:
                    hash = new SHA1CryptoServiceProvider();
                    break;
                case SHA256:
                    hash = new SHA256CryptoServiceProvider();
                    break;
                case SHA384:
                    hash = new SHA384CryptoServiceProvider();
                    break;
                case SHA512:
                    hash = new SHA512CryptoServiceProvider();
                    break;
                default:
                    throw new NoSuchAlgorithmException(planeText + ": Not Found");
            }
            Encoding encoding = Encoding.UTF8;
            byte[] plane = encoding.GetBytes(planeText);
            var sBuilder = new StringBuilder();
            byte[] hashed = hash.ComputeHash(plane);
            for (int i = 0; i < hashed.Length; i++)
            {
                sBuilder.Append(hashed[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        private class NoSuchAlgorithmException : Exception
        {
            public NoSuchAlgorithmException(string msg) : base(msg) {}
        }
        public static string randomText(int len)
        {
            char[] Table = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
                'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
                'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            StringBuilder stb = new StringBuilder();
            var rand = new Random();
            for(int i=0;i<len;i++)
            {
                stb.Append(Table[rand.Next(0, 61)]);
            }
            return stb.ToString();
        }
    }
}
