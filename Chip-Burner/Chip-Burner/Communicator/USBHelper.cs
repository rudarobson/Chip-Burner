using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChipBurner.Communicator
{
    public class USBHelper
    {
        [DllImport("USB.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern Boolean GetDeviceHandle([In] ref Guid guid, ref IntPtr handle);
        
        private static Guid _driverInterfaceGUI = new Guid(0x0410b530, 0x9d8f, 0x44b4, 0xbe, 0x87, 0x5d, 0xa7, 0x78, 0x0e, 0x54, 0xfe);

        public static FileStream OpenDevice()
        {
            IntPtr handler = IntPtr.Zero;
            IntPtr handle = IntPtr.Zero;
            GetDeviceHandle(ref _driverInterfaceGUI, ref handle);
            return new FileStream(handle, FileAccess.ReadWrite);
        }
    }
}
