using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TitleBuilder.Tests
{
    [TestFixture]
    class BuilderTests
    {

        [Test]
        public void AttrTrimmed()
        {
            var actual = Run("{ abc }");

            Assert.That(actual, Is.EqualTo(@"xxx"));
        }

        [Test]
        public void AttrWithSpacesTrimmed()
        {
            var actual = Run("{ v w x }");

            Assert.That(actual, Is.EqualTo(@"789"));
        }
        
        [Test]
        public void TextNotTrimmed()
        {
            var actual = Run(" text ");

            Assert.That(actual, Is.EqualTo(@" text "));
        }

        [Test]
        public void AttrWithValue()
        {
            var actual = Run("{abc}");

            Assert.That(actual, Is.EqualTo(@"xxx"));
        }

        [Test]
        public void AttrWithNullValue()
        {
            var actual = Run("{stu}");

            Assert.That(actual, Is.EqualTo(@""));
        }

        [Test]
        public void AttrWithEmptyStringValue()
        {
            var actual = Run("{pqr}");

            Assert.That(actual, Is.EqualTo(@""));
        }

        [Test]
        public void AttrEqualToValue()
        {
            var actual = Run("{ abc = xxx }");

            Assert.That(actual, Is.EqualTo(@"xxx"));
        }

        [Test]
        public void AttrNotEqualToValue()
        {
            var actual = Run("{abc != zzz}");

            Assert.That(actual, Is.EqualTo(@"xxx"));
        }

        [Test]
        public void AttrFormatted()
        {
            var actual = Run("{mno ?$Kg}");

            Assert.That(actual, Is.EqualTo(@"456Kg"));
        }

        [Test]
        public void AttrDiffAttrFormatted()
        {
            var actual = Run("{abc?{mno}Kg}");

            Assert.That(actual, Is.EqualTo(@"456Kg"));
        }

        [Test]
        public void IfThenElse_True()
        {
            var actual = Run("{abc?QQQ:RRR}");

            Assert.That(actual, Is.EqualTo(@"QQQ"));
        }

        [Test]
        public void IfThenElse_False()
        {
            var actual = Run("{stu?QQQ:RRR}");

            Assert.That(actual, Is.EqualTo(@"RRR"));
        }

        [Test]
        public void IfExpressionThenElse_True()
        {
            var actual = Run("{abc=XXX?QQQ:RRR}");

            Assert.That(actual, Is.EqualTo(@"QQQ"));
        }

        [Test]
        public void IfExpressionThenElse_False()
        {
            var actual = Run("{abc!=XXX?QQQ:RRR}");

            Assert.That(actual, Is.EqualTo(@"RRR"));
        }

        [Test]
        public void IfExpressionThenExpressionElseExpression_True()
        {
            var actual = Run("{abc!=zzz?{def=yyy?{jkl}}:{ghi=zzz?{mno}}}");

            Assert.That(actual, Is.EqualTo(@"123"));
        }

        [Test]
        public void IfExpressionThenExpressionElseExpression_False()
        {
            var actual = Run("{abc!=xxx?{def=yyy?{jkl}}:{ghi=zzz?{mno}}}");

            Assert.That(actual, Is.EqualTo(@"456"));
        }

        [Test]
        public void MultipleNested()
        {
            var actual = Run("{abc?{def?{ghi?{jkl?{mno}}}}}");

            Assert.That(actual, Is.EqualTo(@"456"));
        }

        [Test]
        public void MultipleValues()
        {
            var actual = Run("{abc=xxx|yyy|zzz} {def=xxx|yyy|zzz}");

            Assert.That(actual, Is.EqualTo(@"xxx yyy"));
        }

        private string Run(string template)
        {
            Builder builder = new Builder();
            Expander expander = new Expander();
            Parser parser = new Parser();

            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"abc", "xxx"},
                {"def", "yyy"},
                {"ghi", "zzz"},
                {"jkl", "123"},
                {"mno", "456"},
                {"pqr", ""},
                {"stu", null},
                {"v w x", "789"},
            };

            var parsed = parser.Parse(template);
            var parsedText = NodesToString(parsed);
            var expanded = expander.Expand(parsed);
            var expandedText = NodesToString(expanded);
            var actual = builder.Build(expanded)(values);
            return actual;
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
                    child.TokenType +
                    (child.Text == "" ? "" : ":" + child.Text);

                NodesToString(child, indent + 1, ref ret);
            }
        }
    }
}