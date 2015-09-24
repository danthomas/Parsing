using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Parsing.Tests
{
    [TestFixture]
    class Tests
    {
        [Test]
        public void Text()
        {
            List<Token> tokens = new List<Token> { new Token(TokenType.Text, "abc") };
            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse(tokens));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Text:abc"));
        }

        [Test]
        public void IdentifierStatement()
        {
            List<Token> tokens = new List<Token>
            {
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "abc"),
                new Token(TokenType.RightCurly)
            };

            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse(tokens));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        LeftCurly
        Identifier:abc
        RightCurly"));
        }

        [Test]
        public void IdentifierQuestionTextStatement()
        {
            List<Token> tokens = new List<Token>
            {
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "abc"),
                new Token(TokenType.Question),
                new Token(TokenType.Text, "def"),
                new Token(TokenType.RightCurly)
            };

            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse(tokens));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        LeftCurly
        Identifier:abc
        Question
        Expression
            Text:def
        RightCurly"));
        }

        [Test]
        public void IdentifierQuestionExpressionStatement()
        {
            List<Token> tokens = new List<Token>
            {
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "abc"),
                new Token(TokenType.Question),
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "def"),
                new Token(TokenType.RightCurly),
                new Token(TokenType.RightCurly)
            };

            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse(tokens));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        LeftCurly
        Identifier:abc
        Question
        Expression
            LeftCurly
            Identifier:def
            RightCurly
        RightCurly"));
        }

        [Test]
        public void IdentifierQuestionExpressionColonExpressionStatement()
        {
            List<Token> tokens = new List<Token>
            {
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "abc"),
                new Token(TokenType.Question),
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "def"),
                new Token(TokenType.RightCurly),
                new Token(TokenType.Colon),
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "ghi"),
                new Token(TokenType.RightCurly),
                new Token(TokenType.RightCurly)
            };

            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse(tokens));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        LeftCurly
        Identifier:abc
        Question
        Expression
            LeftCurly
            Identifier:def
            RightCurly
        Colon
        Expression
            LeftCurly
            Identifier:ghi
            RightCurly
        RightCurly"));
        }

        [Test]
        public void TextStatement()
        {
            List<Token> tokens = new List<Token>
            {
                new Token(TokenType.Text, "abc"),
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "def"),
                new Token(TokenType.RightCurly)
            };

            Parser parser = new Parser();

            var node = parser.Parse(tokens);
            var actual = NodesToString(node);

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Text:abc
    Expression
        LeftCurly
        Identifier:def
        RightCurly"));
        }

        [Test]
        public void StatementTextStatement()
        {
            List<Token> tokens = new List<Token>
            {
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "abc"),
                new Token(TokenType.RightCurly),
                new Token(TokenType.Text, " "),
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "def"),
                new Token(TokenType.RightCurly)
            };

            Parser parser = new Parser();

            var node = parser.Parse(tokens);
            var actual = NodesToString(node);

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        LeftCurly
        Identifier:abc
        RightCurly
    Expression
        Text: 
    Expression
        LeftCurly
        Identifier:def
        RightCurly"));
        }

        [Test]
        public void LeftCurly_ExpectedIdentifier()
        {
            List<Token> tokens = new List<Token>
            {
                new Token(TokenType.LeftCurly)
            };

            Parser parser = new Parser();

            string message = "";
            try
            {
                parser.Parse(tokens);
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            Assert.That(message, Is.EqualTo("Expected Identifier"));
        }

        [Test]
        public void LeftCurlyRightCurly_ExpectedIdentifier()
        {
            List<Token> tokens = new List<Token>
            {
                new Token(TokenType.LeftCurly),
                new Token(TokenType.RightCurly)
            };

            Parser parser = new Parser();

            string message = "";
            try
            {
                parser.Parse(tokens);
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            Assert.That(message, Is.EqualTo("Expected Identifier"));
        }

        [Test]
        public void LeftCurlyText_ExpectedRightCurly()
        {
            List<Token> tokens = new List<Token>
            {
                new Token(TokenType.LeftCurly),
                new Token(TokenType.Text, "abc")
            };

            Parser parser = new Parser();

            string message = "";
            try
            {
                parser.Parse(tokens);
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            Assert.That(message, Is.EqualTo("Expected RightCurly"));
        }

        private string NodesToString(Node node)
        {
            string ret = "";
            NodesToString(node, 0, ref ret);
            return ret;
        }

        private void NodesToString(Node node, int indent, ref string ret)
        {
            foreach (Node child in node.Children)
            {
                ret += Environment.NewLine +
                    new string(' ', indent * 4) +
                    child.NodeType +
                    (child.Text == "" ? "" : ":" + child.Text);

                NodesToString(child, indent + 1, ref ret);
            }
        }

    }
}