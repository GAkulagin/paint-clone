using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncreaseBrightness
{
    [Version(1,0)]
    public class IncreaseBrightness : IPlugin
    {
        //private float factor = 0.2f;
        private double factor = 1.2;

        public string Name
        {
            get
            {
                return "Повысить яркость";
            }
        }

        public string Author
        {
            get
            {
                return "KGA";
            }
        }

        public void Transform(Bitmap bitmap)
        {
            for (int i = 0; i < bitmap.Width; ++i)
                for (int j = 0; j < bitmap.Height; ++j)
                {
                    Color curColor = bitmap.GetPixel(i, j);
                    Color newColor = Color.FromArgb(Increase(curColor.R), Increase(curColor.G), Increase(curColor.B));
                    bitmap.SetPixel(i, j, newColor);
                }
        }

        private byte Increase(byte value)
        {
            if (value * factor > 255) return 255;
            else return (byte)(value * factor);
        }
    }
}
