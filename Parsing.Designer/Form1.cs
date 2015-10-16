using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

            Node<NodeType> node;
            try
            {
                node = parser.Parse(grammarText.Text);

                Grammar grammar = generator.BuildGrammar(node);

                RefreshGrammar(grammar);

                genLexer.Text = generator.GenerateLexer(grammar);
                genParser.Text = generator.GenerateParser(grammar);


                Compile();

            }
            catch (Exception exception)
            {
                _parser = null;
                _parseMethod = null;

                MessageBox.Show(exception.Message);
            }
        }

        private void Compile()
        {
            Builder builder = new Builder();

            try
            {
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
                MessageBox.Show(exception.Message);
            }
        }

        private void Parse()
        {
            try
            {
                var node = _parseMethod.Invoke(_parser, new object[] { input.Text });

                var tokens = _parser.GetType().GetProperty("Tokens").GetValue(_parser).ToString();

                output.Text = tokens + Environment.NewLine + "--------------------------------------------------"  + Environment.NewLine + _nodesToStringMethod.Invoke(_walker, new[] { node });
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void RefreshGrammar(Grammar grammar)
        {
            genGrammar.Text = grammar.Name;

            var defs = GetThings(grammar.Root, ThingType.Def);

            foreach(var def in defs)
            {
                RefreshGrammar(def);
            }
        }

        private void RefreshGrammar(Thing parent, int indent = 0)
        {
            genGrammar.Text += Environment.NewLine + new String(' ', indent * 4) + parent.ThingType + " - " + parent.Name + " - " + parent.Text;
            
            if (indent == 0 || parent.ThingType != ThingType.Def)
            {
                foreach (var child in parent.Children)
                {
                    RefreshGrammar(child, indent + 1);
                }
            }
        }

        private List<Thing> GetThings(Thing parent, ThingType thingType)
        {
            List<Thing> things = new List<Thing>();

            Walk(parent, thing =>
            {
                if (thing.ThingType == thingType
                 && !things.Contains(thing))
                {
                    things.Add(thing);
                }
            });

            return things;
        }

        private void Walk(Thing parent, Action<Thing> action)
        {
            action(parent);
            foreach (Thing child in parent.Children)
            {
                Walk(child, action);
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

        private void parseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Compile();
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
