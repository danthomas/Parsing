using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DomainDef.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Test1()
        {
            Node node = new Parser().Parse(@"domain CrossfitJST
entity Account
    prop Id int ident
    prop AccountName string(1, 20) unique
    prop Forenames string(30)
    prop Surname string(30)
    prop IsActive bool default(true)

entity Role
    prop Id short ident
    prop Name string(5, 20) unique

entity AccountRole
    prop Id int ident
    ref Account
    ref Role
    unique (Account, Role)

");

            string actual = NodeToString(node);
        }

        private string NodeToString(Node node)
        {
            string ret = "";

            NodeToString(node, ref ret, 0);

            return ret;
        }

        private void NodeToString(Node node, ref string ret, int indent)
        {
            string str = node.Children.Any(x => x.Children.Count > 0)
                ? Environment.NewLine + new string(' ', indent * 2)
                : " ";

            str = Environment.NewLine + new string(' ', indent * 2);

            ret += str + node.TokenType + (node.Text == "" ? "" : " : " + node.Text);

            foreach (Node child in node.Children)
            {
                NodeToString(child, ref ret, indent + 1);
            }
        }
    }
}
