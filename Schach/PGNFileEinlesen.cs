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
    class PGNFileEinlesen
    {
        private List<string> csvText;
        private List<string> csvHeader;
        private Dictionary<int, string> dtcZuege;

        public List<string> CsvHeader
        {
            get { return this.csvHeader; }
        }

        public Dictionary<int, string> DtcZuege
        {
            get { return this.dtcZuege; }
        }

        public PGNFileEinlesen(string fileName)
        {
            csvText = new List<string>();
            csvHeader = new List<string>();
            dtcZuege = new Dictionary<int, string>();

            string pfad = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..", "PGN", fileName));

            try
            {
                using (StreamReader sr = File.OpenText(pfad))
                {
                    string s = String.Empty;
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.StartsWith("["))
                        {
                            csvHeader.Add(s);
                            continue;
                        }

                        // Kommentare entfernen
                        if (s.IndexOf('{') > 0)
                        {
                            csvText.Add(s.Remove(s.IndexOf('{'), s.IndexOf('}') - s.IndexOf('{') +1).Trim());
                            continue;
                        }
                        csvText.Add(s);
                       
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Fehler: " + e.Message);
            }

            splitStringsToDictionary();
        }

        // Damit man den Bauern erkennt, wird im Dictionary wenn es kein Koenig oder eine
        // andere Figur ist / eine Rochade oder der Sieg/ Unentschieden
        // dem Zug ein P hinzugefuegt
        private void splitStringsToDictionary()
        {
            int startIndex = 0;
            int zug = 1;

            // Jede Zeile durchgehen

            // Zuerst alles in eine Zeile bringen, da es sonst Probleme gibt, weil bei manchen
            // Dateien, die Zuege ueber mehrere Zeilen gehen
            string zuegeInEinerZeile ="";
            foreach (string s in csvText)
            {
                zuegeInEinerZeile = zuegeInEinerZeile.Trim() + " " + s; 
            }
            Debug.WriteLine(zuegeInEinerZeile);
       
            // Solange es einen naechsten Punkt gibt
            while(zuegeInEinerZeile.IndexOf('.', startIndex) > 0)
            {       
                startIndex = zuegeInEinerZeile.IndexOf('.', startIndex);
                int nextIndex = zuegeInEinerZeile.IndexOf('.', startIndex +1);
                    
                string[] tmpArray;
                string tmpString = "";
                    
                // Es gibt einen naechsten Zug
                if (nextIndex > 0)
                {
                    // Die Strings zwischen den einzelnen Puntken
                    // bsp.: 1.e4 c6 2.d4 --> e4 c6 2 --> Zahl nach dem letzten Leerzeichen
                    // ergibt die Runde
                    tmpArray = zuegeInEinerZeile.Substring(startIndex + 1, nextIndex - startIndex - 1).Trim().Split(' ');

                    for (int i = 0; i < tmpArray.Length; i++)
                    {
                        // Solange nicht das letzte Leerzeichen erreicht --> String dem tmp
                        // String hinzufuegen


                        if (i != (tmpArray.Length - 1))
                        {
                            if(!tmpArray[i].StartsWith("K") && !tmpArray[i].StartsWith("R") &&
                                !tmpArray[i].StartsWith("Q") && !tmpArray[i].StartsWith("N") &&
                                !tmpArray[i].StartsWith("B") && !tmpArray[i].StartsWith("O") &&
                                !tmpArray[i].StartsWith("1") && !tmpArray[i].StartsWith("0"))
                            {
                                tmpArray[i] = "P" + tmpArray[i];
                            }
                            tmpString = tmpString + ' ' + tmpArray[i];
                        }
                        else
                        {
                            dtcZuege.Add(zug, tmpString.Replace('+', ' ').Trim());
                        }
                    }
                }
                    /*
                // Letzter beschriebener Zug in einer Reihe
                if( nextIndex < 0)
                {
                    // Den letzten Zug aufsplitten und gegebenenfalls die 
                    // P's zum erkennen der Pawns einfuegen
                    tmpArray = zuegeInEinerZeile.Split('.').Last().Trim().Split(' ');
            
                    for(int j = 0; j < tmpArray.Length; j++)
                    {
                        if (!tmpArray[j].StartsWith("K") && !tmpArray[j].StartsWith("R") &&
                                !tmpArray[j].StartsWith("Q") && !tmpArray[j].StartsWith("N") &&
                                !tmpArray[j].StartsWith("B") && !tmpArray[j].StartsWith("O") &&
                                !tmpArray[j].StartsWith("1") && !tmpArray[j].StartsWith("0"))
                        {
                            tmpArray[j] = "P" + tmpArray[j];
                        }

                        tmpString = tmpString + " " + tmpArray[j];
                    }

                    dtcZuege.Add(zug, tmpString.Replace('+', ' ').Trim());
                }*/
                zug++;
                startIndex++;
            }
            startIndex = 0;

            
            foreach(KeyValuePair<int,string> pair in dtcZuege)
            {
                Debug.WriteLine("Key: {0}" + " Value: {1}", pair.Key, pair.Value);
            }

        }


        
    }
}
