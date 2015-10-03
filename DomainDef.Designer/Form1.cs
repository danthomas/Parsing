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
            _filePath = @"C:\Temp\Domain.txt";
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
            domain.Nodes.Clear();

            var nodeTreeNode = new NodeTreeNode(node);
            domain.Nodes.Add(nodeTreeNode);

            RefreshDomain(nodeTreeNode);

            nodeTreeNode.Expand();
        }

        private void RefreshDomain(NodeTreeNode parent)
        {
            foreach (Node node in parent.Node.Children.Where(x => x.TokenType != TokenType.Name))
            {
                var child = new NodeTreeNode(node);
                parent.Nodes.Add(child);
                RefreshDomain(child);
            }
        }

        private class NodeTreeNode : TreeNode
        {
            public Node Node { get; set; }

            public NodeTreeNode(Node node)
            {
                Node = node;
                Text = node.TokenType + (node.Text == "" ? "" : " - " + node.Text);
                var nameNode = node.Children.FirstOrDefault(x => x.TokenType == TokenType.Name);
                if (nameNode != null)
                {
                    Text += " - " + nameNode.Text;
                }
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
