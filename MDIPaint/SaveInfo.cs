using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDIPaint
{
    public class SaveInfo
    {
        public string FileName { get; set; }
        public bool IsSaved { get; set; }

        public SaveInfo()
        {
            FileName = "";
            IsSaved = false;
        }

        public SaveInfo(string s, bool b)
        {
            FileName = s;
            IsSaved = b;
        }
    }
}
