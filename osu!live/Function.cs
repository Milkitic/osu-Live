using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;

namespace osu_live
{
    class SceneListen
    {
        public static List<Image> BGList { get; set; } = new List<Image>();

        public static void LoadBG()
        {
            DirectoryInfo di = new DirectoryInfo("BG");
            FileInfo[] files = di.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension.ToLower() == ".png" || file.Extension.ToLower() == ".jpg" || file.Extension.ToLower() == ".jpeg")
                {
                    BGList.Add(new Bitmap(file.FullName));
                }
            }

        }
        static Random rnd = new Random();
        static Image BG;
        public static Image GetMapBG(FileInfo fi)
        {
            string root = File.ReadAllText(fi.FullName);
            string bgName;
            try
            {
                StreamReader sr;
                sr = new StreamReader(root);
                string line = sr.ReadLine();
                while (line != null && line != @"//Background and Video events")
                {
                    line = sr.ReadLine();
                }
                string nextLine = sr.ReadLine();
                if (nextLine.Substring(0, 2) != "//")
                {
                    bgName = nextLine.Split('"')[1];
                    string[] tmp = bgName.Split('.');
                    if (tmp[tmp.Length - 1] != "png" && tmp[tmp.Length - 1] != "jpg")
                    {
                        nextLine = sr.ReadLine();
                        if (nextLine.Substring(0, 2) == "//")
                            throw new Exception();
                        bgName = nextLine.Split('"')[1];
                        string[] tmp2 = bgName.Split('.');
                        if ((tmp2[tmp2.Length - 1] != "png" && tmp2[tmp2.Length - 1] != "jpg"))
                            throw new Exception();
                    }

                }
                else
                    bgName = null;
                sr.Close();
            }
            catch
            {
                bgName = null;
            }

            if (bgName != null)
                try
                {
                    return new Bitmap(new FileInfo(root).DirectoryName + "\\" + bgName);
                }
                catch
                {
                    BG = null;
                }
            else BG = null;
            if (BG == null)
            {
                if (BGList.Count > 1) BG = BGList[rnd.Next(0, BGList.Count )];
                else BG = BGList[0];
            }
            return BG;
        }

        public static string GetMapInfo(string root)
        {
            try
            {
                return File.ReadAllText(root);
            }
            catch
            { }
            return "";
        }

    }
}
