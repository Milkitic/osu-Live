using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

using osu_live.Layer;

namespace osu_live
{
    public partial class Form2 : Form
    {
        Graphics display_g;
        Graphics display_g_2;
        int canvas_width = Constant.Canvas.Width,
        canvas_height = Constant.Canvas.Height;

        public Form2()
        {
            InitializeComponent();
        }
        bool flag = false;
        private void button1_Click(object sender, EventArgs e)
        {
            string a, b, c;
            if (flag)
            {
                a = @"C:\Users\acer\source\repos\osu-Live\osu!live\bin\Debug\332201 nomico - DOWNFORCE - BRAVE OUT\nomico  DOWNFORCE - BRAVE OUT (yf_bmp) [Hard].osu";
                b = "BRAVE OUT";
                c = "nomico / DOWNFORCE";
                flag = false;
            }
            else
            {
                a = @"C:\Users\acer\source\repos\osu-Live\osu!live\bin\Debug\588976 Lucas Fader - Main Theme - Dubstep Remix\Lucas Fader - Main Theme - Dubstep Remix (Strategas) [Expert Neo Cortex].osu";
                b = "Main Theme - Dubstep Remix";
                c = "Lucas Fader";
                flag = true;
            }
            File.WriteAllText(@"stream\Files\l_OsuFileLocation", a);
            File.WriteAllText(@"stream\Files\l_TitleUnicode", b);
            File.WriteAllText(@"stream\Files\l_ArtistUnicode", c);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 fm3 = new Form3();
            fm3.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Form1.l_PA.Border = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Form1.l_PA.Rotate = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Form1.ShowFPS = checkBox3.Checked;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
          
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            //Form1..Enabled = false;
            Form1.l_PA.Initialize(trackBar1.Value);
            //timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Form1.idleStatus == IdleStatus.Stopped) return;
            display_g = canvas.CreateGraphics();
            display_g.Clear(Color.White);
            display_g.DrawImage(Form1.l_BG.Bitmap, 0, 0, canvas.Width, canvas.Height);
            display_g.Dispose();

            var a = Form1.l_FG.Rec_Panel;
            float ratio = 0;
            ratio = (float)a.Width / canvas2.Width;
            a.Width = canvas2.Width;
            a.Height = (int)(a.Height / ratio);
            a.Y = (int)(a.Y / ratio);
            display_g_2 = canvas2.CreateGraphics();
            display_g_2.Clear(Color.White);
            display_g_2.DrawImage(Form1.l_FG.Bitmap, a);
            display_g_2.Dispose();

        }
    }
}
