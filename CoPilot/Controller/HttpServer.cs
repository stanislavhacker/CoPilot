using CoPilot.Core.Api;
using CoPilot.Core.Data;
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
        private Data dataController;
        private Int32 pageSize = 20;

        #endregion

        /// <summary>
        /// Initialize HttpServer class
        /// </summary>
        public HttpServer(Data dataController)
        {
            this.dataController = dataController;
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
                var url = new Uri("http://test" + e.uri);
                var request = url.LocalPath.Replace("/copilot/api/", "");

                switch(request) {
                    case "skin":
                        return this.skin();
                    case "language":
                        return this.language();
                    case "data":
                        return this.data(url.Query.Substring(1));
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
            if (test == null)
            {
                test = AppResources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.InvariantCulture, true, false);
            }

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

        #region DATA


        /// <summary>
        /// Data
        /// </summary>
        /// <returns></returns>
        private webResposne data(String query)
        {
            //url: api/data?command=setting&from=&to=&page=

            //data
            var data = parseQueryString(query);

            //data
            String command = getParam(data, "command") as String;
            Int32 from = (Int32)getParam(data, "from");
            Int32 to = (Int32)getParam(data, "to");
            Int32 page = (Int32)getParam(data, "page");

            switch (command)
            {
                case "setting":
                    return createResponse(createSettings());
                case "maintenances":
                    return createResponse(createMaintenances(from, to, page));
                    
                default:
                    return new IDCT.webResposne();
            }
        }

        /// <summary>
        /// Create maintenances
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private object createMaintenances(int from, int to, int page)
        {
            var maintenances = this.dataController.Records.Maintenances;
            var total = maintenances.Count();

            try
            {
                if (from == -1 && to == -1)
                {
                    var take = page == -1 ? total : this.pageSize;
                    var skip = page == -1 ? 0 : this.pageSize * page;
                    return maintenances.Skip(skip).Take(this.pageSize).ToArray();
                }
                else
                {
                    from = from == -1 ? 0 : from;
                    to = to == -1 ? total - from : to - from;
                    return maintenances.Skip(from).Take(to).ToArray();
                }
            }
            catch
            {
                return new Maintenance[0];
            }
        }

        /// <summary>
        /// Get settings
        /// </summary>
        /// <returns></returns>
        private Settings createSettings()
        {
            var controller = this.dataController;
            var stats = new Statistics.Statistics(controller.Records);

            //settings
            var setting = new Settings();
            setting.Consumption = controller.Consumption.ToString();
            setting.Currency = controller.Currency.ToString();
            setting.Distance = controller.Distance.ToString();
            setting.Fills = controller.Fills.Count;
            setting.Maintenances = controller.Maintenances.Count;
            setting.Repairs = controller.Repairs.Count;
            setting.Videos = controller.Videos.Count;
            setting.Pictures = controller.Pictures.Count;
            setting.Paths = stats.getRoutes().Count;
            setting.SummaryFuelPrice = stats.getFuelStats().PaidForFuel(controller.Currency);
            setting.SummaryRepairPrice = stats.getRepairStats().PaidForRepairs(controller.Currency);
            setting.Liters = stats.getFuelStats().TotalRefueled();
            return setting;
        }

        #endregion

        #region STATIC

        /// <summary>
        /// Get param
        /// </summary>
        /// <param name="data"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private static object getParam(Dictionary<string, string> data, String param)
        {
            switch (param)
            {
                case "command":
                    return data.ContainsKey(param) ? data[param] : "unknown";
                case "from":
                    if (data.ContainsKey("from"))
                    {
                        int value;
                        if (Int32.TryParse(data["from"], out value)) 
                        {
                            return value;
                        }
                    }
                    return -1;
                case "to":
                    if (data.ContainsKey("to"))
                    {
                        int value;
                        if (Int32.TryParse(data["to"], out value))
                        {
                            return value;
                        }
                    }
                    return -1;
                case "page":
                    if (data.ContainsKey("page"))
                    {
                        int value;
                        if (Int32.TryParse(data["page"], out value))
                        {
                            return value;
                        }
                    }
                    return -1;
                default:
                    return null;
            }
        }

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

        /// <summary>
        /// Parse query
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static Dictionary<string, string> parseQueryString(string uri)
        {
            //parts
            string[] pairs = uri.Split('&');
            //dic
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach (string piece in pairs)
            {
                string[] pair = piece.Split('=');
                output.Add(pair[0], pair[1]);
            }
            return output;
        }

        /// <summary>
        /// Create response form object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static webResposne createResponse(object value)
        {
            //json
            var json = JsonConvert.SerializeObject(value);
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

        #endregion
    }
}
