using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BattleCity
{
    public partial class MainForm : Form
    {
        // 
        // Embed Font
        // 
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbfont, uint cbfont, IntPtr pdv, [In] ref uint pcFonts);
        FontFamily fontFamily;
        Font font;

        public static List<Tank> tanks;
        public static Tank tank;
        public int [,] map;

        public MainForm()
        {
            InitializeComponent();          
            newGame();
        }

        private void newGame()
        {
            map = new int[26, 26];
            tanks = new List<Tank>();
            tank = new Tank();
        }

        private void Update(object sender, EventArgs e)
        {
            gamePanel.Invalidate();
            Console.WriteLine(tanks.Count);
        }

        private void gamePaint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            canvas.ResetTransform();
            canvas.TranslateTransform(0, 0);
            canvas.FillRectangle(Brushes.Black, 0, 0, 416, 416);

            canvas.ResetTransform();
            canvas.TranslateTransform(0, 0);
            canvas.FillRectangle(Brushes.White, 416, 0, 128, 416);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            loadFont();
        }

        private void loadFont()
        {
            byte[] fontArray = BattleCity.Properties.Resources.prstart;
            int dataLength = BattleCity.Properties.Resources.prstart.Length;
            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontArray, 0, ptrData, dataLength);
            uint cFonts = 0;
            AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddMemoryFont(ptrData, dataLength);
            Marshal.FreeCoTaskMem(ptrData);
            fontFamily = pfc.Families[0];
            font = new Font(fontFamily, 13f, FontStyle.Regular);
        }
    }
}
