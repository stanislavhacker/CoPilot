using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot.Utils
{
    public static class Settings
    {
        private static IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;


        /// <summary>
        /// Get settings
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String Get(string key)
        {
            if (settings.Contains(key))
            {
                return settings[key].ToString();
            }
            return null;
        }

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void Add(string key, string data)
        {
            if (settings.Contains(key))
            {
                settings[key] = data;
            }
            else
            {
                settings.Add(key, data);
            }
            Save();
        }

        /// <summary>
        /// Remove key
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            if (settings.Contains(key))
            {
                settings.Remove(key);
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        public static void Save()
        {
            settings.Save();
        }
    }
}
