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

namespace MeasurementForDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageGenerator imageWindows = new ImageGenerator();
        CA210 ca210 = new CA210();
        CP2012 cp2012 = new CP2012();
        
        private bool connected = false;

        public MainWindow()
        {
            InitializeComponent();

            string[] dataName = new string[] { "Y", "x", "y", "current" };
            int countData = dataName.GetType().GetGenericArguments().Length;

            foreach (string colname in dataName)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = colname;
                textColumn.Binding = new Binding(colname);
                MeasDataGrid.Columns.Add(textColumn);
            }

            // COM port search
            
        }
        private void GetCurrent()
        {
            cp2012.Open("COM2");
            System.Threading.Thread.Sleep(1000);
            double current;
            current = cp2012.GetCurrent(CP2012.Cell_RGBTypes.Blue);
            LogListbox.Items.Add(current);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            imageWindows.Close();
        }

        private void ZeroCalBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connected == false)
                {
                    ca210.Connect();
                    connected = true;
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
            ca210.Disconnect();
            ZeroCalBtn.IsEnabled = true;
        }

        private void Meas1Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ca210.Measure();
            } catch (Exception ex)
            {
                LogListbox.Items.Add(ex.Message);
            }            
        }
        private void PatternBtn_Click(object sender, RoutedEventArgs e)
        {
            GetCurrent();
            //imageWindows.Initialize();
            //imageWindows.ChangeColor(255, 1);
            //imageWindows.Show();
        }
    }
}
