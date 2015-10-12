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
using Parsing.Core;
using Parsing.Core.GrammarDef;
using NodeType = Parsing.Core.GrammarGrammar.NodeType;
using Parser = Parsing.Core.GrammarGrammar.Parser;

namespace Parsing.Designer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            if (File.Exists(@"c:\temp\grammar.txt"))
            {
                grammar.Text = File.ReadAllText(@"c:\temp\grammar.txt");
                Build();
            }
        }

        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Build();
        }

        private void Build()
        {
            Parser parser = new Parser();
            Generator generator = new Generator();

            Node<NodeType> node = (Node<NodeType>)null;
            try
            {
                node = parser.Parse(grammar.Text);
                
                genGrammar.Text = generator.GenerateGrammar(node);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            RefreshNodes(node);
        }

        private void RefreshNodes(Node<NodeType> node)
        {
            nodes.Nodes.Clear();

            if (node != null)
            {
                var root = new NodeTreeNode(node);

                nodes.Nodes.Add(root);

                RefreshNodes(root);

                nodes.ExpandAll();
            }
        }

        private void RefreshNodes(NodeTreeNode parentNodeTreeNode)
        {
            foreach (var childNode in parentNodeTreeNode.Node.Children)
            {
                var childNodeTreeNode = new NodeTreeNode(childNode);

                parentNodeTreeNode.Nodes.Add(childNodeTreeNode);

                RefreshNodes(childNodeTreeNode);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.WriteAllText(@"c:\temp\grammar.txt", grammar.Text);
        }
    }

    public class NodeTreeNode : TreeNode
    {
        public Node<NodeType> Node { get; set; }

        public NodeTreeNode(Node<NodeType> node)
        {
            Node = node;
            Text = node.ToString();
        }
    }
}
