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
        private ImageWindow imageWindow = new ImageWindow();
        private ImageWindow backgroundImage = new ImageWindow();
        private Screen[] screens;
        private Rectangle secondBounds = new Rectangle();
        
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
        }
        public void Show()
        {
            backgroundImage.Show();
            imageWindow.Show();
        }
        public void Close()
        {
            imageWindow.Close();
            backgroundImage.Close();
        }
        public void ChangeColor(byte gray, int color)
        {
            byte red, green, blue;
            switch (color)
            {
                case 1: // white
                    red = gray; green = gray; blue = gray;
                    break;
                case 2: // red
                    red = gray; green = 0; blue = 0;
                    break;
                case 3: // green
                    red = 0; green = gray; blue = 0;
                    break;
                case 4: //blue
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
