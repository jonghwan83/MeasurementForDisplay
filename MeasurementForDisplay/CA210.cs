using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CA200SRVRLib;

namespace MeasurementForDisplay
{
    class CA210
    {
        private readonly Ca200 probe = new Ca200();
        public double lv = 0;
        public double sx = 0;
        public double sy = 0;
        
        public void Connect()
        {
            probe.AutoConnect();
        }
        public void CalZero()
        {
            probe.SingleCa.RemoteMode = 1;
            probe.SingleCa.CalZero();

            probe.SingleCa.SyncMode = 3;
            probe.SingleCa.AveragingMode = 2;
            probe.SingleCa.SetAnalogRange(Convert.ToSingle(2.5), Convert.ToSingle(2.5));
            probe.SingleCa.DisplayMode = 0;
            probe.SingleCa.Memory.ChannelNO = 0;
        }
        public void Measure()
        {
            probe.SingleCa.Measure();
            lv = probe.SingleCa.SingleProbe.Lv;
            sx = probe.SingleCa.SingleProbe.sx;
            sy = probe.SingleCa.SingleProbe.sy;
            
        }
        public void Disconnect()
        {
            // disconnect
            probe.SingleCa.RemoteMode = 0;
        }
    }
}
