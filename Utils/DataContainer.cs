using CoPilot.CoPilot.Controller;
using CoPilot.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot.Utils
{
    public class DataContainer
    {
        public Data DataController { get; set; }
        public Camera CameraController { get; set; }
        public Bluetooth BluetoothController { get; set; }
        public DriveMode DriveModeController { get; set; }
        public Ftp FtpController { get; set; }

        public Fill Fill { get; set; }
        public Repair Repair { get; set; }
        public Picture Picture { get; set; }
        public Video Video { get; set; }
        public Uri Uri { get; set; }
    }
}
