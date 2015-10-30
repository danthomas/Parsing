using NUnit.Framework;
using V2.Parsing.Core.GrammarDef;
using V2.Parsing.Core.Tests.Bases;

namespace V2.Parsing.Core.Tests
{
    [TestFixture]
    public class BuilderTests : TestBase
    {
        [Test]
        public void BuildGrammer()
        {
            var text = GetDef<Parser>();

            Parser parser = new Parser(new Lexer());

            var root = parser.Parse(text);

            var builder = new Builder();

            var grammar = builder.BuildGrammar(root);

            var actual = builder.GrammarToString(grammar);
        }
    }
}