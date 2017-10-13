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
                { }
            return new Bitmap(Image.FromFile("bg.jpg"));
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
