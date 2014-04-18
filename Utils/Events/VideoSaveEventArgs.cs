using CoPilot.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot.Utils.Events
{
    public class VideoSaveEventArgs : EventArgs
    {
        public Video Video { get; set; }
    }
}
