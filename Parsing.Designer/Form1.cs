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
                grammarText.Text = File.ReadAllText(@"c:\temp\grammar.txt");
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
            Builder builder = new Builder();

            Node<NodeType> node = null;
            try
            {
                node = parser.Parse(grammarText.Text);

                node = generator.Rejig(node);

                var generateGrammar = generator.GenerateGrammar(node);  

                genGrammar.Text = generateGrammar;

                var assembly = builder.Build(generateGrammar);

                Grammar grammar = (Grammar)Activator.CreateInstance(assembly.GetType("Xxx.SqlGrammar"));

                genLexer.Text = generator.GenerateLexer(grammar);
                genParser.Text = generator.GenerateParser(grammar);

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
            File.WriteAllText(@"c:\temp\grammar.txt", grammarText.Text);
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
