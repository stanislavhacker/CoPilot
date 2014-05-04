using Microsoft.Phone.Marketplace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoPilot.Utils
{
    public class License
    {
        /// <summary>
        /// Licence info
        /// </summary>
        private static LicenseInformation license = new LicenseInformation();

        /// <summary>
        /// Is Trial
        /// </summary>
        private static bool isTrial = true;
        public static bool IsTrial
        {
            get
            {
                return isTrial;
            }
        }

        /// <summary>
        /// Resolve license
        /// </summary>
        public static void ResolveLicense() 
        {
            isTrial = license.IsTrial();
        }

    }
}
