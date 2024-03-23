using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        static bool mouse = false, tool = true, opens = false;
        static int UndoIndex = 0;
        static int varUndo = 1;
        static Size size, sizeP;
        static int x = 0, y = 0;
        static string NamePen = "Перо";
        static Bitmap pikcha;
        static Color cal;
        float x1, y1;
        static Bitmap[] spik = new Bitmap[0];
        Pen pen, penF;
        Brush br;

        Graphics g;

        //метод сглаживания пера
        private void RPan()
        {
            if (tool)
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            }
        }

        public Form1()
        {
            InitializeComponent();
            size = new Size(this.Width, this.Height);
            sizeP = new Size(pictureBox1.Width, pictureBox1.Height);
        }

        // выбор цвета
        private void Choice_color(object sender, EventArgs e)
        {
            if (tool)
            {
                button1.BackColor = ((Button)sender).BackColor;
                cal = button1.BackColor;
                pen.Color = button1.BackColor;
                br = new SolidBrush(Color.FromArgb(104, button1.BackColor));
                RPan();
            }
            else
            {
                button1.BackColor = ((Button)sender).BackColor;
                penF.Color = button1.BackColor;
            }
        }

        // изменение толщины
        private void Choice_size(object sender, EventArgs e)
        {
            if (tool)
            {
                pen.Width = trackBar1.Value;
                RPan();
            }
            else
            {
                penF.Width = trackBar1.Value;
            }
        }

        //метод выбора цвета из палитры
        private void Choice_colorInDialog(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                if (tool)
                {
                    button1.BackColor = colorDialog1.Color;
                    cal = button1.BackColor;
                    pen.Color = button1.BackColor;
                    br = new SolidBrush(Color.FromArgb(104, button1.BackColor));
                    RPan();
                }
                else
                {
                    button1.BackColor = colorDialog1.Color;
                    penF.Color = button1.BackColor;
                }
            }
        }

        // метод обработки выбора инструмента
        private void Choice_tool(object sender, EventArgs e)
        {
            tool = true;
            groupBox4.Visible = true;
            groupBox1.Visible = true;
            switch (((Button)sender).Text)
            {
                case ("Перо"):
                    NamePen = ((Button)sender).Text;
                    pen.Color = button1.BackColor;
                    break;
                case ("Кисть"):
                    NamePen = ((Button)sender).Text;
                    br = new SolidBrush(Color.FromArgb(104, button1.BackColor));
                    break;
                case ("Ластик"):
                    NamePen = ((Button)sender).Text;
                    pen.Color = pictureBox1.BackColor;
                    groupBox1.Visible = false;

                    break;
                case ("Заливка"):
                    NamePen = ((Button)sender).Text;
                    pen.Color = pictureBox1.BackColor;
                    groupBox4.Visible = false;
                    break;
            }
        }

        //метод выбора фигуры
        private void Choice_Figure(object sender, EventArgs e)
        {
            tool = false;
            RPan();
            groupBox4.Visible = true;
            groupBox1.Visible = true;
            switch (((PictureBox)sender).Name)
            {
                case ("star"):
                    NamePen = "Звезда";
                    penF.Color = button1.BackColor;
                    break;
                case ("circle"):
                    NamePen = "Круг";
                    penF.Color = button1.BackColor;
                    break;
                case ("triange"):
                    NamePen = "Треугольник";
                    penF.Color = button1.BackColor;
                    break;
                case ("square"):
                    NamePen = "Квадрат";
                    penF.Color = button1.BackColor;
                    break;
                case ("pentagon"):
                    NamePen = "Пятиугольник";
                    penF.Color = button1.BackColor;
                    break;
            }
        }

        //меню - файл

        // метод создания
        private void Create(object sender, EventArgs e)
        {
            if (this.label1.Text == "")
            {
                Start();
            }
            else
            {
                var mess = MessageBox.Show("Сохранить перед закрытием?", this.Text, MessageBoxButtons.YesNoCancel);
                switch (mess)
                {
                    case DialogResult.Yes:
                        Save(sender, e); 
                        Start();
                        break;
                    case DialogResult.No:
                        Start();
                        break;
                }

            }
        }

        // процедура для создания
        private void Start()
        {
            label1.Text = null; this.Text = null;
            varUndo = 1;
            UndoIndex = 0;
            Array.Resize(ref spik, 0);
            pictureBox1.Image = null;
            if (!opens)
            {
                pictureBox1.Width += (this.Size.Width - size.Width);
                pictureBox1.Height += (this.Size.Height - size.Height);
            }
            else
            {
                pictureBox1.Width =  sizeP.Width;
                pictureBox1.Height =  sizeP.Height;
            }
            //pictureBox1.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right);
            pictureBox1.Visible = true;
            size = this.Size;

            br = new SolidBrush(button1.BackColor);
            pen = new Pen(button1.BackColor, 10);
            penF = new Pen(button1.BackColor, 10);
            RPan();

            pikcha = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Array.Resize(ref spik, spik.Length + 1);
            spik[UndoIndex] = new Bitmap(pikcha.Width, pikcha.Height);
            UndoIndex++;

            g = Graphics.FromImage(pikcha);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = pikcha;
            //pictureBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            opens = false;
        }

        //процедура для открытия
        private void open(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "*.jpg";
            openFileDialog1.Filter = "JPG Files|*.jpg";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap im = new Bitmap(Image.FromFile(openFileDialog1.FileName));
                pictureBox1.Visible = true;
                label1.Text = openFileDialog1.FileName;
                this.Text = label1.Text;
                varUndo = 1;
                UndoIndex = 0;
                Array.Resize(ref spik, 0);
                
                if (im.Width > 1600 | im.Height > 900)
                {
                    pictureBox1.Width = im.Width > im.Height ? 1600 : (int)((double)(im.Width / (double)im.Height) * 900);
                    pictureBox1.Height = im.Width > im.Height ? (int)(((double)im.Height / (double)im.Width) * 1600) : 900;
                }
                else
                {
                    pictureBox1.Width = im.Width;
                    pictureBox1.Height = im.Height;
                }
                im = new Bitmap(Image.FromFile(openFileDialog1.FileName), pictureBox1.Size);
                //pictureBox1.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right);
                pictureBox1.Image = im;
                pictureBox1.Visible = true;

                br = new SolidBrush(button1.BackColor);
                pen = new Pen(button1.BackColor, 10);
                penF = new Pen(button1.BackColor, 10);
                RPan();

                
                pikcha = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pikcha = (Bitmap)pictureBox1.Image;
                Array.Resize(ref spik, spik.Length + 1);
                spik[UndoIndex] = new Bitmap(pikcha.Width, pikcha.Height);
                UndoIndex++;
                opens = true;
            }
        }

        //Метод открытия файла
        private void Open_Click(object sender, EventArgs e)
        {
            if (this.label1.Text == "")
            {
                open(sender, e);
            }
            else
            {
                var mess = MessageBox.Show("Сохранить перед закрытием?", this.Text, MessageBoxButtons.YesNoCancel);
                switch (mess)
                {
                    case DialogResult.Yes:
                        Save(sender, e);
                        break;
                    case DialogResult.No:
                        open(sender, e);
                        break;
                }

            }
        }


        //метод для кодировки
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        //метод для сохранения как
        private void SaveAs(object sender, EventArgs e)
        {
            if (this.label1.Text != "")
            {
                saveFileDialog1.DefaultExt = "*.jpg";
                saveFileDialog1.Filter = "JPG Files|*.jpg";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName.Length > 0)
                {
                    label1.Text = saveFileDialog1.FileName;
                    EncoderParameter P = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                    EncoderParameters parameters = new EncoderParameters(1);
                    parameters.Param[0] = P;
                    pictureBox1.Image.Save(label1.Text, GetEncoderInfo("image/jpeg"), parameters);
                    this.Text = saveFileDialog1.FileName;
                }
            }
        }

        //метод для сохранения
        private void Save(object sender, EventArgs e)
        {
            EncoderParameter P = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = P;
            if (this.label1.Text == "")
            {
                saveFileDialog1.DefaultExt = "*.jpg";
                saveFileDialog1.Filter = "JPG Files|*.jpg";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName.Length > 0)
                {
                    label1.Text = saveFileDialog1.FileName;
                    pictureBox1.Image.Save(label1.Text, GetEncoderInfo("image/jpeg"), parameters);
                    this.Text = saveFileDialog1.FileName;
                }
            }
            else
            {
                pictureBox1.Image.Save(label1.Text, GetEncoderInfo("image/jpeg"), parameters);
            }
        }

        //меню - правка

        //метод очистки холста
        private void Clear(object sender, EventArgs e)
        {
            g = Graphics.FromImage(pikcha);
            Rectangle clone = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            Array.Resize(ref spik, spik.Length + 1);
            spik[UndoIndex] = new Bitmap(pikcha.Width, pikcha.Height);
            spik[UndoIndex] = pikcha.Clone(clone, pikcha.PixelFormat);
            UndoIndex++;
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = pikcha;
        }

        //метод отмены
        private void Cancel(object sender, EventArgs e)
        {
            try
            {
                g = Graphics.FromImage(pikcha);
                if (spik.Length - varUndo > 0)
                {
                    pikcha = spik[spik.Length - 1 - varUndo];
                    pictureBox1.Image = pikcha;
                    varUndo++;
                }
            }
            catch { }
        }

        //метод повтора действия
        private void Repeat(object sender, EventArgs e)
        {
            if (spik.Length - varUndo > 0 & varUndo > 1)
            {
                g = Graphics.FromImage(pikcha);
                Rectangle clone = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
                Array.Resize(ref spik, spik.Length + 1);
                pikcha = spik[spik.Length - varUndo].Clone(clone, pikcha.PixelFormat);
                varUndo--;
                pictureBox1.Image = pikcha;
            }

        }

        //метод рисования звезды
        private void Star(double X, double Y, double al)
        {
            int n = 5;               // число вершин
            double alpha = al;        // поворот
            double x0 = x < X ? x + ((X - x) / 2) : X + ((x - X) / 2), y0 = y < Y ? y + ((Y - y) / 2) : Y + ((y - Y) / 2); // центр
            double r = Math.Abs(x - X) < Math.Abs(y - Y) ? Math.Abs(y - y0) : Math.Abs(x - x0), R = r / 2;   // радиусы

            PointF[] points = new PointF[2 * n + 1];
            double a = alpha, da = Math.PI / n, l;
            for (int k = 0; k < 2 * n + 1; k++)
            {
                l = k % 2 == 0 ? r : R;
                points[k] = new PointF((float)(x0 + l * Math.Cos(a)), (float)(y0 + l * Math.Sin(a)));
                a += da;
            }
            g.DrawLines(penF, points);
        }


        //метод детекта отжатия мыши
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouse)
            {
                //создание звезды
                if (NamePen == "Звезда")
                {
                    Star(e.X, e.Y, 0.95);
                    Star(e.X, e.Y, 2.21);
                }
                //создание элипса
                if (NamePen == "Круг")
                {
                    float x0 = x < e.X ? x : e.X, y0 = y < e.Y ? y : e.Y, lx = Math.Abs(e.X - x), ly = Math.Abs(e.Y - y);
                    g.DrawEllipse(penF, x0, y0, lx, ly);
                }
                //создание треугольника
                if (NamePen == "Треугольник")
                {
                    Point[] p =
                    {
                        new Point(x < e.X ? x : e.X, y < e.Y ? e.Y : y),
                        new Point(x < e.X ? x + ((e.X - x) / 2) : e.X + ((x - e.X) / 2), y < e.Y ? y : e.Y),
                        new Point(x < e.X ? e.X : x, y < e.Y ? e.Y : y),
                        new Point(x < e.X ? x : e.X, y < e.Y ? e.Y : y)
                    };
                    g.DrawLines(penF, p);
                    Array.Clear(p, 0, p.Length);
                    p = new Point[4]
                    {
                        new Point(x < e.X ? x + ((e.X - x) / 2) : e.X + ((x - e.X) / 2), y < e.Y ? y : e.Y),
                        new Point(x < e.X ? e.X : x, y < e.Y ? e.Y : y),
                        new Point(x < e.X ? x : e.X, y < e.Y ? e.Y : y),
                        new Point(x < e.X ? x + ((e.X - x) / 2) : e.X + ((x - e.X) / 2), y < e.Y ? y : e.Y)
                    };
                    g.DrawLines(penF, p);
                }
                //создание прямоугольника
                if (NamePen == "Квадрат")
                {
                    float x0 = x < e.X ? x : e.X, y0 = y < e.Y ? y : e.Y, lx = Math.Abs(e.X - x), ly = Math.Abs(e.Y - y);
                    g.DrawRectangle(penF, x0, y0, lx, ly);
                }
                //создание пятиугольника
                if (NamePen == "Пятиугольник")
                {
                    Point[] p =
                    {
                        new Point(x < e.X ? e.X : x, y < e.Y ? Convert.ToInt32(y + ((e.Y - y)*0.4)) : Convert.ToInt32(e.Y + ((y - e.Y)*0.4))),
                        new Point(x < e.X ? e.X - ((e.X - x)/4) : x - ((x - e.X)/4), y > e.Y ? y : e.Y),
                        new Point(x < e.X ? x + ((e.X - x)/4) : e.X + ((x - e.X)/4), y > e.Y ? y : e.Y),
                        new Point(x > e.X ? e.X : x, y < e.Y ? Convert.ToInt32(y + ((e.Y - y) * 0.4)) : Convert.ToInt32(e.Y + ((y - e.Y)*0.4))),
                        new Point(x < e.X ? x + ((e.X - x) / 2) : e.X + ((x - e.X) / 2), y < e.Y ? y : e.Y),
                        new Point(x < e.X ? e.X : x, y < e.Y ? Convert.ToInt32(y + ((e.Y - y) * 0.4)) : Convert.ToInt32(e.Y + ((y - e.Y) * 0.4)))
                    };
                    g.DrawLines(penF, p);
                    Array.Clear(p, 0, p.Length);
                    p = new Point[]
                    {
                        new Point(x < e.X ? x + ((e.X - x) / 2) : e.X + ((x - e.X) / 2), y < e.Y ? y : e.Y),
                        new Point(x < e.X ? e.X : x, y < e.Y ? Convert.ToInt32(y + ((e.Y - y) * 0.4)) : Convert.ToInt32(e.Y + ((y - e.Y) * 0.4))),
                        new Point(x < e.X ? e.X - ((e.X - x)/4) : x - ((x - e.X)/4), y > e.Y ? y : e.Y),
                        new Point(x < e.X ? x + ((e.X - x)/4) : e.X + ((x - e.X)/4), y > e.Y ? y : e.Y),
                        new Point(x > e.X ? e.X : x, y < e.Y ? Convert.ToInt32(y + ((e.Y - y) * 0.4)) : Convert.ToInt32(e.Y + ((y - e.Y) * 0.4))),
                        new Point(x < e.X ? x + ((e.X - x) / 2) : e.X + ((x - e.X) / 2), y < e.Y ? y : e.Y)
                    };
                    g.DrawLines(penF, p);
                }
                pictureBox1.Image = pikcha;
                x = 0; y = 0;

                //сохранения изменений
                Rectangle clone = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
                if (e.Button == MouseButtons.Left)
                {
                    Array.Resize(ref spik, spik.Length + 1);
                    spik[UndoIndex] = new Bitmap(pikcha.Width, pikcha.Height);
                    spik[UndoIndex] = pikcha.Clone(clone, pikcha.PixelFormat);
                    UndoIndex++;
                }
                mouse = false;
            }
        }


        //метод детекта нажатия мыши
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (varUndo > 1) { Array.Resize(ref spik, 0); UndoIndex = 0; varUndo = 1; }
            mouse = true;
            if (NamePen != "Перо" | NamePen != "Кисть" | NamePen != "Ластик")
            {
                x = e.X; y = e.Y;
            }
            if (NamePen == "Заливка")
            {
                Fill();
            }
        }

        //метод обработки движения мыши
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            g = Graphics.FromImage(pikcha);
            if (e.Button == MouseButtons.Left)
            {
                if (NamePen == "Кисть")
                {
                    g.FillEllipse(br, e.X - (trackBar1.Value / 2), e.Y - (trackBar1.Value / 2), trackBar1.Value, trackBar1.Value);
                    pictureBox1.Image = pikcha;
                }
                if (NamePen == "Ластик")
                {
                    g.DrawLine(pen, x1, y1, e.X, e.Y);
                    pictureBox1.Image = pikcha;
                }
                if (NamePen == "Перо")
                {
                    g.DrawLine(pen, x1, y1, e.X, e.Y);
                    pictureBox1.Image = pikcha;
                }
            }
            x1 = e.X;
            y1 = e.Y;
        }

        //Метод заливки изображения
        public static void Fill()
        {
            using (LowLevelImageHandler llih = new LowLevelImageHandler(pikcha))
            {
                Point start = new Point(x, y);
                llih.LockImage();
                var oldcolor = llih.GetLockedPixel(start);
                if (oldcolor.ToArgb() == cal.ToArgb()) { return; }
                LinkedList<Point> check = new LinkedList<Point>();
                check.AddLast(start);
                while (check.Count > 0)
                {
                    Point cur = check.First.Value;
                    check.RemoveFirst();
                    foreach (Point off in pattern)
                    {
                        Point next = new Point(cur.X + off.X, cur.Y + off.Y);
                        if (llih.ContainsPoint(next))
                        {
                            if (llih.GetLockedPixel(next).ToArgb() == oldcolor.ToArgb())
                            {
                                check.AddLast(next);
                                llih.SetLockedPixel(next, cal);
                            }
                        }
                    }
                }
                llih.SaveAndUnlock();
            }
        }

        private static Point[] pattern = new Point[] {
                new Point(0, -1), new Point(0, 1),
                new Point(-1, 0), new Point(1, 0)};
    }
}





