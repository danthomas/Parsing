using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Parsing.Core;
using Parsing.Core.Domain;
using Parsing.Core.GrammarDef;
using NodeType = Parsing.Core.GrammarGrammar.NodeType;
using Parser = Parsing.Core.GrammarGrammar.Parser;

namespace Parsing.Designer
{
    public partial class Form1 : Form
    {
        private object _parser;

        private MethodInfo _parseMethod;
        private object _walker;
        private MethodInfo _nodesToStringMethod;

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
            genGrammar.Text = "";
            genLexer.Text = "";
            genParser.Text = "";

            Parser parser = new Parser();
            Generator generator = new Generator();
            Builder builder = new Builder();

            Node<NodeType> node = null;
            try
            {
                node = parser.Parse(grammarText.Text);

                Grammar grammar = generator.BuildGrammar(node);
                
                genLexer.Text = generator.GenerateLexer(grammar);
                genParser.Text = generator.GenerateParser(grammar);
                
                var assembly = builder.Build(genLexer.Text, genParser.Text);
                
                var parserType = assembly.GetType("Xxx.Parser");
                _parser = Activator.CreateInstance(parserType);
                _parseMethod = parserType.GetMethod("Parse");
                
                var walkerType = assembly.GetType("Xxx.Walker");
                _walker = Activator.CreateInstance(walkerType);
                _nodesToStringMethod = walkerType.GetMethod("NodesToString");

            }
            catch (Exception exception)
            {
                _parser = null;
                _parseMethod = null;

                MessageBox.Show(exception.Message);
            }

            RefreshNodes(node);
        }

        private void Parse()
        {
            try
            {
                var node = _parseMethod.Invoke(_parser, new object[] { input.Text });
                output.Text = _nodesToStringMethod.Invoke(_walker, new object[] { node }).ToString();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
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

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Parse();
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
