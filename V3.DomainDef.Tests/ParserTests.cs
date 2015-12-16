using System;
using System.Linq;
using NUnit.Framework;
using V3.Parsing.Core;

namespace V3.DomainDef.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Test()
        {
            Parser parser = new Parser();

            var domainDef = @"domain CrossFitJST
entity App.Entity
      Id byte
    , Name string(50)
    , Plural string(52)
entity App.MenuItem
      Id smallint
    , ParentId MenuItem null
    , Url string(50)
    , Text string(30)
    , DisplayIndex smallint
entity App.TaskType enum
      Id byte readonly
    , Name string(50) unique
    , SelectedIds byte
data
    [1, 'Add', 15]
    [2, 'Edit', 2]
    [3, 'Delete', 14]
    [4, 'Activate', 14]
    [5, 'Deactivate', 14]
    [6, 'MoveUp', 2]
    [7, 'MoveDown', 2]
    [255, 'Custome', 14]
entity Account
      Id int
    , Name string(5, 30) unique
    , PasswordHash string(1000) null
    , Forenames string(2, 50)
    , Surname string(2, 30)
    , PreferredName string(2, 30)
    , IsActive bool
entity AccountType
      Id byte
    , Name string(30)";

            var node = parser.Parse(domainDef);

            var nodeString = NodeToString(node);

            var domain = new Domain(node);

            var actual = domain.ToString();

            Assert.That(actual, Is.EqualTo(domainDef));
        }

        private string NodeToString(Node<NodeType> node)
        {
            string ret = "";

            NodeToString(node, ref ret, 0);

            return ret;
        }

        private void NodeToString(Node<NodeType> node, ref string ret, int indent)
        {
            string str = node.Nodes.Any(x => x.Nodes.Count > 0)
                ? Environment.NewLine + new string(' ', indent * 2)
                : " ";

            str = Environment.NewLine + new string(' ', indent * 2);

            ret += str + node.NodeType + (node.Text == "" ? "" : " : " + node.Text);

            foreach (Node<NodeType> child in node.Nodes)
            {
                NodeToString(child, ref ret, indent + 1);
            }
        }
    }

}
