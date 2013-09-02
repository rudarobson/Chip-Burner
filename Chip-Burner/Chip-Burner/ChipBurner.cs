using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hex;
using ChipBurner.Communicator;
namespace ChipBurner
{
    public class Programmer
    {
        public static void Write(string hexFileName)
        {
            HexParser hexParser = new HexParser(hexFileName);
            hexParser.StartReading();
            int pageSize = 32;

            HexRecord record = hexParser.ReadLine();
            byte[] data = new byte[pageSize];


            int totalWritten = 0;

            Commander cmd = new Commander();
            try
            {
                FileStream usb = cmd.GetUSB();
                byte[] nill = new byte[100];
                //    usb.Read(nill, 0, 100);
            }
            catch
            {
            }

            if (cmd.BeginProgramming())
            {
                cmd.ChipErase();

                while (record.Type != HexRecord.EOF)
                {
                    switch (record.Type)
                    {
                        case HexRecord.DataType:
                            for (int i = 0; i < record.Length; i++)
                            {
                                if (totalWritten == pageSize || (((record.FullAddress + i) / (2 * pageSize)) != (cmd.Address / pageSize)))
                                {
                                    cmd.Write(data, totalWritten);
                                    cmd.Address = (int)(cmd.Address) + (totalWritten / 2);

                                    totalWritten = 0;
                                }

                                data[totalWritten++] = record.Data[i];
                            }

                            break;
                        case HexRecord.ExtendedLinearAddressRecord:
                        case HexRecord.ExtendedSegmentAddress:
                            throw new Exception("Not Supporte Yet");
                    }
                    record = hexParser.ReadLine();
                }

                if (totalWritten != 0)
                    cmd.Write(data, totalWritten);
            }

            cmd.Dispose();

            hexParser.Dispose();
        }

        public static bool Verify(string hexFileName)
        {
            HexParser hexParser = new HexParser(hexFileName);
            hexParser.StartReading();
            int pageSize = 32;

            HexRecord record = hexParser.ReadLine();
            byte[] data = new byte[pageSize];

            Commander cmd = new Commander();
            try
            {
                FileStream usb = cmd.GetUSB();
                byte[] nill = new byte[100];
                //    usb.Read(nill, 0, 100);
            }
            catch
            {
            }
            bool success = true;
            if (cmd.BeginProgramming())
            {
                while (record.Type != HexRecord.EOF && success)
                {
                    switch (record.Type)
                    {
                        case HexRecord.DataType:
                            cmd.Address = record.Address / 2;
                            cmd.Read(data, record.Length);
                            for (int i = 0; i < record.Length; i++)
                            {
                                if (data[i] != record.Data[i])
                                {
                                    success = false;
                                    break;
                                }
                            }

                            break;
                        case HexRecord.ExtendedLinearAddressRecord:
                        case HexRecord.ExtendedSegmentAddress:
                            throw new Exception("Not Supporte Yet");
                    }
                    record = hexParser.ReadLine();
                }

            }
            cmd.Dispose();

            hexParser.Dispose();

            return success;
        }

        public static void Run(bool run)
        {
            Commander cmd = new Commander();
            cmd.Start(run);
        }
    }
}
