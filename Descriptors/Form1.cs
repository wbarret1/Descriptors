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
    public struct atom
    {
        public double x;
        public double y;
        public double z;
        public string element;
        public int massDiff;
        public int charge;
        public int stereoParity;
        public int hydrogenCount;
        public int stereoCareBox;
        public int valence;
        public int HO;
        public string rNotUsed;
        public string iNotUsed;
        public int atomMapping;
        public int inversionRetension;
        public int exactChange;
    }

    public struct Bond
    {
        public int firstAtom;
        public int secondAtom;
        public int bondType;
        public int bondStereo;
        public string xNotUsed;
        public int bondTopology;
        public int reactingCenter;
    }

    public partial class Form1 : Form
    {
        string[] directories;
        List<descriptor> descriptors;
        List<descriptor> nonZeroDescriptors;
        List<descriptor> nonZeroFragments;
        List<Fragment> assignedFragments;
        List<atom> atoms;
        List<Bond> bonds;
        System.Data.DataTable atomTable;
        System.Data.DataTable bondTable;

        public Form1()
        {
            InitializeComponent();
            this.listView1.Columns.Add("Name", -2, HorizontalAlignment.Left);
            directories = System.IO.Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ToxRuns", "StructureData", System.IO.SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(directory + "\\structure.png");
                imageList1.Images.Add(image);
                System.IO.DirectoryInfo info = System.IO.Directory.GetParent(directory);
                ListViewItem item = new ListViewItem(info.Name) { ImageIndex = 0 };
                this.listView1.Items.Add(item);
            }
            descriptors = new List<descriptor>();
            nonZeroDescriptors = new List<descriptor>();
            nonZeroFragments = new List<descriptor>();
            assignedFragments = new List<Fragment>();
            this.CreateDataTables();

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string run = this.listView1.SelectedItems[0].Text;
            string fileName = directories[0] + "\\2d_descriptors.txt";
            descriptors.Clear();
            nonZeroDescriptors.Clear();
            nonZeroFragments.Clear();
            assignedFragments.Clear();
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
            Form2 form = new Descriptors.Form2(nonZeroFragments);
            //form.Show();

            bool phosphate = false;
            double phosphateCount = 0;
            bool oxPhos = false;
            double oxPhosCount = 0;
            foreach (descriptor d in nonZeroDescriptors)
            {
                if (d.name == "P(=O)")
                {
                    phosphate = true;
                    phosphateCount = phosphateCount + d.value;
                }
            }
            if (phosphate)
            {
                foreach (descriptor d in nonZeroDescriptors)
                {
                    if (d.name == "-O- [phosphorus attach]")
                    {
                        oxPhos = true;
                        oxPhosCount = oxPhosCount + d.value;
                    }
                }
            }

            fileName = directories[0] + "\\AssignedFragments.html";
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

            int numAtoms = 0;
            int numBonds = 0;
            string[] fileNames = System.IO.Directory.GetFiles(directories[0], "*.mol");
            using (var reader = new System.IO.StreamReader(fileNames[0]))
            {
                string[] lines = reader.ReadToEnd().Split('\n');
                string name = lines[0];
                if (lines[1] != string.Empty)
                {
                    // Line format: IIPPPPPPPPMMDDYYHHmmddSSssssssssssEEEEEEEEEEEERRRRRR
                    //              A2<--A8--><---A10-->A2I2<--F10.5-><---F12.5--><-I6->
                    //User's first and last initials (l), program name (P), date/time (M/D/Y,H:m),
                    //dimensional codes (d), scaling factors (S, s), energy (E) if modeling program input,
                    //internal registry number (R) if input through MDL form.
                    string user = lines[1].Substring(0, 2); // II
                    string program = lines[1].Substring(2, 8); // PPPPPPPP
                    string date = lines[1].Substring(10, 10); // MMDDYYHHmm
                    string dimensionalCodes = lines[1].Substring(20, 2); // dd
                    string scalingFactor1 = lines[1].Substring(22, 2); //SS
                    string scalingFactor2 = lines[1].Substring(24, 10); //ss
                    string energy = lines[1].Substring(34, 12); //EEEEEEEEEEEE
                    string registryNumber = lines[1].Substring(46, 6); //RRRRRR

                }
                string comments = lines[2];

                // Counts Line
                // aaabbblllfffcccsssxxxrrrpppiiimmmvvvvvv
                numAtoms = Convert.ToInt32(lines[3].Substring(0, 3));
                numBonds = Convert.ToInt32(lines[3].Substring(3, 3));
                int atomLists = Convert.ToInt32(lines[3].Substring(6, 3));
                int fObsolete = Convert.ToInt32(lines[3].Substring(9, 3));
                bool chiral = false;
                if (Convert.ToInt32(lines[3].Substring(12, 3)) == 1) chiral = true;
                int sText = Convert.ToInt32(lines[3].Substring(15, 3));
                int xObsolete = Convert.ToInt32(lines[3].Substring(18, 3));
                int rObsolete = Convert.ToInt32(lines[3].Substring(21, 3));
                int pObsolete = Convert.ToInt32(lines[3].Substring(24, 3));
                int iObsolete = Convert.ToInt32(lines[3].Substring(27, 3));
                int properties = Convert.ToInt32(lines[3].Substring(30, 3));
                string version = lines[3].Substring(33, 6);
                atoms = new List<atom>();
                for (int i = 0; i < numAtoms; i++)
                {
                    atom a = new atom();
                    // xxxxx.xxxxyyyyy.yyyyzzzzz.zzzz aaaddcccssshhhbbbvvvHHHrrriiimmmnnneee
                    a.x = Convert.ToDouble(lines[4 + i].Substring(0, 10));
                    a.y = Convert.ToDouble(lines[4 + i].Substring(10, 10));
                    a.z = Convert.ToDouble(lines[4 + i].Substring(20, 10));
                    a.element = lines[4 + i].Substring(31, 3);
                    string text = lines[4 + i].Substring(34, 2);
                    a.massDiff = Convert.ToInt32(lines[4 + i].Substring(34, 2));
                    a.charge = Convert.ToInt32(lines[4 + i].Substring(36, 3));
                    a.stereoParity = Convert.ToInt32(lines[4 + i].Substring(39, 3));
                    a.hydrogenCount = Convert.ToInt32(lines[4 + i].Substring(42, 3));
                    a.stereoCareBox = Convert.ToInt32(lines[4 + i].Substring(45, 3));
                    a.valence = Convert.ToInt32(lines[4 + i].Substring(48, 3));
                    a.HO = Convert.ToInt32(lines[4 + i].Substring(51, 3));
                    a.rNotUsed = lines[4 + i].Substring(54, 3);
                    a.iNotUsed = lines[4 + i].Substring(57, 3);
                    a.atomMapping = Convert.ToInt32(lines[4 + i].Substring(60, 3));
                    a.inversionRetension = Convert.ToInt32(lines[4 + i].Substring(63, 3));
                    a.exactChange = Convert.ToInt32(lines[4 + i].Substring(66, 3));
                    atoms.Add(a);
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
                    atomRow["H0"] = a.HO;
                    atomRow["rNotUsed"] = a.rNotUsed;
                    atomRow["iNotUsed"] = a.iNotUsed;
                    atomRow["AtomMapping"] = a.atomMapping;
                    atomRow["InversionRetention"] = a.inversionRetension;
                    atomRow["ExactChange"] = a.exactChange;

                }
                bonds = new List<Bond>();
                for (int i = 0; i < numBonds; i++)
                {
                    Bond b = new Bond();
                    // 111222tttsssxxxrrrccc
                    b.firstAtom = Convert.ToInt32(lines[4 + numAtoms + i].Substring(0, 3));
                    b.secondAtom = Convert.ToInt32(lines[4 + numAtoms + i].Substring(3, 3));
                    b.bondType = Convert.ToInt32(lines[4 + numAtoms + i].Substring(6, 3));
                    b.bondStereo = Convert.ToInt32(lines[4 + numAtoms + i].Substring(9, 3));
                    b.xNotUsed = lines[4 + numAtoms + i].Substring(12, 3);
                    b.bondTopology = Convert.ToInt32(lines[4 + numAtoms + i].Substring(15, 3));
                    b.reactingCenter = Convert.ToInt32(lines[4 + numAtoms + i].Substring(18, 3));
                    bonds.Add(b);
                    DataRow bondRow = bondTable.NewRow();
                    bondRow["firstAtom"] = b.firstAtom;
                    bondRow["secondAtom"] = b.secondAtom;
                    bondRow["bondType"] = b.bondType;
                    bondRow["bondStereo"] = b.bondStereo;
                    bondRow["xNotUsed"] = b.xNotUsed;
                    bondRow["bondTopology"] = b.bondTopology;
                    bondRow["reactingCenter"] = b.reactingCenter;

                }
            }

            int[,] connectivity = new int[numAtoms, numAtoms];
            foreach(Bond b in bonds)
            {
                connectivity[b.firstAtom - 1, b.secondAtom - 1] = b.bondType;
                connectivity[b.secondAtom - 1, b.firstAtom - 1] = b.bondType;
            }
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
    }
}
