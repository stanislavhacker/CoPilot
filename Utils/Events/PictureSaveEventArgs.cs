using CoPilot.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot.Utils.Events
{
    public class PictureSaveEventArgs : EventArgs
    {
        public Picture Picture { get; set; }
    }
}
