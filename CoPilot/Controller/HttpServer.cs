using CoPilot.Core.Api;
using CoPilot.Resources;
using IDCT;
using Microsoft.Phone.Net.NetworkInformation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.Networking.Connectivity;

namespace CoPilot.CoPilot.Controller
{
    public class HttpServer : Base
    {
        #region PROPERTY

        /// <summary>
        /// Is Ip adress
        /// </summary>
        private Boolean isIPAddress;
        public Boolean IsIPAddress
        {
            get
            {
                return isIPAddress;
            }
            set
            {
                isIPAddress = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Url
        /// </summary>
        public String Url
        {
            get
            {
                if (!IsIPAddress)
                {
                    return "";
                }
                return "http://" + this.IPAddress.ToString() + "/copilot";
            }
        }

        /// <summary>
        /// Ip adress
        /// </summary>
        private IPAddress iPAddress;
        public IPAddress IPAddress
        {
            get
            {
                return iPAddress;
            }
            set
            {
                iPAddress = value;

                if (value != null)
                {
                    IsIPAddress = true;
                    this.createServer();
                }
                else
                {
                    IsIPAddress = false;
                }
                RaisePropertyChanged();
                RaisePropertyChanged("Url");
            }
        }

        #endregion

        #region PRIVATE

        private Boolean isServer = false;
        private Skin skinData;

        #endregion

        /// <summary>
        /// Initialize HttpServer class
        /// </summary>
        public HttpServer()
        {
        }

        /// <summary>
        /// Get device ip
        /// </summary>
        /// <returns></returns>
        public void IdentifyDeviceIp()
        {
            var hostnames = NetworkInformation.GetHostNames();

            List<string> adresses = new List<string>();
            foreach (var hostname in hostnames)
            {
                if (hostname.IPInformation != null)
                {
                    adresses.Add(hostname.DisplayName);
                }
            }
            if (adresses.Count == 0 || !DeviceNetworkInformation.IsWiFiEnabled)
            {
                this.IPAddress = null;
            }
            else
            {
                this.IPAddress = IPAddress.Parse(adresses[0]);
            } 
        }

        /// <summary>
        /// Resolve data
        /// </summary>
        public void ResolveData()
        {
            //data
            App.Current.RootVisual.Dispatcher.BeginInvoke(() =>
            {
                this.skinData = new Skin();
                this.skinData.Background = getColor("PhoneChromeBrush");
                this.skinData.Foreground = getColor("PhoneAccentBrush");
            });
        }

        #region PRIVATE

        /// <summary>
        /// Create server
        /// </summary>
        private void createServer()
        {
            if (isServer) {
                return;
            }
            isServer = true;

            Dictionary<Regex, RuleDeletage> rules = new Dictionary<Regex, RuleDeletage>();
            rules.Add(new Regex("^/copilot/api/.*$"), (e) =>
            {
                var request = e.uri.Replace("/copilot/api/", "");

                switch(request) {
                    case "skin":
                        return this.skin();
                    case "language":
                        return this.language();
                    default:
                        return new IDCT.webResposne();
                }
            });
            rules.Add(new Regex("^/copilot/.*$"), (e) =>
            {
                var file = e.uri.Replace("/copilot/", "");
                return loadFile(file.Length == 0 ? "index.html" : file);
            });
            rules.Add(new Regex("^/copilot"), (e) =>
            {
                return loadFile("index.html");
            });

            WebServer myWebServer = new WebServer(rules, IPAddress.ToString(), "80");
        }

        /// <summary>
        /// Language
        /// </summary>
        /// <returns></returns>
        private webResposne language()
        {
            var test = AppResources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, false);

            //json
            var json = JsonConvert.SerializeObject(test);
            var stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            //response
            var response = new IDCT.webResposne();
            response.content = stream;
            response.header = new Dictionary<string, string>();
            response.header.Add("Content-Type", "text/json");

            return response;
        }

        /// <summary>
        /// Get skin
        /// </summary>
        /// <returns></returns>
        private webResposne skin()
        {
            //json
            var json = JsonConvert.SerializeObject(this.skinData);
            var stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            //response
            var response = new IDCT.webResposne();
            response.content = stream;
            response.header = new Dictionary<string, string>();
            response.header.Add("Content-Type", "text/json");

            return response;
        }

        /// <summary>
        /// Get color
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private String getColor(String name)
        {
            var brush = (App.Current.Resources[name] as SolidColorBrush);

            return "#" + brush.Color.R.ToString("X2") + brush.Color.G.ToString("X2") + brush.Color.B.ToString("X2");
        }

        #endregion

        #region STATIC

        /// <summary>
        /// Load file
        /// </summary>
        /// <returns></returns>
        private static webResposne loadFile(String url)
        {
            //create
            var response = new IDCT.webResposne();

            //remove ?
            if (url.IndexOf("?") > -1)
            {
                url = url.Substring(0, url.IndexOf("?"));
            }

            //set headers
            response.header = new Dictionary<string, string>();

            //load resource
            var uri = new Uri("Interface/" + url, UriKind.Relative);
            var resource = App.GetResourceStream(uri);
            if (resource != null)
            {
                response.content = resource.Stream;
                response.header.Add("Content-Type", contentType(uri));
            }
            else
            {
                response.content = App.GetResourceStream(new Uri("Interface/errors/404.html", UriKind.Relative)).Stream;
                response.header.Add("Content-Type", "text/html");
            }

            //send
            return response;
        }

        /// <summary>
        /// Content type
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static string contentType(Uri uri)
        {
            var extension = Path.GetExtension(uri.OriginalString);
            var type = "";
            switch (extension)
            {
                case ".eot":
                    type = "font/woff2";
                    break;
                case ".ttf":
                    type = "application/vnd.ms-fontobject";
                    break;
                case ".otf":
                    type = "font/opentype";
                    break;
                case ".svg":
                    type = "image/svg+xml";
                    break;
                case ".woff2":
                    type = "font/woff2";
                    break;
                case ".woff":
                    type = "application/octet-stream";
                    break;
                case ".css":
                    type = "text/css";
                    break;
                case ".png":
                    type = "image/png";
                    break;
                case ".js":
                    type = "text/javascript";
                    break;
                default:
                    type = "text/html";
                    break;
            }
            return type;
        }

        #endregion
    }
}
