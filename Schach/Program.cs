using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schach
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
       {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormSchachbrett());
        }

        // Binde alle Objekte an die Felder, daher hast du deine Bilder usw, da jedes Objekt mindestens ein Bild 
        // zur verfuegung stellen muss, da ich das so gesagt habe lel, tu es
    }
}
