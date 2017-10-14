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

namespace osu_live
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        bool flag = false;
        private void button1_Click(object sender, EventArgs e)
        {
            string a,b,c;
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
            File.WriteAllText(@"Files\l_OsuFileLocation", a);
            File.WriteAllText(@"Files\l_TitleUnicode", b);
            File.WriteAllText(@"Files\l_ArtistUnicode", c);
        }
    }
}
