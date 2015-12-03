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
using V2.Parsing.Core.Domain;
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
            _filePath = @"c:\temp\parsing\v2.parsing.core\grammardef\def.grm";
            RefreshGrammarDef();
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
            grammarDef.Text = File.ReadAllText(_filePath);
        }

        private void parseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parser = new Parser();

            Node<NodeType> node = parser.Parse(grammarDef.Text);

            nodes.Text = _utils.NodeToString(node);
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var builder = new Builder();
            var parser = new Parser();

            Node<NodeType> root = parser.Parse(grammarDef.Text);

            builder.PreProcess(root);

            nodes.Text = _utils.NodeToString(root);


            var grammar = builder.BuildGrammar(root);

            RefreshGrammarTree(grammar);

            lexerDef.Text = builder.BuildLexer(grammar);
        }

        private void RefreshGrammarTree(Grammar grammar)
        {
            grammarTree.Nodes.Clear();

            TreeNode treeNode = new TreeNode(grammar.Name);

            grammarTree.Nodes.Add(treeNode);

            AddDefNodes(grammar, treeNode);

            grammarTree.ExpandAll();
        }

        private void AddDefNodes(Grammar grammar, TreeNode parentNode)
        {
            foreach(Def def in grammar.Defs)
            {
                TreeNode defNode = new TreeNode(def.Name);
                parentNode.Nodes.Add(defNode);

                AddElements(def.Elements, defNode);
            }
        }

        private void AddElements(List<Element> elements, TreeNode parentNode)
        {
            foreach (Element element in elements)
            {

                if (element is Identifier)
                {
                    TreeNode identifierNode = new TreeNode(((Identifier)element).Name);
                    parentNode.Nodes.Add(identifierNode);
                }
                else if (element is OneOf)
                {
                    OneOf oneOf = element as OneOf;
                    
                    TreeNode elementNode = new TreeNode(element.GetType().Name);
                    parentNode.Nodes.Add(elementNode);
                    AddElements(oneOf.Identifiers.Cast<Element>().ToList(), elementNode);
                }
                else if (element is AllOf)
                {
                    AllOf oneOf = element as AllOf;
                    
                    TreeNode elementNode = new TreeNode("*" + element.GetType().Name);
                    parentNode.Nodes.Add(elementNode);
                    AddElements(oneOf.Identifiers.Cast<Element>().ToList(), elementNode);
                }
                else if (element is Optional)
                {
                    Optional optional = element as Optional;
                    
                    TreeNode elementNode = new TreeNode("*" + element.GetType().Name);
                    parentNode.Nodes.Add(elementNode);
                    AddElements(new List<Element> { optional.Element }, elementNode);
                }
                else if (element is OneOrMore)
                {
                    OneOrMore oneOrMore = element as OneOrMore;
                    
                    TreeNode elementNode = new TreeNode("*" + element.GetType().Name);
                    parentNode.Nodes.Add(elementNode);
                    AddElements(new List<Element> { oneOrMore.Element }, elementNode);
                }
                else if (element is ZeroOrMore)
                {
                    ZeroOrMore zeroOrMore = element as ZeroOrMore;
                    
                    TreeNode elementNode = new TreeNode("*" + element.GetType().Name);
                    parentNode.Nodes.Add(elementNode);
                    AddElements(new List<Element> { zeroOrMore.Element }, elementNode);
                }
            }
        }
    }

    class Settings
    {
        public string DirPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Parsing");
    }
}
