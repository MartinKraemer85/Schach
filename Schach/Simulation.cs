using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schach
{
    class Simulation
    {
        public static Dictionary<string, Figur> DctFigur
        {
            set
            {
                dctFigur = value;
            }
        }

        public static void Zug(string zug, Farbe farbe)
        {
            KeyValuePair<string, Figur> figur = figurErmitteln(zug, farbe);
            DictionaryAktualisieren(zielFeld(zug), figur);
        }




        private static Dictionary<string, Figur> dctFigur;

   
        private static string zielFeld(string zug)
        {
            return zug.Substring(zug.Length - 2, 2);
        }

      
        private static void setDictionary(string key, Figur figur )
        {
            if (dctFigur.ContainsKey(key))
            {
                dctFigur.Remove(key);
            }

            dctFigur.Add(key, figur);

        }

        private static void removeVonDictionary(string key)
        {
            if (dctFigur.ContainsKey(key))
            {
                dctFigur.Remove(key);
            }

        }

        private static string linienFeldVerschiebung(string aktuellePosition, int verschiebung)
        {
            return ((char)(aktuellePosition[0] + verschiebung)).ToString() + aktuellePosition[1];
        }

        private static KeyValuePair<string, Figur> figurErmitteln(string zug,Farbe farbe)
        {
   
            switch (zug[0])
            {
                // Rochade
                
                case 'O':
                    Rochade rochade = null;
                    foreach (KeyValuePair<string, Figur> figur in dctFigur)
                    {
                        if (figur.Value.Farbe == farbe && (figur.Value.GetType() == typeof(Koenig)))
                        {
                            if(zug == "O-O")
                            {
                                rochade = getRochade(figur.Value, linienFeldVerschiebung(figur.Value.AktuellePosition,+3));
                            }
                            else
                            {
                                rochade = getRochade(figur.Value, linienFeldVerschiebung(figur.Value.AktuellePosition, -4));
                            }

                        }
                    }
                    try
                    {
                        setDictionary(rochade.ZielFeldKoenigRochade, rochade.Koenig);
                        setDictionary(rochade.ZielFeldTurmRochade, rochade.Turm);
                        removeVonDictionary(rochade.FeldKoenig);
                        removeVonDictionary(rochade.FeldTurm);
                        rochade.Turm.AktuellePosition = rochade.ZielFeldTurmRochade;
                        rochade.Koenig.AktuellePosition = rochade.ZielFeldKoenigRochade;
                    } catch(Exception e) { Debug.WriteLine("Rochade: " +e.Message); }
                    
                    break;

                case 'P':
                    
                    foreach (KeyValuePair<string, Figur> figur in dctFigur)
                    {
                        if(figur.Value.Farbe == farbe && (figur.Value.GetType() == typeof(Bauer)))
                        {
                            // Ist beim Bauernzug ein "=" enthalten, wird eine Bauernumwandlung durchgefuehrt
                            // Das heisst, das aus dem Bauer eine neue FIgur entshtet
                            // bsp e8=Q --> Bauer wird zur Dame
                            if (zug.Contains("="))
                            {
                                string tmpZielFeld = zug.Substring(1, 2);

                                if (Bewegen(figur.Value, zug.Substring(0, 3)))
                                {
                                    Debug.WriteLine("--------------Anfang-----------------------");
                                    foreach (KeyValuePair<string, Figur> miau in dctFigur)
                                    {
                                        Debug.WriteLine("Key: " + miau.Key + " figur" + miau.Value);
                                    }
                                    Debug.WriteLine("-------------------------------------------");
                                    Figur neueFigur = BauernUmwandlung(zug[zug.Length - 1], figur.Value, tmpZielFeld);
#if true
                                    Debug.WriteLine("neue Figur:" + neueFigur + " aktPos " + neueFigur.AktuellePosition + "Bildname " + neueFigur.BildName);
                                    Debug.WriteLine("alte Figur:" + figur.Value + " aktPos " + figur.Value.AktuellePosition + "Bildname " + figur.Value.BildName);
#endif
                                    setDictionary(tmpZielFeld, neueFigur);
                                    removeVonDictionary(figur.Value.AktuellePosition);
                                    break;
                                }
                                // Den Rest ueberspringen, weil nicht relevant
                                continue;
                            }

                            if (Bewegen(figur.Value, zug))
                            {
                                return figur;
                            }
                            // Pferd schmeisst, ohne das anderes Pferd der gleichen Farbe schmeissen kann
                            // Nxe4 --> x = schmeissen, e4 = Zielfeld
                            // x steht immer an der 2ten Stelle
                            if (Schmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            if (MehrereFigurenGleicheLinieSchmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            // bewegt sich, mehrere Figuren koennen den Zug durchfuehren
                            if (MehrereFigurenGleicheLinieBewegen(figur.Value, zug))
                            {
                                return figur;
                            }

                            if (MehrereFigurenGleicheReiheSchmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            // bewegt sich, mehrere Figuren koennen den Zug durchfuehren
                            if (MehrereFigurenGleicheReiheBewegen(figur.Value, zug))
                            {
                                return figur;
                            }
                        }
                        

                    }
                    break;
                case 'K':
                    foreach (KeyValuePair<string, Figur> figur in dctFigur)
                    {

                        if (figur.Value.Farbe == farbe && (figur.Value.GetType() == typeof(Koenig)))
                        {

                            if (Bewegen(figur.Value, zug))
                            {
                                return figur;
                            }
                            // Pferd schmeisst, ohne das anderes Pferd der gleichen Farbe schmeissen kann
                            // Nxe4 --> x = schmeissen, e4 = Zielfeld
                            // x steht immer an der 2ten Stelle
                            if (Schmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            if (MehrereFigurenGleicheLinieSchmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            // bewegt sich, mehrere Figuren koennen den Zug durchfuehren
                            if (MehrereFigurenGleicheLinieBewegen(figur.Value, zug))
                            {
                                return figur;
                            }

                        }
                    }
                    break;
                case 'Q':
                    foreach (KeyValuePair<string, Figur> figur in dctFigur)
                    {

                        if (figur.Value.Farbe == farbe && (figur.Value.GetType() == typeof(Dame)))
                        {

                            if (Bewegen(figur.Value, zug))
                            {
                                return figur;
                            }
                            // Pferd schmeisst, ohne das anderes Pferd der gleichen Farbe schmeissen kann
                            // Nxe4 --> x = schmeissen, e4 = Zielfeld
                            // x steht immer an der 2ten Stelle
                            if (Schmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            if (MehrereFigurenGleicheLinieSchmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            // bewegt sich, mehrere Figuren koennen den Zug durchfuehren
                            if (MehrereFigurenGleicheLinieBewegen(figur.Value, zug))
                            {
                                return figur;
                            }

                        }
                    }
                    break;
                case 'B':
                    foreach (KeyValuePair<string, Figur> figur in dctFigur)
                    {

                        if (figur.Value.Farbe == farbe && (figur.Value.GetType() == typeof(Laeufer)))
                        {

                            if (Bewegen(figur.Value, zug))
                            {
                                return figur;
                            }
                            // Pferd schmeisst, ohne das anderes Pferd der gleichen Farbe schmeissen kann
                            // Nxe4 --> x = schmeissen, e4 = Zielfeld
                            // x steht immer an der 2ten Stelle
                            if (Schmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            if (MehrereFigurenGleicheLinieSchmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            // bewegt sich, mehrere Figuren koennen den Zug durchfuehren
                            if (MehrereFigurenGleicheLinieBewegen(figur.Value, zug))
                            {
                                return figur;
                            }

                        }
                    }
                    break;
                case 'N':
                    foreach (KeyValuePair<string, Figur> figur in dctFigur)
                    {

                        if (figur.Value.Farbe == farbe && (figur.Value.GetType() == typeof(Pferd)))
                        {

                            if (Bewegen(figur.Value, zug))
                            {
                                return figur;
                            }
                            // Pferd schmeisst, ohne das anderes Pferd der gleichen Farbe schmeissen kann
                            // Nxe4 --> x = schmeissen, e4 = Zielfeld
                            // x steht immer an der 2ten Stelle
                            if (Schmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            if (MehrereFigurenGleicheLinieSchmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            // bewegt sich, mehrere Figuren koennen den Zug durchfuehren
                            if (MehrereFigurenGleicheLinieBewegen(figur.Value, zug))
                            {
                                return figur;
                            }

                            if (MehrereFigurenGleicheReiheSchmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            // bewegt sich, mehrere Figuren koennen den Zug durchfuehren
                            if (MehrereFigurenGleicheReiheBewegen(figur.Value, zug))
                            {
                                return figur;
                            }

                        }
                    }
                    break;
                case 'R':
                     foreach (KeyValuePair<string, Figur> figur in dctFigur)
                    {

                        if (figur.Value.Farbe == farbe && (figur.Value.GetType() == typeof(Turm)))
                        {
                            

                            if (Bewegen(figur.Value, zug))
                            { 
                                return figur;
                            }
                            // Pferd schmeisst, ohne das anderes Pferd der gleichen Farbe schmeissen kann
                            // Nxe4 --> x = schmeissen, e4 = Zielfeld
                            // x steht immer an der 2ten Stelle
                            if (Schmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            if (MehrereFigurenGleicheLinieSchmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            // bewegt sich, mehrere Figuren koennen den Zug durchfuehren
                            if (MehrereFigurenGleicheLinieBewegen(figur.Value, zug))
                            {
                                return figur;
                            }

                            if (MehrereFigurenGleicheReiheSchmeissen(figur.Value, zug))
                            {
                                return figur;
                            }

                            // bewegt sich, mehrere Figuren koennen den Zug durchfuehren
                            if (MehrereFigurenGleicheReiheBewegen(figur.Value, zug))
                            {
                                return figur;
                            }

                        }
                    }
                    break;
            }

            return new KeyValuePair<string, Figur>();
        }

        // Bauernumwandlung
        private static Figur BauernUmwandlung(char neueFigur, Figur figur, string neuePos)
        {
            string farbe;
            if(figur.Farbe == Farbe.Weiss)
            {
                farbe = "w";
            }
            else
            {
                farbe = "s";
            }
            switch (neueFigur)
            {
                
                case 'B': return new Laeufer(figur.Farbe, neuePos, farbe+"Laeufer.png", dctFigur);          
                case 'Q': return new Dame(figur.Farbe, neuePos, farbe + "Dame.png", dctFigur);
                case 'N': return new Pferd(figur.Farbe, neuePos, farbe + "Koenig.png", dctFigur); 
                case 'R': return new Turm(figur.Farbe, neuePos, farbe + "Turm.png", dctFigur);
                default: return figur;
            }
        }

        // Figur geht zu Feld --> Nc3 (N=Pferd, c3 = Zielfeld)
        private static bool Bewegen(Figur figur, string zug)
        {
            return figur.listMoeglicheFelder().Contains(zielFeld(zug))
                                && zug.Length == 3;
        }

        
        // Fgiur schmeisst, ohne das andere (gleiche) Figur der gleichen Farbe schmeissen kann
        // Nxe4 --> x = schmeissen, e4 = Zielfeld
        // x steht immer an der 2ten Stelle
        private static bool Schmeissen(Figur figur, string zug)
        {
            return figur.listMoeglicheFelder().Contains(zielFeld(zug))
                                && zug[1] == 'x';
        }

        // Schmeisst, mehrere (gleiche) Figuren koennen den Zug durchfuehren auf gleicher Linie (a-h)
        private static bool MehrereFigurenGleicheLinieSchmeissen(Figur figur, string zug)
        {
            return figur.listMoeglicheFelder().Contains(zielFeld(zug))
                                && zug.Contains("x")
                                && zug[1] != 'x'
                                && figur.AktuellePosition.StartsWith(zug[1].ToString());
        }

        // bewegt sich, mehrere (gleiche) Figuren koennen den Zug durchfuehren auf gleicher Linie (a-h)
        private static bool MehrereFigurenGleicheLinieBewegen(Figur figur, string zug)
        {
            return figur.listMoeglicheFelder().Contains(zielFeld(zug))
                                && zug.Length > 3
                                && figur.AktuellePosition.StartsWith(zug[1].ToString());
        }

        // bewegt sich, mehrere (gleiche) Figuren koennen den Zug durchfuehren auf gleicher Reihe (1-8)
        private static bool MehrereFigurenGleicheReiheBewegen(Figur figur, string zug)
        {
            return figur.listMoeglicheFelder().Contains(zielFeld(zug))
                                && zug.Length > 3
                                && figur.AktuellePosition.EndsWith(zug[1].ToString());
        }

        // bewegt sich, mehrere (gleiche) Figuren koennen den Zug durchfuehren auf gleicher Reihe (1-8)
        private static bool MehrereFigurenGleicheReiheSchmeissen(Figur figur, string zug)
        {
            return figur.listMoeglicheFelder().Contains(zielFeld(zug))
                                && zug.Contains("x")
                                && zug[1] != 'x'
                                && figur.AktuellePosition.EndsWith(zug[1].ToString());
        }

        private static Rochade getRochade(Figur figur, string zug)
        {
            return ((Koenig)figur).Rochade(zug);
        }

        private static void DictionaryAktualisieren(string neuerKey, KeyValuePair<string, Figur> aktuelleFigur)
        {
            if (neuerKey.Contains("O"))
            {
                return;
            }

            if(neuerKey.Contains("="))
            {
                return;
            }
            if (dctFigur.ContainsKey(neuerKey))
            {
                dctFigur.Remove(neuerKey);
            }
            
            try
            {
                dctFigur.Remove(aktuelleFigur.Key);
                dctFigur.Add(neuerKey, aktuelleFigur.Value);
            }
            catch(Exception e) { Debug.WriteLine("Dict remove / add " + e.Message + " Key: " + aktuelleFigur.Key); }

            // position aktualisieren
            try
            {
                aktuelleFigur.Value.AktuellePosition = neuerKey;
            }
            catch (Exception e) { Debug.WriteLine("Figur neue Position " +e.Message + " neue Pos: " + neuerKey); }
            Debug.WriteLine("--------------Result-----------------------");
            foreach(KeyValuePair<string, Figur> figur in dctFigur)
            {
                Debug.WriteLine("Key: " + figur.Key + " figur" + figur.Value);
            }
        }
    }
}
