using PluginInterface;
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
    public partial class PluginsInfo : Form
    {
        public PluginsInfo(Dictionary<string, IPlugin> plugins)
        {
            InitializeComponent();
            PrintPluginsInfo(plugins);
        }

        private void PrintPluginsInfo(Dictionary<string, IPlugin> plugins)  
        {
            int i = 0;
            foreach(IPlugin plugin in plugins.Values)
            {
                richTextBox.AppendText($"Имя: {plugin.Name}\n");
                richTextBox.AppendText($"Автор: {plugin.Author}\n");
                VersionAttribute version = (VersionAttribute)Attribute.GetCustomAttribute(plugin.GetType(), typeof(VersionAttribute));
                richTextBox.AppendText($"Версия: {version.Major}.{version.Minor}\n");
                richTextBox.AppendText("\n");

                i++;
            }
        }
    }
}
