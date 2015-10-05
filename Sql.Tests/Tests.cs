using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Parsing.Core;
using static System.String;

namespace Sql.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void SelectStar()
        {
            Run(@"
select *", @"
Root
  Select
  SelectFields
    Star");
        }

        [Test]
        public void SelectFields()
        {
            Run(@"
select Field1, Field2 FieldB, FieldB as FieldC", @"
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
      Text : FieldC");
        }

        [Test]
        public void SelectCountStar()
        {
            Run(@"select count(*)", @"");
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

            ret += str + node.TokenType + (node.Text == "" ? "" : " : " + node.Text);

            foreach (Node<NodeType> child in node.Children)
            {
                NodeToString(child, ref ret, indent + 1);
            }
        }
    }
}
