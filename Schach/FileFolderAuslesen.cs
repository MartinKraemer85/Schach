using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schach
{
    class FileFolderAuslesen
    {
        private static string pfad = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..", "PGN"));

        public static string[] dateiNamen()
        {
            
            DirectoryInfo d = new DirectoryInfo(pfad);//Assuming Test is your Folder

            FileInfo[] files = d.GetFiles("*.txt"); //Getting Text files
            string[] dateiNamen = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                dateiNamen[i] = files[i].Name;
                Debug.WriteLine(dateiNamen[i]);
            }

            return dateiNamen;
        }
    }
}

