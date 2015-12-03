using System.IO;
using NUnit.Framework;
using V2.Parsing.Core.Domain;
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

            Core.GrammarDef.Parser parser = new Core.GrammarDef.Parser();

            var root = parser.Parse(text);

            utils.NodeToString(root);

            var builder = new Builder();

            var grammar = builder.BuildGrammar(root);

            var actual = utils.GrammarToDefString(grammar);

            Assert.That(actual, Is.EqualTo(text));
        }

        [Test]
        public void BuildGrammar()
        {
            var utils = new Utils();

            var builder = new Builder();

            var text = GetDef<Core.GrammarDef.Parser>();

            Core.GrammarDef.Parser parser = new Core.GrammarDef.Parser();

            Node<Core.GrammarDef.NodeType> root = parser.Parse(text);

            builder.PreProcess(root);

            File.WriteAllText(@"C:\temp\node.txt", utils.NodeToString(root));

            Grammar grammar = builder.BuildGrammar(root);

            var actual = utils.GrammarToDefString(grammar);

            Assert.That(actual, Is.EqualTo(text));

            File.WriteAllText(@"C:\temp\parser.cs", builder.BuildParser2(grammar));

            //var parser2 = builder.CreateParser(grammar);
            //
            //var root2 = parser2.GetType().GetMethod("Parse").Invoke(parser2, new object[] { text });
        }

        [Test]
        public void BuildParser()
        {
            var builder = new Builder();

            var text = @"grammar Tester
defs
    Main : Things
    Things : newLine things Thing+
    Thing : newLine Identifier colon Identifier+
patterns
    newLine : '\n'
    colon : ':'
    things
    Identifier : '^[a-zA-Z_][a-zA-Z1-9_]*$'";

            Core.GrammarDef.Parser parser = new Core.GrammarDef.Parser();

            Node<Core.GrammarDef.NodeType> root = parser.Parse(text);

            builder.PreProcess(root);

            Grammar grammar = builder.BuildGrammar(root);

            var parserDef = builder.BuildParser2(grammar);
        }
    }
}