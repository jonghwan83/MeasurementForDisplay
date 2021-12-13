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
        private bool isOpen = false;
        private SerialPort sp = new SerialPort();
        
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
            isOpen = true;
        }
        public double GetCurrent(Cell_RGBTypes rgb)
        {
            TargetTypes gCM = TargetTypes.GCM;
            uint startAddress = 0x12_0040;
            if (rgb == Cell_RGBTypes.Green)
            {
                startAddress += 4;
            } else if (rgb == Cell_RGBTypes.Blue)
            {
                startAddress += 8;
            }
            return (((double)ReadParameter(gCM, startAddress)) / 100.0);
        }
        private void ESC()
        {
            byte[] buffer = new byte[] { 0x1b }; // 0x1b는 PCC 소스코드에서 복사한 내용
            sp.Write(buffer, 0, buffer.Length);
        }
        
        public ushort ReadParameter(TargetTypes target, uint startAddress)
        {
            int num2 = 0;
            if (target == TargetTypes.MASTER_SLAVE)
            {
                throw new Exception("Master_Slave로는 Read할 수 없습니다.!\n Master, Slave 하나만 선택해주세요.");
            }
            ESC();
            sp.DiscardInBuffer();
            string text = (((target != TargetTypes.EEPROM0) && ((target != TargetTypes.PMIC) && (target != TargetTypes.GAMIC))) && (target != TargetTypes.GAMIC2)) ? $"param? {target} 0x{startAddress.ToString("X6")}" : $"param? {target} 0x{startAddress.ToString("X4")}";
            string str2 = (((target != TargetTypes.EEPROM0) && ((target != TargetTypes.PMIC) && (target != TargetTypes.GAMIC))) && (target != TargetTypes.GAMIC2)) ? $"param,{target},0x{startAddress.ToString("X6")},0x{0.ToString("X4")}" : $"param,{target},0x{startAddress.ToString("X4")},0x{0.ToString("X4")}";
            int num = str2.Length + 1;

            while (true)
            {
                if ((num <= sp.BytesToRead) || (num2 >= 500))
                {
                    if (num2 == 500)
                    {
                        throw new TimeoutException();
                    }
                    string syntax = sp.ReadLine();
                    string[] separator = new string[] { ",", " " };
                    string[] strArray = syntax.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    return ((target != TargetTypes.PMIC) ? Convert.ToUInt16(strArray[3], 0x10) : ((byte)Convert.ToUInt16(strArray[3], 0x10)));
                }
                int num4 = num2;
                num2 = num4 + 1;
                System.Threading.Thread.Sleep(5);
            }
        }
    }
}
