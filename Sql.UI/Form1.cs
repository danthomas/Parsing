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
        private SqlBuilder _sqlBuilder;

        public Form1()
        {
            InitializeComponent();
            _parser = new Parser();
            _sqlBuilder = new SqlBuilder();

            if (File.Exists(@"c:\temp\sql.sql"))
            {
                input.Text = File.ReadAllText(@"c:\temp\sql.sql");
                Run();
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void Run()
        {
            nodes.Text = "";
            output.Text = "";

            try
            {
                Node<NodeType> node = _parser.Parse(input.Text);
                WriteNode(node);
                output.Text = _sqlBuilder.Build(node);
            }
            catch (Exception exception)
            {
                nodes.Text = exception.Message;
            }
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

    class SqlBuilder
    {
        public string Build(Node<NodeType> node)
        {
            string sql = "select";

            if (node.Children.Any(x => x.NodeType == NodeType.Distinct))
            {
                sql += @" distinct
            ";
            }
            else
            {
                sql += "      ";
            }

            foreach (Node<NodeType> selectField in node.Children.First(x => x.NodeType == NodeType.SelectFields).Children)
            {
                if (selectField.NodeType == NodeType.Star)
                {
                    sql += "*";
                }
                else
                {
                    foreach (Node<NodeType> child in selectField.Children)
                    {
                        if (child.NodeType == NodeType.ObjectRef)
                        {
                            sql += " ";
                            sql = ObjectRef(child, sql);
                        }
                    }
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

            return sql;
        }

        private static string ObjectRef(Node<NodeType> child, string sql)
        {
            foreach (Node<NodeType> xx in child.Children)
            {
                if (xx.NodeType == NodeType.Text)
                {
                    sql += xx.Text;
                }
                else if (xx.NodeType == NodeType.Dot)
                {
                    sql += ".";
                }
            }
            return sql;
        }

        private string TableAndAlias(Node<NodeType> table)
        {
            return Join(" ", table.Children.Where(x => x.NodeType == NodeType.Text).Select(x => x.Text));
        }

        private string JoinAndAlias(Node<NodeType> join)
        {
            string ret = "";

            foreach (Node<NodeType> node in join.Children)
            {
                if (node.NodeType == NodeType.Join)
                {
                    ret += "join";
                }
            }

            Node<NodeType> table = join.Children.First(x => x.NodeType == NodeType.Table);

            ret = ret.PadRight(12);

            ret += TableAndAlias(table);

            ret += " on ";

            return ret;
        }
    }
}
