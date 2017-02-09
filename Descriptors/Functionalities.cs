using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Descriptors
{
    static class Functionalities
    {
        static public Atom[] findPhosphate(Molecule m)
        {
            List<Atom> retVal = new List<Atom>();
            Atom[] atoms = m.GetAtoms();
            foreach (Atom a1 in atoms)
            {
                if (a1.element == "P")
                {
                    if (a1.BondedAtoms.Length == 4)
                    {
                        bool allOxygen = true;
                        foreach (Atom a2 in a1.BondedAtoms)
                        {
                            if (a2.element != "O") allOxygen = false;
                        }
                        if (allOxygen) retVal.Add(a1);
                    }
                }
            }
            return retVal.ToArray<Atom>();
        }

        static public String[] Elements(Molecule m)
        {
            List<string> retVal = new List<string>();
            Atom[] atoms = m.GetAtoms();
            foreach (Atom a in atoms)
            {
                if (!retVal.Contains(a.element)) retVal.Add(a.element);
            }
            return retVal.ToArray();
        }

        static public Atom[] FindElement(Molecule m, String element)
        {
            List<Atom> retVal = new List<Atom>();
            Atom[] atoms = m.GetAtoms();
            foreach (Atom a in atoms)
            {
                if (a.element == element)
                {
                    retVal.Add(a);
                }
            }
            return retVal.ToArray<Atom>();
        }

        static public Atom[] FindElements(Molecule m, string[] elements)
        {
            List<Atom> retVal = new List<Atom>();
            Atom[] atoms = m.GetAtoms();
            foreach (Atom a in atoms)
            {
                if (elements.Contains(a.element))
                {
                    retVal.Add(a);
                }
            }
            return retVal.ToArray<Atom>();
        }

        static public Atom[] FindChloride(Molecule m)
        {
            return FindElement(m, "Cl");
        }

        static public Atom[] FindHalides(Molecule m)
        {
            string[] halogens = { "F", "Cl", "Br", "I" };
            return FindElements(m, halogens);
        }

        static public Atom[] FindBromide(Molecule m)
        {
            return FindElement(m, "Br");
        }

        static public Atom[] BranchAtoms(Molecule m)
        {
            List<Atom> retVal = new List<Atom>();
            foreach (Atom a in m.GetAtoms())
            {
                if (a.BondedAtoms.Length > 2) retVal.Add(a);
            }
            return retVal.ToArray();
        }

        static public Atom[][] HeteroCyclic(Molecule m, string element)
        {
            //if (!cyclesFound) this.GetDFS();
            Atom[][] rings = m.FindRings();
            List<Atom[]> retVal = new List<Atom[]>();
            foreach (Atom[] atoms in rings)
            {
                foreach (Atom a in atoms)
                {
                    if (a.element == element)
                    {
                        retVal.Add(atoms);
                        break;
                    }
                }
            }
            return retVal.ToArray();
        }

        static public Atom[][] HeteroCyclic(Molecule m)
        {
            //if (!cyclesFound) this.GetDFS();
            Atom[][] rings = m.FindRings();
            List<Atom[]> retVal = new List<Atom[]>();
            foreach (Atom[] atoms in rings)
            {
                foreach (Atom a in atoms)
                {
                    if (a.element != "C")
                    {
                        retVal.Add(atoms);
                        break;
                    }
                }
            }
            return retVal.ToArray();
        }

        static public string[] PhosphorousFunctionality(Molecule m)
        {
            List<string> retVal = new List<string>();

            // Get the P atons.
            Atom[] p = FindElement(m, "P");

            // If there aren't any, returnn an array with zero elements.
            if (p.Length == 0) return retVal.ToArray<string>();

            // Look for a heterocyclic containing P
            Atom[][] pHeteorRings = HeteroCyclic(m, "P");
            if (pHeteorRings.Length != 0)
            {
                // If there is one, add the 'phosphorine' functionality.
                retVal.Add("phosphorine");
            }

            // Now lets look what is attached to each Phosphorous atom...
            foreach(Atom a in p)
            {
                List<Atom> O = new List<Atom>();
                List<Atom> N = new List<Atom>();
                List<Atom> S = new List<Atom>();
                List<Atom> C = new List<Atom>();
                List<Atom> P = new List<Atom>();
                //Atom a = m.GetAtoms()[p[i]];
                //Atom[] bonded = a.BondedAtoms;
                //for (int j = 0; j < bonded.Length; j++)
                // If it connected to two other atoms...
                foreach (Atom bonded in a.BondedAtoms)
                {
                    switch (bonded.element)
                    {
                        case "C":
                            C.Add(bonded);
                            break;
                        case "N":
                            N.Add(bonded);
                            break;
                        case "O":
                            O.Add(bonded);
                            break;
                        case "P":
                            P.Add(bonded);
                            break;
                        case "S":
                            S.Add(bonded);
                            break;
                    }
                }

                if (a.BondedAtoms.Length == 2)
                {

                }
                if (a.BondedAtoms.Length == 3)
                {
                    if (C.Count == 3)
                    {
                        retVal.Add("phosphine");
                    }
                    if (O.Count == 3)
                    {
                        retVal.Add("phosphite");
                    }
                    if ((O.Count == 2) && (C.Count == 1))
                    {
                        retVal.Add("phosphonite");
                    }
                    if ((O.Count == 1) && (C.Count == 2))
                    {
                        retVal.Add("phosphinite");
                    }
                    if ((O.Count == 2) && (N.Count == 1))
                    {
                        retVal.Add("phosphoramidite");
                    }
                    if ((O.Count == 1) && (N.Count == 2))
                    {
                        retVal.Add("phosphorodiamidite");
                    }
                    if ((O.Count == 1) && (N.Count == 1) && (C.Count == 1))
                    {
                        retVal.Add("phosphonamidite");
                    }
                    if ((O.Count == 2) && (S.Count == 1))
                    {
                        retVal.Add("phosphorothioite");
                    }
                    if ((O.Count == 1) && (S.Count == 2))
                    {
                        retVal.Add("phosphorodithioite");
                    }
                }
                if (a.BondedAtoms.Length == 4)
                {
                    if (O.Count == 4)
                    {
                        retVal.Add("phosphate");
                    }
                    if ((O.Count == 3) && (C.Count == 1))
                    {
                        retVal.Add("phosphonate");
                    }
                    if ((O.Count == 2) && (C.Count == 2))
                    {
                        retVal.Add("phosphinate");
                    }
                    if ((O.Count == 1) && (C.Count == 3))
                    {
                        retVal.Add("phospine oxide");
                    }
                    if ((O.Count == 3) && (N.Count == 1))
                    {
                        retVal.Add("phosphoramidate");
                    }
                    if ((O.Count == 2) && (N.Count == 2))
                    {
                        retVal.Add("phosphoramidate");
                    }
                    if ((O.Count == 1) && (N.Count == 3))
                    {
                        retVal.Add("phosphoramide");
                    }
                    if ((O.Count == 2) && (N.Count == 1) && (C.Count == 1))
                    {
                        retVal.Add("phosphonamidate");
                    }
                    if ((O.Count == 1) && (N.Count == 2) && (C.Count == 1))
                    {
                        retVal.Add("phosphonamide");
                    }
                    if ((O.Count == 1) && (N.Count == 1) && (C.Count == 2))
                    {
                        retVal.Add("phosphinamide");
                    }
                    if ((O.Count == 2) && (S.Count == 1))
                    {
                        retVal.Add("phosphorothioite");
                    }
                    if ((O.Count == 1) && (S.Count == 2))
                    {
                        retVal.Add("phosphorodithioite");
                    }
                }
            }
            return retVal.ToArray<string>();
        }

    }
}
