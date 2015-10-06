using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Parsing.Core.Grammar;

namespace Parsing.Core.Tests.Grammar
{
    [TestFixture]
    class ParserTests
    {
        [Test]
        public void Test()
        {
            Run(@"Select : select SelectFields
SelectFields : star | Field*", @"");
        }

        private void Run(string text, string expected)
        {
            Parser parser = new Parser();

            var node = parser.Parse(text);

            var actual = NodeToString(node);

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
