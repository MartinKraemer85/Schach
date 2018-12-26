using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schach
{
    public class Pferd : Figur
    {



        public Pferd(Farbe farbe, string aktuellePosition, string bildName, Dictionary<string, Figur> dctFigur) : base(farbe, aktuellePosition, bildName, dctFigur)
        {

        }

        public override Bitmap FigurBild
        {
            get { return this.figurBild; }
        }
        


        // 1-8 --> 49-56
        // a-h --> 61-68
        // Ueberdenken ob dort die abfragen reinkommen ob das Zielfeld ein gleichfarbiges ding hat
        // usw.
        
            // reihe --> y
            // spalte --> x
        public override List<String> listMoeglicheFelder()
        {

            listMoeglicheZuege = new List<string>();

            // Die verschiedenen Zuege des Pferdes:

            

            bool reihePlusZwei = (this.reiheEinsBisAchtValide(2, '8', GroesserKleiner.Kleiner) );
            bool reiheMinusZwei = (this.reiheEinsBisAchtValide(-2, '1', GroesserKleiner.Groesser));
            bool spaltePlusZwei = (this.spalteAbisHvalide(2, 'h', GroesserKleiner.Kleiner) );
            bool spalteMinusZwei = (this.spalteAbisHvalide(-2, 'a', GroesserKleiner.Groesser) );
            
            bool reihePlusEins = (this.reiheEinsBisAchtValide(1, '8', GroesserKleiner.Kleiner));
            bool reiheMinusEins = (this.reiheEinsBisAchtValide(-1, '1', GroesserKleiner.Groesser));
            bool spaltePlusEins = (this.spalteAbisHvalide(1, 'h', GroesserKleiner.Kleiner) );
            bool spalteMinusEins = (this.spalteAbisHvalide(-1, 'a', GroesserKleiner.Groesser));

            // zwei rechts, eins nach oben
            if ((spaltePlusZwei && reihePlusEins) && !figurImWegEigeneFarbe(this.feldName(2, 1)))
            {
                listMoeglicheZuege.Add(this.feldName(2, 1));
            }
            // zwei rects, eins nach unten
            if ((spaltePlusZwei && reiheMinusEins) && !figurImWegEigeneFarbe(this.feldName(2, -1)))
            {
                listMoeglicheZuege.Add(this.feldName(2, -1));
            }

            // zwei links, eins nach oben
            if ((spalteMinusZwei && reihePlusEins) && !figurImWegEigeneFarbe(this.feldName(-2, 1)))
            {
                listMoeglicheZuege.Add(this.feldName(-2, 1));
            }

            // zwei links, eins nach unten
            if ((spalteMinusZwei && reiheMinusEins) && !figurImWegEigeneFarbe(this.feldName(-2, -1)))
            {
                listMoeglicheZuege.Add(this.feldName(-2, -1));
            }

            // zwei hoch, eins nach rechts
            if ((spaltePlusEins &&  reihePlusZwei) && !figurImWegEigeneFarbe(this.feldName(1, 2)))
            {
                listMoeglicheZuege.Add(this.feldName(1, 2));
            }
            // zwei hoch, eins nach links
            if ((spalteMinusEins &&  reihePlusZwei) && !figurImWegEigeneFarbe(this.feldName(-1, 2)))
            {
                listMoeglicheZuege.Add(this.feldName(-1, 2));
            }

            // zwei runter, eins nach rechts
            if ((spaltePlusEins  && reiheMinusZwei) && !figurImWegEigeneFarbe(this.feldName(1, -2)))
            {
                listMoeglicheZuege.Add(this.feldName(1, -2));
            }

            // zwei runter, eins nach links
            if ((spalteMinusEins  && reiheMinusZwei) && !figurImWegEigeneFarbe(this.feldName(-1, -2)))
            {
                listMoeglicheZuege.Add(this.feldName(-1, -2));
            }

    
            

            return listMoeglicheZuege;


        }
    }
}
