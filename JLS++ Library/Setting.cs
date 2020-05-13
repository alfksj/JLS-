using System.IO;
using Newtonsoft.Json.Linq;

namespace JLS___Library
{
    public class Setting
    {
        private static string ST_PATH = System.Environment.GetEnvironmentVariable("appdata") + "/.JLS++/setting.json";
        private static bool loadCache, loadDatAtSet;
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
        public static void load()
        {
            string jsn = File.ReadAllText(ST_PATH);
            JObject root = JObject.Parse(jsn);
            LoadCache = (bool)root.SelectToken("rather_cache");
            LoadDatAtSet = (bool)root.SelectToken("loadWhenStart");
        }
        public static void save()
        {
            JObject root = new JObject();
            root.Add("rather_cache", LoadCache);
            root.Add("loadWhenStart", LoadDatAtSet);
            File.WriteAllText(ST_PATH, root.ToString());
        }
    }
}
