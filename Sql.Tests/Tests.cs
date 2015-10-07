using System;
using System.Linq;
using NUnit.Framework;
using Parsing.Core;
using Parsing.Core.GrammarDef;

namespace Sql.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void SelectStarFrom()
        {
            Run(@"
select * from Account as a", @"
Root
  Select
  SelectFields
    Star
  From
  Table
    Text : Account
    As
    Text : a");
        }

        [Test]
        public void SelectStarFromJoin()
        {
            Run(@"
select * from Account a join AccountType at", @"
Root
  Select
  SelectFields
    Star
  From
  Table
    Text : Account
    Text : a
  Join
    Join
    Table
      Text : AccountType
      Text : at");
        }

        [Test]
        public void SelectFields()
        {
            Run(@"
select Field1, Field2 FieldB, FieldB as FieldC from Account", @"
Root
  Select
  SelectFields
    SelectField
      Text : Field1
    SelectField
      Text : Field2
      Text : FieldB
    SelectField
      Text : FieldB
      As
      Text : FieldC
  From
  Table
    Text : Account");
        }

        [Test]
        public void SelectCountStarMaxMin()
        {
            Run(@"select count(*), min(a.CreateDate), max(CreateDate) from Account a", @"
Root
  Select
  SelectFields
    SelectField
      Count
      OpenParen
      Field
        Star
      CloseParen
    SelectField
      Min
      OpenParen
      Field
        Text : a
        Dot
        Text : CreateDate
      CloseParen
    SelectField
      Max
      OpenParen
      Field
        Text : CreateDate
      CloseParen
  From
  Table
    Text : Account
    Text : a");
        }

        private void Run(string text, string expected)
        {
            Parser parser = new Parser();

            var node = parser.Parse(text);

            string actual = NodeToString(node);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private string NodeToString(Node<NodeType> node)
        {
            string ret = "";

            NodeToString(node, ref ret, 0);

            return ret;
        }

        private void NodeToString(Node<NodeType> node, ref string ret, int indent)
        {
            string str = node.Children.Any(x => x.Children.Count > 0)
                ? Environment.NewLine + new string(' ', indent * 2)
                : " ";

            str = Environment.NewLine + new string(' ', indent * 2);

            ret += str + node.NodeType + (node.Text == "" ? "" : " : " + node.Text);

            foreach (Node<NodeType> child in node.Children)
            {
                NodeToString(child, ref ret, indent + 1);
            }
        }
    }
}
