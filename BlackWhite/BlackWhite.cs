using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackWhite
{
    [Version(1, 0)]
    public class BlackWhite : IPlugin
    {
        public string Name
        {
            get
            {
                return "Черно-белый фильтр";
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
                    Color color = bitmap.GetPixel(i, j);
                    int avg = (color.R + color.G + color.B) / 3;
                    bitmap.SetPixel(i, j, Color.FromArgb(avg, avg, avg));
                }
        }
    }
}
