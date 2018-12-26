using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;





namespace Schach
{

    public enum Reset
    {
        Rochade,
        Bauernumwandlung,
        enPeasant,
        Komplett
    }
    public partial class FormSchachbrett : Form
    {

        private List<String> moeglicheZuege;
        private List<Control> moeglicheFelder;

        public Dictionary<string, Figur> dctFigur = new Dictionary<string, Figur>();
        
        private Figur ausgewaehlteFigur;
        private bool figurAngewaehlt = false;

        private Farbe farbeAmZug;

        private int zeitIntervall;

        public FormSchachbrett()
        { 
            InitializeComponent();
            startPositionierungen();
            farbeAmZug = setFarbeAmZugRadioButtons();
            lblTrackBar.Text = trackBar.Value.ToString() + " ms";
            this.zeitIntervall = trackBar.Value;

            cBoxDateiNamen.Items.AddRange( FileFolderAuslesen.dateiNamen());
            cBoxDateiNamen.SelectedIndex = 0;
        }

        // Je nachdem was gechecked ist, die Farbe ermitteln, die am Zug ist
        private Farbe setFarbeAmZugRadioButtons()
        {
            if (rdBWeiss.Checked)
            {
                return Farbe.Weiss;
            }
            return Farbe.Schwarz;            
        }
        // Null setzen aller Panel Tags / Dictionary
        // Figuren positionieren
        private void btnReset_Click(object sender, EventArgs e)
        { 
            DialogResult result = MessageBox.Show("Das Spiel neu starten?", "Reset", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }
            reset(Reset.Komplett);
        }

        //------------------------------ Zum testen--------------------------
        private void reset(Reset resetValue)
        {
            zurueckSetzen();
            switch (resetValue)
            {
                case Reset.Bauernumwandlung:
                    startPositionierungenB();
                    break;
                case Reset.enPeasant:
                    startPositionierungenEn();
                    break;
                case Reset.Komplett:
                    startPositionierungen();
                    break;
                case Reset.Rochade:
                    startPositionierungenR();
                    break;
            }
            
            rdBWeiss.Checked = true;
            btnStart.Enabled = true;
            grpBoxFarbeBeginnt.Enabled = true;
            this.ausgewaehlteFigur = null;
            this.figurAngewaehlt = false;
            this.farbeAmZug = Farbe.Weiss;
            this.grpBoxFarbeAmZug.BackColor = Color.White;
            this.rTextBoxZuege.Clear();
        }

        //------------------------------ Zum testen--------------------------
        private void btnStart_Click(object sender, EventArgs e)
        {
            reset(Reset.Komplett);
            panelsEnabled();
            btnStart.Enabled = false;
            grpBoxFarbeBeginnt.Enabled = false;
            btnReset.Enabled = true;
        }
        
