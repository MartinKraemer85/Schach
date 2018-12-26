using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schach
{
    class Rochade
    {
        private string feldTurm;
        private string zielFeldTurmRochade;
        private string feldKoenig;
        private string zielFeldKoenigRochade;
        private Figur turm;
        private Figur koenig;

        public Rochade(string feldTurm, string zielFeldTurmRochade, 
                       string feldKoenig, string zielFeldKoenigRochade,
                       Figur turm, Figur koenig)
        {
            this.feldTurm = feldTurm;
            this.zielFeldTurmRochade = zielFeldTurmRochade;
            this.feldKoenig = feldKoenig;
            this.zielFeldKoenigRochade = zielFeldKoenigRochade;
            this.turm = turm;
            this.koenig = koenig;
        }

        public string FeldTurm
        {
            get { return this.feldTurm; }
        }

        public string ZielFeldTurmRochade
        {
            get { return this.zielFeldTurmRochade; }
        }

        public string FeldKoenig
        {
            get { return this.feldKoenig; }
        }

        public string ZielFeldKoenigRochade
        {
            get { return this.zielFeldKoenigRochade; }
        }

        public Figur Turm
        {
            get { return this.turm; }
        }

        public Figur Koenig
        {
            get { return this.koenig; }
        }
    }
}
