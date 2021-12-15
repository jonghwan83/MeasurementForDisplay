using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace MeasurementForDisplay
{
    class ImageGenerator
    {
        public ImageWindow imageWindow = new ImageWindow();
        private ImageWindow backgroundImage = new ImageWindow();
        private Screen[] screens;
        private Rectangle secondBounds = new Rectangle();
        public bool isOn = false;
        
        public void Initialize()
        {
            screens = Screen.AllScreens;
            if (screens.Length > 1)
            {
                secondBounds = screens[1].Bounds;
            } else
            {
                secondBounds = screens[0].Bounds;
            }
            backgroundImage.Width = secondBounds.Width;
            backgroundImage.Height = secondBounds.Height;

            Rectangle mainBounds = new Rectangle();
            mainBounds = screens[0].Bounds;
            backgroundImage.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            backgroundImage.Left = mainBounds.Width;
            backgroundImage.Top = 0;
            imageWindow.Topmost = true;
            
            ChangeColor(32, "W"); // 초기 32 gray
        }
        public void Show()
        {
            backgroundImage.Show();
            imageWindow.Show();
            isOn = true;
        }
        public void Close()
        {
            imageWindow.Close();
            backgroundImage.Close();
            isOn = false;
        }
        public void Hide()
        {
            imageWindow.Hide();
            backgroundImage.Hide();
            isOn = false;
        }
        public double[] GetSize()
        {
            double[] size = new double[]
            {
                imageWindow.Width,
                imageWindow.Height
            };
            
            return size;
        }
        public void ChangeSize(double width, double height)
        {
            imageWindow.Width = width;
            imageWindow.Height = height;
        }
        public void ChangeColor(byte gray, string color)
        {
            byte red, green, blue;
            switch (color)
            {
                case "W": // white
                    red = gray; green = gray; blue = gray;
                    break;
                case "R": // red
                    red = gray; green = 0; blue = 0;
                    break;
                case "G": // green
                    red = 0; green = gray; blue = 0;
                    break;
                case "B": //blue
                    red = 0; green = 0; blue = gray;
                    break;
                default:
                    red = gray; green = gray; blue = gray;
                    break;
            }
            var brush = new SolidColorBrush(Color.FromRgb(red, green, blue));
            imageWindow.Background = brush;
        }
    }
}
