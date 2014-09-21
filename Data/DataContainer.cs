using CoPilot.CoPilot.Controller;
using CoPilot.Core.Data;
using CR = CoPilot.CoPilot.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot.Data
{
    public class DataContainer
    {
        public CR.Data DataController { get; set; }
        public Camera CameraController { get; set; }
        public Bluetooth BluetoothController { get; set; }
        public DriveMode DriveModeController { get; set; }
        public Ftp FtpController { get; set; }
        public Scheduler SchedulerController { get; set; }
        public Stats StatsController { get; set; }

        public Fill Fill { get; set; }
        public Repair Repair { get; set; }
        public Picture Picture { get; set; }
        public Video Video { get; set; }
        public Maintenance Maintenance { get; set; }
        public Uri Uri { get; set; }

        public String GrapType { get; set; }
    }
}
