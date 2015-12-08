using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace V3.Parsing.Core.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void SimpleText()
        {
            Parse("abc", @"
Expr : 
    Text : abc");
        }

        [Test]
        public void AttrEqualsValue()
        {
            Parse("{abc=def}", @"
Expr : 
    SubExpr : 
        OpenCurly : {
        Identifier : abc
        Equals : =
        Value : def
        CloseCurly : }");
        }

        [Test]
        public void SimpleSubExpr()
        {
            Parse("{abc}", @"
Expr : 
    SubExpr : 
        OpenCurly : {
        Identifier : abc
        CloseCurly : }");
        }

        [Test]
        public void TextWithWhiteSpace()
        {
            Parse("abc def", @"
Expr : 
    Text : abc def");
        }

        [Test]
        public void SimpleTextSubExprText()
        {
            Parse("abc{def}ghi", @"
Expr : 
    Text : abc
    SubExpr : 
        OpenCurly : {
        Identifier : def
        CloseCurly : }
    Text : ghi");
        }

        private void Parse(string text, string expected)
        {
            Parser parser = new Parser();

            var actual = NodeToString(parser.Parse(text));

            Assert.That(actual, Is.EqualTo(expected));

        }

        private string NodeToString(Node<NodeType> root)
        {
            StringBuilder stringBuilder = new StringBuilder();

            NodeToString(root, stringBuilder, 0);

            return stringBuilder.ToString();
        }

        private void NodeToString(Node<NodeType> parent, StringBuilder stringBuilder, int indent)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(new string(' ', indent*4));
            stringBuilder.Append(parent.NodeType);
            stringBuilder.Append(" : ");
            stringBuilder.Append(parent.Text);

            foreach(Node<NodeType> child in parent.Nodes)
            {
                NodeToString(child, stringBuilder, indent + 1);
            }
        }

        class Parser : ParserBase<NodeType>
        {
            public Parser()
                : base(new Lexer())
            {
            }

            protected override Node<NodeType> Root()
            {
                var root = new Node<NodeType>(NodeType.Expr);

                while (AreNodeTypes(NodeType.Text) ||
                    AreNodeTypes(NodeType.OpenCurly))
                {
                    TextOrSubExpr(root);
                }

                return root;
            }

            private void TextOrSubExpr(Node<NodeType> parent)
            {
                if (AreNodeTypes(NodeType.Text))
                {
                    Consume(parent, NodeType.Text);
                }
                else if (AreNodeTypes(NodeType.OpenCurly))
                {
                    SubExpr(parent);
                }
            }

            private void SubExpr(Node<NodeType> parent)
            {
                var child = Add(parent, NodeType.SubExpr);

                Consume(child, NodeType.OpenCurly);
                Consume(child, NodeType.Identifier);
                if(AreNodeTypes(NodeType.Equals))
                {
                    Consume(child, NodeType.Equals);
                    Consume(child, NodeType.Value);
                }
                Consume(child, NodeType.CloseCurly);
            }

            public override bool CaseSensitive => false;
        }

        class Lexer : LexerBase<NodeType>
        {
            public Lexer()
            {
                Patterns = new List<PatternBase<NodeType>>
                {
                    new TokenPattern<NodeType>(NodeType.OpenCurly, "{"),
                    new TokenPattern<NodeType>(NodeType.CloseCurly, "}"),
                    new TokenPattern<NodeType>(NodeType.Equals, "="),
                    new TokenPattern<NodeType>(NodeType.Question, "?"),
                    new TokenPattern<NodeType>(NodeType.Colon, ":"),
                    new RegexPattern<NodeType>(NodeType.Text, "^[^{}]+$"),
                    new RegexPattern<NodeType>(NodeType.Value, "^[^?:{}]+$"),
                    new RegexPattern<NodeType>(NodeType.Identifier, "^[a-zA-Z1-9_]+$"),
                };
            }
        }

        public enum NodeType
        {
            Text,
            OpenCurly,
            CloseCurly,
            Identifier,
            EndOfFile,
            Expr,
            TextOrSubExpr,
            SubExpr,
            Colon,
            Question,
            Equals,
            Value
        }
    }
}