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
    public partial class Form2 : Form
    {
        public Form2(List<descriptor> descriptors)
        {
            InitializeComponent();
            this.listView1.Columns.Add("Descriptor", 250);
            this.listView1.Columns.Add("Value");
            foreach (descriptor d in descriptors)
            {
                ListViewItem item = new ListViewItem(d.name);
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, d.value.ToString()));
                this.listView1.Items.Add(item);
            }
        }
    }
}
