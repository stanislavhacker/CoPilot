using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoPilot.Data
{
    [XmlRoot("speech")]
    public class Commands
    {
        /// <summary>
        /// Load
        /// </summary>
        /// <returns></returns>
        public static Commands Load()
        {
            Uri uri = new Uri("Resources/Speech/Grammars/en-gb.xml", UriKind.Relative);
            var resource = App.GetResourceStream(uri);

            XmlSerializer xml = new XmlSerializer(typeof(Commands));
            Commands tmpCommands = xml.Deserialize(resource.Stream) as Commands;

            resource.Stream.Close();
            resource.Stream.Dispose();

            return tmpCommands;
        }

        [XmlElement("i-want-to")]
        public String IWantTo { get; set; }

        [XmlElement("add-fill")]
        public String AddFill { get; set; }

        [XmlElement("make-a-photo")]
        public String MakeAPhoto { get; set; }

        [XmlElement("start-recording")]
        public String StartRecording { get; set; }

        [XmlElement("go-back")]
        public String GoBack { get; set; }

        [XmlElement("cancel")]
        public String Cancel { get; set; }

        [XmlElement("fill")]
        public String Fill { get; set; }

        [XmlElement("check")]
        public String Check { get; set; }

        [XmlElement("odometer")]
        public String Odometer { get; set; }

        [XmlElement("fuel-price")]
        public String FuelPrice { get; set; }

        [XmlElement("price-per-unit")]
        public String PricePerUnit { get; set; }

        [XmlElement("fueled")]
        public String Fueled { get; set; }

        [XmlElement("full-tank")]
        public String FullTank { get; set; }

        [XmlElement("save-record")]
        public String SaveRecord { get; set; }
    }
}
