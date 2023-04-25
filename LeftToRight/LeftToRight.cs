using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeftToRight
{
    [Version(1, 0)]
    public class LeftToRight : IPlugin
    {
        public string Name
        {
            get
            {
                return "Развернуть слева направо";
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
            for (int i = 0; i < bitmap.Width / 2; ++i)
                for (int j = 0; j < bitmap.Height; ++j)
                {
                    Color color = bitmap.GetPixel(i, j);
                    bitmap.SetPixel(i, j, bitmap.GetPixel(bitmap.Width - 1 - i, j));
                    bitmap.SetPixel(bitmap.Width - 1 - i, j, color);
                }
        }
    }
}
