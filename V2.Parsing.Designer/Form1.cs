using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using V2.Parsing.Core;
using V2.Parsing.Core.Domain;
using V2.Parsing.Core.GrammarDef;

namespace V2.Parsing.Designer
{
    public partial class Form1 : Form
    {
        private readonly Utils _utils;
        private readonly Settings _settings;
        private Builder _builder;
        private Parser _parser;
        private object _parser2;

        public Form1()
        {
            _utils = new Utils();
            _builder = new Builder();
            _parser = new Parser();
            _settings = new Settings();
            _settings.Load();

            InitializeComponent();

            LoadGrammerDefFile();
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
                _settings.GrammarDefFilePath = openFileDialog.FileName;

                LoadGrammerDefFile();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(_settings.GrammarDefFilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Grammar Def Files|*.grm"
                };

                var result = saveFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    _settings.GrammarDefFilePath = saveFileDialog.FileName;
                }
            }

            if (!String.IsNullOrWhiteSpace(_settings.GrammarDefFilePath))
            {
                File.WriteAllText(_settings.GrammarDefFilePath, grammarDef.Text);
            }
        }

        private void LoadGrammerDefFile()
        {
            if (File.Exists(_settings.GrammarDefFilePath))
            {
                grammarDef.Text = File.ReadAllText(_settings.GrammarDefFilePath);
            }
        }

        private void parseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parser = new Parser();

            Node<NodeType> node = parser.Parse(grammarDef.Text);

            nodes.Text = _utils.NodeToString(node);
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Node<NodeType> root = _parser.Parse(grammarDef.Text);

            _builder.PreProcess(root);

            nodes.Text = _utils.NodeToString(root);

            var grammar = _builder.BuildGrammar(root);

            RefreshGrammarTree(grammar);

            lexerDef.Text = _builder.BuildLexer(grammar);
            parserDef.Text = _builder.BuildParser2(grammar);
            //tokenTypeDef.Text = _builder.b
        }


        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parser = new Parser();

            Node<NodeType> root = parser.Parse(grammarDef.Text);

            _builder.PreProcess(root);

            Grammar grammar = _builder.BuildGrammar(root);

            var actual = _utils.GrammarToDefString(grammar);
            
            _parser2 = _builder.CreateParser(grammar);
            //
            //
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var root2 = _parser2.GetType().GetMethod("Parse").Invoke(_parser2, new object[] { text });
        }

        private void RefreshGrammarTree(Grammar grammar)
        {
            try
            {

                grammarTree.Nodes.Clear();

                TreeNode treeNode = new TreeNode(grammar.Name);

                grammarTree.Nodes.Add(treeNode);

                AddDefNodes(grammar, treeNode);

                grammarTree.ExpandAll();
            }
            catch (Exception)
            {

            }
        }

        private void AddDefNodes(Grammar grammar, TreeNode parentNode)
        {
            foreach (Def def in grammar.Defs)
            {
                TreeNode defNode = new TreeNode(def.Name);
                parentNode.Nodes.Add(defNode);

                AddElements(grammar, def.Elements, defNode);
            }
        }

        private void AddElements(Grammar grammar, List<Element> elements, TreeNode parentNode)
        {
            foreach (Element element in elements)
            {
                var patternIdentifier = element as PatternIdentifier;
                var defIdentifier = element as DefIdentifier;
                var oneOf = element as OneOf;
                var allOf = element as AllOf;
                var optional = element as Optional;
                var oneOrMore = element as OneOrMore;
                var zeroOrMore = element as ZeroOrMore;

                TreeNode childNode;

                if (patternIdentifier != null)
                {
                    childNode = new PatternIdentifierTreeNode(patternIdentifier);
                }
                else if (defIdentifier != null)
                {
                    childNode = new DefIdentifierTreeNode(defIdentifier);

                    AddElements(grammar, grammar.Defs.Single(x => x.Name == (element).Name).Elements, childNode);
                }
                else if (oneOf != null)
                {
                    childNode = new OneOfTreeNode(oneOf);
                    AddElements(grammar, oneOf.Elements.ToList(), childNode);
                }
                else if (allOf != null)
                {
                    childNode = new AllOfTreeNode(allOf);
                    AddElements(grammar, allOf.Elements.ToList(), childNode);
                }
                else if (optional != null)
                {
                    childNode = new OptionalTreeNode(optional);
                    AddElements(grammar, new List<Element> { optional.Element }, childNode);
                }
                else if (oneOrMore != null)
                {
                    childNode = new OneOrMoreTreeNode(oneOrMore);
                    AddElements(grammar, new List<Element> { oneOrMore.Element }, childNode);
                }
                else if (zeroOrMore != null)
                {
                    childNode = new ZeroOrMoreTreeNode(zeroOrMore);
                    AddElements(grammar, new List<Element> { zeroOrMore.Element }, childNode);
                }
                else
                {
                    throw new Exception();
                }

                parentNode.Nodes.Add(childNode);
            }
        }

        private void grammarTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tokenTypePaths.Lines = GetTokenPaths(e.Node).ToArray();
        }


        private List<string> GetTokenPaths(TreeNode node)
        {
            Stack<string> stack = new Stack<string>();

            return GetTokenPaths(node, stack);
        }

        private List<string> GetTokenPaths(TreeNode node, Stack<string> stack)
        {
            foreach (TreeNode childNode in node.Nodes)
            {
                PatternIdentifierTreeNode patternIdentifierTreeNode = childNode as PatternIdentifierTreeNode;
                DefIdentifierTreeNode defIdentifierTreeNode = childNode as DefIdentifierTreeNode;
                OneOfTreeNode oneOfTreeNode = childNode as OneOfTreeNode;
                AllOfTreeNode allOfTreeNode = childNode as AllOfTreeNode;
                OptionalTreeNode optionalTreeNode = childNode as OptionalTreeNode;
                OneOrMoreTreeNode oneOrMoreTreeNode = childNode as OneOrMoreTreeNode;
                ZeroOrMoreTreeNode zeroOrMoreTreeNode = childNode as ZeroOrMoreTreeNode;

                if (patternIdentifierTreeNode != null)
                {

                }
                else if (defIdentifierTreeNode != null)
                {

                }
                else if (oneOfTreeNode != null)
                {

                }
                else if (allOfTreeNode != null)
                {

                }
                else if (optionalTreeNode != null)
                {

                }
                else if (oneOrMoreTreeNode != null)
                {

                }
                else if (zeroOrMoreTreeNode != null)
                {

                }
            }

            return new List<string>() { "sdffsa", "sfsf" };
        }
    }

    class PatternIdentifierTreeNode : TreeNode
    {
        public PatternIdentifier PatternIdentifier { get; set; }

        public PatternIdentifierTreeNode(PatternIdentifier patternIdentifier)
        {
            PatternIdentifier = patternIdentifier;
            Text = patternIdentifier.Name;
            ForeColor = Color.Blue;
        }
    }

    class DefIdentifierTreeNode : TreeNode
    {
        public DefIdentifier DefIdentifier { get; set; }

        public DefIdentifierTreeNode(DefIdentifier defIdentifier)
        {
            DefIdentifier = defIdentifier;
            Text = defIdentifier.Name;
            ForeColor = Color.Red;
        }
    }

    class OneOfTreeNode : TreeNode
    {
        public OneOf OneOf { get; set; }

        public OneOfTreeNode(OneOf oneOf)
        {
            OneOf = oneOf;
            Text = "One Of";
        }
    }

    class AllOfTreeNode : TreeNode
    {
        public AllOf AllOf { get; set; }

        public AllOfTreeNode(AllOf allOf)
        {
            AllOf = allOf;
            Text = "All Of";
        }
    }

    class OptionalTreeNode : TreeNode
    {
        public Optional Optional { get; set; }

        public OptionalTreeNode(Optional optional)
        {
            Optional = optional;
            Text = "Optional";
        }
    }

    class OneOrMoreTreeNode : TreeNode
    {
        public OneOrMore OneOrMore { get; set; }

        public OneOrMoreTreeNode(OneOrMore oneOrMore)
        {
            OneOrMore = oneOrMore;
            Text = "One or More";
        }
    }

    class ZeroOrMoreTreeNode : TreeNode
    {
        public ZeroOrMore ZeroOrMore { get; set; }

        public ZeroOrMoreTreeNode(ZeroOrMore zeroOrMore)
        {
            ZeroOrMore = zeroOrMore;
            Text = "Zero or More";
        }
    }
}
