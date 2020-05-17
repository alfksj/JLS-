using System.IO;
using Newtonsoft.Json.Linq;

namespace JLS___Library
{
    public class Setting
    {
        private static string ST_PATH = System.Environment.GetEnvironmentVariable("appdata") + "/.JLS++/setting.json";
        private static bool loadCache, loadDatAtSet;
        private static string langCode;
        public static bool LoadCache
        {
            get
            {
                return loadCache;
            }
            set
            {
                loadCache = value;
            }
        }
        public static bool LoadDatAtSet
        {
            get
            {
                return loadDatAtSet;
            }
            set
            {
                loadDatAtSet = value;
            }
        }
        public static string LangCode
        {
            get
            {
                return langCode;
            }
            set
            {
                langCode = value;
            }
        }
        public static void load()
        {
            string jsn = File.ReadAllText(ST_PATH);
            JObject root = JObject.Parse(jsn);
            LoadCache = (bool)root.SelectToken("rather_cache");
            LoadDatAtSet = (bool)root.SelectToken("loadWhenStart");
            LangCode = (string)root.SelectToken("Language");
        }
        public static void save()
        {
            JObject root = new JObject();
            root.Add("rather_cache", LoadCache);
            root.Add("loadWhenStart", LoadDatAtSet);
            root.Add("Language", LangCode);
            File.WriteAllText(ST_PATH, root.ToString());
        }
    }
}
