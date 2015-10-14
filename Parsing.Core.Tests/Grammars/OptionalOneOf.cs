using System;
using System.Collections.Generic;
using NUnit.Framework;
using Parsing.Core.Domain;
using Parsing.Core.GrammarDef;

namespace Parsing.Core.Tests.Grammars
{
    [TestFixture]
    public class OptionalOneOfTest
    {
        [Test]
        public void Test()
        {
            var def = @"OptionalOneOf
defs
Statement : xxx [one | two | three] xxx
keywords
xxx : xxx
one : one
two : two
three : three
punctuation
space : "" """;

            var parser = new Parser();
            var walker = new Walker();

            string actual = walker.NodesToString(parser.Parse("xxx one xxx"));

            Assert.That(actual, Is.EqualTo(@"
Statement
 Xxx
 One
 Xxx"));

            actual = walker.NodesToString(parser.Parse("xxx two xxx"));

            Assert.That(actual, Is.EqualTo(@"
Statement
 Xxx
 Two
 Xxx"));
        }

        public class OptionalOneOfGrammar : Grammar
        {
            private Def _root;

            public OptionalOneOfGrammar()
            {
                //punctuation
                var @space = new Token("space", " ");

                //keywords
                var @one = new Token("one");
                var @two = new Token("two");
                var @three = new Token("three");

                //texts

                //defs

                var @Statement = new Def("Statement", new Optional(new OneOf( @one, @two, @three)));

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
                { "xxx", TokenType.Xxx },
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
            Xxx,
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
                Consume(parent, TokenType.Xxx, NodeType.Xxx);
                if (IsTokenType(TokenType.One, TokenType.Two, TokenType.Three))
                {
                    if (IsTokenType(TokenType.One))
                    {
                        Consume(parent, TokenType.One, NodeType.One);
                    }
                    else if (IsTokenType(TokenType.Two))
                    {
                        Consume(parent, TokenType.Two, NodeType.Two);
                    }
                    else if (IsTokenType(TokenType.Three))
                    {
                        Consume(parent, TokenType.Three, NodeType.Three);
                    }
                }
                Consume(parent, TokenType.Xxx, NodeType.Xxx);
            }
        }

        public enum NodeType
        {
            Statement,
            Xxx,
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