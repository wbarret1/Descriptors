using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Descriptors
{
    public struct descriptor
    {
        public string name;
        public double value;
    }

    public struct Fragment
    {
        public int atomNumber;
        public string element;
        public string fragment;
    }

    public partial class Form1 : Form
    {
        List<descriptor> descriptors;
        List<descriptor> nonZeroDescriptors;
        List<descriptor> nonZeroFragments;
        List<Fragment> assignedFragments;
        List<group> functionalGroups;
        Molecule molecule;
        System.Data.DataTable atomTable;
        System.Data.DataTable bondTable;

        public Form1()
        {
            //this.parseSmiles("c1ccccc1");
            InitializeComponent();
            this.importGroups();
            this.listView1.Columns.Add("Name", -2, HorizontalAlignment.Left);
            string[] directories = System.IO.Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ToxRuns", "StructureData", System.IO.SearchOption.AllDirectories);
            int i = 0;
            foreach (string directory in directories)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(directory + "\\structure.png");
                imageList1.Images.Add(image);
                System.IO.DirectoryInfo info = System.IO.Directory.GetParent(directory);
                ListViewItem item = new ListViewItem(info.Name) { ImageIndex = i++ };
                this.listView1.Items.Add(item);
            }
            descriptors = new List<descriptor>();
            nonZeroDescriptors = new List<descriptor>();
            nonZeroFragments = new List<descriptor>();
            assignedFragments = new List<Fragment>();
            this.CreateDataTables();
        }

        void importGroups()
        {
            functionalGroups = new List<Descriptors.group>();
            string[] lines = Properties.Resources.Copy_of_Full_Functional_Group_List.Split('\n');
            group g = new group();
            foreach (string line in lines)
            {
                string[] parts = line.Split('\t');
                if (!string.IsNullOrEmpty(parts[0]))
                {
                    g.name = parts[0];
                    List<int> counts = new List<int>();
                    List<string> frags = new List<string>();
                    List<string> testFrags = new List<string>();
                    for (int i = 2; i < 20; i = i + 3)
                    {
                        if (!string.IsNullOrEmpty(parts[i]))
                        {
                            counts.Add(Convert.ToInt32(parts[i]));
                            frags.Add(parts[i + 1]);
                            testFrags.Add(parts[i + 2]);
                        }
                    }
                    g.count = counts.ToArray<int>();
                    g.fragments = frags.ToArray<string>();
                    g.testFragments = testFrags.ToArray<string>();
                    functionalGroups.Add(g);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ToxRuns\\" + this.listView1.SelectedItems[0].Text;
            this.ImportDescriptors(directory + "\\StructureData\\2d_descriptors.txt");
            this.ImportAssignedFragments(directory + "\\StructureData\\AssignedFragments.html");
            string[] fileNames = System.IO.Directory.GetFiles(directory + "\\StructureData", "*.mol");
            foreach (string filename in fileNames)
            {
                this.ImportMolFile(filename);
            }
            string[] groups = this.TestGroups(nonZeroFragments);
            string[] elements = molecule.Elements();
            Atom[][] rings = molecule.FindRings();
            Atom[] phosphates = Functionalities.findPhosphate(molecule);
            Atom[] chlorides = Functionalities.FindChloride(molecule);
            Atom[] bromides = Functionalities.FindBromide(molecule);
            Atom[] halides = Functionalities.FindHalides(molecule);
            Atom[][] hetero = Functionalities.HeteroCyclic(molecule);
            Atom[][] thio = Functionalities.HeteroCyclic(molecule, "S");
            Atom[][] furan = Functionalities.HeteroCyclic(molecule, "O");
            Atom[][] azo = Functionalities.HeteroCyclic(molecule, "N");
            Atom[] branchAtoms = Functionalities.BranchAtoms(molecule);
            string[] phosphs = Functionalities.PhosphorousFunctionality(molecule);
            //Form2 form = new Descriptors.Form2(nonZeroFragments);
            //form.Show();
        }

        string[] TestGroups(List<descriptor> fragments)
        {
            List<group> groups = new List<group>();
            groups.AddRange(functionalGroups);
            for (int i = 0; i < 5; i++)
            {

                foreach (group g in groups.Reverse<group>())
                {
                    if (g.testFragments != null)
                    {
                        if (g.testFragments.Length > i)
                        {

                            bool outer = false;
                            foreach (descriptor d in fragments)
                            {
                                if (g.testFragments[i] == d.name) outer = true;
                            }
                            //if (g.testFragments[i] == "-Cl [aliphatic attach]") outer = true;
                            if (!outer) groups.Remove(g);
                        }
                        if (g.testFragments.Length == 0) groups.Remove(g);
                    }
                    else groups.Remove(g);
                }
            }
            List<string> retVal = new List<string>();
            foreach (group g in groups)
            {
                retVal.Add(g.name);
            }
            return retVal.ToArray<string>();
        }

        void CreateDataTables()
        {
            atomTable = new System.Data.DataTable();
            atomTable.Columns.Add(new DataColumn("X", typeof(System.Double)));
            atomTable.Columns.Add(new DataColumn("Y", typeof(System.Double)));
            atomTable.Columns.Add(new DataColumn("Z", typeof(System.Double)));
            atomTable.Columns.Add(new DataColumn("element", typeof(System.String)));
            atomTable.Columns.Add(new DataColumn("MassDiff", typeof(System.Int32)));
            atomTable.Columns.Add(new DataColumn("Charge", typeof(System.Int32)));
            atomTable.Columns.Add(new DataColumn("StereoParity", typeof(System.Int32)));
            atomTable.Columns.Add(new DataColumn("HydrogenCount", typeof(System.Int32)));
            atomTable.Columns.Add(new DataColumn("StereoBoxCare", typeof(System.Int32)));
            atomTable.Columns.Add(new DataColumn("Valence", typeof(System.Int32)));
            atomTable.Columns.Add(new DataColumn("H0", typeof(System.Int32)));
            atomTable.Columns.Add(new DataColumn("rNotUsed", typeof(System.String)));
            atomTable.Columns.Add(new DataColumn("iNotUsed", typeof(System.String)));
            atomTable.Columns.Add(new DataColumn("AtomMapping", typeof(System.Int32)));
            atomTable.Columns.Add(new DataColumn("InversionRetention", typeof(System.Int32)));
            atomTable.Columns.Add(new DataColumn("ExactChange", typeof(System.Int32)));
            bondTable = new System.Data.DataTable();
            bondTable.Columns.Add(new DataColumn("firstAtom", typeof(System.Int32)));
            bondTable.Columns.Add(new DataColumn("secondAtom", typeof(System.Int32)));
            bondTable.Columns.Add(new DataColumn("bondType", typeof(System.Int32)));
            bondTable.Columns.Add(new DataColumn("bondStereo", typeof(System.Int32)));
            bondTable.Columns.Add(new DataColumn("xNotUsed", typeof(System.String)));
            bondTable.Columns.Add(new DataColumn("bondTopology", typeof(System.Int32)));
            bondTable.Columns.Add(new DataColumn("reactingCenter", typeof(System.Int32)));
        }

        void ImportDescriptors(string fileName)
        {
            descriptors.Clear();
            nonZeroDescriptors.Clear();
            nonZeroFragments.Clear();
            atomTable.Clear();
            bondTable.Clear();
            descriptor pairVal;
            string[] fragments = Properties.Resources.fragments.Replace("\r", String.Empty).Split('\n');
            using (var reader = new System.IO.StreamReader(fileName))
            {
                string line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] pair = line.Split('|');
                    pairVal.name = pair[0];
                    pairVal.value = Convert.ToDouble(pair[1]);
                    descriptors.Add(pairVal);
                    if (pairVal.value != 0.0)
                    {
                        nonZeroDescriptors.Add(pairVal);
                        foreach (string fragment in fragments)
                        {
                            if (pairVal.name == fragment)
                            {
                                nonZeroFragments.Add(pairVal);
                                break;
                            }
                        }
                    }
                }
            }
        }

        void ImportAssignedFragments(string fileName)
        {
            assignedFragments.Clear();
            using (var reader = new System.IO.StreamReader(fileName))
            {
                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.Load(reader);
                HtmlAgilityPack.HtmlNode node = document.DocumentNode;
                HtmlAgilityPack.HtmlNodeCollection tableRows = node.SelectNodes("/html[1]/table[1]/tr[1]/td[1]/table[1]/tr");
                for (int i = 1; i < tableRows.Count; i++)
                {
                    string text = tableRows[i].InnerHtml;
                    string text2 = tableRows[i].InnerText;
                    HtmlAgilityPack.HtmlNodeCollection nodes = tableRows[i].ChildNodes;
                    Fragment f = new Descriptors.Fragment();
                    f.atomNumber = Convert.ToInt32(nodes[1].InnerText);
                    f.element = nodes[3].InnerText;
                    f.fragment = nodes[5].InnerText;
                    assignedFragments.Add(f);
                }
            }
        }

        void ImportMolFile(string filename)
        {
            atomTable.Clear();
            bondTable.Clear();
            int numAtoms = 0;
            int numBonds = 0;

            // This information is contained in the header (first 3 lines) of the mol file. It is currently not being used, but code has been created for
            // future use.
            string name = string.Empty;
            string program = string.Empty;
            string user = string.Empty;
            //// used to test Line 2 reading below.
            //string MM = string.Empty;
            //string DD = string.Empty;
            //string YY = string.Empty;
            //string HH = string.Empty;
            //string mm = string.Empty;
            int MM = 1;
            int DD = 1;
            int YY = 0;
            int HH = 0;
            int mm = 0;
            string dimensionalCodes = string.Empty;
            //// used to test Line 2 reading below.
            //string scalingFactor1 = string.Empty;
            int scalingFactor1 = 0;
            //// used to test Line 2 reading below.
            //string scalingFactor2 = string.Empty;
            double scalingFactor2 = 0;
            // used to test Line 2 reading below.
            string energy = string.Empty;
            string registryNumber = string.Empty;
            string comments = string.Empty;

            using (var reader = new System.IO.StreamReader(filename))
            {
                string s = reader.ReadToEnd();
                string[] lines = s.Split('\n');
                // Line 1 contains the compound name. It can not be longer than 80 characters, and is allowed to be empty.
                // The length of the line is not relevant in how this is read, so it is not checked.
                name = lines[0];
                //// used to test Line 2 reading below.
                //lines[1] = "IIPPPPPPPPMMDDYYHHmmddSSssssssssssEEEEEEEEEEEERRRRRR";
                // Line 2 is optional. Skip this if it is not there.
                if (lines[1] != string.Empty)
                {
                    // Line format: IIPPPPPPPPMMDDYYHHmmddSSssssssssssEEEEEEEEEEEERRRRRR
                    //              A2<--A8--><---A10-->A2I2<--F10.5-><---F12.5--><-I6->
                    //User's first and last initials (l), program name (P), date/time (M/D/Y,H:m),
                    //dimensional codes (d), scaling factors (S, s), energy (E) if modeling program input,
                    //internal registry number (R) if input through MDL form.
                    if (lines[1].Length > 2) user = lines[1].Substring(0, 2); // II
                    if (lines[1].Length > 10) program = lines[1].Substring(2, 8); // PPPPPPPP
                    if (lines[1].Length > 20)
                    {
                        //// used to test Line 2 reading below.
                        //MM = lines[1].Substring(10, 2); // MMDDYYHHmm
                        //DD = lines[1].Substring(12, 2); // MMDDYYHHmm
                        //YY = lines[1].Substring(14, 2); // MMDDYYHHmm
                        //HH = lines[1].Substring(16, 2); // MMDDYYHHmm
                        //mm = lines[1].Substring(18, 2); // MMDDYYHHmm
                        MM = Convert.ToInt32(lines[1].Substring(10, 2)); // MMDDYYHHmm
                        DD = Convert.ToInt32(lines[1].Substring(12, 2)); // MMDDYYHHmm
                        YY = Convert.ToInt32(lines[1].Substring(14, 2)); // MMDDYYHHmm
                        HH = Convert.ToInt32(lines[1].Substring(16, 2)); // MMDDYYHHmm
                        mm = Convert.ToInt32(lines[1].Substring(18, 2)); // MMDDYYHHmm
                    }
                    if (lines[1].Length > 22) dimensionalCodes = lines[1].Substring(20, 2); // dd
                    //// used to test Line 2 reading below.
                    // if (lines[1].Length > 24) scalingFactor1 = lines[1].Substring(22, 2); //SS
                    if (lines[1].Length > 24) scalingFactor1 = Convert.ToInt32(lines[1].Substring(22, 2)); //SS
                    //// used to test Line 2 reading below.
                    // if (lines[1].Length > 34) scalingFactor2 = lines[1].Substring(24, 10); //ss
                    if (lines[1].Length > 34) scalingFactor2 = Convert.ToDouble(lines[1].Substring(24, 10)); //ss
                    if (lines[1].Length > 46) energy = lines[1].Substring(34, 12); //EEEEEEEEEEEE
                    if (lines[1].Length == 52) registryNumber = lines[1].Substring(46, 6); //RRRRRR
                }
                comments = lines[2];

                // Counts Line
                // aaabbblllfffcccsssxxxrrrpppiiimmmvvvvvv
                numAtoms = Convert.ToInt32(lines[3].Substring(0, 3));
                numBonds = Convert.ToInt32(lines[3].Substring(3, 3));
                int atomLists = Convert.ToInt32(lines[3].Substring(6, 3));
                //int fObsolete = Convert.ToInt32(lines[3].Substring(9, 3));
                bool chiral = false;
                if (Convert.ToInt32(lines[3].Substring(12, 3)) == 1) chiral = true;
                int sText = Convert.ToInt32(lines[3].Substring(15, 3));
                //int xObsolete = Convert.ToInt32(lines[3].Substring(18, 3));
                //int rObsolete = Convert.ToInt32(lines[3].Substring(21, 3));
                //int pObsolete = Convert.ToInt32(lines[3].Substring(24, 3));
                //int iObsolete = Convert.ToInt32(lines[3].Substring(27, 3));
                int properties = Convert.ToInt32(lines[3].Substring(30, 3));
                string version = lines[3].Substring(33, 6);
                molecule = new Molecule();
                for (int i = 0; i < numAtoms; i++)
                {
                    Atom a = new Atom();
                    // xxxxx.xxxxyyyyy.yyyyzzzzz.zzzz aaaddcccssshhhbbbvvvHHHrrriiimmmnnneee
                    a.x = Convert.ToDouble(lines[4 + i].Substring(0, 10));
                    a.y = Convert.ToDouble(lines[4 + i].Substring(10, 10));
                    a.z = Convert.ToDouble(lines[4 + i].Substring(20, 10));
                    a.element = lines[4 + i].Substring(31, 3).Replace(" ", string.Empty);
                    string text = lines[4 + i].Substring(34, 2);
                    a.massDiff = Convert.ToInt32(lines[4 + i].Substring(34, 2));
                    a.charge = Convert.ToInt32(lines[4 + i].Substring(36, 3));
                    a.stereoParity = Convert.ToInt32(lines[4 + i].Substring(39, 3));
                    a.hydrogenCount = Convert.ToInt32(lines[4 + i].Substring(42, 3));
                    a.stereoCareBox = Convert.ToInt32(lines[4 + i].Substring(45, 3));
                    a.valence = Convert.ToInt32(lines[4 + i].Substring(48, 3));
                    // string H0 = lines[4 + i].Substring(51, 3);
                    // a.HO = Convert.ToInt32(lines[4 + i].Substring(51, 3));
                    a.rNotUsed = lines[4 + i].Substring(54, 3);
                    a.iNotUsed = lines[4 + i].Substring(57, 3);
                    a.atomMapping = Convert.ToInt32(lines[4 + i].Substring(60, 3));
                    a.inversionRetension = Convert.ToInt32(lines[4 + i].Substring(63, 3));
                    a.exactChange = Convert.ToInt32(lines[4 + i].Substring(66, 3));
                    molecule.AddAtom(a);
                    DataRow atomRow = atomTable.NewRow();
                    atomTable.Rows.Add(atomRow);
                    atomRow["X"] = a.x;
                    atomRow["Y"] = a.y;
                    atomRow["Z"] = a.z;
                    atomRow["element"] = a.element;
                    atomRow["MassDiff"] = a.massDiff;
                    atomRow["Charge"] = a.charge;
                    atomRow["StereoParity"] = a.stereoParity;
                    atomRow["HydrogenCount"] = a.hydrogenCount;
                    atomRow["StereoBoxCare"] = a.stereoCareBox;
                    atomRow["Valence"] = a.valence;
                    //atomRow["H0"] = a.HO;
                    atomRow["rNotUsed"] = a.rNotUsed;
                    atomRow["iNotUsed"] = a.iNotUsed;
                    atomRow["AtomMapping"] = a.atomMapping;
                    atomRow["InversionRetention"] = a.inversionRetension;
                    atomRow["ExactChange"] = a.exactChange;

                }
                for (int i = 0; i < numBonds; i++)
                {
                    Bond b = new Bond();
                    // 111222tttsssxxxrrrccc
                    string line = lines[4 + numAtoms + i];
                    b.firstAtom = Convert.ToInt32(lines[4 + numAtoms + i].Substring(0, 3));
                    b.secondAtom = Convert.ToInt32(lines[4 + numAtoms + i].Substring(3, 3));
                    b.bondType = (BondType)Convert.ToInt32(lines[4 + numAtoms + i].Substring(6, 3));
                    b.bondStereo = (BondStereo)Convert.ToInt32(lines[4 + numAtoms + i].Substring(9, 3));
                    b.xNotUsed = lines[4 + numAtoms + i].Substring(12, 3);
                    b.bondTopology = (BondTopology)Convert.ToInt32(lines[4 + numAtoms + i].Substring(15, 3));
                    int reactingCenter = Convert.ToInt32(lines[4 + numAtoms + i].Substring(18, 3));
                    if (reactingCenter == 13) b.reactingCenter = BondReactingCenterStatus.bondMadeOrBroken | BondReactingCenterStatus.bondOrderChanges | BondReactingCenterStatus.aCenter;
                    else if (reactingCenter == 12) b.reactingCenter = BondReactingCenterStatus.bondMadeOrBroken | BondReactingCenterStatus.bondOrderChanges;
                    else if (reactingCenter == 9) b.reactingCenter = BondReactingCenterStatus.bondOrderChanges | BondReactingCenterStatus.aCenter;
                    else if (reactingCenter == 5) b.reactingCenter = BondReactingCenterStatus.bondMadeOrBroken | BondReactingCenterStatus.aCenter;
                    else b.reactingCenter = (BondReactingCenterStatus)reactingCenter;
                    DataRow bondRow = bondTable.NewRow();
                    bondRow["firstAtom"] = b.firstAtom;
                    bondRow["secondAtom"] = b.secondAtom;
                    bondRow["bondType"] = b.bondType;
                    bondRow["bondStereo"] = b.bondStereo;
                    bondRow["xNotUsed"] = b.xNotUsed;
                    bondRow["bondTopology"] = b.bondTopology;
                    bondRow["reactingCenter"] = b.reactingCenter;
                    molecule.AddBond(b.firstAtom-1, b.secondAtom-1);
                }
            }
        }

        void parseSmiles(string smile)
        {
            //string grammarString = Properties.Resources.smilesGrammar;
            //string test = grammarString.Substring(0, 844);
            //var grammar = new Eto.Parse.Grammars.EbnfGrammar(Eto.Parse.Grammars.EbnfStyle.W3c);
            //grammar.Initialize(new Eto.Parse.ParserInitializeArgs(grammar));
            //Eto.Parse.Parsers.StringParser parser = new Eto.Parse.Parsers.StringParser();
            //parser.
            //grammar.Build(grammarString, "grammar");
            //grammar.Match(smile);

        }
    }
}
