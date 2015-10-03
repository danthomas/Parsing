using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DomainDef.Designer
{
    public partial class TreeTest : Form
    {
        public TreeTest()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TreeTest_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void TreeTest_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Node node = new Node(TokenType.None);

                string prefix = "";

                string[] lines = textBox1.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    node.AddChild((TokenType) Enum.Parse(typeof(TokenType), parts[0]), parts.Length == 1 ? "" : parts[1]);
                }
            }
        }
    }
}
