/*using NUnit.Framework;
using Parsing.Core.Domain;
using Parsing.Core.GrammarDef;

namespace Parsing.Core.Tests.Grammars
{
    [TestFixture]
    public class Temp
    {
        [Test]
        public void Test()
        {
            var def = @"OneOrMoreOptional
defs
  Statement : xxx [one]+ xxx
keywords
  xxx : xxx
  one : one
punctuation
  space : "" ""
ignore
  space";

            var parser = new Core.GrammarGrammar.Parser();

            var generator = new Generator();

            var node = parser.Parse(def);
            /*

           node = generator.Rejig(node);

           string grammar = generator.GenerateGrammar(node);
           string lexerDef = generator.GenerateLexer(new OneOrMoreOptionalGrammar());
           string parserDef = generator.GenerateParser(new OneOrMoreOptionalGrammar());
           #1#
        }

        public class OneOrMoreOptionalGrammar : Grammar
        {
            private Def _root;

            public OneOrMoreOptionalGrammar()
            {
                //punctuation
                var @space = new Token("space", " ");

                //keywords
                var @xxx = new Token("xxx");
                var @one = new Token("one");

                //texts

                //defs

                var @Statement = new Def("Statement", @xxx, new OneOrMore(new Optional(@one)), @xxx);

                _root = @Statement;

                IgnoreTokens = new[] { @space, };
            }

            public override Thing Root { get { return _root; } }

            public override char StringQuote { get { return '\''; } }
        }
    }
}*/