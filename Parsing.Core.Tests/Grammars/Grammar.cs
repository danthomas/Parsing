using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Parsing.Core.Tests.Grammars
{
    /*
    Grammar
defs
  Grammar : Stringy [newLine _defs 'Defs'] [newLine _texts 'Texts'] [newLine _keywords 'Keywords'] [newLine _punctuation 'Punctuations']  [newLine _ignore 'Ignores']  [newLine _discard 'Discards']
  Defs : Def*
  Def : newLine colon Def2+
  Def2 : [openSquare] [Stringy | pipe]* [closeSquare] [plus | star]
  Texts : Text*
  Text : newLine Stringy colon Stringy
  Keywords : Keyword*
  Keyword : newLine Stringy colon Stringy
  Punctuations : Punctuation*
  Punctuation : newLine Stringy colon Stringy
  Ignores : Ignore*
  Ignore : newLine Stringy
  Discards : Discard*
  Discard : newLine Stringy
texts
  Stringy : '"".*""'
keywords
  _defs : 'defs'
  _texts : 'texts'
  _keywords : 'keywords'
  _punctuation : 'punctuation'
  _ignore : 'ignore'
  _discard : 'discard'
punctuation
  return : '\r'
  newLine : '\n'
  colon : ':'
  openSquare : '['
  closeSquare : ']'
  star : '*'
  plus : '+'
  pipe : '|'
  space : ' '
ignore
  space
discard
  newLine
  _defs
  _texts
  _keywords
  _punctuation
  _ignore
  _discard        
        */

    [TestFixture]
    public class Grammar
    {
        [Test]
        public void Test()
        {
            var node = new GrammarParser().Parse(@"Grammar
defs
  Grammar : Stringy [newLine _defs 'Defs'] [newLine _texts 'Texts'] [newLine _keywords 'Keywords'] [newLine _punctuation 'Punctuations']  [newLine _ignore 'Ignores']  [newLine _discard 'Discards']
  'Defs' : Def*
  Def : newLine colon Def2+
  Def2 : [openSquare] Stringy* [closeSquare] [plus | star]
  'Texts' : Text*
  'Text' : newLine Stringy colon Stringy
  'Keywords' : Keyword*
  'Keyword' : newLine Stringy colon Stringy
  'Punctuations' : 'Punctuation'*
  'Punctuation' : newLine Stringy colon Stringy
  'Ignores' : 'Ignore'*
  'Ignore' : newLine Stringy
  'Discards' : 'Discard'*
  'Discard' : newLine Stringy
texts
  Stringy : '"".*""'
keywords
  _defs : 'defs'
  _texts : 'texts'
  _keywords : 'keywords'
  _punctuation : 'punctuation'
  _ignore : 'ignore'
  _discard : 'discard'");

            string actual = new Walker().NodesToString(node);
        }

        public class GrammarParser : ParserBase<TokenType, NodeType>
        {
            private readonly List<string> _discardThings;

            public GrammarParser() : base(new GrammarLexer())
            {
                _discardThings = new List<string>
            {
                "_defs",
                "_texts",
                "_keywords",
                "_punctuation",
                "_ignore",
                "_discard",
                "NewLine",
            };
            }

            public override List<string> DiscardThings { get { return _discardThings; } }

            public override Node<NodeType> Root()
            {
                return Grammar(null);
            }

            public Node<NodeType> Grammar(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Grammar);
                Consume(child, TokenType.Stringy, NodeType.Stringy);
                if (AreTokenTypes(TokenType.NewLine, TokenType._defs))
                {
                    Consume(child, TokenType.NewLine, NodeType.NewLine);
                    Consume(child, TokenType._defs, NodeType._defs);
                    Defs(child);
                }
                if (AreTokenTypes(TokenType.NewLine, TokenType._texts))
                {
                    Consume(child, TokenType.NewLine, NodeType.NewLine);
                    Consume(child, TokenType._texts, NodeType._texts);
                    Texts(child);
                }
                if (AreTokenTypes(TokenType.NewLine, TokenType._keywords))
                {
                    Consume(child, TokenType.NewLine, NodeType.NewLine);
                    Consume(child, TokenType._keywords, NodeType._keywords);
                    Keywords(child);
                }
                if (AreTokenTypes(TokenType.NewLine, TokenType._punctuation))
                {
                    Consume(child, TokenType.NewLine, NodeType.NewLine);
                    Consume(child, TokenType._punctuation, NodeType._punctuation);
                    Punctuations(child);
                }
                if (AreTokenTypes(TokenType.NewLine, TokenType._ignore))
                {
                    Consume(child, TokenType.NewLine, NodeType.NewLine);
                    Consume(child, TokenType._ignore, NodeType._ignore);
                    Ignores(child);
                }
                if (AreTokenTypes(TokenType.NewLine, TokenType._discard))
                {
                    Consume(child, TokenType.NewLine, NodeType.NewLine);
                    Consume(child, TokenType._discard, NodeType._discard);
                    Discards(child);
                }

                return child;
            }

            public Node<NodeType> Defs(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Defs);
                while (AreTokenTypes(TokenType.NewLine, TokenType.Stringy, TokenType.Colon))
                {
                    Def(child);
                }

                return child;
            }

            public Node<NodeType> Def(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Def);
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Consume(child, TokenType.Stringy, NodeType.Stringy);
                Consume(child, TokenType.Colon, NodeType.Colon);
                Def2(child);
                while (IsTokenType(TokenType.Stringy, TokenType.Colon, TokenType.OpenSquare, TokenType.CloseSquare, TokenType.Plus, TokenType.Star))
                {
                    Def2(child);
                }

                return child;
            }

            public Node<NodeType> Def2(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Def2);
                if (IsTokenType(TokenType.OpenSquare))
                {
                    Consume(child, TokenType.OpenSquare, NodeType.OpenSquare);
                }
                while (IsTokenType(TokenType.Stringy, TokenType.Pipe))
                {
                    if (IsTokenType(TokenType.Stringy))
                    {
                        Consume(child, TokenType.Stringy, NodeType.Stringy);
                    }
                    else if (IsTokenType(TokenType.Pipe))
                    {
                        Consume(child, TokenType.Pipe, NodeType.Pipe);
                    }
                }
                if (IsTokenType(TokenType.CloseSquare))
                {
                    Consume(child, TokenType.CloseSquare, NodeType.CloseSquare);
                }
                if (IsTokenType(TokenType.Plus, TokenType.Star))
                {
                    if (IsTokenType(TokenType.Plus))
                    {
                        Consume(child, TokenType.Plus, NodeType.Plus);
                    }
                    else if (IsTokenType(TokenType.Star))
                    {
                        Consume(child, TokenType.Star, NodeType.Star);
                    }
                }

                return child;
            }

            public Node<NodeType> Texts(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Texts);
                while (IsTokenType(TokenType.NewLine))
                {
                    Text(child);
                }

                return child;
            }

            public Node<NodeType> Text(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Text);
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Consume(child, TokenType.Stringy, NodeType.Stringy);
                Consume(child, TokenType.Colon, NodeType.Colon);
                Consume(child, TokenType.Stringy, NodeType.Stringy);

                return child;
            }

            public Node<NodeType> Keywords(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Keywords);
                while (IsTokenType(TokenType.NewLine))
                {
                    Keyword(child);
                }

                return child;
            }

            public Node<NodeType> Keyword(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Keyword);
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Consume(child, TokenType.Stringy, NodeType.Stringy);
                Consume(child, TokenType.Colon, NodeType.Colon);
                Consume(child, TokenType.Stringy, NodeType.Stringy);

                return child;
            }

            public Node<NodeType> Punctuations(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Punctuations);
                while (IsTokenType(TokenType.NewLine))
                {
                    Punctuation(child);
                }

                return child;
            }

            public Node<NodeType> Punctuation(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Punctuation);
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Consume(child, TokenType.Stringy, NodeType.Stringy);
                Consume(child, TokenType.Colon, NodeType.Colon);
                Consume(child, TokenType.Stringy, NodeType.Stringy);

                return child;
            }

            public Node<NodeType> Ignores(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Ignores);
                while (IsTokenType(TokenType.NewLine))
                {
                    Ignore(child);
                }

                return child;
            }

            public Node<NodeType> Ignore(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Ignore);
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Consume(child, TokenType.Stringy, NodeType.Stringy);

                return child;
            }

            public Node<NodeType> Discards(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Discards);
                while (IsTokenType(TokenType.NewLine))
                {
                    Discard(child);
                }

                return child;
            }

            public Node<NodeType> Discard(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.Discard);
                Consume(child, TokenType.NewLine, NodeType.NewLine);
                Consume(child, TokenType.Stringy, NodeType.Stringy);

                return child;
            }
        }

        public enum NodeType
        {
            Grammar,
            Defs,
            Def,
            Def2,
            Texts,
            Text,
            Keywords,
            Keyword,
            Punctuations,
            Punctuation,
            Ignores,
            Ignore,
            Discards,
            Discard,
            Stringy,
            NewLine,
            _defs,
            Colon,
            OpenSquare,
            CloseSquare,
            Plus,
            Star,
            Pipe,
            _texts,
            _keywords,
            _punctuation,
            _ignore,
            _discard,
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

        public class GrammarLexer : LexerBase<TokenType>
        {
            private readonly Dictionary<char, TokenType> _punctuation;
            private readonly Dictionary<string, TokenType> _keywords;
            private readonly Dictionary<string, TokenType> _texts;
            private readonly List<TokenType> _ignoreTokenTypes;
            private readonly char _stringQuote;

            public GrammarLexer()
            {
                _punctuation = new Dictionary<char, TokenType>
            {
                { '\r', TokenType.Return },
                { '\n', TokenType.NewLine },
                { ':', TokenType.Colon },
                { '[', TokenType.OpenSquare },
                { ']', TokenType.CloseSquare },
                { '*', TokenType.Star },
                { '+', TokenType.Plus },
                { '|', TokenType.Pipe },
                { ' ', TokenType.Space },
            };

                _keywords = new Dictionary<string, TokenType>
            {
                { "defs", TokenType._defs },
                { "texts", TokenType._texts },
                { "keywords", TokenType._keywords },
                { "punctuation", TokenType._punctuation },
                { "ignore", TokenType._ignore },
                { "discard", TokenType._discard },
            };

                _texts = new Dictionary<string, TokenType>
            {
                { "^.*$", TokenType.Stringy },
            };

                _ignoreTokenTypes = new List<TokenType>
            {
                TokenType.Space,
                TokenType.Return,
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
            Return,
            NewLine,
            Colon,
            OpenSquare,
            CloseSquare,
            Star,
            Plus,
            Pipe,
            Space,
            _defs,
            _texts,
            _keywords,
            _punctuation,
            _ignore,
            _discard,
            Stringy,
        }
    }
}