using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CoPilot.Utils.Dependencies
{
    public static class MediaPlayerDependency
    {
        private static IsolatedStorageFileStream stream;

        /// <summary>
        /// Is playing property
        /// </summary>
        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.RegisterAttached("IsPlaying", typeof(bool), typeof(MediaPlayerDependency), new PropertyMetadata(onIsPlayingPropertyChange));
        private static void onIsPlayingPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;
            MediaElement player = (MediaElement)uie;
            bool isPlaying = (bool)e.NewValue;

            if (isPlaying)
            {
                if (player.Source == null)
                {
                    var videoPath = player.GetValue(VideoPathProperty) as String;
                    try
                    {
                        stream = Storage.OpenFile(videoPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Write);
                        player.SetSource(stream);
                    }
                    catch
                    {
                        FromUrl(player, videoPath);
                    }
                }

                player.MediaEnded += MediaEnded;
                player.Play();
            }
            else
            {
                player.Pause();
            }
        }

        /// <summary>
        /// VideoPath property
        /// </summary>
        public static readonly DependencyProperty VideoPathProperty = DependencyProperty.RegisterAttached("VideoPath", typeof(string), typeof(MediaPlayerDependency), new PropertyMetadata(onVideoPathPropertyChange));
        private static void onVideoPathPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;
            MediaElement player = (MediaElement)uie;
            string videoPath = (string)e.NewValue;

            if (e.NewValue != e.OldValue)
            {
                try
                {
                    stream = Storage.OpenFile(videoPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Write);
                    player.SetSource(stream);
                }
                catch
                {
                    FromUrl(player, videoPath);
                }
            }
        }

        /// <summary>
        /// IsOpen property
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.RegisterAttached("IsOpen", typeof(bool), typeof(MediaPlayerDependency), new PropertyMetadata(onIsOpenPropertyChange));
        private static void onIsOpenPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;
            MediaElement player = (MediaElement)uie;
            bool isOpen = (bool)e.NewValue;

            if (e.NewValue != e.OldValue && !isOpen)
            {
                player.Stop();
                player.MediaEnded -= MediaEnded;
                player.Source = null;
                stream.Close();
            }
        }

        /// <summary>
        /// From Url
        /// </summary>
        /// <param name="player"></param>
        /// <param name="videoPath"></param>
        private static void FromUrl(MediaElement player, string videoPath)
        {
            Uri url;
            if (Uri.TryCreate(videoPath, UriKind.Absolute, out url))
            {
                player.Source = url;
            }
            else
            {
                player.Source = null;
            }
        }


        #region Getters and Setters

        public static bool GetIsPlaying(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPlayingProperty);
        }
        public static void SetIsPlaying(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPlayingProperty, value);
        }

        public static string GetVideoPath(DependencyObject obj)
        {
            return (string)obj.GetValue(VideoPathProperty);
        }
        public static void SetVideoPath(DependencyObject obj, string value)
        {
            obj.SetValue(VideoPathProperty, value);
        }

        public static bool GetIsOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOpenProperty);
        }
        public static void SetIsOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOpenProperty, value);
        }

        #endregion

        /// <summary>
        /// Media ended
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement player = (MediaElement)sender;
            player.Stop();
            MediaPlayerDependency.SetIsPlaying(player, false);
        }
    }
}
