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
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ftp = App.FtpController;
            var type = value.GetType();

            if (type == typeof(ObservableCollection<Picture>)) 
            {
                ObservableCollection<Picture> pictures = (ObservableCollection<Picture>)value;
                var sorted = pictures.OrderBy((e) =>
                {
                    return e.Backup != null;
                }).Select<Picture, Progress>((e) =>
                {
                    if (e.Data == null)
                    {
                        e.Data = new Progress();
                    }
                    Progress progress = e.Data as Progress;
                    progress.Cancel = new CancellationToken();
                    progress.Selected = false;
                    progress.Type = FileType.Photo;
                    progress.Url = new Uri(e.Path, UriKind.Relative);
                    progress.Data = e;
                    progress.Preferences = ProgressPreferences.AllowOnWifiAndBatery;

                    this.updateProgress(ftp, progress);
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
                    if (e.Data == null)
                    {
                        e.Data = new Progress();
                    }
                    Progress progress = e.Data as Progress;
                    progress.Cancel = new CancellationToken();
                    progress.Selected = false;
                    progress.Type = FileType.Video;
                    progress.Url = new Uri(e.Path, UriKind.Relative);
                    progress.Data = e;
                    progress.Preferences = ProgressPreferences.AllowOnWifiAndExternalPower;

                    this.updateProgress(ftp, progress);
                    return progress;
                });
                return sorted;
            }
            return null;
        }

        /// <summary>
        /// Convert back
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }


        /// <summary>
        /// Update progress
        /// </summary>
        /// <param name="ftp"></param>
        /// <param name="progress"></param>
        private async void updateProgress(CoPilot.Controller.Ftp ftp, Progress progress)
        {
            //connect
            ftp.Connect(progress);
            //get file size
            await this.getFileSize(progress);
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
                    Double size = (int)Storage.GetSize(progress.Url.OriginalString) / 1048576.0;
                    App.RootFrame.Dispatcher.BeginInvoke(() =>
                    {
                        progress.Size = Math.Round(size, size < 1 ? 3 : 1).ToString();
                    });
                } 
                catch {}
            });
        }
    }
}
