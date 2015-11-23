using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2.Parsing.Core;
using V2.Parsing.Core.GrammarDef;

namespace V2.Parsing.Designer
{
    public partial class Form1 : Form
    {
        private string _filePath;
        private Utils _utils;

        public Form1()
        {
            _utils = new Utils();
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Grammar Def Files|*.grm"
            };

            var result = openFileDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                _filePath = openFileDialog.FileName;

                

                RefreshGrammarDef();
            }
        }

        private void RefreshGrammarDef()
        {
            grammar.Text = File.ReadAllText(_filePath);
        }

        private void parseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parser = new Parser();
            
            Node<NodeType> node = parser.Parse(grammar.Text);
            
            nodes.Text = _utils.NodeToString(node);
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parser = new Parser();

            Node<NodeType> root = parser.Parse(grammar.Text);

            nodes.Text = _utils.NodeToString(root);

            var builder = new Builder();

            var grammar2 = builder.BuildGrammar(root);

            lexer.Text = builder.BuildLexer(grammar2);
        }
    }

    class Settings
    {
        public string DirPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Parsing");
    }
}
