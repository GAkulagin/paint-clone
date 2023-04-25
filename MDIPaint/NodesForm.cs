using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIPaint
{
    public partial class NodesForm : Form
    {
        public NodesForm()
        {
            InitializeComponent();
        }

        public int NodesCount
        {
            get { return (int)nudNodes.Value; }
            set { nudNodes.Value = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }
    }
}
