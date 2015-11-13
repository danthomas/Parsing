using NUnit.Framework;
using V2.Parsing.Core.GrammarDef;
using V2.Parsing.Core.Tests.Bases;

namespace V2.Parsing.Core.Tests
{
    [TestFixture]
    public class BuilderTests : TestBase
    {
        [Test]
        public void CommaSeperatedNames()
        {
            var text = @"grammar CommaSeperatedNames
defs
    Name : Identifier
    Names : Name [comma Name]*
patterns
    comma : ','
    Identifier : '^[a-zA-Z_][a-zA-Z1-9_]*$'
";
            var utils = new Utils();

            Parser parser = new Parser();

            var root = parser.Parse(text);
            
            utils.NodeToString(root);

            var builder = new Builder();

            var grammar = builder.BuildGrammar(root);

            //grammar.Defs[1].Elements[1] = new Domain.ZeroOrMore
            //{
            //    Element = grammar.Defs[1].Elements[1]
            //};

            var actual = utils.GrammarToString(grammar);

            Assert.That(actual, Is.EqualTo(text));
        }

        [Test]
        public void BuildGrammer()
        {
            var text = GetDef<Parser>();

            Parser parser = new Parser();

            var root = parser.Parse(text);

            var utils = new Utils();
            
            var builder = new Builder();

            var grammar = builder.BuildGrammar(root);
            
            var actual = utils.GrammarToString(grammar);

            Assert.That(actual, Is.EqualTo(text));

            actual = builder.ToLexer(grammar);
        }
    }
}