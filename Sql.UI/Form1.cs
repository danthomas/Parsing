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
using static System.String;

namespace Sql.UI
{
    public partial class Form1 : Form
    {
        private readonly Parser _parser;

        public Form1()
        {
            InitializeComponent();
            _parser = new Parser();

            if (File.Exists(@"c:\temp\sql.sql"))
            {
                input.Text = File.ReadAllText(@"c:\temp\sql.sql");
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nodes.Text = "";
            output.Text = "";

            try
            {
                Node<NodeType> node = _parser.Parse(input.Text);
                WriteNode(node);
                WriteSql(node);
            }
            catch (Exception exception)
            {
                nodes.Text = exception.Message;
                throw;
            }
        }

        private void WriteSql(Node<NodeType> node)
        {
            string sql = "select      ";

            foreach (Node<NodeType> selectField in node.Children.First(x => x.NodeType == NodeType.SelectFields).Children)
            {
                if (selectField.NodeType == NodeType.Star)
                {
                    sql += "*";
                }
            }

            Node<NodeType> table = node.Children.First(x => x.NodeType == NodeType.Table);

            sql += $@"
from        {TableAndAlias(table)}";

            foreach (Node<NodeType> selectField in node.Children.Where(x => x.NodeType == NodeType.Join))
            {
                sql += $@"
{JoinAndAlias(selectField)}";
            }

            output.Text = sql;
        }

        private string TableAndAlias(Node<NodeType> table)
        {
            return Join(" ", table.Children.Where(x => x.NodeType == NodeType.Text).Select(x => x.Text));
        }

        private string JoinAndAlias(Node<NodeType> join)
        {
            string ret = "";

            foreach(Node<NodeType> node in join.Children)
            {
                if (node.NodeType == NodeType.Join)
                {
                    ret += "join";
                }
            }

            Node<NodeType> table = join.Children.First(x => x.NodeType == NodeType.Table);

            ret = ret.PadRight(12);

            ret += TableAndAlias(table);

            return ret;
        }

        private void WriteNode(Node<NodeType> node)
        {
            WriteNode(node, 0);
            nodes.Text += Environment.NewLine + "-----------------";
        }

        private void WriteNode(Node<NodeType> node, int indent)
        {
            foreach (Node<NodeType> child in node.Children)
            {
                nodes.Text += Environment.NewLine +
                    new string(' ', indent * 4) +
                    child.NodeType +
                    (child.Text == "" ? "" : ":" + child.Text);

                WriteNode(child, indent + 1);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.WriteAllText(@"c:\temp\sql.sql", input.Text);
        }
    }
}
