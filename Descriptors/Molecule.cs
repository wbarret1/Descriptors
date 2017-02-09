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
        //List<List<int>> cycles;
        List<List<Atom>> cycles;
        bool cyclesFound;

        public Molecule()
        {
            atoms = new List<Atom>();
            cyclesFound = false;
        }

        public Atom[] GetAtoms()
        {
            return this.atoms.ToArray<Atom>();
        }

        public Atom[] findElement(string elementToFind)
        {
            List<Atom> retVal = new List<Atom>();
            foreach (Atom a in atoms)
            {
                if (a.element == elementToFind)
                {
                    retVal.Add(a);
                }
            }
            return retVal.ToArray();
        }

        public int[] FindElementIndices(string elementToFind)
        {
            List<int> retVal = new List<int>();
            foreach (Atom a in atoms)
            {
                if (a.element == elementToFind)
                {
                    retVal.Add(atoms.IndexOf(a));
                }
            }
            return retVal.ToArray();
        }

        public String[] Elements()
        {
            List<string> retVal = new List<string>();
            foreach (Atom a in atoms)
            {
                if (!retVal.Contains(a.element)) retVal.Add(a.element);
            }
            return retVal.ToArray();
        }

        public void AddAtom(Atom a)
        {
            atoms.Add(a);
            cyclesFound = false;
        }

        public void AddBond(int atomOne, int atomTwo)
        {
            atoms[atomOne].AddBond(atoms[atomTwo]);
            atoms[atomTwo].AddBond(atoms[atomOne]);
            cyclesFound = false;
        }

        public void AddBond(Atom atomOne, Atom atomTwo)
        {
            atomOne.AddBond(atomTwo);
            atomTwo.AddBond(atomOne);
            cyclesFound = false;
        }

        //public int[][] GetDFS()
        //{
        //    this.FindRings();
        //    Stack<int> myStack = new Stack<int>();
        //    foreach (IGraphObject a in atoms)
        //    {
        //        a.visited = false;
        //        a.finished = false;
        //    }
        //    cycles = new List<List<int>>();
        //    for (int i = 0; i < atoms.Count; i++)
        //    {
        //        search(i, i, myStack, cycles);
        //    }
        //    cyclesFound = true;
        //    this.ExtractRings();
        //    return convertToArrayArray(cycles);
        //}

        public int[][] FindRings()
        {
            Stack<Atom> myStack = new Stack<Atom>();
            foreach (IGraphObject a in atoms)
            {
                a.visited = false;
                a.finished = false;
            }
            cycles = new List<List<Atom>>();
             //for (int i = 0; i < atoms.Count; i++)
           foreach (Atom a in atoms)
            {
                search(a, a, myStack, cycles);
            }
            cyclesFound = true;
            this.ExtractRings();
            return this.convertToArrayArray(convertToInts(cycles));
        }

        //void search(int current, int parent, Stack<int> stack, List<List<int>> cycles)
        //{
        //    stack.Push(current + 1);
        //    for (int i = 0; i < ((IGraphObject)atoms[current]).connections().Length; i++)
        //    {
        //        int next = ((IGraphObject)atoms[current]).connections()[i] + 1;
        //        if (next != parent + 1)
        //        {
        //            if (stack.Contains(next))
        //            {
        //                List<int> cycle = new List<int>();
        //                foreach (int a in stack)
        //                {
        //                    cycle.Add(a);
        //                    if (a == next) break;
        //                }
        //                cycles.Add(cycle);
        //                stack.Pop();
        //                return;
        //            }
        //            else if (!((IGraphObject)atoms[next - 1]).finished)
        //            {
        //                //((IGraphObject)atoms[current]).visited = true;
        //                search(next - 1, current, stack, cycles);
        //            }
        //        }
        //    }
        //    if (stack.Count != 0) stack.Pop();
        //    ((IGraphObject)atoms[current]).finished = true;
        //}

        void search(Atom current, Atom parent, Stack<Atom> stack, List<List<Atom>> cycles)
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
                    else if (!((IGraphObject)next).finished)
                    {
                        search(next, current, stack, cycles);
                    }
                }
            }
            if (stack.Count != 0) stack.Pop();
            ((IGraphObject)current).finished = true;
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
            //if (!cyclesFound) this.GetDFS();
            List<List<int>> loops = this.convertToInts(cycles);
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
                                cycles[j].Insert(cycles[j].IndexOf(m[0])+1, bridge);
                            }
                        }
                    }
                }
            }
        }

        List<List<int>> convertToInts(List<List<Atom>> lists)
        {
            List<List<int>> retVal = new List<List<int>>();
            foreach (List<Atom> list in lists)
            {
                List<int> intList = new List<int>();
                foreach (Atom a in list)
                {
                    intList.Add(atoms.IndexOf(a) + 1);
                }
                retVal.Add(intList);
            }
            return retVal;
        }

        //int[][] FusedRings()
        //{
        //    if (!cyclesFound) this.GetDFS();
        //    for (int i = 0; i < cycles.Count; i++)
        //    {
        //        for (int j = i + 1; j < cycles.Count; j++)
        //        {
        //            List<int> matches = new List<int>();
        //            foreach (int val in cycles[j])
        //            {
        //                int location = cycles[i].IndexOf(val);
        //                if (location >= 0) matches.Add(val);
        //            }
        //        }
        //    }
        //    return null;// convertToIntArrayArray(rings);
        //}

        int CompareListsByLength<T>(List<T> list1, List<T> list2)
        {
            return list1.Count - list2.Count;
        }

        public Atom[][] HeteroCyclic()
        {
            if (!cyclesFound) this.FindRings();
            List<List<Atom>> retVal = new List<List<Atom>>();
            foreach (List<Atom> cycle in cycles)
            {
                foreach (Atom i in cycle)
                {
                    if (i.element != "C")
                    {
                        retVal.Add(cycle);
                        break;
                    }
                }
            }
            return convertToArrayArray(retVal);
        }

        public Atom[][] HeteroCyclic(string element)
        {
            if (!cyclesFound) this.FindRings();
            List<List<Atom>> retVal = new List<List<Atom>>();
            foreach (List<Atom> cycle in cycles)
            {
                foreach (Atom i in cycle)
                {
                    if (i.element == element)
                    {
                        retVal.Add(cycle);
                        break;
                    }
                }
            }
            return convertToArrayArray(retVal);
        }

        public int[] BranchAtomIndices()
        {
            List<int> retVal = new List<int>();
            for (int i = 0; i < atoms.Count; i++)
            {
                if (atoms[i].BondedAtoms.Length > 2) retVal.Add(i);
            }
            return retVal.ToArray<int>();
        }
    }
}