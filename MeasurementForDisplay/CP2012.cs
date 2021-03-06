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
        public bool isOpen = false;

        // COM port search in MainWindow
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
            isOpen = sp.IsOpen;
        }
        public void Close()
        {
            sp.Close();
            isOpen = sp.IsOpen;
        }
        public void GetCurrent()
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
            current = num2;
        }        
        private void ESC()
        {
            byte[] buffer = new byte[] { 0x1b }; // 0x1b는 PCC 소스코드에서 복사한 내용
            sp.Write(buffer, 0, buffer.Length);
        }
    }
}
