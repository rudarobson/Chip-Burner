using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipBurner.Communicator
{
    public class Commander : IDisposable
    {
        /* Commands
         * 0 -> Begin programming
         * 1 -> Push Data (write)
         * 2 -> Pull Data (read)
         * 3 -> Chip Erase
         * 4 -> Run
         */

        public int Address { get; set; }
        private LowLevelWrite _usb { get; set; }
        
        private const int MaxPacketsize = 64;

        public Commander()
        {
            Address = 0;
            _usb = new LowLevelWrite();
        }

        /* 0 - Begin Programming
        * [0] - 0
        * 
        * expects:
        * Success - 0x53
        * Fail - 0x35
        */

        public bool BeginProgramming()
        {
            byte[] cmd = new byte[100];
            cmd[0] = 0;
            _usb.Write(cmd, 1);
            if (_usb.Read(cmd, 100) > 0)
            {
                if (cmd[0] == 0x53)
                    return true;
            }

            return false;
        }

        /* 1 - Push data (write)
        * [0] - 1
        * [1 ~ 4] - 32 bit Data Address
        * [5] - number of bytes to write
        * [6 ~ 6 + number of bytes] - data
        * 
        * expects:
        * { 'O', 'K' }
        */

        public void Write(byte[] data,int numBytes)
        {
            byte[] packet = new byte[MaxPacketsize];
            packet[0] = 1;
            packet[1] = (byte)((Address >> 24) & 0xFF);
            packet[2] = (byte)((Address >> 16) & 0xFF);
            packet[3] = (byte)((Address >> 8) & 0xFF);
            packet[4] = (byte)(Address & 0xFF);
            packet[5] = (byte)(numBytes & 0xFF);

            for (int i = 0; i < numBytes; i++)
                packet[6 + i] = data[i];

            _usb.Write(packet, numBytes + 6);
            _usb.Read(data, 2);

            if (!IsOK(data))
                throw new Exception("Device MalFunctioning");
        }

        /* 1 - Pull data (read)
         * [0] - 2
         * [1 ~ 4] - 32 bit Data Address
         * [5] - number of bytes to read
         * expects:
         * { byte0, byte1, ..., byte number of bytes to read - 1 }
         */
        public int Read(byte[] data,int numBytes)
        {
            byte[] packet = new byte[MaxPacketsize];
            packet[0] = 2;
            packet[1] = (byte)((Address >> 24) & 0xFF);
            packet[2] = (byte)((Address >> 16) & 0xFF);
            packet[3] = (byte)((Address >> 8) & 0xFF);
            packet[4] = (byte)(Address & 0xFF);
            packet[5] = (byte)(numBytes & 0xFF);

            _usb.Write(packet, 6);

            return _usb.Read(data, numBytes);
        }


        /* 3 - Chip Erase
         * [0] - 3
         * expects:
         * { byte0, byte1, ..., byte number of bytes to read - 1 }
         */

        public void ChipErase()
        {
            byte[] pck = new byte[2];
            pck[0] = 3;
            _usb.Write(pck, 1);
            _usb.Read(pck, 2);

            if(!IsOK(pck))
                throw new Exception("Device MalFunctioning");
        }

        /* [4] - Run
         * [0] - 4
         * [1] - 1 for turn on, 0 for turn off
         */
        public void Start(bool run)
        {
            byte[] pck = new byte[2];
            pck[0] = 4;
            if (run)
                pck[1] = 1;
            else
                pck[1] = 0;
            
            _usb.Write(pck, 2);
        }

        private bool IsOK(byte[] ok)
        {
            return (ok[0] == 'O' && ok[1] == 'K');
        }

        public FileStream GetUSB()
        {
            return _usb.GetStream();
        }

        public void Dispose()
        {
            _usb.Dispose();
        }
    }
}
