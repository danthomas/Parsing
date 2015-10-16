using System;
using NUnit.Framework;
using Parsing.Core.Domain;
using Parsing.Core.GrammarDef;
using Text = Parsing.Core.Domain.Text;

namespace Parsing.Core.Tests.GrammarDef
{
    [TestFixture]
    public class GeneratorTests
    {
        /*
Test
defs
  Statement : TextOrExpression*
  TextOrExpression : Text | Expression
  Expression : OpenCurly Text [If] CloseCurly
  If : Question Text [Else]
  Else : Colon Text    
texts
  Text : ".*"
punctuation
  OpenCurly : {
  CloseCurly : }
  Question : ?
  Colon : ":"
            */

        [Test]
        public void Text()
        {
            var text = @"Text
defs
  Statement : Text
texts
  Text : "".*""";

            Run(text, "one", @"
Statement
 Text - one");
        }

        [Test]
        public void TextText()
        {
            var text = @"Text
defs
  Statement : Text Text
texts
  Text : "".*""
punctuation
  space : "" ""
ignore
  space";

            Run(text, "one two", @"
Statement
 Text - one
 Text - two");
        }

        [Test]
        public void TextOptionalText()
        {
            var text = @"Text
defs
  Statement : Text [Text]
texts
  Text : "".*""
punctuation
  space : "" ""
ignore
  space";

            Run(text, "one", @"
Statement
 Text - one");

            Run(text, "one two", @"
Statement
 Text - one
 Text - two");
        }

        [Test]
        public void ZeroOrMoreText()
        {
            var text = @"Text
defs
  Statement : Text*
texts
  Text : "".*""
punctuation
  space : "" ""
ignore
  space";

            Run(text, "", @"
Statement");

            Run(text, "one two three", @"
Statement
 Text - one
 Text - two
 Text - three");
        }

        [Test]
        public void OneOrMoreText()
        {
            var text = @"Text
defs
  Statement : Text+
texts
  Text : "".*""
punctuation
  space : "" ""
ignore
  space";

            Run(text, "one two three", @"
Statement
 Text - one
 Text - two
 Text - three");
        }


        [Test]
        public void Keywords()
        {
            var text = @"Text
defs
  Statement : one two three
keywords
    one : one
    two : two
    three : three
punctuation
  space : "" ""
ignore
  space";

            Run(text, "one two three", @"
Statement
 One
 Two
 Three");
        }


        [Test]
        public void Sql()
        {
            var parser = new Core.GrammarGrammar.Parser();

            var node = parser.Parse(@"Sql
defs
Statement : select StartOrFieldList
StartOrFieldList : star | FieldList
FieldList : FieldDef [comma FieldDef]
FieldDef : [Agg openBracket] Field [closeBracket]
Agg : count | min | max | avg
Field : [openSquare] Text [closeSquare]
texts
Text : "".*""
keywords
select: select
count: count
min: min
max: max
avg: avg
punctuation
star: ""*""
comma: ,
openBracket: (
closeBracket : )
openSquare: ""[""
closeSquare: ""]""");

            var generator = new Generator();
            var builder = new Builder();
            
            string nodeTree = generator.GenerateNodeTree(node);

            var grammar = generator.BuildGrammar(node);
            
            string actual = GenerateAndBuildParser(grammar, "select abc def ghi");
        }

        [Test]
        [Ignore]
        public void ObjectRef()
        {
            var dot = new Token("dot", ".");
            var text = new Text("Text", ".+");

            var root = new Def("ObjectRef", text, new Optional(dot, text), new Optional(dot, text));

            Grammar grammar = new Grammar
            {
                StringQuote = '\'',
                Root = root
            };

            string actual = GenerateAndBuildParser(grammar, "abc");

            Assert.That(actual, Is.EqualTo(@"
ObjectRef
 Text - abc"));

            actual = GenerateAndBuildParser(grammar, "abc.def");

            Assert.That(actual, Is.EqualTo(@"
ObjectRef
 Text - abc
 Dot
 Text - def"));

            actual = GenerateAndBuildParser(grammar, "abc.def.ghi");

            Assert.That(actual, Is.EqualTo(@"
ObjectRef
 Text - abc
 Dot
 Text - def
 Dot
 Text - ghi"));
        }

        [Test]
        [Ignore]
        public void Table()
        {
            var _as = new Token("as");
            var _space = new Token("WhiteSpace", " ");
            var text = new Text("Text", ".+");

            var root = new Def("Table", text, new Optional(_as), new Optional(text));

            Grammar grammar = new Grammar
            {
                StringQuote = '\'',
                Root = root,
                IgnoreTokens = new[] { _space }
            };

            string actual = GenerateAndBuildParser(grammar, "abc");

            Assert.That(actual, Is.EqualTo(@"
Table
 Text - abc"));

            actual = GenerateAndBuildParser(grammar, "abc ABC");

            Assert.That(actual, Is.EqualTo(@"
Table
 Text - abc
 Text - ABC"));

            actual = GenerateAndBuildParser(grammar, "abc as ABC");

            Assert.That(actual, Is.EqualTo(@"
Table
 Text - abc
 As
 Text - ABC"));
        }

        [Test]
        [Ignore]
        public void StarOrObjectRef()
        {
            var dot = new Token("dot", ".");
            var text = new Text("Text", ".+");
            var star = new Token("star", "*");
            var _space = new Token("whitespace", " ");

            var objectRef = new Def("ObjectRef", text, new Optional(dot, text), new Optional(dot, text));
            var root = new Def("StarOrObjectRef", new OneOf(star, objectRef));

            Grammar grammar = new Grammar
            {
                StringQuote = '\'',
                Root = root,
                IgnoreTokens = new[] { _space }
            };

            string actual = GenerateAndBuildParser(grammar, "*");

            Assert.That(actual, Is.EqualTo(@""));
        }

        private void Run(string grammarDef, string input, string expected)
        {
            var parser = new Core.GrammarGrammar.Parser();

            var generator = new Generator();
            var builder = new Builder();

            var node = parser.Parse(grammarDef);

           var tree=  generator.GenerateNodeTree(node);
            
            var grammar =  generator.BuildGrammar(node);

            string actual = GenerateAndBuildParser(grammar, input);
            
            Assert.That(actual, Is.EqualTo(expected));
        }

        private string GenerateAndBuildParser(Grammar grammar, string text)
        {

            Generator generator = new Generator();
            Builder builder = new Builder();
            string lexerDef = generator.GenerateLexer(grammar);

            string parserDef = generator.GenerateParser(grammar);

            var assembly = builder.Build(lexerDef, parserDef);
            
            object parser = Activator.CreateInstance(assembly.GetType("Xxx.Parser"));
            object walker = Activator.CreateInstance(assembly.GetType("Xxx.Walker"));

            var node = parser.GetType().GetMethod("Parse").Invoke(parser, new object[] { text });
            return (string)walker.GetType().GetMethod("NodesToString").Invoke(walker, new[] { node });
        }

    }
}