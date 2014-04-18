using CoPilot.Core.Data;
using CoPilot.Core.Utils;
using CoPilot.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
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
                var sorted = pictures.Select<Picture, MediaWithProgress>((e) =>
                {
                    var progress = new MediaWithProgress();
                    progress.Picture = e;
                    progress.Progress = new ProgressUpdater();
                    this.getFileSize(progress);
                    return progress;
                }).OrderBy((e) =>
                {
                    return e.Picture.Backups.Count;
                });
                return sorted;
            }

            if (type == typeof(ObservableCollection<Video>))
            {
                ObservableCollection<Video> videos = (ObservableCollection<Video>)value;
                var sorted = videos.Select<Video, MediaWithProgress>((e) =>
                {
                    var progress = new MediaWithProgress();
                    progress.Video = e;
                    progress.Progress = new ProgressUpdater();
                    this.getFileSize(progress);
                    return progress;
                }).OrderBy((e) =>
                {
                    return e.Video.VideoBackups.Count;
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
        private async Task getFileSize(MediaWithProgress progress)
        {
            if (progress.Size != null)
            {
                return;
            }

            await Task.Run(() =>
            {
                progress.Size = AppResources.UnknownSize;

                IsolatedStorageFileStream file = null;
                if (progress.Picture != null)
                {
                    file = Storage.OpenFile(progress.Picture.Path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                }
                if (progress.Video != null)
                {
                    file = Storage.OpenFile(progress.Video.Path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                }

                Double size = (int)file.Length / 1048576.0;
                progress.Size = Math.Round(size, size < 1 ? 3 : 1) + " MB";
                file.Dispose();
            });
        }
    }
}
