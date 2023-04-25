using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIPaint
{
    public partial class CanvasSize : Form
    {
        public CanvasSize()
        {
            InitializeComponent();
        }

        public int CanvasWidth
        {
            get { return (int)nudWidth.Value; }
            set { nudWidth.Value = value; }
        }

        public int CanvasHeight
        {
            get { return (int)nudHeight.Value; }
            set { nudHeight.Value = value; }
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
        }
    }
}
