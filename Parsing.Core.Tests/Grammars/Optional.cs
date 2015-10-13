using System;
using System.Collections.Generic;
using NUnit.Framework;
using Parsing.Core.GrammarDef;

namespace Parsing.Core.Tests.Grammars
{
    [TestFixture]
    public class OptionalTest
    {
        [Test]
        public void Test()
        {
            var def = @"Optional
defs
Statement : [one] [two] [three]
keywords
one : one
two : two
three : three
punctuation
space : "" """;

            var parser = new Parser();
            var walker = new Walker();

            string actual = walker.NodesToString(parser.Parse("one three"));

            Assert.That(actual, Is.EqualTo(@"
Statement
 One
 Three"));

            actual = walker.NodesToString(parser.Parse("two"));

            Assert.That(actual, Is.EqualTo(@"
Statement
 Two"));

            actual = walker.NodesToString(parser.Parse("three"));

            Assert.That(actual, Is.EqualTo(@"
Statement
 Three"));
        }


        public class OptionalGrammar : Grammar
        {
            private Def _root;

            public OptionalGrammar()
            {
                //punctuation
                var @space = new Token("space", " ");

                //keywords
                var @one = new Token("one");
                var @two = new Token("two");
                var @three = new Token("three");

                //texts

                //defs

                var @Statement = new Def("Statement", new Optional(@one), new Optional(@two), new Optional(@three));

                _root = @Statement;

                IgnoreTokens = new[] { @space };
            }

            public override Thing Root { get { return _root; } }

            public override char StringQuote { get { return '\''; } }
        }
        
        public class Lexer : LexerBase<TokenType>
        {
            private readonly Dictionary<char, TokenType> _punctuation;
            private readonly Dictionary<string, TokenType> _keywords;
            private readonly Dictionary<string, TokenType> _texts;
            private readonly List<TokenType> _ignoreTokenTypes;
            private readonly char _stringQuote;

            public Lexer()
            {
                _punctuation = new Dictionary<char, TokenType>
            {
                { ' ', TokenType.Space },
            };

                _keywords = new Dictionary<string, TokenType>
            {
                { "one", TokenType.One },
                { "two", TokenType.Two },
                { "three", TokenType.Three },
            };

                _texts = new Dictionary<string, TokenType>
                {
                };

                _ignoreTokenTypes = new List<TokenType>
            {
                TokenType.Space,
            };
                _stringQuote = '\'';
            }

            public override TokenType EndOfFileTokenType { get { return TokenType.EndOfFile; } }
            public override TokenType StringTokenType { get { return TokenType.String; } }
            public override Dictionary<char, TokenType> Punctuation { get { return _punctuation; } }
            public override Dictionary<string, TokenType> KeyWords { get { return _keywords; } }
            public override Dictionary<string, TokenType> Texts { get { return _texts; } }
            public override List<TokenType> IgnoreTokenTypes { get { return _ignoreTokenTypes; } }
            public override char StringQuote { get { return _stringQuote; } }
        }
        
        public enum TokenType
        {
            EndOfFile,
            String,
            One,
            Two,
            Three,
            Space,
        }
        public class Parser : ParserBase<TokenType, NodeType>
        {
            public Parser() : base(new Lexer())
            {
            }

            public override Node<NodeType> Root()
            {
                Node<NodeType> root = new Node<NodeType>(null, NodeType.Statement);

                Statement(root);

                return root;
            }

            public void Statement(Node<NodeType> parent)
            {
                if (IsTokenType(TokenType.One))
                {
                    Consume(parent, TokenType.One, NodeType.One);
                }
                if (IsTokenType(TokenType.Two))
                {
                    Consume(parent, TokenType.Two, NodeType.Two);
                }
                if (IsTokenType(TokenType.Three))
                {
                    Consume(parent, TokenType.Three, NodeType.Three);
                }
            }
        }

        public enum NodeType
        {
            Statement,
            One,
            Two,
            Three,
        }

        public class Walker
        {
            public string NodesToString(object node)
            {
                Node<NodeType> parent = node as Node<NodeType>;

                string ret = "";

                NodesToString(parent, ref ret, 0);

                return ret;
            }

            private void NodesToString(Node<NodeType> parent, ref string ret, int indent)
            {
                ret += Environment.NewLine + new string(' ', indent) + parent.NodeType + (String.IsNullOrWhiteSpace(parent.Text) ? "" : " - " + parent.Text);
                foreach (Node<NodeType> child in parent.Children)
                {
                    NodesToString(child, ref ret, indent + 1);
                }
            }
        }
    }
}