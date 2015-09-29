using System.Collections.Generic;
using NUnit.Framework;

namespace Parsing.Tests
{
    [TestFixture]
    class BuidlerTests
    {
        [Test]
        public void Text()
        {
            var actual = Run("text");

            Assert.That(actual, Is.EqualTo(@"text"));
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
            var actual = Run("{abc=xxx}");

            Assert.That(actual, Is.EqualTo(@"xxx"));
        }

        [Test]
        public void AttrNotEqualToValue()
        {
            var actual = Run("{abc!=zzz}");

            Assert.That(actual, Is.EqualTo(@"xxx"));
        }

        [Test]
        public void AttrFormatted()
        {
            var actual = Run("{mno?$Kg}");

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

        private static string Run(string template)
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
            };

            var actual = builder.Build(expander.Expand(parser.Parse(template)))(values);
            return actual;
        }
    }
}