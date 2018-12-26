using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schach
{

    public enum Farbe
    {
        Weiss,
        Schwarz
    }

    public abstract class Figur
    {
      
        protected enum GroesserKleiner
        {
            Groesser,
            Kleiner
        }

        protected Bitmap figurBild;
        // 0 = schwarz, 1 = weiss
        // Wird ein enum
        protected Farbe farbe;
        protected string aktuellePosition;
        protected char spalte, reihe;
        protected bool ersterZug = true;
        protected List<String> listMoeglicheZuege;
        protected Dictionary<string, Figur> dctFigur;

        public bool ErsterZug
        {
            get { return this.ersterZug; }
        }

        private string bildName;
        public string BildName
        {
            get { return this.bildName; }
        }
        public Figur(Farbe farbe, string aktuellePosition, string bildName, Dictionary<string, Figur> dctFigur)
        {
            this.farbe = farbe;
            // aktuelle Position ergibt sich aus dem Namen des Panels, zu dem die Figur als Tag gehoert
            this.aktuellePosition = aktuellePosition;
            // Daraus ergibt sich die Spalte und die Reihe
            setReiheSpalte();
            this.figurBild = new Bitmap(Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..", "Bilder", bildName)));
            this.dctFigur = dctFigur;
            this.bildName = bildName;
        }

        
        /*                              Abstracte Methoden
         * -------------------------------------------------------------------------- */

        public abstract Bitmap FigurBild { get;}

        public abstract List<String> listMoeglicheFelder();


        /* -------------------------------------------------------------------------- */

        /*
         * wird die aktuelle Position geaendert, wurde der erste Zug gemacht
         * Wichtig fuer die Bauern / Tuerme
         */

        // alle enPassanteMoeglich der Bauern der eigenen Farbe wieder zuruecksetzen,
        // Denn EnPassante nur moeglich, wenn der Zug unmittelbar nach Zug des Gegners durchgefuehrt wird
        // Ist weiss drane, hat schwarz jede Chance auf En Passante fuer diese Runde vertan

        // Es muss separat bei dem Bauern noch mal implementiert werden, weil er
        // die AktuellePositions Property ueberschreibt
        protected void setEnPassanteFalse()
        {
            foreach (KeyValuePair<string, Figur> figur in dctFigur)
            {
                if (figur.Value.Farbe != this.farbe
                    && figur.Value.GetType() == typeof(Bauer))
                {
                    ((Bauer)figur.Value).EnPassanteMoeglich = false;
                }
            }
        }

        public virtual string AktuellePosition
        {
            get { return this.aktuellePosition; }
            set
            { 
                setEnPassanteFalse();
                this.aktuellePosition = value;
                setReiheSpalte();
                ersterZug = false;
            }
        }

        /* 
         * Aktualisieren der Position des Objektes
         */
        protected void setReiheSpalte()
        {
            this.spalte = (char)aktuellePosition[0];
            this.reihe = (char)AktuellePosition[1];
        }

        // 
        protected string feldName(int spalteAdd, int reiheAdd)
        {
            return ((char)(this.spalte + spalteAdd)).ToString() + ((char)(this.reihe + reiheAdd)).ToString();
        }

        /*
         * Testen, ob die neue Spalte Valide ist (im Bereich von a-h)
         */
        protected bool spalteAbisHvalide(int i, char spalte, GroesserKleiner groesserKleiner)
        {
            if (groesserKleiner == GroesserKleiner.Groesser)
            {
                return (char)(this.spalte + i) >= spalte;
            }
            return (char)(this.spalte + i) <= spalte;
        }

        /* 
         * Testen, ob die neue Reihe Valide ist (im Bereich von 1-8)
         */
        protected bool reiheEinsBisAchtValide(int i, char reihe, GroesserKleiner groesserKleiner)
        {
            if (groesserKleiner == GroesserKleiner.Groesser)
            {
                return (char)(this.reihe + i) >= reihe;
            }
            return (char)(this.reihe + i) <= reihe;
        }

        /* 
         * Ist das gewaehlte Feld in der liste der Moeglichen Zuegen?
         */
        public static bool zugMoeglich(string feld, List<String> listMoeglicheZuege)
        {
            foreach(string s in listMoeglicheZuege)
            {
                if (feld.Equals(s))
                {
                    return true;
                }
            }
            return false;
        }

        /*
         *  Spalte / Reihe incrementieren / decrementieren
         */
        protected void setSpalteReihe(int spalte, int reihe)
        {
            this.spalte += (char)spalte;
            this.reihe += (char)reihe;
        }

        /*
         * Spalten/Reihen werte zuruecksetzen
         */
        protected void reset(char tmpSpalte , char tmpReihe)
        {

            this.spalte = tmpSpalte;
            this.reihe = tmpReihe;
        }

        // Gibt es das Feld im Dictionary, steht auch eine FIgur drauf
        protected bool figurImWeg(string dctKey)
        {
            if (dctFigur.ContainsKey(dctKey))
            {
                return true;
            }

            return false;
        }

        // Im Dictionary nachschauen, ob auf dem Feld eine Figur steht, und falls ja, ob es
        // die gleiche Farbe ist
        protected bool figurImWegEigeneFarbe(string dctKey)
        {
            if (dctFigur.ContainsKey(dctKey))
            {
                if (dctFigur[dctKey].Farbe == this.Farbe)
                {
                    return true;
                }
            }

            return false;
        }

        protected bool figurImWegAndereFarbe(string dctKey)
        {
            if (dctFigur.ContainsKey(dctKey))
            {
                if (((Figur)dctFigur[dctKey]).Farbe != this.Farbe)
                {
                    return true;
                }
            }

            return false;
        }

        // Hier werden der List der moegliczhen zuege die Spalten hinzugefuegt
        // (X Achse)
        // 
        protected void spaltenHinzufuegen(int richtung, char grenze, GroesserKleiner bedingung, List<string> listMoeglicheZuege)
        {
            while (this.spalteAbisHvalide(richtung, grenze, bedingung))
            {
                // Ist eine Figur der eigenen Farbe auf einem Feld im Weg
                if (this.figurImWegEigeneFarbe(this.feldName(richtung, 0)))
                {
                    break;
                }
                // Wenn eine Figur der anderen Farbe im wege ist,
                // dann danach aufhoeren, da dies die Grenze ist, wie weit gezogen
                // werden darf
                if (figurImWegAndereFarbe(this.feldName(richtung, 0)))
                {
                    listMoeglicheZuege.Add(this.feldName(richtung, 0));
                    this.setSpalteReihe(richtung, 0);
                    break;
                }

                listMoeglicheZuege.Add(this.feldName(richtung, 0));
                this.setSpalteReihe(richtung, 0);

            }
        }

        // Hier werden der Liste der moeglichen Zuege die Reihen hinzugefuegt
        // (Y Achse)
        // Selbes Prinzip wie spaltenHinzufuegen
        protected void reihenHinzufuegen(int richtung, char grenze, GroesserKleiner bedingung, List<string> listMoeglicheZuege)
        {
            while (this.reiheEinsBisAchtValide(richtung, grenze, bedingung))
            {

                if (this.figurImWegEigeneFarbe(this.feldName(0, richtung)))
                {
                    break;
                }

                if (figurImWegAndereFarbe(this.feldName(0, richtung)))
                {
                    listMoeglicheZuege.Add(this.feldName(0, richtung));
                    this.setSpalteReihe(0, richtung);
                    break;
                }

                listMoeglicheZuege.Add(this.feldName(0, richtung));
                this.setSpalteReihe(0, richtung);

            }

        }

        // Die Diagonalen Felder werden der Liste hinzugefuegt
        // Ebenfalls abfragen, ob auf dem naechsten Zug eine eigene Figur oder eine Gegnerische Fugur steht
        // 
        protected void diagonaleZeugeHinzufuegen(int richtungAbisH, char grenzeAbisH, GroesserKleiner bedingungAbisH,
                                                int richtungEinsBisAcht, char grenzeEinsBisAcht, GroesserKleiner bedingungEinsBisAcht,
                                                List<string> listMoeglicheZuege)
        {
            while (this.spalteAbisHvalide(richtungAbisH, grenzeAbisH, bedingungAbisH) &&
                        this.reiheEinsBisAchtValide(richtungEinsBisAcht, grenzeEinsBisAcht, bedingungEinsBisAcht))
            {
                if (figurImWegEigeneFarbe(this.feldName(richtungAbisH, richtungEinsBisAcht)))
                {
                    break;
                }

                if (figurImWegAndereFarbe(this.feldName(richtungAbisH, richtungEinsBisAcht)))
                {
                    listMoeglicheZuege.Add(this.feldName(richtungAbisH, richtungEinsBisAcht));
                    this.setSpalteReihe(richtungAbisH, richtungEinsBisAcht);
                    break;
                }

                listMoeglicheZuege.Add(this.feldName(richtungAbisH, richtungEinsBisAcht));
                this.setSpalteReihe(richtungAbisH, richtungEinsBisAcht);
            }
        }

        public Farbe Farbe
        {
            get { return this.farbe; }
        }
    }

    

    

}

