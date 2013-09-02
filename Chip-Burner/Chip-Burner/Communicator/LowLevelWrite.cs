using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipBurner.Communicator
{
    public class LowLevelWrite : IDisposable
    {
        private FileStream _usb { get; set; }

        public LowLevelWrite()
        {
            _usb = USBHelper.OpenDevice();
        }

        public void Write(byte[] data, int num)
        {
            if (num > 64)
                throw new Exception("Exceding 64 bytes of usb max packet size must handle segmentation");

            _usb.Write(data, 0, num);
            _usb.Flush();
        }

        public int Read(byte[] data, int num)
        {
            return _usb.Read(data, 0, num);
        }

        public FileStream GetStream()
        {
            return _usb;
        }

        public void Dispose()
        {
            _usb.Close();
        }
    }
}
