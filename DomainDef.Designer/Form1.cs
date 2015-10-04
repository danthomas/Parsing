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
using System.Xml;

namespace DomainDef.Designer
{
    public partial class Form1 : Form
    {
        private string _filePath;

        public Form1()
        {
            _filePath = @"C:\Dev\Parsing\DomainDef.Designer\Domain.txt";
            InitializeComponent();
            LoadDomainDef();
            Build();
        }

        private void SaveDomainDef()
        {
            File.WriteAllText(_filePath, domainText.Text);
        }

        private void LoadDomainDef()
        {
            if (File.Exists(_filePath))
            {
                domainText.Text = File.ReadAllText(_filePath);
            }
        }
        private void Build()
        {
            Parser parser = new Parser();

            ParseResult parseResult = parser.Parse(domainText.Text);

            if (parseResult.IsSuccess)
            {
                RefreshDomain(parseResult.Node);

                splitContainer3.Panel2Collapsed = true;
            }
            else
            {
                errors.Text = parseResult.Error;
                splitContainer3.Panel2Collapsed = false;
            }
        }

        private void RefreshDomain(Node node)
        {
            NodeTreeNode nodeTreeNode = domain.Nodes.Cast<NodeTreeNode>().FirstOrDefault();

            if (nodeTreeNode == null)
            {
                nodeTreeNode = new NodeTreeNode(node);
                domain.Nodes.Add(nodeTreeNode);
            }
            else
            {
                nodeTreeNode.Node = node;
            }

            RefreshDomain(nodeTreeNode);

            nodeTreeNode.Expand();
        }

        private void RefreshDomain(NodeTreeNode parent)
        {
            List<Match> matches = parent.Node.Children
                .Select(x => new Match { Node = x })
                .ToList();

            int i = 0;

            foreach (Match match in matches)
            {
                for (int j = i; j < parent.Nodes.Count; ++j)
                {
                    if (match.Node.ToString() == parent.Nodes[i].Text)
                    {
                        match.NodeTreeNode = (NodeTreeNode)parent.Nodes[i];
                        i = j + 1;
                    }
                }
            }

            List<NodeTreeNode> matched = matches.Where(x => x.NodeTreeNode != null).Select(x => x.NodeTreeNode).ToList();

            var notMatched = parent.Nodes.Cast<NodeTreeNode>().Where(x => !matched.Contains(x)).ToList();

            foreach (NodeTreeNode nodeTreeNode in notMatched)
            {
                parent.Nodes.Remove(nodeTreeNode);
            }

            for (int j = 0; j < matches.Count; ++j)
            {
                if (matches[j].NodeTreeNode == null)
                {
                    parent.Nodes.Insert(j, new NodeTreeNode(matches[j].Node));
                }
                else
                {
                    ((NodeTreeNode) parent.Nodes[j]).Node = matches[j].Node;
                }
            }

            foreach (NodeTreeNode child in parent.Nodes)
            {
                RefreshDomain(child);
            }
        }

        class Match
        {
            public Node Node { get; set; }
            public NodeTreeNode NodeTreeNode { get; set; }
        }

        private class NodeTreeNode : TreeNode
        {
            private Node _node;

            public Node Node
            {
                get { return _node; }
                set
                {
                    _node = value;
                    SetText();
                }
            }

            public NodeTreeNode(Node node)
            {
                Node = node;
                SetText();
            }

            private void SetText()
            {
                Text = Node.ToString();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDomainDef();
        }


        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Build();
        }
    }
}
