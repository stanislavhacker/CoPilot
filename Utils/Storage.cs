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
        private static String lastFreeSpaceString = "";
        private static long lastFreeSpace = 0;
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
        /// Delete file
        /// </summary>
        /// <param name="path"></param>
        public static Boolean DeleteFile(string path)
        {
            if (FileExists(path)) {
                try
                {
                    storage.DeleteFile(path);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// File exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Boolean FileExists(string path)
        {
            return path != null && storage.FileExists(path);
        }

        /// <summary>
        /// Get storage
        /// </summary>
        /// <returns></returns>
        public static IsolatedStorageFile Get()
        {
            return storage;
        }

        /// <summary>
        /// Get free space size
        /// </summary>
        /// <returns></returns>
        public static String GetAvailableSpace()
        {
            //cached
            long len = storage.AvailableFreeSpace;
            if (len == lastFreeSpace) 
            {
                return lastFreeSpaceString;
            }

            //return new size
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            lastFreeSpace = len;
            lastFreeSpaceString = String.Format("{0:0.##} {1}", len, sizes[order]);

            return lastFreeSpaceString;
        }

        /// <summary>
        /// Get file size
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static long GetSize(string path) 
        {
            var stream = storage.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var length = stream.Length;
            stream.Close();
            return length;
        }
    }
}
