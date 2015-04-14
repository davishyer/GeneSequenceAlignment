using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticsLab
{
    class GeneSequence
    {
        public GeneSequence(string name, string sequence)
        {
            this.name = name;
            this.sequence = sequence;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Sequence
        {
            get
            {
                return sequence.ToUpper();
            }
        }

        private string name, sequence;
    }
}