        /*
         * Hier werden die verschiedenen Moeglichkeiten bei der Figurauswahl ausgewertet
         * 1) Keine Figur ausgewaehl, und Feld ist leer
         * 2) Keine Figur ausgewaehlt, auf dem Feld ist eine Figur
         * 3) Figur ist ausgewaehlt, aber andere Figur wird ausgewaehlt (kein schmeissen, da eigene Farbe)
         * 4) Figur ist ausgewaehlt, und Feld ohne Figur / mit Gegner Figur wird angeklickt
        */
        private void panel_Click(object sender, EventArgs e)
        {

            Figur figur = (Figur)((Panel)sender).Tag;
            Panel panel = ((Panel)sender);
    

            // 1) Ist keine Figur angewaehlt und das Feld ist leer --> return
            if (figur == null && !figurAngewaehlt)
            { 
                return;
            }



            // 2) keine Figur angewaehlt, das Feld ist nicht leer --> Felder markieren und Figur merken
            // Figurfarbe muss der FarbeAmZug entsprechen.
            if (figur != null && !figurAngewaehlt && figur.Farbe == farbeAmZug)
            {

                this.ausgewaehlteFigur = figur;

                if (ausgewaehlteFigur.listMoeglicheFelder() == null)
                {
                    ausgewaehlteFigur = null;
                    figurAngewaehlt = false;
                    return;
                }

                felderMarkieren(figur);
                figurAngewaehlt = true;
                return;
            }

            // 3) Figur ist angewaehlt, andere Figur wird ausgewaehlt (kein moeglicher Zug)
            // --> Es ist eine Figur angewaehlt, und das Panel was angeklickt wird,
            // faellt nicht unter die Liste der moeglichen Zuege.
            // Figurfarbe muss der FarbeAmZug entsprechen. 
            if (figurAngewaehlt && figur != null
                && !Figur.zugMoeglich(panel.Name,ausgewaehlteFigur.listMoeglicheFelder())
                && figur.Farbe == farbeAmZug
                )
            {

                // Falls keine moeglichen Zuege, figur abwaehlen und abbrechen
                if (ausgewaehlteFigur.listMoeglicheFelder() == null)
                {
                    ausgewaehlteFigur = null;
                    figurAngewaehlt = false;
                    return;
                }

                zurueckSetzenBorder();
                ausgewaehlteFigur = figur;
                felderMarkieren(figur);
                return;
            }

            // 4) Figur ist angewaehlt --> pruefen ob Feld in moegliche Zuege, und dann ziehen, ansonsten 
            // Figur angewaehlt lassen
            if (figurAngewaehlt 
             && Figur.zugMoeglich(panel.Name, ausgewaehlteFigur.listMoeglicheFelder()))
            {
                // Keine Zuege moeglich (z.B. Turm ganz am Anfang, wenn noch Bauern drum stehen)
                // --> Figuren abwaehlen --> Pruefen!
                if (ausgewaehlteFigur.listMoeglicheFelder() == null)
                {
                    figurAngewaehlt = false;
                    ausgewaehlteFigur = null;
                    return;
                }
                
                // Rochade
                //--------------------------------------------------------------
                if(ausgewaehlteFigur.GetType() == typeof(Koenig))
                {
                    // Ist das angeklickte Feld ein Turm, mit dem eine Rochade moeglich ist?
                    if (((Koenig)ausgewaehlteFigur).Rochade(panel.Name) != null)
                    {
                        Rochade rochade = ((Koenig)ausgewaehlteFigur).Rochade(panel.Name);
                        Debug.WriteLine("Feld Koenig " + rochade.FeldKoenig + " Feld Turm " + rochade.FeldTurm +
                            " Zielfeld Koenig  " + rochade.ZielFeldKoenigRochade + "Zielfeld Turm " + rochade.ZielFeldTurmRochade);
                        rochadeZug(rochade);

                        figurAngewaehlt = false;
                        zurueckSetzenBorder();
                        this.farbeAmZug = farbeAmZugAendern();

                        return;
                    }
                }

                

                //--------------------------------------------------------------
                // Die Figur an das angeklickte Panel verschieben
                figurVerschieben(this.Controls.Find(panel.Name, true).First());


                // Figur vom alten Panel "entfernen
                figureVonPanelEntfernen(this.Controls.Find(ausgewaehlteFigur.AktuellePosition, true).First());

                // Position vom Objekt aktualisieren
                ausgewaehlteFigur.AktuellePosition = panel.Name;
                if (ausgewaehlteFigur.GetType() == typeof(Bauer)
                    && ((Bauer)ausgewaehlteFigur).EntfernterKey != String.Empty)
                {
     
                    setControlNull(this.Controls.Find(((Bauer)ausgewaehlteFigur).EntfernterKey, true).First());

                }
                // Bauernumwandlung
                if (ausgewaehlteFigur.GetType() == typeof(Bauer) 
                    && ((Bauer)ausgewaehlteFigur).spielFeldEndeEreicht())
                {
                
                    frmBauernumwandlung bauernumWandlung = new frmBauernumwandlung(ausgewaehlteFigur.Farbe, ausgewaehlteFigur.AktuellePosition, dctFigur);

                    if (bauernumWandlung.ShowDialog() == DialogResult.OK)
                    {
                        removeVonDictionary(bauernumWandlung.NeueFigur.AktuellePosition);
                        setControlNull(this.Controls.Find(bauernumWandlung.NeueFigur.AktuellePosition, true).First());
                        setControl(bauernumWandlung.NeueFigur, this.Controls.Find(bauernumWandlung.NeueFigur.AktuellePosition, true).First());

                    }

                }

                figurAngewaehlt = false;
                zurueckSetzenBorder();

                this.farbeAmZug = farbeAmZugAendern();
            }

        }

        private void rochadeZug(Rochade rochade)
        {
            rochadeDictUndFigurAktualisieren(rochade.FeldKoenig, rochade.ZielFeldKoenigRochade, rochade.Koenig);
            rochadeDictUndFigurAktualisieren(rochade.FeldTurm, rochade.ZielFeldTurmRochade, rochade.Turm);
        }

