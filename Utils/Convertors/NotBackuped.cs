using CoPilot.Core.Data;
using CoPilot.Core.Utils;
using CoPilot.Interfaces;
using CoPilot.Interfaces.Types;
using CoPilot.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoPilot.Utils.Convertors
{
    public class NotBackuped : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value.GetType();

            if (type == typeof(ObservableCollection<Picture>)) 
            {
                ObservableCollection<Picture> pictures = (ObservableCollection<Picture>)value;
                var sorted = pictures.OrderBy((e) =>
                {
                    return e.Backup != null;
                }).Select<Picture, Progress>((e) =>
                {
                    var progress = new Progress();
                    progress.Cancel = new CancellationToken();
                    progress.Selected = false;
                    progress.Type = FileType.Photo;
                    progress.Url = new Uri(e.Path, UriKind.Relative);
                    progress.Data = e;
                    this.getFileSize(progress);
                    return progress;
                });
                return sorted;
            }

            if (type == typeof(ObservableCollection<Video>))
            {
                ObservableCollection<Video> videos = (ObservableCollection<Video>)value;
                var sorted = videos.OrderBy((e) =>
                {
                    return e.VideoBackup != null;
                }).Select<Video, Progress>((e) =>
                {
                    var progress = new Progress();
                    progress.Cancel = new CancellationToken();
                    progress.Selected = false;
                    progress.Type = FileType.Video;
                    progress.Url = new Uri(e.Path, UriKind.Relative);
                    progress.Data = e;
                    this.getFileSize(progress);
                    return progress;
                });
                return sorted;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        /// <summary>
        /// Get file size
        /// </summary>
        /// <param name="progress"></param>
        private async Task getFileSize(Progress progress)
        {
            if (!string.IsNullOrEmpty(progress.Size))
            {
                return;
            }

            Random r = new Random();

            //unknown
            progress.Size = AppResources.UnknownSize;

            await Task.Delay(r.Next(20, 200));
            await Task.Run(() =>
            {
                //check size
                try 
                {
                    IsolatedStorageFileStream file = Storage.OpenFile(progress.Url.OriginalString, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                    Double size = (int)file.Length / 1048576.0;
                    file.Dispose();
                    App.RootFrame.Dispatcher.BeginInvoke(() =>
                    {
                        progress.Size = Math.Round(size, size < 1 ? 3 : 1) + " MB";
                    });
                } 
                catch {}
            });
        }
    }
}
