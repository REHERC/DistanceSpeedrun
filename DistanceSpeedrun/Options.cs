using Spectrum.API.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable IDE0044 
namespace DistanceSpeedrun
{
    static class Options
    {
        private static Settings Data;

        public static void SetupDefaults()
        {
            Data = new Settings("Config");
            Dictionary<string, object> DefaultSettings = new Dictionary<string, object>()
            {
                {"Enabled", false}
            };
            foreach (KeyValuePair<string, object> item in DefaultSettings)
                if (!Data.ContainsKey<string>(item.Key))
                    Data[item.Key] = item.Value;
            Data.Save();
        }

        public static void Set(string key, object value)
        {
            Data[key] = value;
            Data.Save();
        }

        public static object Get(string key)
        {
            if (Data.ContainsKey(key))
                return Data[key];
            return null;
        }

        public static T Get<T>(string key)
        {
            return (T)Get(key);
        }
    }
}