        // Die Zuege der Rochade machen --> loeschen der Figur vom Feld, setzen der Figur aufs neue Feld
        // Position aktualisieren
        private void rochadeDictUndFigurAktualisieren(string aktuellesFeld, string zielFeld, Figur figur)
        {
            Control c = this.Controls.Find(zielFeld, true).First();
            setControl(figur, c);
            setControlNull(this.Controls.Find(aktuellesFeld, true).First());
            setDictionary(zielFeld, figur);
            removeVonDictionary(figur.AktuellePosition);
            figur.AktuellePosition = zielFeld;
        }

        // Setzen des Tags und des Bildes des Panels
        private void setControl(Figur figur, Control c)
        {
            c.Tag = figur;
            c.BackgroundImage = figur.FigurBild;
        }

        // Loeschen einer Figur
        private void setControlNull(Control c)
        {
            c.Tag = null;
            c.BackgroundImage = null;
        }

        // Dictionry aktualisieren
        private void setDictionary(string key, Figur figur)
        {
            if (this.dctFigur.ContainsKey(key))
            {
                this.dctFigur.Remove(key);
            }
            
            this.dctFigur.Add(key, figur);
       
        }
        private void removeVonDictionary(string key)
        {
            if (this.dctFigur.ContainsKey(key))
            {
                this.dctFigur.Remove(key);
            }

        }
        private Farbe farbeAmZugAendern()
        {
            if(farbeAmZug == Farbe.Schwarz)
            {
                grpBoxFarbeAmZug.BackColor = Color.White;
                grpBoxFarbeAmZug.Refresh();
                return Farbe.Weiss;
            }
            grpBoxFarbeAmZug.BackColor = Color.Black;
            grpBoxFarbeAmZug.Refresh();
            return Farbe.Schwarz;
        }

        

        private void figurVerschieben(Control c)
        {
 
            // Sollte der Koenig geworfen werden --> Schachmatt!
            if(c.Tag != null)
            {    
                if(((Figur)c.Tag).GetType() == typeof(Koenig))
                {
                    c.Tag = this.ausgewaehlteFigur;
                    c.BackgroundImage = this.ausgewaehlteFigur.FigurBild;
                    figureVonPanelEntfernen(this.Controls.Find(ausgewaehlteFigur.AktuellePosition, true).First());

                    MessageBox.Show("Schachmatt!");
                    return;
                }
            }
            setControl(this.ausgewaehlteFigur, c);
            setDictionary(c.Name, this.ausgewaehlteFigur);
            
        }

        private void figureVonPanelEntfernen(Control c)
        {
            try
            {
                if (this.dctFigur.ContainsKey(c.Name))
                {
                    this.dctFigur.Remove(c.Name);
                }

                setControlNull(c);

            } catch (Exception e)
            {
                Debug.WriteLine("figureVonPanelEntfernen: " + e.Message);
            }
            
        }

        private void dictionaryAktualisieren()
        {
            this.dctFigur.Clear();

        }
        /*
         * Beim Anklicken werden Felder hervorgehoben, auf die gezogen werden kann
         * Dies wird hier zurueckgesetzt (falls z.B. eine neue Figur ausgewaehlt wurde, oder der Zug beendet wurde)
         * Dazu wird der paint Handler gewechselt
         */
          
        private void zurueckSetzenBorder()
        {
            // Zuruecksetzen der vorigen gewaehlten Figur
            foreach (Control c in this.moeglicheFelder)
            {

                // Gewaehltes Feld ist kein moeglicher Zug, aber eine neue Figur
                c.Paint -= panel_PaintBorder;
                c.Paint += panel_Paint;
                c.Invalidate();
            }
        }
        /*
         * Beim Anklicken einer Figur werden Felder hervorgehoben, die die moeglichen Zuege darstellen
         */
        private void felderMarkieren(Figur neueFigur)
        {
            

            foreach (string t in neueFigur.listMoeglicheFelder())
            {

                Control c = this.Controls.Find(t, true).First();
                try
                {
                    moeglicheFelder.Add(c);
                }catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
               

                c.Paint += panel_PaintBorder;
                c.Invalidate();

            }
        }



