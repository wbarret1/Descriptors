using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Descriptors
{
    public enum BondType
    {
        Single = 1,
        Double = 2,
        Triple = 3,
        Aromatic = 4,
        SingleOrDouble = 5,
        SingleOrAromatic = 6,
        DoubleOrAromatic = 7,
        Any = 8
    }

    public enum BondStereo
    {
        NotStereoOrUseXYZ = 0,
        Up = 1,
        cisOrTrans = 3,
        Down = 4,
        Either = 6,
    }

    public enum BondTopology
    {
        Either = 0,
        Ring = 1,
        Chain = 3,
    }

    [Flags]
    public enum BondReactingCenterStatus
    {
        notACenter = -1,
        Unmarked = 0,
        aCenter = 1,
        noChange = 2,
        bondMadeOrBroken = 4,
        bondOrderChanges = 8
    }

    public struct Bond
    {
        public int firstAtom;
        public int secondAtom;
        public BondType bondType;
        public BondStereo bondStereo;
        public string xNotUsed;
        public BondTopology bondTopology;
        public BondReactingCenterStatus reactingCenter;
    }

    //functional group based upon Kidus' fragments from TEST.
    public struct group
    {
        public string name;
        public string[] fragments;
        public string[] testFragments;
        public int[] count;
    }

    class Molecule
    {
        List<Atom> atoms;
        List<List<Atom>> cycles;
        List<List<Atom>> fusedRings;
        bool ringsFound;

        public Molecule()
        {
            atoms = new List<Atom>();
            ringsFound = false;
        }

        public Atom[] GetAtoms()
        {
            return this.atoms.ToArray<Atom>();
        }

        //Atom[] findElement(string elementToFind)
        //{
        //    List<Atom> retVal = new List<Atom>();
        //    foreach (Atom a in atoms)
        //    {
        //        if (a.element == elementToFind)
        //        {
        //            retVal.Add(a);
        //        }
        //    }
        //    return retVal.ToArray();
        //}

        //int[] FindElementIndices(string elementToFind)
        //{
        //    List<int> retVal = new List<int>();
        //    foreach (Atom a in atoms)
        //    {
        //        if (a.element == elementToFind)
        //        {
        //            retVal.Add(atoms.IndexOf(a));
        //        }
        //    }
        //    return retVal.ToArray();
        //}

        //public String[] Elements()
        //{
        //    List<string> retVal = new List<string>();
        //    foreach (Atom a in atoms)
        //    {
        //        if (!retVal.Contains(a.element)) retVal.Add(a.element);
        //    }
        //    return retVal.ToArray();
        //}

        public void AddAtom(Atom a)
        {
            atoms.Add(a);
            ringsFound = false;
        }

        public void AddBond(int atomOne, int atomTwo)
        {
            atoms[atomOne].AddBond(atoms[atomTwo]);
            atoms[atomTwo].AddBond(atoms[atomOne]);
            ringsFound = false;
        }

        public void AddBond(Atom atomOne, Atom atomTwo)
        {
            atomOne.AddBond(atomTwo);
            atomTwo.AddBond(atomOne);
            ringsFound = false;
        }

        public Atom[][] FindRings()
        {
            if (!ringsFound)
            {
                Stack<Atom> myStack = new Stack<Atom>();
                bool[] finished = new bool[atoms.Count];
                for (int i = 0; i < atoms.Count; i++) finished[i] = false;
                cycles = new List<List<Atom>>();
                foreach (Atom a in atoms)
                {
                    search(a, a, finished, myStack, cycles);
                }
                this.ExtractRings();
                this.FusedRings();
                ringsFound = true;
            }
            return this.convertToArrayArray(cycles);
        }

        void search(Atom current, Atom parent, bool[] f, Stack<Atom> stack, List<List<Atom>> cycles)
        {
            stack.Push(current);
            foreach (Atom next in current.BondedAtoms)
            {
                if (next != parent)
                {
                    if (stack.Contains(next))
                    {
                        List<Atom> cycle = new List<Atom>();
                        foreach (Atom a in stack)
                        {
                            cycle.Add(a);
                            if (a == next) break;
                        }
                        cycles.Add(cycle);
                        stack.Pop();
                        return;
                    }
                    else if (!f[atoms.IndexOf(next)])
                    {
                        search(next, current, f, stack, cycles);
                    }
                }
            }
            if (stack.Count != 0) stack.Pop();
            f[atoms.IndexOf(current)] = true;
        }

        T[][] convertToArrayArray<T>(List<List<T>> lists)
        {
            T[][] retVal = new T[lists.Count][];
            for (int i = 0; i < lists.Count; i++)
            {
                retVal[i] = lists[i].ToArray<T>();
            }
            return retVal;
        }

        List<T> matches<T>(List<T> list1, List<T> list2)
        {
            List<T> retVal = new List<T>();
            foreach (T val in list1)
            {
                int location = list2.IndexOf(val);
                if (location >= 0)
                {
                    retVal.Add(val);
                }
            }
            return retVal;
        }

        void ExtractRings()
        {
            cycles.Sort(CompareListsByLength);
            for (int i = 0; i < cycles.Count; i++)
            {
                for (int j = i + 1; j < cycles.Count; j++)
                {
                    List<Atom> m = matches(cycles[j], cycles[i]);
                    if (m.Count > 2)
                    {
                        if (m.Count == cycles[i].Count)
                        {
                            Atom[] start = m[0].BondedAtoms;
                            int first = cycles[j].IndexOf(m[0]);
                            int last = cycles[j].IndexOf(m[m.Count - 1]);
                            if (last < first)
                            {
                                cycles[j].RemoveRange(first, last - first);
                            }
                            else
                            {
                                if (cycles[j][0] == m[0])
                                {
                                    last = cycles[j].IndexOf(m[1]);
                                    int count = cycles[j].Count - last;
                                    cycles[j].RemoveRange(last + 1, count - 1);
                                }
                                else
                                {
                                    List<int> ints = new List<int>();
                                    foreach (Atom a in m)
                                    {
                                        ints.Add(atoms.IndexOf(a) + 1);
                                    }
                                    cycles[j].RemoveRange(cycles[j].IndexOf(m[1]), m.Count - 2);
                                }
                            }
                        }
                        else
                        {
                            Atom[] start = m[0].BondedAtoms;
                            Atom[] end = m[m.Count - 1].BondedAtoms;
                            Atom bridge = null;
                            foreach (Atom n in end)
                            {
                                if (start.Contains(n)) bridge = n;
                            }
                            if (bridge != null)
                            {
                                for (int k = 1; k < m.Count - 1; k++)
                                {
                                    cycles[j].Remove(m[k]);
                                }
                                cycles[j].Insert(cycles[j].IndexOf(m[0]) + 1, bridge);
                            }
                        }
                    }
                }
            }
        }

        void FusedRings()
        {
            fusedRings = new List<List<Atom>>();
            bool[] touched = new bool[cycles.Count];
            bool[] fused = new bool[cycles.Count];
            List<int[]> touching = new List<int[]>();
            List<List<int>> groups = new List<List<int>>();
            for (int i = 0; i < cycles.Count; i++)
            {
                touched[i] = false;
                fused[i] = false;
                for (int j = i + 1; j < cycles.Count; j++)
                {
                    touched[i] = true;
                    List<Atom> m = matches(cycles[j], cycles[i]);
                    if (m.Count > 1)
                    {
                        int[] f = new int[2];
                        f[0] = i;
                        f[1] = j;
                        touching.Add(f);
                    }
                }
            }
            foreach (int[] ar in touching)
            {
                if (!fused[ar[0]] && !fused[ar[1]])
                {
                    List<int> g = new List<int>();
                    fused[ar[0]] = true;
                    fused[ar[1]] = true;
                    g.AddRange(ar);
                    groups.Add(g);
                }
                else if (!fused[ar[0]] && fused[ar[1]])
                {
                    foreach (List<int> g in groups)
                    {
                        if (g.Contains(ar[1])) g.Add(ar[0]);
                        fused[ar[0]] = true;
                    }
                }
                else if (fused[ar[0]] && !fused[ar[1]])
                {
                    foreach (List<int> g in groups)
                    {
                        if (g.Contains(ar[0])) g.Add(ar[1]);
                        fused[ar[1]] = true;
                    }
                }
            }
            foreach (List<int> g in groups)
            {
                List<Atom> atoms = new List<Atom>();
                for (int i = 0; i < g.Count; i++)
                {
                    List<Atom> a = cycles[g[i]];
                    foreach(Atom at in a)
                    {
                        if (!atoms.Contains(at)) atoms.Add(at);
                    }
                }
                fusedRings.Add(atoms);
            }
        }

        int CompareListsByLength<T>(List<T> list1, List<T> list2)
        {
            return list1.Count - list2.Count;
        }
    }
}