using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schach
{
    class Turm : Figur
    {

        public Turm(Farbe farbe, string aktuellePosition, string bildName, Dictionary<string, Figur> dctFigur) : base(farbe, aktuellePosition, bildName, dctFigur)
        {

        }

        public override Bitmap FigurBild
        {
            get { return this.figurBild; }
        }


        // ASCII
        // 1-8 --> 49-56
        // a-h --> 61-68
        // Ueberdenken ob dort die abfragen reinkommen ob das Zielfeld ein gleichfarbiges ding hat
        // usw.

        public override List<String> listMoeglicheFelder()
        {

            listMoeglicheZuege = new List<string>();

            // Die verschiedenen Zuege des Turmes:
            
            char tmpReihe = this.reihe;
            char tmpSpalte = this.spalte;

          
            // rechts
            reihenHinzufuegen(1, '8', GroesserKleiner.Kleiner, listMoeglicheZuege);
            this.reset(tmpSpalte, tmpReihe);
            // links
            reihenHinzufuegen(-1, '1', GroesserKleiner.Groesser, listMoeglicheZuege);
            this.reset(tmpSpalte, tmpReihe);
            // oben
            spaltenHinzufuegen(1, 'h', GroesserKleiner.Kleiner, listMoeglicheZuege);
            this.reset(tmpSpalte, tmpReihe);
            // unten
            spaltenHinzufuegen(-1, 'a', GroesserKleiner.Groesser, listMoeglicheZuege);
            this.reset(tmpSpalte, tmpReihe);

            return listMoeglicheZuege;

        }
    }
}
