using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace MeasurementForDisplay
{
    class CP2012
    {
        private SerialPort sp = new SerialPort();
        public double current = 0.0;

        // COM port search in MainWindow
        public enum TargetTypes
        {
            MASTER = 0,
            SLAVE = 1,
            EEPROM0 = 2,
            GCM = 3,
            FRONT = 4,
            LOGO = 5,
            NPCP = 6,
            AP = 7,
            MASTER_SLAVE = 8,
            ANALOG = 9,
            PLL = 10,
            GAMIC = 11,
            GAMIC2 = 12,
            PMIC = 13
        }
        public enum Cell_RGBTypes
        {
            Red = 0,
            Green = 1,
            Blue = 2
        }
        public void Open(string portName)
        {
            sp.PortName = portName;
            sp.BaudRate = 0x1_c200;
            sp.DataBits = 8;
            sp.Parity = Parity.None;
            sp.ReadBufferSize = 0x1_86a0;
            sp.StopBits = StopBits.One;
            sp.ReadTimeout = 500;
            sp.Open();
        }
        public double GetCurrent()
        {
            string str;
            int num = 0;
            double num2 = 0.0;

            ESC();
            sp.DiscardInBuffer();
            sp.WriteLine("CURRENT?");

            while (true)
            {
                if ((5 <= sp.BytesToRead) || (num >= 8000))
                {
                    if (num == 800)
                    {
                        throw new TimeoutException();
                    }
                    str = sp.ReadLine();
                    break;
                }
            }
            char[] separator = new char[] { ',' };
            string[] strArray = str.Split(separator);
            if ((strArray.Length > 1) && strArray[0].Equals("CURRENT"))
            {
                num2 = Convert.ToDouble(strArray[1]) / 0x8000;
            }
            return num2;
        }
        
        private void ESC()
        {
            byte[] buffer = new byte[] { 0x1b }; // 0x1b는 PCC 소스코드에서 복사한 내용
            sp.Write(buffer, 0, buffer.Length);
        }
    }
}
