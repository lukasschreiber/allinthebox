using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace allinthebox.utils
{
    class usb
    {

        //get Label with given pid
        public static string getLabel(string _pid, List<USBDeviceInfo> usbDevices)
        {
            foreach (var usbDevice in usbDevices)
            {
                if (usbDevice.DeviceID == _pid)
                {
                    return usbDevice.Label;
                }
            }
            return null;
        }

    }
}
