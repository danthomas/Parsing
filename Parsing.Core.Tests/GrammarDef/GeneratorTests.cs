using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using NUnit.Framework;
using Parsing.Core.GrammarDef;
using static System.String;

namespace Parsing.Core.Tests.GrammarDef
{
    [TestFixture]
    public class GeneratorTests
    {

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

            node = generator.Rejig(node);

            string nodeTree = generator.GenerateNodeTree(node);

            var grammar = generator.GenerateGrammar(node);

            var assembly = builder.Build(grammar);

            Grammar xxx = (Grammar) Activator.CreateInstance(assembly.GetType("Xxx.SqlGrammar"));
            
            string actual = GenerateAndBuildParser(xxx, "select abc def ghi");
        }



        [Test]
        public void ObjectRef()
        {
            var dot = new Token("dot", ".");
            var text = new Core.GrammarDef.Text("Text", ".+");

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
        public void Table()
        {
            var _as = new Token("as");
            var _space = new Token("WhiteSpace", " ");
            var text = new Core.GrammarDef.Text("Text", ".+");

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
        public void StarOrObjectRef()
        {
            var dot = new Token("dot", ".");
            var text = new Core.GrammarDef.Text("Text", ".+");
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

        private string GenerateAndBuildParser(Grammar grammar, string text)
        {
            Xxx.Parser parser = new Xxx.Parser();
            var node = parser.Parse(text);
            //return new Xxx.Walker().NodesToString(node);

            return Generated(grammar, text);
        }

        private static string Generated(Grammar grammar, string text)
        {
            Generator generator = new Generator();
            Builder builder = new Builder();
            string lexerDef = generator.GenerateLexer(grammar);

            string parserDef = generator.GenerateParser(grammar);

            var assembly = builder.Build(lexerDef, parserDef);
            
            File.WriteAllText(@"C:\Temp\Parsing\Parsing.Core.Tests\GrammarDef\Lexer.cs", lexerDef);
            File.WriteAllText(@"C:\Temp\Parsing\Parsing.Core.Tests\GrammarDef\Parser.cs", parserDef);

            object parser = Activator.CreateInstance(assembly.GetType("Xxx.Parser"));
            object walker = Activator.CreateInstance(assembly.GetType("Xxx.Walker"));

            var node = parser.GetType().GetMethod("Parse").Invoke(parser, new object[] { text });
            return (string)walker.GetType().GetMethod("NodesToString").Invoke(walker, new[] { node });
        }

    }
}