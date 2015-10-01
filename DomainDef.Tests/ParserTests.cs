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
            Node node = new Parser(new Lexer(@"entity Account
Id int
AccountName string
Forenames string")).Parse();
        }
    }
}
