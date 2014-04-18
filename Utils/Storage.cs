using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot.Utils
{
    public static class Storage
    {
        private static IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

        /// <summary>
        /// Create file
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IsolatedStorageFileStream CreateFile(string name)
        {
            return storage.CreateFile(name);
        }

        /// <summary>
        /// Open file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileMode"></param>
        /// <param name="fileAccess"></param>
        /// <param name="fileShare"></param>
        /// <returns></returns>
        public static IsolatedStorageFileStream OpenFile(string path, FileMode fileMode, FileAccess fileAccess = FileAccess.ReadWrite, FileShare fileShare = FileShare.Read)
        {
            return storage.OpenFile(path, fileMode, fileAccess, fileShare);
        }

        /// <summary>
        /// File exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Boolean FileExists(string path)
        {
            return storage.FileExists(path);
        }

        /// <summary>
        /// Get storage
        /// </summary>
        /// <returns></returns>
        public static IsolatedStorageFile Get()
        {
            return storage;
        }
    }
}
