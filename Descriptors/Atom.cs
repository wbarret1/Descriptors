using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Descriptors
{
 
    class Atom
    {
        List<Atom> bondedAtoms;

        public Atom(string element)
        {
            bondedAtoms = new List<Atom>();
            this.SetElement(element);
        }

        ELEMENTS e;
        void SetElement(string element)
        {
            e = (ELEMENTS)Enum.Parse(typeof(ELEMENTS), element);
        }

        public ELEMENTS Element { get { return e; } }

        public double x { get; set; } = 0.0;
        public double y { get; set; } = 0.0;
        public double z { get; set; } = 0.0;
        public string AtomicSymbol
        {
            get
            {
                return e.ToString();
            }
        }

        public string AtomicName
        {
            get
            {
                return Descriptors.Element.Name(e);
            }
        }
        public int massDiff { get; set; } = 0;
        public int charge { get; set; } = 0;
        public int stereoParity { get; set; } = 0;
        public int hydrogenCount { get; set; } = 0;
        public int stereoCareBox { get; set; } = 0;
        public int valence { get; set; } = 0;
        //public int HO;
        public string rNotUsed { get; set; } = string.Empty;
        public string iNotUsed { get; set; } = string.Empty;
        public int atomMapping { get; set; } = 0;
        public int inversionRetension { get; set; } = 0;
        public int exactChange { get; set; } = 0;

        public void AddBond(Atom atom)
        {
            this.bondedAtoms.Add(atom);
        }

        public Atom[] BondedAtoms
        {
            get { return bondedAtoms.ToArray<Atom>(); }
        }
    }
}
