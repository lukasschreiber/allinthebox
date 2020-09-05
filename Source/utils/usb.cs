using System.Collections.Generic;

namespace allinthebox.utils
{
    internal class usb
    {
        //get Label with given pid
        public static string getLabel(string _pid, List<USBDeviceInfo> usbDevices)
        {
            foreach (var usbDevice in usbDevices)
                if (usbDevice.DeviceID == _pid)
                    return usbDevice.Label;
            return null;
        }
    }
}