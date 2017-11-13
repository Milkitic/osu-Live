using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osu_live
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string ini = @"console = False
LogLevel = 0
LastRunVersion = v170620.14
firstRun = False
OsuFallback = False
SongsFolderLocation = Songs
PatternFileNames = l_DiffName|,~l_OsuFileLocation|,~l_dir|,~l_TitleUnicode|,~l_ArtistUnicode
Patterns = !DiffName!|,~!OsuFileLocation!|,~!dir!|,~!TitleUnicode!|,~!ArtistUnicode!
saveEvents = 2|,~1|,~1|,~1|,~1
keyList = 
keyNames = 
keyCounts = 
rightMouseCount = 0
leftMouseCount = 0
ResetKeysOnRestart = False
HookMouse = False
EnableKPM = False
LoadingRawBeatmaps = False
osuPostEnabled = False
EnableMemoryScanner = True
StartHidden = False
osuPostLogin = 
osuPostPassword = 
MemoryOffset = -1
NoModsDisplayText = None
UseLongMods = False
EnableModImages = False
ImageWidth = 720
ModHeight = 64
ModWidth = 64
ModImageSpacing = -25
ModImageOpacity = 85
DrawOnRightSide = False
DrawFromRightToLeft = False
";
            System.IO.File.WriteAllText("stream\\settings.ini", ini);
            Constant.Canvas.Zoom = 0.7;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
