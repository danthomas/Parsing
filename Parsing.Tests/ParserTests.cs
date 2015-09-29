using System;
using NUnit.Framework;

namespace Parsing.Tests
{
    [TestFixture]
    class ParserTests
    {
        [Test]
        public void IdentifierEqualsText()
        {
            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse("{def=yyy}"));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Identifier:def
        EqualTo
        Text:yyy"));
        }

        [Test]
        public void Text()
        {
            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse("abc"));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Text:abc"));
        }

        [Test]
        public void IdentifierStatement()
        {
            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse("{abc}"));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Identifier:abc"));
        }

        [Test]
        public void IdentifierEqualToTextStatement()
        {
            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse("{abc=def ghi}"));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Identifier:abc
        EqualTo
        Text:def ghi"));
        }

        [Test]
        public void IdentifierNotEqualToTextStatement()
        {
            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse("{abc!=def ghi}"));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Identifier:abc
        NotEqualTo
        Text:def ghi"));
        }

        [Test]
        public void IdentifierQuestionTextStatement()
        {
            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse("{abc?def}"));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Identifier:abc
        Question
        Expressions
            Expression
                Text:def"));
        }

        [Test]
        public void IdentifierQuestionExpressionStatement()
        {
            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse("{abc?{def}}"));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Identifier:abc
        Question
        Expressions
            Expression
                Identifier:def"));
        }

        [Test]
        public void IdentifierQuestionExpressionColonExpressionStatement()
        {
            Parser parser = new Parser();

            var actual = NodesToString(parser.Parse("{abc?{def}:{ghi}}"));

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Identifier:abc
        Question
        Expressions
            Expression
                Identifier:def
        Colon
        Expressions
            Expression
                Identifier:ghi"));
        }

        [Test]
        public void TextStatement()
        {
            Parser parser = new Parser();

            var node = parser.Parse("abc{def}");
            var actual = NodesToString(node);

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Text:abc
    Expression
        Identifier:def"));
        }

        [Test]
        public void StatementTextStatement()
        {
            Parser parser = new Parser();

            var node = parser.Parse("{abc} {def}");
            var actual = NodesToString(node);

            Assert.That(actual, Is.EqualTo(@"
Expressions
    Expression
        Identifier:abc
    Expression
        Text: 
    Expression
        Identifier:def"));
        }

        [Test]
        public void LeftCurly_ExpectedIdentifier()
        {
            Parser parser = new Parser();

            string message = "";
            try
            {
                parser.Parse("{");
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
            Parser parser = new Parser();

            string message = "";
            try
            {
                parser.Parse("{}");
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
            Parser parser = new Parser();

            string message = "";
            try
            {
                parser.Parse("{abc");
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