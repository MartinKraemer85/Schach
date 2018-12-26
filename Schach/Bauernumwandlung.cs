using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schach
{
    public  enum  Test
    {
        Weiss,
        Schwarz
    }

    public partial class frmBauernumwandlung : Form
    {

        private Farbe farbe = Farbe.Weiss;
        private string position;
        private Figur neueFigur;
        private Dictionary<string, Figur> dctFigur;

        public Figur NeueFigur
        {
            get { return this.neueFigur; }
        }

        

        private Bitmap bild(string bildName)
        {
            return new Bitmap(Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..", "Bilder", bildName+".png")));

        }


        public frmBauernumwandlung(Farbe farbe,string position, Dictionary<string, Figur> dctFigur)
        {
            InitializeComponent();
            this.farbe = farbe;
            this.position = position;
            this.dctFigur = dctFigur;
            setBilder();
        }

        private void setBilder()
        {
            string prefix = "w";
            if (Farbe.Schwarz == this.farbe)
            {
                prefix = "s";
            }
            btnDame.BackgroundImage = bild(prefix + "Dame");
            btnDame.Tag = (prefix + "Dame" + ".png");
            btnLaeufer.BackgroundImage = bild(prefix + "Laeufer");
            btnLaeufer.Tag = (prefix + "Laeufer" + ".png");
            btnPferd.BackgroundImage = bild(prefix + "Pferd");
            btnPferd.Tag = (prefix + "Pferd" + ".png");
            btnTurm.BackgroundImage = bild(prefix + "Turm");
            btnTurm.Tag = (prefix + "Turm" + ".png");
        }


        private void btn_Click(object sender, EventArgs e)
        {
            Button tmpSender = (Button)sender;
            switch (tmpSender.Name)
            {
                case "btnDame":
                    neueFigur = new Dame(this.farbe, this.position, tmpSender.Tag.ToString(), dctFigur);
                    return;

                case "btnLaeufer":
                    neueFigur = new Laeufer(this.farbe, this.position, tmpSender.Tag.ToString(), dctFigur);
                    return;

                case "btnPferd":
                    neueFigur = new Pferd(this.farbe, this.position, tmpSender.Tag.ToString(), dctFigur);
                    return;

                case "btnTurm":
                    neueFigur = new Turm(this.farbe, this.position, tmpSender.Tag.ToString(), dctFigur);
                    return;
            }

        }
    }
}
