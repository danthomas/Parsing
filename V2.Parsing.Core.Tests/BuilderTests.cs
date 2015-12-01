﻿using NUnit.Framework;
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

            var actual = utils.GrammarToString(grammar);

            Assert.That(actual, Is.EqualTo(text));
        }

        [Test]
        public void BuildGrammer()
        {
            var text = GetDef<Core.GrammarDef.Parser>();

            Core.GrammarDef.Parser parser = new Core.GrammarDef.Parser();

            Node<Core.GrammarDef.NodeType> root = parser.Parse(text);

            var utils = new Utils();

            var builder = new Builder();

            var grammar = builder.BuildGrammar(root);

            var actual = utils.GrammarToString(grammar);

            Assert.That(actual, Is.EqualTo(text));

            var parser2 = builder.CreateParser(grammar);

            var root2 = parser2.GetType().GetMethod("Parse").Invoke(parser2, new object[] { text });
        }
    }
}