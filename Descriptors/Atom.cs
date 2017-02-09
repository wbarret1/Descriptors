using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Descriptors
{
    enum element
    {
        hydrogen = 1,
        carbon = 6,
        nitrogen = 7,
        oxygen = 8,

    };

    class Atom
    {
        List<Atom> bondedAtoms;

        public Atom()
        {
            bondedAtoms = new List<Atom>();
        }

        public double x { get; set; } = 0.0;
        public double y { get; set; } = 0.0;
        public double z { get; set; } = 0.0;
        public string element { get; set; } = string.Empty;
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
