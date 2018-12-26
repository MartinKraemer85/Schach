using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schach
{
    public class Bauer : Figur
    {



        public Bauer(Farbe farbe, string aktuellePosition, string bildName, Dictionary<string, Figur> dctFigur) : base(farbe, aktuellePosition, bildName, dctFigur)
        {

        }

        public override Bitmap FigurBild
        {
            get { return this.figurBild; }
        }

        public bool spielFeldEndeEreicht()
        {
            if (this.farbe == Farbe.Weiss && aktuellePosition[1] == '8')
            {
                return true;
            }
            if (this.farbe == Farbe.Schwarz && aktuellePosition[1] == '1')
            {
                return true;
            }
            return false;
        }

        private bool enPassanteMoeglich = false;

        public bool EnPassanteMoeglich
        {
            set { this.enPassanteMoeglich = value; }
        }

        // 1-8
        private string spaltenFeldVerschiebung(string aktuellePosition, int verschiebung)
        {
            return aktuellePosition[0]  + ((char)(aktuellePosition[1] + verschiebung)).ToString();
        }
        // A-H
        private string linienFeldVerschiebung(int verschiebung)
        {
            return ((char)(this.aktuellePosition[0] + verschiebung)).ToString() + this.aktuellePosition[1];
        }

        // Der Bauer ueberschreibt die Property aktuelle Position
        // Dadurch kann man bei einem Zug herausfinden, ob es am Anfang ein Doppelzug war
        // Information wichtig fuer den Zug En Passante
        public override string AktuellePosition
        {
            get { return this.aktuellePosition; }
            set
            {
                setEnPassanteFalse();
                int verschiebung = 2;
                if(this.farbe == Farbe.Schwarz)
                {
                    verschiebung = -2;
                }

                // Wurde ein Dopoelzug durchgefuehrt?
                if(value == this.spaltenFeldVerschiebung(this.aktuellePosition,verschiebung))
                {
                    enPassanteMoeglich = true;
                }
                else
                {
                    enPassanteMoeglich = false;
                }
                
                foreach(string zielFeld in enPassanteMoeglicheZuege)
                {
                    if(zielFeld == value)
                    {
                        Debug.WriteLine("En le Passante");
                        // Entfernen aus dem dct und key speichern, damit die Figur auch von den Controls
                        // entfernt werden kann
                        Debug.WriteLine(spaltenFeldVerschiebung(value,richtung*-1));
                        entfernterKey = spaltenFeldVerschiebung(value, richtung * -1);
                        dctFigur.Remove(spaltenFeldVerschiebung(value, richtung * -1));
                    }
                }
                this.aktuellePosition = value;
                setReiheSpalte();
                ersterZug = false;                
            }
        }
        private string entfernterKey;

        public string EntfernterKey
        {
            get { return entfernterKey; }
        }

         
        private List<string> enPassanteMoeglicheZuege;
        // En Passant nur moeglich, wenn der Bauer der gegnerischen Farbe im Zug vor dem eigenen
        // Zug einen Doppelzug gemacht hat

        

        public void enPassant()
        {
            foreach(KeyValuePair<string,Figur> figur in dctFigur)
            {
                // alle Bauern, der entgegengesetzten Farbe, 
                //gerade einen Doppelzug gemacht haben
                

                if(figur.Value.Farbe != this.farbe
                    && figur.Value.GetType() == typeof(Bauer) 
                    && ((Bauer)figur.Value).enPassanteMoeglich
                    &&     (this.linienFeldVerschiebung(-1) == figur.Value.AktuellePosition
                            || this.linienFeldVerschiebung(1) == figur.Value.AktuellePosition)
                    )
                {
                    this.listMoeglicheZuege.Add(this.spaltenFeldVerschiebung(figur.Value.AktuellePosition, richtung));
                    enPassanteMoeglicheZuege.Add(this.spaltenFeldVerschiebung(figur.Value.AktuellePosition, richtung));
                }
            }
        }

        private int richtung = 1;

        public override List<string> listMoeglicheFelder()
        {
            entfernterKey = string.Empty;

            if (this.farbe == Farbe.Schwarz)
            {
                richtung = -1;
            }
            listMoeglicheZuege = new List<string>();
            enPassanteMoeglicheZuege = new List<string>();
            // Die verschiedenen Zuege des Pferdes:

            enPassant();
            // Ist Diagonal eine Figur der anderen Farbe
            // Und ist das Feld dahinter frei?

            // Ausserdem muss das feld im SPielfeldbereich liegen

            // Uebernaechstes Diagonales Feld existiert in Richtung rechts
            if (this.spalteAbisHvalide(1, 'h', GroesserKleiner.Kleiner) &&
               this.reiheEinsBisAchtValide(1 * richtung, '8', GroesserKleiner.Kleiner) &&
               this.reiheEinsBisAchtValide(1 * richtung, '1', GroesserKleiner.Groesser))
            {
                // Steht auf dem naechsten Diagonalen Feld in Richtung (je nach Farbe)
                // eine gegnerische Figur und ist das Feld dahinter unbesetzt?
                if (figurImWegAndereFarbe(this.feldName(1, 1 * richtung)))
                {
                    listMoeglicheZuege.Add(this.feldName(1, 1 * richtung));
                }     
            }

            // Uebernaechstes Diagonales Feld existiert in Richtung links
            if (this.spalteAbisHvalide(-1, 'a', GroesserKleiner.Groesser) &&
               this.reiheEinsBisAchtValide(1 * richtung, '8', GroesserKleiner.Kleiner) &&
               this.reiheEinsBisAchtValide(1 * richtung, '1', GroesserKleiner.Groesser))
            {
                // Steht auf dem naechsten Diagonalen Feld in Richtung (je nach Farbe)
                // eine gegnerische Figur und ist das Feld dahinter unbesetzt?
                if (figurImWegAndereFarbe(this.feldName(-1, 1 * richtung)))
                {
                    listMoeglicheZuege.Add(this.feldName(-1, 1 * richtung));
                }
            }         

            // Falls es der erste Zug ist, koennen zwei Felder gezogen werden
            if (ersterZug )
            {
                if(!figurImWeg(this.feldName(0, 1 * richtung)))
                {
                    listMoeglicheZuege.Add(this.feldName(0, 1 * richtung));
                    if(!figurImWeg(this.feldName(0, 2 * richtung)))
                    {
                        listMoeglicheZuege.Add(this.feldName(0, 2 * richtung));
                    }
                }
  
                
                return listMoeglicheZuege;
            }

            // ist das naechste Feld ueber / unter dem Bauern frei?
            // Standardzug
            if (!figurImWeg(this.feldName(0, 1 * richtung)))
            {
                listMoeglicheZuege.Add(this.feldName(0, 1 * richtung));
            }
            return listMoeglicheZuege;

        }

 
    }
}
