using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MDIPaint
{
    public partial class MainForm : Form
    {
        public static Color CurColor { get; set; }
        public static int CurWidth = 3;
        public static int nodesCount = 4;
        private Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();
        private static Color checkedColor = Color.FromArgb(64, Color.Blue);
        private static ToolStripButton pressedBtn = null;
        private int countChild = 0;


        public MainForm()
        {
            InitializeComponent();
            txtBrushSize.Text = CurWidth.ToString();
            picboxColor.BackColor = Color.Black;
            CurColor = Color.Black;

            FindPlugins();
            CreatePluginMenu();
        }


        private void btnBrush_Click(object sender, EventArgs e)
        {
            if ((Canvas)ActiveMdiChild != null)
            {
                Canvas cs = ((Canvas)ActiveMdiChild);
                ChangePressedButton(btnBrush);
                cs.NullifyEvents();
                cs.ToolChanged += cs.DrawLine;
            }
        }

        private void btnLastic_Click(object sender, EventArgs e)
        {
            if ((Canvas)ActiveMdiChild != null)
            {
                Canvas cs = ((Canvas)ActiveMdiChild);
                ChangePressedButton(btnLastic);
                cs.NullifyEvents();
                cs.ToolChanged += cs.Erase;
            }
        }

        private void btnStraightLine_Click(object sender, EventArgs e)
        {
            if ((Canvas)ActiveMdiChild != null)
            {
                Canvas cs = ((Canvas)ActiveMdiChild);
                ChangePressedButton(btnStraightLine);
                cs.NullifyEvents();
                cs.ToolChanged += cs.SetEndCoords;
                cs.DrawImage += cs.DrawStraightLine;
                cs.DrawGhostImage += cs.GhostLine;
            }
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            if ((Canvas)ActiveMdiChild != null)
            {
                Canvas cs = ((Canvas)ActiveMdiChild);
                ChangePressedButton(btnEllipse);
                cs.NullifyEvents();
                cs.ToolChanged += cs.SetEndCoords;
                cs.DrawGhostImage += cs.GhostEllipse;
                cs.DrawImage += cs.DrawEllipse;
            }
        }

        private void btnStar_ButtonClick(object sender, EventArgs e)
        {
            if ((Canvas)ActiveMdiChild != null)
            {
                Canvas cs = ((Canvas)ActiveMdiChild);

                if (pressedBtn != null) pressedBtn.BackColor = Color.Transparent;
                btnStar.BackColor = checkedColor;

                cs.NullifyEvents();

                cs.DrawImage += cs.CreateStar;
            }
        }


        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutPaint form = new AboutPaint();
            form.ShowDialog();
        }

        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Canvas child = new Canvas();
            child.SetCanvasText(++countChild);
            child.MdiParent = this;
            child.Show();
        }

        private void размерХолстаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CanvasSize cs = new CanvasSize();

            cs.CanvasWidth = ((Canvas)ActiveMdiChild).CanvasWidth;
            cs.CanvasHeight = ((Canvas)ActiveMdiChild).CanvasHeight;
            if (cs.ShowDialog() == DialogResult.OK)
            {
                ((Canvas)ActiveMdiChild).CanvasWidth = cs.CanvasWidth;
                ((Canvas)ActiveMdiChild).CanvasHeight = cs.CanvasHeight;
            }
        }

        private void красныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColor = Color.Red;
            picboxColor.BackColor = CurColor;
        }

        private void зеленыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColor = Color.Green;
            picboxColor.BackColor = CurColor;
        }

        private void синийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColor = Color.Blue;
            picboxColor.BackColor = CurColor;
        }

        private void другойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            if (cd.ShowDialog() == DialogResult.OK)
            {
                CurColor = cd.Color;
                picboxColor.BackColor = cd.Color;
            }
        }

        private void каскадомToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void слеваНаправоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void сверхуВнизToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void упорядочитьЗначкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Canvas)ActiveMdiChild).SaveAs();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpeg, *.jpg)|*.jpeg;*.jpg|Все файлы ()*.*|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Canvas child = new Canvas(dlg.FileName);
                child.MdiParent = this;
                child.Show();
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Canvas)ActiveMdiChild).Save();
        }

        private void числоВершинToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodesForm nf = new NodesForm();
            nf.NodesCount = nodesCount;

            if (nf.ShowDialog() == DialogResult.OK) nodesCount = nf.NodesCount;
        }


        private void OnPluginClick(object sender, EventArgs args)
        {
            IPlugin plugin = plugins[((ToolStripMenuItem)sender).Text];
            Canvas cs = ((Canvas)ActiveMdiChild);

            plugin.Transform((Bitmap)cs.PicBox.Image);

            cs.PicBox.Invalidate();
            cs.MarkAsUnsaved();
        }


        private void txtBrushSize_Click(object sender, EventArgs e)
        {
            txtBrushSize.SelectAll();
        }


        private void рисунокToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            размерХолстаToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
        }

        private void файлToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            сохранитьToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
            сохранитьКакToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
        }

        private void окноToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            каскадомToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
            слеваНаправоToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
            сверхуВнизToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
            упорядочитьЗначкиToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
        }

        private void фильтрыToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in фильтрыToolStripMenuItem.DropDownItems)
            {
                item.Enabled = !(ActiveMdiChild == null);
            }
            оФильтрахToolStripMenuItem.Enabled = true;
        }


        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                scaleBar.Value = ((Canvas)ActiveMdiChild).Scale;
                scaleLabel.Text = $"Масштаб: {scaleBar.Value}%";
            }
            else scaleLabel.Text = "Масштаб:";
        }


        private void scaleBar_Scroll(object sender, EventArgs e)
        {
            scaleLabel.Text = $"Масштаб: {scaleBar.Value}%";

            if (ActiveMdiChild != null)
            {
                ((Canvas)ActiveMdiChild).Scale = scaleBar.Value;
                double sc = scaleBar.Value / 100.0;
                ((Canvas)ActiveMdiChild).Rescale(sc);
            }
        }


        private void txtBrushSize_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CurWidth = int.Parse(txtBrushSize.Text);
            }
            catch
            {
                MessageBox.Show("Значение должнo быть целым числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }


        private void ChangePressedButton(ToolStripButton newBtn)
        {
            if (pressedBtn == null)
            {
                newBtn.BackColor = checkedColor;
            }
            else 
            {
                pressedBtn.BackColor = Color.Transparent;
                newBtn.BackColor = checkedColor;
            }
            btnStar.BackColor = Color.Transparent;
            pressedBtn = newBtn;
        }

        private void CreatePluginMenu()
        {
            foreach (IPlugin p in plugins.Values)
            {
                var item = new ToolStripMenuItem(p.Name);
                item.Click += OnPluginClick;
                фильтрыToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private void FindPlugins()
        {
            string folder = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] files = Directory.GetFiles(folder, "*.dll");

            foreach (string file in files)
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);

                    foreach (Type type in assembly.GetTypes())
                    {
                        Type iface = type.GetInterface("PluginInterface.IPlugin");

                        if (iface != null)
                        {
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                            plugins.Add(plugin.Name, plugin);
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Ошибка загрузки плагина\n" + ex.Message); }
        }

        public void SetCursorCoordinates(string str)
        {
            labelCoords.Text = str;
        }

        private void оФильтрахToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginsInfo form = new PluginsInfo(plugins);
            form.Show();
        }
    }
}
