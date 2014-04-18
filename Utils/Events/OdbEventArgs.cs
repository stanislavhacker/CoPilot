using CoPilot.CoPilot.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;

namespace CoPilot.Utils.Events
{
    public class BluetoothEventArgs : EventArgs
    {
        public BluetoothErrorType ErrorType { get; set; }
        public PeerInformation Device { get; set; }
        public StreamSocket Socket { get; set; }
    }
}