public class LowLevelImageHandler : IDisposable //класс для низкоуровневой обработки изображений
{
    private Bitmap img; //рисунок
    private BitmapData bmpData; //объект BitmapData, нужный для блокировки изображения в памяти
    private int byteLen = 4;
    private byte[] bitmapBuffer; //буфер, с которым работаем
    private Rectangle imageRect;
    public int ImageWidth { get { return img.Width; } }
    public int ImageHeigth { get { return img.Height; } }
    public Rectangle ImageRectangle { get { return imageRect; } }
    public LowLevelImageHandler(Bitmap img) { this.img = img; imageRect = new Rectangle(0, 0, img.Width, img.Height); }

    // Блокирует изображение в памяти и копирует байты, из которых оно состоит, в массив
    public void LockImage()
    {
        bmpData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, img.PixelFormat);
        bitmapBuffer = new byte[bmpData.Stride * bmpData.Height];
        Marshal.Copy(bmpData.Scan0, bitmapBuffer, 0, bitmapBuffer.Length);
        byteLen = bmpData.Stride / bmpData.Width;
    }

    // Блокирует изображение в памяти и копирует байты, из которых оно состоит, в массив
    public bool ContainsPoint(Point pnt)
    {
        return imageRect.Contains(pnt);
    }

    // Получает цвет пикселя с заблокированного в памяти изображения.
    // name="location" Координаты запрашиваемого пикселя
    public Color GetLockedPixel(Point location)
    {
        Color col;
        try
        {
            int index = bmpData.Stride * location.Y + location.X * byteLen;
            col = Color.FromArgb(bitmapBuffer[index + 2], bitmapBuffer[index + 1], bitmapBuffer[index + 0]);
        }
        catch (IndexOutOfRangeException) { throw new ArgumentException("Запрашеваемый находится за границами рисунка"); }
        catch (NullReferenceException) { throw new ArgumentException("В памяти не заблокировано изображение"); }
        return col;
    }

    // Получает цвет пикселя с заблокированного в памяти изображения.
    // name="x" Координата x запрашиваемого пикселя
    // name="y" Координата y запрашиваемого пикселя
    public Color GetLockedPixel(int x, int y)
    {
        return GetLockedPixel(new Point(x, y));
    }

    // Задает цвет пикселя заблокированного в памяти изображения.
    // name="x"Координаты задаваемого пикселя
    public void SetLockedPixel(Point location, Color col)
    {
        try
        {
            int index = bmpData.Stride * location.Y + location.X * byteLen;
            bitmapBuffer[index + 2] = col.R;
            bitmapBuffer[index + 1] = col.G;
            bitmapBuffer[index + 0] = col.B;
        }
        catch (IndexOutOfRangeException) { throw new ArgumentException("Запрашеваемый находится за границами рисунка"); }
        catch (NullReferenceException) { throw new ArgumentException("В памяти не заблокировано изображение"); }
    }


    //Задает цвет пикселя заблокированного в памяти изображения.
    // name="x">Координата x задаваемого пикселя
    // name="y">Координата y задаваемого пикселя
    public void SetLockedPixel(int x, int y, Color col)
    {
        SetLockedPixel(new Point(x, y), col);
    }


    // Сохраняет изменения и разблокирует изображение.
    public void SaveAndUnlock()
    {
        try
        {
            Marshal.Copy(bitmapBuffer, 0, bmpData.Scan0, bitmapBuffer.Length);
            img.UnlockBits(bmpData);
        }
        catch (NullReferenceException) { throw new InvalidOperationException("Изображение не заблокированно в памяти"); }
        catch (ArgumentException) { throw new InvalidOperationException("Изображение не заблокированно в памяти"); }
    }


    // Разблокирует изображение без сохранения изменений.
    public void UnlockWInthoutSaving()
    {
        try
        {
            img.UnlockBits(bmpData);
        }
        catch (NullReferenceException) { throw new InvalidOperationException("Изображение не заблокированно в памяти"); }
        catch (ArgumentException) { throw new InvalidOperationException("Изображение не заблокированно в памяти"); }
    }

    #region Члены IDisposable
    void IDisposable.Dispose()
    {
        try { UnlockWInthoutSaving(); }
        catch { }
    }

    #endregion
}