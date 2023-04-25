using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

namespace MDIPaint
{
    public partial class Canvas : Form
    {
        public delegate void ToolHandler(MouseEventArgs e);
        public event ToolHandler ToolChanged;
        public event ToolHandler DrawImage;

        public delegate void PaintHandler(PaintEventArgs e);
        public event PaintHandler DrawGhostImage;

        private Point oldPoint;
        private Point newPoint;
        private Bitmap bmp;
        private SaveInfo saveData;
        private int startWidth;
        private int startHeight;


        public Canvas()
        {
            InitializeComponent();

            bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
            pictureBox1.Image = bmp;

            Scale = 100;
            saveData = new SaveInfo("Безымянный", true);
        }

        public Canvas(String str)
        {
            InitializeComponent();

            FileStream fs = new FileStream(str, FileMode.Open);
            bmp = new Bitmap(fs);
            fs.Close();

            Graphics g = Graphics.FromImage(bmp);

            pictureBox1.Width = bmp.Width;
            pictureBox1.Height = bmp.Height;
            pictureBox1.Image = bmp;
            this.Width = bmp.Width;
            this.Height = bmp.Height;
            startWidth = bmp.Width;
            startHeight = bmp.Height;

            Scale = 100;
            saveData = new SaveInfo(str, true);
        }

        public int CanvasWidth
        {
            get { return pictureBox1.Width; }
            set
            {
                this.Width = value + 16;  // различия в размере picturebox и формы по ширине
                pictureBox1.Width = value;
                Bitmap tbmp = new Bitmap(value, pictureBox1.Height);
                Graphics g = Graphics.FromImage(tbmp);
                g.Clear(Color.White);
                g.DrawImage(bmp, new Point(0, 0));
                bmp = tbmp;
                pictureBox1.Image = bmp;

                startWidth = bmp.Width;
            }
        }

        public int CanvasHeight
        {
            get { return pictureBox1.Height; }
            set
            {
                this.Height = value + 39;  // различия в размере picturebox и формы по высоте
                pictureBox1.Height = value;
                Bitmap tbmp = new Bitmap(pictureBox1.Width, value);
                Graphics g = Graphics.FromImage(tbmp);
                g.Clear(Color.White);
                g.DrawImage(bmp, new Point(0, 0));
                bmp = tbmp;
                pictureBox1.Image = bmp;

                startHeight = bmp.Height;
            }
        }

        public int Scale { get; set; }

        public PictureBox PicBox { get{ return pictureBox1; } }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            MainForm parent = (MainForm)ParentForm;

            if (e.Button == MouseButtons.Left)
            {
                ToolChanged?.Invoke(e);

                pictureBox1.Invalidate();
            }

            parent.SetCursorCoordinates($"X: {e.X}; Y: {e.Y}");
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            MainForm parent = (MainForm)ParentForm;
            parent.SetCursorCoordinates(string.Empty);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            oldPoint = e.Location;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DrawImage?.Invoke(e);
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawGhostImage?.Invoke(e);
        }


        public void SaveAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpg)|*.jpg";
            ImageFormat[] formats = { ImageFormat.Bmp, ImageFormat.Jpeg };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try { bmp.Save(dlg.FileName, formats[dlg.FilterIndex - 1]); }
                catch
                {
                    MessageBox.Show("Невозможно сохранить изображениe!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                MarkAsSaved();
                saveData.FileName = dlg.FileName;

            }
        }

        public void Save()
        {
            if (saveData.FileName == "Безымянный") SaveAs();
            else
            {
                try { bmp.Save(saveData.FileName); }
                catch
                {
                    MessageBox.Show("Невозможно сохранить изображениe!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                MarkAsSaved();
            }
        }


        public void DrawLine(MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.DrawLine(new Pen(MainForm.CurColor, MainForm.CurWidth), oldPoint, new Point(e.X, e.Y));
            oldPoint = e.Location;

            MarkAsUnsaved();
        }

        public void Erase(MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.DrawLine(new Pen(Color.White, MainForm.CurWidth), oldPoint, new Point(e.X, e.Y));
            oldPoint = e.Location;

            MarkAsUnsaved();
        }

        public void SetEndCoords(MouseEventArgs e)
        {
            newPoint = e.Location;
        }

        public void DrawStraightLine(MouseEventArgs e)
        {
            newPoint = e.Location;
            Graphics g = Graphics.FromImage(bmp);
            g.DrawLine(new Pen(MainForm.CurColor, MainForm.CurWidth), oldPoint, newPoint);

            MarkAsUnsaved();
        }

        public void DrawEllipse(MouseEventArgs e)
        {
            newPoint = e.Location;
            Graphics g = Graphics.FromImage(bmp);
            int width = newPoint.X - oldPoint.X;
            int height = newPoint.Y - oldPoint.Y;
            Rectangle rect = new Rectangle(oldPoint.X, oldPoint.Y, width, height);
            g.DrawEllipse(new Pen(MainForm.CurColor, MainForm.CurWidth), rect);

            MarkAsUnsaved();
        }

        public void CreateStar(MouseEventArgs e)
        { 
            int nodes = MainForm.nodesCount;
            double R = 50.0;
            double r = 25.0;
            double a = 0.0;
            double d = Math.PI / nodes;
            double radius = 0;
            PointF[] points = new PointF[2 * nodes + 1];

            for( int i = 0; i < 2 * nodes + 1; i++)
            {
                if (i % 2 == 0) radius = R;
                else radius = r;

                points[i] = new PointF((float)(e.X + radius * Math.Cos(a)), (float)(e.Y + radius * Math.Sin(a)));
                a += d;
            }

            Graphics g = Graphics.FromImage(bmp);
            g.DrawLines(new Pen(MainForm.CurColor, MainForm.CurWidth), points);


            MarkAsUnsaved();
        }

        public void GhostLine(PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(MainForm.CurColor, MainForm.CurWidth), oldPoint, newPoint);
        }

        public void GhostEllipse(PaintEventArgs e)
        {
            int width = newPoint.X - oldPoint.X;
            int height = newPoint.Y - oldPoint.Y;
            Rectangle rect = new Rectangle(oldPoint.X, oldPoint.Y, width, height);
            e.Graphics.DrawEllipse(new Pen(MainForm.CurColor, MainForm.CurWidth), rect);
        }


        private void Canvas_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!saveData.IsSaved)
            {
                var result = MessageBox.Show($"Сохранить изменения в файле {saveData.FileName}?","Предупреждение", 
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (result == DialogResult.Yes) this.SaveAs();
                else if (result == DialogResult.No) e.Cancel = false;
                else e.Cancel = true;
            }
        }


        public void SetCanvasText(int n)
        {
            this.Text = "Холст" + n;
        }

        private void MarkAsSaved()
        {
            saveData.IsSaved = true;
            this.Text += " - Сохранен";
        }

        public void MarkAsUnsaved()
        {
            saveData.IsSaved = false;
            int i = this.Text.IndexOf(' ');
            if(i != -1) this.Text = this.Text.Remove(i);
        }

        public void NullifyEvents()
        {
            ToolChanged = null;
            DrawGhostImage = null;
            DrawImage = null;
        }


        public void Rescale(double scaleCoef)
        { 
            int p1 = (int)(startWidth * scaleCoef);
            int p2 = (int)(startHeight * scaleCoef);

            Bitmap newBmp = new Bitmap(p1, p2);
            Graphics g = Graphics.FromImage(newBmp);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.White);
            g.DrawImage(bmp, 0, 0, p1, p2);
            bmp = newBmp;
            pictureBox1.Image = bmp;
        }
    }
}
