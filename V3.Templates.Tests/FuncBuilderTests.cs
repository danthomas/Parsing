using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace V3.Templates.Tests
{
    [TestFixture]
    public class FuncBuilderTests
    {
        [Test]
        public void Text()
        {
            var actual = Build("xxx");

            Assert.That(actual, Is.EqualTo(@"xxx"));
        }

        [Test]
        public void Attr()
        {
            var actual = Build("{abc}");

            Assert.That(actual, Is.EqualTo(@"ABC"));
        }

        [Test]
        public void AttrIsEqualTo()
        {
            var actual = Build("{abc=ABC?XXX}");

            Assert.That(actual, Is.EqualTo(@"XXX"));
        }

        [Test]
        public void AttrIsNotEqualTo()
        {
            var actual = Build("{abc!=DEF?XXX}");

            Assert.That(actual, Is.EqualTo(@"XXX"));
        }

        [Test]
        public void TextAttrText()
        {
            var actual = Build("xxx{abc}yyy");

            Assert.That(actual, Is.EqualTo(@"xxxABCyyy"));
        }

        [Test]
        public void AttrAttrAttr()
        {
            var actual = Build("{abc}{def}{ghi}");

            Assert.That(actual, Is.EqualTo(@"ABCDEF"));
        }

        [Test]
        public void ConditionalText()
        {
            var actual = Build("{abc=ABC?XXX}{abc!=DEF?YYY}");

            Assert.That(actual, Is.EqualTo(@"XXXYYY"));
        }

        [Test]
        public void ConditionalPlaceholder()
        {
            var actual = Build("{abc=ABC?$}");

            Assert.That(actual, Is.EqualTo(@"ABC"));
        }

        [Test]
        public void ConditionalTextAndPlaceholder()
        {
            var actual = Build("{abc=ABC?xxx$yyy}");

            Assert.That(actual, Is.EqualTo(@"xxxABCyyy"));
        }

        [Test]
        public void ConditionalTextAndOtherAttr()
        {
            var actual = Build("{abc=ABC?xxx{def}yyy}");

            Assert.That(actual, Is.EqualTo(@"xxxDEFyyy"));
        }

        [Test]
        public void Regex()
        {
            var actual = Build("{mno(A[+]*)}");

            Assert.That(actual, Is.EqualTo(@"A+++"));
        }

        [Test]
        public void Complex1()
        {
            var actual = Build("1{abc}2{abc=XXX?$}3{abc=ABC?{def}}4");

            Assert.That(actual, Is.EqualTo(@"1ABC23DEF4"));
        }

        [Test]
        public void Complex2()
        {
            var actual = Build("xxx{abc=ABC?{def}}yyy");

            Assert.That(actual, Is.EqualTo(@"xxxDEFyyy"));
        }

        [Test]
        public void Complex3()
        {
            var actual = Build("{ghi?XXX}{mno(A[+]*)}{def=DEF?{ghi}}");

            Assert.That(actual, Is.EqualTo(@"A+++"));
        }

        [Test]
        public void EqualToMultiValue()
        {
            var actual = Build("{abc=MMM|NNN|ABC?XXX:YYY}");

            Assert.That(actual, Is.EqualTo(@"XXX"));
        }

        [Test]
        public void NotEqualToMultiValueTrue()
        {
            var actual = Build("{abc!=MMM|NNN|OOO?XXX:YYY}");

            Assert.That(actual, Is.EqualTo(@"XXX"));
        }

        [Test]
        public void NotEqualToMultiValueFalse()
        {
            var actual = Build("{abc!=MMM|ABC|OOO?XXX:YYY}");

            Assert.That(actual, Is.EqualTo(@"YYY"));
        }

        [Test]
        public void TrimWhitespace()
        {
            var actual = Build("  { abc }   { def = DEF ?  xxx  :  yyy  }  ");

            Assert.That(actual, Is.EqualTo(@"ABC yyy"));
        }

        [Test]
        public void FixCommas()
        {
            var actual = Build(",,  { abc },   { def = DEF ? , xxx  : , yyy  },,  qwerty");

            Assert.That(actual, Is.EqualTo(@", ABC, yyy, qwerty"));
        }

        [Test]
        public void FixCommas2()
        {
            var actual = Build("  { abc }, ,, ,, ,, ,  ,,, { def },, ,,, , qwerty");

            Assert.That(actual, Is.EqualTo(@"ABC, DEF, qwerty"));
        }

        [Test]
        public void FixCommas3()
        {
            var actual = Build("a,, ,,,, , ,, ,, ,,,, , , ,, ,,, , b,, , , ,, ,, , ,,,,,,, ,, , , c    , , , , ,, , , ,, ,,, , ,  , , , ,, , d,, ,  ,  , , ,,,, e");

            Assert.That(actual, Is.EqualTo(@"a, b, c, d, e"));
        }

        private string Build(string template)
        {
            Dictionary<string, string> attrs = new Dictionary<string, string>
            {
                {"abc", "ABC"},
                {"def", "DEF"},
                {"ghi", null},
                {"jkl", ""},
                {"mno", "A+++ (-10%)"}
            };

            Func<Dictionary<string, string>, string> func = new FuncBuilder().Build(template);

            return func(attrs);
        }
    }
}