using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using LiveCharts;
using LiveCharts.Wpf;
using System.Diagnostics;

namespace MeasurementForDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageGenerator imageWindow = new ImageGenerator();
        CA210 ca210 = new CA210();
        CP2012 cp2012 = new CP2012();
        Measured measured;

        bool isAbort = false;
        public MainWindow()
        {
            InitializeComponent();
            InitializeChart();

            string[] dataName = new string[] { "No", "Time", "Lv", "Sx", "Sy", "Current" };

            foreach (string colname in dataName)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn
                {
                    Header = colname,
                    Binding = new Binding(colname)
                };
                MeasDataGrid.Columns.Add(textColumn);
            }
            foreach (string s in SerialPort.GetPortNames()) // COM port search
            {
                PortComboBox.Items.Add(s);
            }

            TimeTextBox.Text = "20, 10, 20";
            GrayTextBox.Text = "128, 255, 128";
            ColorTextBox.Text = "W, W, W";
        }
        public void Wait(int milliseconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }
        public class Measured
        {
            public int No { get; set; }
            public string Time { get; set; }
            public double Lv { get; set; }
            public double Sx { get; set; }
            public double Sy { get; set; }
            public double Current { get; set; }
        }
        private void MeasWriteData(int milliseconds)
        {
            Stopwatch stopwatch = new Stopwatch();
            double remaind;
            stopwatch.Start();
            Measure();
            ReadData();
            TimeSpan ts = stopwatch.Elapsed;
            remaind = milliseconds - ts.TotalMilliseconds;
            if (remaind > 0)
            {
                Wait(Convert.ToInt32(remaind));
            }
        }
        private void ReadData()
        {
            string format = "MM/dd/yyyy HH:mm:ss";
            string now = DateTime.Now.ToString(format);
            measured = new Measured {
                No = MeasDataGrid.Items.Count,
                Time = now,
                Lv = Math.Round(ca210.lv, 4),
                Sx = Math.Round(ca210.sx, 4),
                Sy = Math.Round(ca210.sy, 4),
                Current = Math.Round(cp2012.current, 4)
            };
            MeasDataGrid.Items.Add(measured);
            MeasDataGrid.ScrollIntoView(measured);
            DataChart.Series[0].Values.Add(measured.Lv);
        }
        private void InitializeChart()
        {
            DataChart.Series.Clear();
            DataChart.Series.Add(new LineSeries()
            {
                Title = "Y",
                Values = new ChartValues<double> { },
                PointGeometry = null,       
            });
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            imageWindow.Close();
        }
        private void Measure() // measure and gather data
        {
            if (ca210.isConnected == true)
            {
                ca210.Measure();
            }
            if (cp2012.isOpen == true)
            {
                cp2012.GetCurrent();
            }
        }
        public void GetPatternSize()
        {
            double[] size = new double[] { 0, 0 };
            size = imageWindow.GetSize();

            WidthTextBox.Text = size[0].ToString();
            HeightTextBox.Text = size[1].ToString();
        }
        private void ZeroCalBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ca210.isConnected == false)
                {
                    ca210.Connect();
                }
                ca210.CalZero();
                ZeroCalBtn.IsEnabled = false;
            } catch (Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }
        }
        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ca210.Disconnect();
                ZeroCalBtn.IsEnabled = true;
            } catch (Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }            
        }
        private void Meas1Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MeasWriteData(980);
            } catch (Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }
        }        
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string portName = PortComboBox.Text;
                cp2012.Open(portName);
            }
            catch (Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }            
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cp2012.Close();
            } catch (Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }
        }
        private void GetCurrent()
        {
            cp2012.GetCurrent();
        }

        private void ContinuousBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogListbox.Items.Add("Measuring...");
                isAbort = false;
                while (!isAbort)
                {
                    MeasWriteData(980);
                }
            } catch (Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }
        }
        private void AbortBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isAbort = true;
                LogListbox.Items.Add("Aborted...");
            } catch(Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }
        }
        private void PatternBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (imageWindow.isOn == false)
                {
                    imageWindow.Initialize();
                    imageWindow.Show();
                    PatternBtn.IsEnabled = false;

                    GetPatternSize();
                }
            }
            catch (Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }
        }
        private void PatternCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imageWindow.Hide();
                PatternBtn.IsEnabled = true;
            } catch (Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }
        }
        private void WidthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (imageWindow.isOn == true)
                {
                    imageWindow.ChangeSize(double.Parse(WidthTextBox.Text), double.Parse(HeightTextBox.Text));
                }                
            } catch (Exception ex)
            {
                GetPatternSize();
                LogListbox.Items.Add(ex.Message);
            }            
        }
        private void HeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (imageWindow.isOn == true)
                {
                    imageWindow.ChangeSize(double.Parse(WidthTextBox.Text), double.Parse(HeightTextBox.Text));
                }                
            }
            catch (Exception ex)
            {
                GetPatternSize();
                LogListbox.Items.Add(ex.Message);
            }
        }
        private void Meas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isAbort = false;
                LogListbox.Items.Add("Measuring...");
                if (imageWindow.isOn == false)
                {
                    LogListbox.Items.Add("패턴이 없습니다.");
                    return;
                }
                string[] time = TimeTextBox.Text.Split(',');
                string[] grays = GrayTextBox.Text.Split(',');
                string[] colors = ColorTextBox.Text.Split(',');
                
                for (int i = 0; i < time.Length; i++)
                {
                    imageWindow.ChangeColor(byte.Parse(grays[i]), colors[i]);
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    while (true)
                    {
                        if (isAbort == true)
                        {
                            stopwatch.Stop();
                            return;
                        }
                        MeasWriteData(990);
                        TimeSpan ts = stopwatch.Elapsed;
                        if (double.Parse(time[i]) - ts.TotalSeconds <= 0)
                        {
                            stopwatch.Stop();
                            break;
                        }
                    }                    
                }
                System.Media.SystemSounds.Beep.Play();
                LogListbox.Items.Add("Complete!!");
            } catch (Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }            
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            MeasDataGrid.Items.Clear();
            InitializeChart();
        }
    }
}