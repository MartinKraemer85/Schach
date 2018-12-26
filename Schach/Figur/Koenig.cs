using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schach
{
    class Koenig : Figur
    {
        public Koenig(Farbe farbe, string aktuellePosition, string bildName, Dictionary<string, Figur> dctFigur) : base(farbe, aktuellePosition, bildName, dctFigur)
        {

        }

        public override Bitmap FigurBild
        {
            get { return this.figurBild; }
        }




        private string linienFeldVerschiebung(int verschiebung)
        {
            return ((char)(this.aktuellePosition[0] + verschiebung)).ToString() + this.aktuellePosition[1];
        }

        private bool istZugRochade()
        {
            return false;
        }

        public Rochade Rochade(string feldKoordinaten)
        {
            int neuePosKoenig =1;
            int neuePosTurm = 2;
            int richtung = 1;
            Figur turm = turmFuerRochade(feldKoordinaten);
            if(turm == null)
            {
                return null;
            }
            // DIE POSITION IST VOM KOENIG AUSGEHEND!
            // --> 'a' ist die grosse Rochade
            if(turm.AktuellePosition[0] == 'h')
            {
                neuePosKoenig = 2;
                neuePosTurm = 1;
            }
            // Grosse Rochade
            if (turm.AktuellePosition[0] == 'a')
            {
                neuePosKoenig = -2;
                neuePosTurm = -1;
                richtung = -1;
            }

            if (this.ersterZug && turm != null && turm.Farbe == this.farbe)
            {
                // Pruefen, ob eine Figur im Weg ist, bzw der Koenig auf den Feldern bedroht wird
                foreach (KeyValuePair<string, Figur> figur in dctFigur)
                {
                    if (figurImWeg(richtung))
                    {
                        return null;
                    }
                    if (figur.Value.Farbe != this.farbe
                        && rochadeFeldBedroht(figur.Value, richtung))
                    {
                        return null;
                    }
                }
            }

            return new Rochade(turm.AktuellePosition, linienFeldVerschiebung(neuePosTurm), this.aktuellePosition,
                                           linienFeldVerschiebung(neuePosKoenig), turm, this);

        }

        /*
        bool kleineRochadeMoeglich = false;

        public bool KleineRochadeMoeglich
        {
            get { return kleineRochadeMoeglich; }
        }

        bool groesseRochadeMoeglich = false;

        public bool GroesseRochadeMoeglich
        {
            get { return groesseRochadeMoeglich; }
        }
        */

        bool rochadeMoeglich = false;

        public bool RochadeMoeglich
        {
            get { return rochadeMoeglich; }
        }

        public override List<String> listMoeglicheFelder()
        {

            // Die verschiedenen Zuege des Laeufers:
            listMoeglicheZuege = new List<string>();
            char tmpReihe = this.reihe;
            char tmpSpalte = this.spalte;

            this.rochadeMoeglich = false;
            //this.groesseRochadeMoeglich = false;
            // Rochade:
            /* Von Wikipedia --> Notation kurze Rochade O-O, lange Rochade O-O-O
             * Es gibt also insgesamt 4 mögliche Rochadezüge:

                Ke1–c1 und Ta1–d1 (lange weiße Rochade)
                Ke1–g1 und Th1–f1 (kurze weiße Rochade)
                Ke8–c8 und Ta8–d8 (lange schwarze Rochade)
                Ke8–g8 und Th8–f8 (kurze schwarze Rochade)
                Eine Rochade kann nur dann ausgeführt werden, wenn

                - der König noch nicht gezogen wurde,
                - der beteiligte Turm noch nicht gezogen wurde,
                - zwischen dem König und dem beteiligten Turm keine andere Figur steht,
                - der König über kein Feld ziehen muss, das durch eine feindliche Figur bedroht wird,
                - der König vor und nach Ausführung der Rochade nicht im Schach steht.

                Rochade ist ein Koenigszug.
            */





            // Den Turm fuer die kurze Rochade raussuchen
            RochadeMoeglichSetzen(false);

            if (rochadeMoeglich)
            {
                listMoeglicheZuege.Add(linienFeldVerschiebung(-4));
            }
            rochadeMoeglich = false;

            RochadeMoeglichSetzen(true);

            if (rochadeMoeglich)
            {
                listMoeglicheZuege.Add(linienFeldVerschiebung(3));
            }
            

            RochadeMoeglichSetzen(false);

            if (rochadeMoeglich)
            {
                listMoeglicheZuege.Add(linienFeldVerschiebung(-4));
            }

            // Diagonal links oben, wenn keine Figur der Farbe weiss im Weg ist
            zugValidieren(this.spalteAbisHvalide(-1, 'a', GroesserKleiner.Groesser), this.reiheEinsBisAchtValide(1, '8', GroesserKleiner.Kleiner),
                        -1, 1, tmpReihe, tmpSpalte);

            // Diagonal rechts oben
            zugValidieren(this.spalteAbisHvalide(1, 'h', GroesserKleiner.Kleiner), this.reiheEinsBisAchtValide(1, '8', GroesserKleiner.Kleiner),
                         1, 1, tmpReihe, tmpSpalte);

            // Diagonal links unten
            zugValidieren(this.spalteAbisHvalide(-1, 'a', GroesserKleiner.Groesser), this.reiheEinsBisAchtValide(-1, '1', GroesserKleiner.Groesser),
                         -1, -1, tmpReihe, tmpSpalte);

            // Diagonal rechts unten
            zugValidieren(this.spalteAbisHvalide(1, 'h', GroesserKleiner.Kleiner), this.reiheEinsBisAchtValide(-1, '1', GroesserKleiner.Groesser),
                         1, -1, tmpReihe, tmpSpalte);

            // Nach oben
            zugValidieren(this.reiheEinsBisAchtValide(1, '8', GroesserKleiner.Kleiner), 
                                0, 1, tmpReihe, tmpSpalte);
            // Nach unten
            zugValidieren(this.reiheEinsBisAchtValide(-1, '1', GroesserKleiner.Groesser), 
                                0, -1, tmpReihe, tmpSpalte);

            // Nach rechts
            zugValidieren(this.spalteAbisHvalide(1, 'h', GroesserKleiner.Kleiner),  
                                1,0, tmpReihe, tmpSpalte);

            // Nach links
            zugValidieren(this.spalteAbisHvalide(-1, 'a', GroesserKleiner.Groesser),  
                                -1,0, tmpReihe, tmpSpalte);
            return listMoeglicheZuege;

        }

        

        private void zugValidieren(bool spalteAbisHvalide, 
            int spalte,int reihe, char tmpReihe, char tmpSpalte)
        {
            if (spalteAbisHvalide)
            {
                if (!figurImWegEigeneFarbe(this.feldName(spalte, reihe)))
                {
                    this.listMoeglicheZuege.Add(this.feldName(spalte, reihe));
                    this.setSpalteReihe(spalte, reihe);
                    this.reset(tmpSpalte, tmpReihe);
                }
            }

        }

        private void zugValidieren(bool spalteAbisHvalide, bool reiheEinsBisAchtValide, 
                                    int spalte, int reihe, char tmpReihe, char tmpSpalte)
        {
            if (spalteAbisHvalide && reiheEinsBisAchtValide)
            {
                if (!figurImWegEigeneFarbe(this.feldName(spalte, reihe)))
                {
                    this.listMoeglicheZuege.Add(this.feldName(spalte, reihe));
                    this.setSpalteReihe(spalte, reihe);
                    this.reset(tmpSpalte, tmpReihe);
                }
            }

        }
        // Ist es der erste Zug des Turmes?
        // Der Turm muss die selbe Farbe haben, wie der Koenig, und in der selben Linie stehen
        private Figur turmFuerRochade(bool kleineRochade)
        {
            int verschiebung = 3;
            if (!kleineRochade)
            {
                verschiebung = -4;
            }
            foreach (KeyValuePair<string, Figur> figur in dctFigur)
            {
                if (figur.Value.Farbe == this.farbe
                    && figur.Value.GetType() == typeof(Turm)
                    && figur.Value.ErsterZug
                    && figur.Value.AktuellePosition == linienFeldVerschiebung(verschiebung)
                   )
                {
                    return figur.Value;

                }
            }
            return null;
        }

        private Figur turmFuerRochade(string kurzeLangeRochadeAoderH)
        {
            foreach (KeyValuePair<string, Figur> figur in dctFigur)
            {
                if (figur.Value.Farbe == this.farbe
                    && figur.Value.GetType() == typeof(Turm)
                    && figur.Value.ErsterZug
                    && figur.Value.AktuellePosition == kurzeLangeRochadeAoderH
                   )
                {
                    return figur.Value;

                }
            }
            return null;
        }

        // Ist eines der RochadeFelder bedroht? (um die Uebersicht zu wahren in einer Extra Funktion
        // da die Abfrage sonst extrem lang wird)
        // ---------------------------------------------------------------------------------------
        private bool rochadeFeldBedroht(Figur figur, int richtung)
        {
            return figur.listMoeglicheFelder().Contains(linienFeldVerschiebung(1 * richtung))
                         || figur.listMoeglicheFelder().Contains(linienFeldVerschiebung(2 * richtung))
                         || figur.listMoeglicheFelder().Contains(this.aktuellePosition);
        }
        // gleich ueberschreiben
        private bool figurImWeg(int richtung)
        {
            return figurImWeg(linienFeldVerschiebung(1 * richtung))
                    || figurImWeg(linienFeldVerschiebung(2 * richtung));
        }

        // ---------------------------------------------------------------------------------------


        private void RochadeMoeglichSetzen(bool kleineRochade)
        {
            int richtung = 1;
            if (!kleineRochade)
            {
                richtung = -1;
            }

            if (ersterZug && turmFuerRochade(kleineRochade) != null)
            {
                rochadeMoeglich = true;

                // Pruefen, ob eine Figur im Weg ist, bzw der Koenig auf den Feldern bedroht wird
                foreach (KeyValuePair<string, Figur> figur in dctFigur)
                {

                    if (figur.Value.GetType() != typeof(Koenig))
                    {
                        if (figurImWeg(richtung))
                        {
                            rochadeMoeglich = false;
                            break;
                        }

                        if (figur.Value.Farbe != this.farbe
                            && rochadeFeldBedroht(figur.Value, richtung))
                        {
                            rochadeMoeglich = false;
                            break;
                        }
                    }

                }
            }
        }
    }
}
