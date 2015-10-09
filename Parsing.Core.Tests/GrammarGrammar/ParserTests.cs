using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Parsing.Core.GrammarGrammar;

namespace Parsing.Core.Tests.GrammarGrammar
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Test()
        {
            Parser parser = new Parser();

            Node<NodeType> node = parser.Parse("grammar Grammar");
        }
    }
}
