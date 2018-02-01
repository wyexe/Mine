using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public enum em_Chess_Type
        {
            em_Chess_Type_None      = 0x1,
            em_Chess_Type_Dir       = 0x2,
            em_Chess_Type_Num       = 0x3,
            em_Chess_Type_Thunder   = 0x4,
            em_Chess_Type_Flag      = 0x5
        }

        public class T_Chess
        {
            public em_Chess_Type em_Type;
            public int nNum;
            public Point Location;
            public bool bVisit;
            public T_Chess()
            {
                em_Type = em_Chess_Type.em_Chess_Type_None;
                nNum = 0;
                bVisit = false;
            }
        }

        public static Bitmap gs_bmp_Mini_Box_Show = new Bitmap(20, 20);
        public static Bitmap gs_bmp_Mini_Box_None_Show = new Bitmap(20, 20);
        public static Bitmap gs_bmp_Mini_Box_Thunder = new Bitmap(20, 20);
        public static Bitmap gs_bmp_Mini_Box_Null_Show = new Bitmap(20, 20);
        public static Bitmap gs_bmp_Mini_Flag = new Bitmap(20, 20);
        public static Bitmap gs_bmp_Mini_Num_1 = new Bitmap(20, 20);
        public static Bitmap gs_bmp_Mini_Num_2 = new Bitmap(20, 20);
        public static Bitmap gs_bmp_Mini_Num_3 = new Bitmap(20, 20);
        public static Bitmap gs_bmp_Mini_Num_4 = new Bitmap(20, 20);
        public static int gs_nRowCount = 30;
        public static int gs_nColumCount = 30;
        public const int gc_nPicLength = 20;
        public static Image g_Image_Chess = null;
        public static T_Chess[,] gs_Chess = null;
        public static bool gs_bInit = false;
        public const int gc_ThunderCount = 200;

        public void InitBitmap()
        {
            try
            {
                gs_bmp_Mini_Box_Show = new Bitmap(System.Environment.CurrentDirectory + @"\Pic\Mine_Box_Show.jpg");
                gs_bmp_Mini_Box_None_Show = new Bitmap(System.Environment.CurrentDirectory + @"\Pic\Mine_Box_NoneShow.jpg");
                gs_bmp_Mini_Box_Thunder = new Bitmap(System.Environment.CurrentDirectory + @"\Pic\Mine_Box_Thunder.jpg");
                gs_bmp_Mini_Flag = new Bitmap(System.Environment.CurrentDirectory + @"\Pic\Mine_Flag.jpg");
                gs_bmp_Mini_Num_1 = new Bitmap(System.Environment.CurrentDirectory + @"\Pic\Mine_Num_1.jpg");
                gs_bmp_Mini_Num_2 = new Bitmap(System.Environment.CurrentDirectory + @"\Pic\Mine_Num_2.jpg");
                gs_bmp_Mini_Num_3 = new Bitmap(System.Environment.CurrentDirectory + @"\Pic\Mine_Num_3.jpg");
                gs_bmp_Mini_Num_4 = new Bitmap(System.Environment.CurrentDirectory + @"\Pic\Mine_Num_4.jpg");
                gs_bmp_Mini_Box_Null_Show = new Bitmap(System.Environment.CurrentDirectory + @"\Pic\Mine_Box_Null_Show.jpg");

                Random rd = new Random();
                for (int i = 0; i < gc_ThunderCount; i++)
                {
                    int nX = rd.Next(0, gs_nRowCount);
                    int nY = rd.Next(0, gs_nColumCount);
                    if (gs_Chess[nX, nY].em_Type == em_Chess_Type.em_Chess_Type_Thunder)
                    {
                        --i;
                        continue;
                    }
                    gs_Chess[nX, nY].em_Type = em_Chess_Type.em_Chess_Type_Thunder;

                    Point[] AroundLocation = GetAroundPoint(new Point(nX, nY));
                    for (int j = 0; j < AroundLocation.Length; j++)
                    {
                        if (AroundLocation[j].X == -1)
                            continue;

                        //gs_Chess[AroundLocation[j].X, AroundLocation[j].Y].em_Type = em_Chess_Type.em_Chess_Type_Num;
                        gs_Chess[AroundLocation[j].X, AroundLocation[j].Y].nNum++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitBitmap:" +  ex.Message);
            }
        }

        public void DrawChees()
        {
            try
            {
                g_Image_Chess = new Bitmap(gs_nRowCount * gc_nPicLength, gs_nColumCount * gc_nPicLength);
                Graphics g = Graphics.FromImage(g_Image_Chess);
                Pen m_Pen = new Pen(Color.Black);
                SolidBrush brush = new SolidBrush(Color.Black);  //Defult Background was Blue

                //Draw Square
                for (int i = 0; i < gs_nRowCount/*Square Count in Row*/; i++)
                {
                    g.DrawLine(m_Pen, 0,i == 0 ? 0 : i * gc_nPicLength, (gs_nRowCount - 1) * gc_nPicLength, i * gc_nPicLength);
                    g.DrawLine(m_Pen, i == 0 ? 0 : i * gc_nPicLength, 0, i * gc_nPicLength, (gs_nColumCount - 1) * gc_nPicLength);
                }

                //Fill Chess
                for (int i = 0; i < gs_nRowCount; i++)
                {
                    for (int j = 0; j < gs_nColumCount; j++)
                    {
                        g.DrawImage(gs_bmp_Mini_Box_None_Show, i * gc_nPicLength, j * gc_nPicLength);
                    }
                }

                Graphics gg = Graphics.FromHwnd(panel1.Handle);
                //Graphics gg = bombPanel.CreateGraphics();
                gg.DrawImage(g_Image_Chess, 0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("DrawChees:" + ex.Message);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            gs_bInit = true;
            DrawChees();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gs_Chess = new T_Chess[gs_nRowCount, gs_nColumCount];
            for (int i = 0; i < gs_nRowCount; i++)
            {
                for (int j = 0; j < gs_nColumCount; j++)
                {
                    gs_Chess[i, j] = new T_Chess();
                    gs_Chess[i, j].Location = new Point(i, j);
                }
            }
            InitBitmap();
        }

        public bool FindBox_For_Point(Point pt, ref T_Chess m_Chess)
        {
            int nRow = pt.X / gc_nPicLength;
            int nCell = pt.Y / gc_nPicLength;

            //Prevent Stack OverFlow
            if (nRow >= 0 && nRow < gs_nRowCount &&
                nCell >= 0 && nCell < gs_nColumCount)
            {
                m_Chess = gs_Chess[nRow, nCell];
                return true;
            }

            return false;
        }

        public static T_Chess gs_Last_Chess = new T_Chess();
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            int nX = e.X;
            int nY = e.Y;
            T_Chess ms_Last_Chess = gs_Last_Chess;
            if (gs_bInit && FindBox_For_Point(new Point(nX, nY), ref gs_Last_Chess))
            {
                Graphics g = Graphics.FromImage(g_Image_Chess);
                if (gs_Last_Chess.em_Type == em_Chess_Type.em_Chess_Type_None)
                    g.DrawImage(gs_bmp_Mini_Box_Show, gs_Last_Chess.Location.X * gc_nPicLength, gs_Last_Chess.Location.Y * gc_nPicLength);

                if (gs_Last_Chess.Location != ms_Last_Chess.Location && ms_Last_Chess.em_Type == em_Chess_Type.em_Chess_Type_None)
                    g.DrawImage(gs_bmp_Mini_Box_None_Show, ms_Last_Chess.Location.X * gc_nPicLength, ms_Last_Chess.Location.Y * gc_nPicLength);

                //Point[] pt = GetAroundPoint(gs_Last_Chess.Location);
               // for (int i = 0; i < pt.Length; i++)
                  //  if (pt[i].X != -1)
                      //  g.DrawImage(gs_bmp_Mini_Box_Show, gs_Chess[pt[i].X, pt[i].Y].Location.X * gc_nPicLength, gs_Chess[pt[i].X, pt[i].Y].Location.Y * gc_nPicLength);
                

                Graphics gg = Graphics.FromHwnd(panel1.Handle);
                gg.DrawImage(g_Image_Chess, 0, 0);
            }
        }

        public static Point[] GetAroundPoint(Point m_Location)
        {
            Point [] m_Point = new Point[8];
            for (int i = 0; i < 8; i++)
                m_Point[i] = new Point(-1, -1);

            //Upper Left Corner
            if (m_Location.X > 0 && m_Location.Y > 0)
            {
                m_Point[0].X = m_Location.X - 1;
                m_Point[0].Y = m_Location.Y - 1;
            }
            //Top
            if (m_Location.X > 0)
            {
                m_Point[1].X = m_Location.X - 1;
                m_Point[1].Y = m_Location.Y;
            }
            //Upper Right Corner
            if (m_Location.X > 0 && m_Location.Y + 1 < gs_nColumCount)
            {
                m_Point[2].X = m_Location.X - 1;
                m_Point[2].Y = m_Location.Y + 1;
            }
            //Left
            if (m_Location.Y > 0)
            {
                m_Point[3].X = m_Location.X;
                m_Point[3].Y = m_Location.Y - 1;
            }
            //Right
            if (m_Location.Y + 1 < gs_nColumCount)
            {
                m_Point[4].X = m_Location.X;
                m_Point[4].Y = m_Location.Y + 1;
            }
            //Lower Left Corner
            if (m_Location.X + 1 < gs_nRowCount && m_Location.Y > 0)
            {
                m_Point[5].X = m_Location.X + 1;
                m_Point[5].Y = m_Location.Y - 1;
            }
            //Down
            if (m_Location.X + 1 < gs_nRowCount)
            {
                m_Point[6].X = m_Location.X + 1;
                m_Point[6].Y = m_Location.Y;
            }
            //Lower Right Corner
            if (m_Location.X + 1 < gs_nRowCount && m_Location.Y + 1 < gs_nColumCount)
            {
                m_Point[7].X = m_Location.X + 1;
                m_Point[7].Y = m_Location.Y + 1;
            }

            return m_Point;
        }

        public void DFS(int nRow, int nColum)
        {
            try
            {
                //Set Current Box was Dir
                //if (gs_Chess[nRow, nColum].em_Type == em_Chess_Type.em_Chess_Type_None)
                gs_Chess[nRow, nColum].em_Type = em_Chess_Type.em_Chess_Type_Dir;

                Point[] AroundLocation = GetAroundPoint(new Point(nRow, nColum));
                for (int i = 0; i < AroundLocation.Length; i++)
                {
                    if (AroundLocation[i].X == -1)
                        continue;

                    
                    if (gs_Chess[AroundLocation[i].X, AroundLocation[i].Y].em_Type == em_Chess_Type.em_Chess_Type_None)
                    {
                        if (gs_Chess[AroundLocation[i].X, AroundLocation[i].Y].nNum == 0)
                            DFS(AroundLocation[i].X, AroundLocation[i].Y);
                        else
                            gs_Chess[AroundLocation[i].X, AroundLocation[i].Y].em_Type = em_Chess_Type.em_Chess_Type_Num;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void BFS(int nRow, int nColum)
        {

        }

        public bool IsOver()
        {
            //if None_Show Count == Thunder Count

            int nUnDirCount = 0;
            for (int i = 0; i < gs_nRowCount; i++)
                for (int j = 0; j < gs_nColumCount; j++)
                    if (gs_Chess[i,j].em_Type == em_Chess_Type.em_Chess_Type_Flag)
                        ++nUnDirCount;


            return false;
        }

        public void DrawChess_Check()
        {
            Graphics g = Graphics.FromImage(g_Image_Chess);
            SolidBrush brush = new SolidBrush(Color.Blue);  //Defult Background was Blue
            int nBoxCount = 0;
            //Fill Chess
            for (int i = 0; i < gs_nRowCount; i++)
            {
                for (int j = 0; j < gs_nColumCount; j++)
                {
                    Image m_Image = null;
                    if (gs_Chess[i, j].nNum != 0 && gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_Dir)
                        gs_Chess[i, j].em_Type = em_Chess_Type.em_Chess_Type_Num;

                    nBoxCount++;

                    if (gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_None)
                        m_Image = gs_bmp_Mini_Box_None_Show;
                    else if (gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_Dir)
                        m_Image = gs_bmp_Mini_Box_Null_Show;
                    else if (gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_Flag)
                        m_Image = gs_bmp_Mini_Flag;
                    else if (gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_Thunder)
                        m_Image = gs_bmp_Mini_Box_None_Show;
                       // m_Image = gs_bmp_Mini_Box_Thunder;
                    else if (gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_Num && gs_Chess[i, j].nNum == 1)
                        m_Image = gs_bmp_Mini_Num_1;
                    else if (gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_Num && gs_Chess[i, j].nNum == 2)
                        m_Image = gs_bmp_Mini_Num_2;
                    else if (gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_Num && gs_Chess[i, j].nNum == 3)
                        m_Image = gs_bmp_Mini_Num_3;
                    else if (gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_Num && gs_Chess[i, j].nNum == 4)
                        m_Image = gs_bmp_Mini_Num_4;
                   // else
                        //MessageBox.Show("Error");

                    if (gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_None || gs_Chess[i, j].em_Type == em_Chess_Type.em_Chess_Type_Thunder)
                        nBoxCount--;

                    if (m_Image == null)
                    {
                        FontFamily fontFamily = new FontFamily("Arial");
                        Font font = new Font(fontFamily, 16, FontStyle.Regular, GraphicsUnit.Pixel);
                        g.DrawImage(gs_bmp_Mini_Box_Null_Show, i * gc_nPicLength, j * gc_nPicLength);
                        g.DrawString("5", font, new SolidBrush(Color.Red), i * gc_nPicLength, j * gc_nPicLength);
                        continue;
                    }
                    g.DrawImage(m_Image, i * gc_nPicLength, j * gc_nPicLength);
                }
            }

            Graphics gg = Graphics.FromHwnd(panel1.Handle);
            gg.DrawImage(g_Image_Chess, 0, 0);

            if (nBoxCount == gs_nColumCount * gs_nRowCount - gc_ThunderCount)
            {
                MessageBox.Show("卧槽!竟然做完了!");
                gs_bInit = false;
            }
        }

        public void CheckAround_For_Num(int nRow, int nColum)
        {
            
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            int nX = e.X;
            int nY = e.Y;
            T_Chess m_Chess = new T_Chess();
            if (gs_bInit && FindBox_For_Point(new Point(nX, nY), ref m_Chess))
            {
                //dired
                label1.Text = "X=" + m_Chess.Location.X.ToString() + "Y=" + m_Chess.Location.Y.ToString();
                if (m_Chess.em_Type == em_Chess_Type.em_Chess_Type_Dir)
                    return;

                //Show Thunder
                if (m_Chess.em_Type == em_Chess_Type.em_Chess_Type_Num)
                    return;

                if(e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    //Draw Flag
                    Image m_Image = null;
                    if (m_Chess.em_Type == em_Chess_Type.em_Chess_Type_None)
                    {
                        m_Image = gs_bmp_Mini_Flag;
                        gs_Chess[m_Chess.Location.X, m_Chess.Location.Y].em_Type = em_Chess_Type.em_Chess_Type_Flag;
                    }
                    else if (m_Chess.em_Type == em_Chess_Type.em_Chess_Type_Flag)
                    {
                        m_Image = gs_bmp_Mini_Box_None_Show;
                        gs_Chess[m_Chess.Location.X, m_Chess.Location.Y].em_Type = em_Chess_Type.em_Chess_Type_None;
                    }
                    else
                    {
                        return;
                    }

                   
                    Graphics g = Graphics.FromImage(g_Image_Chess);
                    g.DrawImage(m_Image, gs_Last_Chess.Location.X * gc_nPicLength, gs_Last_Chess.Location.Y * gc_nPicLength);

                    Graphics gg = Graphics.FromHwnd(panel1.Handle);
                    gg.DrawImage(g_Image_Chess, 0, 0);
                    return;
                }

                //is Thunder?
                if(m_Chess.em_Type == em_Chess_Type.em_Chess_Type_Thunder)
                {
                    MessageBox.Show("你踩到雷了……兄台");
                    return;
                }

                DFS(m_Chess.Location.X, m_Chess.Location.Y);
                DrawChess_Check();
            }
        }

        List<T_Chess> NullShowList = new List<T_Chess>();
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            int nX = e.X;
            int nY = e.Y;
            T_Chess m_Chess = new T_Chess();
            if (gs_bInit && FindBox_For_Point(new Point(nX, nY), ref m_Chess))
            {
                if (m_Chess.em_Type != em_Chess_Type.em_Chess_Type_Num)
                    return;

                NullShowList.Clear();
                Point[] AroundPoint = GetAroundPoint(new Point(m_Chess.Location.X, m_Chess.Location.Y));
                Graphics g = Graphics.FromImage(g_Image_Chess);
                for (int i = 0; i < AroundPoint.Length; i++)
                {
                    if (AroundPoint[i].X != -1)
                    {
                        T_Chess t = gs_Chess[AroundPoint[i].X, AroundPoint[i].Y];
                        if (t.em_Type == em_Chess_Type.em_Chess_Type_None)
                        {
                            NullShowList.Add(t);
                            g.DrawImage(gs_bmp_Mini_Box_Null_Show, t.Location.X * gc_nPicLength, t.Location.Y * gc_nPicLength);
                        }
                    }
                }
                Graphics gg = Graphics.FromHwnd(panel1.Handle);
                gg.DrawImage(g_Image_Chess, 0, 0);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (gs_bInit)
            {
                DrawChess_Check();
            }
        }
    }
}