        /*
         * Eventhandler zum Entfernen der Border
         */
        private void panel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ((Panel)sender).ClientRectangle, ((Panel)sender).BackColor, ButtonBorderStyle.None);
        }

        /*
         * Eventhandler zum Malen der Border
         */
        private void panel_PaintBorder(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ((Panel)sender).ClientRectangle, 
                Color.GreenYellow, 7, ButtonBorderStyle.Outset,
                Color.GreenYellow, 7, ButtonBorderStyle.Outset,
                Color.GreenYellow, 7, ButtonBorderStyle.Outset,
                Color.GreenYellow, 7, ButtonBorderStyle.Outset);
        }

        private void FormSchachbrett_Load(object sender, EventArgs e)
        {

        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if(rdBSchwarz.Checked)
            {
                grpBoxFarbeAmZug.BackColor = Color.Black;
                farbeAmZug = Farbe.Schwarz;
            }
            if(rdBWeiss.Checked)
            {
                grpBoxFarbeAmZug.BackColor = Color.White;
                farbeAmZug = Farbe.Weiss;
            }
            
        }
        /**
         * Dem Panel werden die Figuren als Tags hinzugefuegt, weiterhin wird jede Figur 
         * mit dem Key (Spielfeldname) in ein Dictionary geschrieben
         **/
        private void addFigur(Figur figur, Panel p)
        {
            p.Tag = figur;
            p.BackgroundImage = figur.FigurBild;
            this.dctFigur.Add(p.Name, figur);
        }

        // Loeschen aller verbleibenden Figuren
        private void zurueckSetzen()
        {
            foreach (Control c in groupBoxFeld.Controls)
            {
                setControlNull(c);
                c.Enabled = false;
                c.Paint -= panel_PaintBorder;
                c.Paint += panel_Paint;
                c.Invalidate();
            }
            dctFigur.Clear();
        }

        private void panelsEnabled()
        {
            foreach (Control c in groupBoxFeld.Controls)
            {
                ((Panel)c).Enabled = true;
            }
        }

        private void panelsDisabled()
        {
            foreach (Control c in groupBoxFeld.Controls)
            {
                ((Panel)c).Enabled = false;
            }
        }
        private void startPositionierungen()
        {
            this.moeglicheZuege = new List<String>();
            this.moeglicheFelder = new List<Control>();

            panelsDisabled();

            // Komplett
            addFigur(new Bauer(Farbe.Weiss, a2.Name, "wBauer.png", this.dctFigur), a2);
            addFigur(new Bauer(Farbe.Weiss, b2.Name, "wBauer.png", this.dctFigur), b2);
            addFigur(new Bauer(Farbe.Weiss, c2.Name, "wBauer.png", this.dctFigur), c2);
            addFigur(new Bauer(Farbe.Weiss, d2.Name, "wBauer.png", this.dctFigur), d2);
            addFigur(new Bauer(Farbe.Weiss, e2.Name, "wBauer.png", this.dctFigur), e2);
            addFigur(new Bauer(Farbe.Weiss, f2.Name, "wBauer.png", this.dctFigur), f2);
            addFigur(new Bauer(Farbe.Weiss, g2.Name, "wBauer.png", this.dctFigur), g2);
            addFigur(new Bauer(Farbe.Weiss, h2.Name, "wBauer.png", this.dctFigur), h2);

            addFigur(new Bauer(Farbe.Schwarz, a7.Name, "sBauer.png", this.dctFigur), a7);
            addFigur(new Bauer(Farbe.Schwarz, b7.Name, "sBauer.png", this.dctFigur), b7);
            addFigur(new Bauer(Farbe.Schwarz, c7.Name, "sBauer.png", this.dctFigur), c7);
            addFigur(new Bauer(Farbe.Schwarz, d7.Name, "sBauer.png", this.dctFigur), d7);
            addFigur(new Bauer(Farbe.Schwarz, e7.Name, "sBauer.png", this.dctFigur), e7);
            addFigur(new Bauer(Farbe.Schwarz, f7.Name, "sBauer.png", this.dctFigur), f7);
            addFigur(new Bauer(Farbe.Schwarz, g7.Name, "sBauer.png", this.dctFigur), g7);
            addFigur(new Bauer(Farbe.Schwarz, h7.Name, "sBauer.png", this.dctFigur), h7);

            addFigur(new Turm(Farbe.Weiss, a1.Name, "wTurm.png", this.dctFigur), a1);
            addFigur(new Turm(Farbe.Weiss, h1.Name, "wTurm.png", this.dctFigur), h1);
            addFigur(new Turm(Farbe.Schwarz, a8.Name, "sTurm.png", this.dctFigur), a8);
            addFigur(new Turm(Farbe.Schwarz, h8.Name, "sTurm.png", this.dctFigur), h8);

            addFigur(new Laeufer(Farbe.Weiss, c1.Name, "wLaeufer.png", this.dctFigur), c1);
            addFigur(new Laeufer(Farbe.Weiss, f1.Name, "wLaeufer.png", this.dctFigur), f1);
            addFigur(new Laeufer(Farbe.Schwarz, c8.Name, "sLaeufer.png", this.dctFigur), c8);
            addFigur(new Laeufer(Farbe.Schwarz, f8.Name, "sLaeufer.png", this.dctFigur), f8);

            addFigur(new Dame(Farbe.Weiss, d1.Name, "wDame.png", this.dctFigur), d1);
            addFigur(new Dame(Farbe.Schwarz, d8.Name, "sDame.png", this.dctFigur), d8);

            addFigur(new Koenig(Farbe.Weiss, e1.Name, "wKoenig.png", this.dctFigur), e1);
            addFigur(new Koenig(Farbe.Schwarz, e8.Name, "sKoenig.png", this.dctFigur), e8);

            addFigur(new Pferd(Farbe.Weiss, b1.Name, "wPferd.png", this.dctFigur), b1);
            addFigur(new Pferd(Farbe.Weiss, g1.Name, "wPferd.png", this.dctFigur), g1);
            addFigur(new Pferd(Farbe.Schwarz, b8.Name, "sPferd.png", this.dctFigur), b8);
            addFigur(new Pferd(Farbe.Schwarz, g8.Name, "sPferd.png", this.dctFigur), g8);


        }
        private void startPositionierungenR()
        {
            this.moeglicheZuege = new List<String>();
            this.moeglicheFelder = new List<Control>();
            
            // Rochade
            addFigur(new Turm(Farbe.Weiss, a1.Name, "wTurm.png", this.dctFigur), a1);
            addFigur(new Koenig(Farbe.Weiss, e1.Name, "wKoenig.png", this.dctFigur), e1);
            addFigur(new Turm(Farbe.Weiss, h1.Name, "wTurm.png", this.dctFigur), h1);
            addFigur(new Koenig(Farbe.Schwarz, e8.Name, "sKoenig.png", this.dctFigur), e8);
            addFigur(new Turm(Farbe.Schwarz, a8.Name, "sTurm.png", this.dctFigur), a8);
            addFigur(new Turm(Farbe.Schwarz, h8.Name, "sTurm.png", this.dctFigur), h8);
            panelsDisabled();
        }

        private void startPositionierungenEn()
        {
            this.moeglicheZuege = new List<String>();
            this.moeglicheFelder = new List<Control>();
            // EnPesant
            addFigur(new Turm(Farbe.Schwarz, h8.Name, "sTurm.png", this.dctFigur), h8);
            addFigur(new Bauer(Farbe.Weiss, a2.Name, "wBauer.png", this.dctFigur), a2);
            addFigur(new Bauer(Farbe.Weiss, c2.Name, "wBauer.png", this.dctFigur), c2);
            addFigur(new Bauer(Farbe.Schwarz, b4.Name, "sBauer.png", this.dctFigur), b4);
            addFigur(new Bauer(Farbe.Schwarz, d4.Name, "sBauer.png", this.dctFigur), d4);
            panelsDisabled();
        }

        private void startPositionierungenB()
        {
            this.moeglicheZuege = new List<String>();
            this.moeglicheFelder = new List<Control>();
            // Bauernumwandlung
            addFigur(new Bauer(Farbe.Weiss, a6.Name, "wBauer.png", this.dctFigur), a6);
            addFigur(new Bauer(Farbe.Schwarz, b3.Name, "sBauer.png", this.dctFigur), b3);

            panelsDisabled();
        }



        private void btnTestSimulation_Click(object sender, EventArgs e)
        {
            PGNFileEinlesen pGN = new PGNFileEinlesen(cBoxDateiNamen.Text);
            Simulation.DctFigur = dctFigur;
            //reset(Reset.Komplett);

            // Den Header in die Textbox schreiben
            foreach (string s in pGN.CsvHeader)
            {
                rTextBoxZuege.AppendText(s + Environment.NewLine);
            }
            rTextBoxZuege.AppendText(Environment.NewLine);

            // Sleep vor erstem Zug
            System.Threading.Thread.Sleep(zeitIntervall);
            for (int i = 1; i <= pGN.DtcZuege.Count; i++)
            {

                // Falls Siegesbedingung oder Spiel vorzeitit beendet: (*)
                // 1-0 --> weiss gewinnt
                // 0-1 --> schwarz gewinnt
                // 1/2-1/2 --> Remi
                // * --> Partie ist noch nicht beendet   
                addTextRichTextbox(i, pGN.DtcZuege[i].Split(' ').First());

                if (partieBeendet(pGN.DtcZuege[i].Split(' ').First()))
                {
                    return;
                }
                Simulation.Zug(pGN.DtcZuege[i].Split(' ').First(), Farbe.Weiss);
                
                try
                {
                    simulationsZug();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("SimulationsZug() " + ex.Message);
                }
                

                System.Threading.Thread.Sleep(zeitIntervall);
                this.farbeAmZug = farbeAmZugAendern();

                addTextRichTextbox(pGN.DtcZuege[i].Split(' ').Last());

                if (partieBeendet(pGN.DtcZuege[i].Split(' ').Last()))
                {
                    btnReset.PerformClick();
                    return;
                }

                Simulation.Zug(pGN.DtcZuege[i].Split(' ').Last(), Farbe.Schwarz);
                try
                {
                    simulationsZug();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("SimulationsZug() " + ex.Message);
                }
               

                System.Threading.Thread.Sleep(zeitIntervall);
                this.farbeAmZug = farbeAmZugAendern();
            }

            btnReset.Enabled = true;


        }

        private void addTextRichTextbox(int zugnummer, string text)
        {
            rTextBoxZuege.AppendText(zugnummer + ". " + text);
            rTextBoxZuege.Refresh();
        }

        private void addTextRichTextbox(string text)
        {
            rTextBoxZuege.AppendText("       " + text + Environment.NewLine);
            
            rTextBoxZuege.Refresh();
        }

        private bool partieBeendet(string zug)
        {
            switch (zug)
            {
                default: return false;
                case "*": MessageBox.Show("Das Spiel wird an einem anderen Tag fortgesetzt");
                    return true;
                case "1-0": MessageBox.Show("Weiss hat das Spiel gewonnen");
                    return true;
                case "0-1": MessageBox.Show("Schwarz hat das Spiel gewonnen");
                    return true;
                case "1/2-1/2": MessageBox.Show("Das Spiel endete in einem Remi");
                    return true;
            }
        }
        private void simulationsZug()
        {
            // Alle Figuren "loeschen"
            foreach(Control c in groupBoxFeld.Controls)
            {
                c.BackgroundImage = null;
                c.Tag = null;
            }
            // Die Positionen der Figuren im neuen Dictionarys an die GUI uebertragen
            foreach (KeyValuePair<string, Figur> figur in dctFigur)
            {
                Control c = this.Controls.Find(figur.Key, true).First();
                c.BackgroundImage = figur.Value.FigurBild;
                c.Tag = figur.Value;
            }
            // Neu Zeichnen aller Elemente in der GroupBox erzwingen
            groupBoxFeld.Refresh();
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            lblTrackBar.Text = ((TrackBar)sender).Value.ToString() + " ms";
            zeitIntervall = ((TrackBar)sender).Value;
        }

        private void rTextBoxZuege_TextChanged(object sender, EventArgs e)
        {
            rTextBoxZuege.SelectionStart = rTextBoxZuege.Text.Length;
            rTextBoxZuege.ScrollToCaret();
        }

        private void btnRochade_Click(object sender, EventArgs e)
        {
            reset(Reset.Rochade);
            panelsEnabled();
            btnStart.Enabled = false;
            grpBoxFarbeBeginnt.Enabled = false;
            btnReset.Enabled = true;
        }

        private void btnBauernumwandlung_Click(object sender, EventArgs e)
        {
            reset(Reset.Bauernumwandlung);
            panelsEnabled();
            btnStart.Enabled = false;
            grpBoxFarbeBeginnt.Enabled = false;
            btnReset.Enabled = true;
        }

        private void btnEnPassant_Click(object sender, EventArgs e)
        {
            reset(Reset.enPeasant);
            panelsEnabled();
            btnStart.Enabled = false;
            grpBoxFarbeBeginnt.Enabled = false;
            btnReset.Enabled = true;
        }
    }
}

