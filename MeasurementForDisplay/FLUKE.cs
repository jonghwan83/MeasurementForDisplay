using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace MeasurementForDisplay
{
    class FLUKE
    {
        private SerialPort sp = new SerialPort();
        public bool isOpen = false;
        public string value = "0";

        public void Open(string portName)
        {
            sp.PortName = portName;
            sp.BaudRate = 115200;
            sp.DataBits = 8;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            sp.ReadTimeout = 500;
            sp.Open();
            isOpen = sp.IsOpen;

            sp.WriteLine("QM\r");
        }
        public void Close()
        {
            sp.Close();
            isOpen = sp.IsOpen;
        }
        public void GetFluke()
        {
            sp.WriteLine("QM\r");
            string[] output = sp.ReadExisting().Trim().Substring(2).Split(',');
            value = output[0];
        }
    }
}
