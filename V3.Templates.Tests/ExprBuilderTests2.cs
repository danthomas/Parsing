using System;
using System.Linq;
using NUnit.Framework;

namespace V3.Templates.Tests
{
    [TestFixture]
    public class ExprBuilderTests2
    {
        [Test]
        public void Whitespace()
        {
            var actual = Build("  { abc }   { def = DEF ?  xxx  :  yyy  }  ");

            Assert.That(actual, Is.EqualTo(@"
Text-  
Conditional-abc!=null?
	Attr-abc
Text-   
Conditional-def= DEF ?
	Text-  xxx  :
	Text-  yyy  
Text-  "));
        }

        [Test]
        public void Text()
        {
            var actual = Build("abc");

            Assert.That(actual, Is.EqualTo(@"
Text-abc"));
        }

        [Test]
        public void Attr()
        {
            var actual = Build("{abc}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc!=null?
	Attr-abc"));
        }

        [Test]
        public void MultipleAttr()
        {
            var actual = Build("{abc}{def}{ghi}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc!=null?
	Attr-abc
Conditional-def!=null?
	Attr-def
Conditional-ghi!=null?
	Attr-ghi"));
        }

        [Test]
        public void MultipleAttrAndText()
        {
            var actual = Build("{abc}xxx{def}yyy{ghi}zzz");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc!=null?
	Attr-abc
Text-xxx
Conditional-def!=null?
	Attr-def
Text-yyy
Conditional-ghi!=null?
	Attr-ghi
Text-zzz"));
        }

        [Test]
        public void MultiValue()
        {
            var actual = Build("{abc=xxx|yyy|abc?XXX:YYY}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc=xxx|yyy|abc?
	Text-XXX:
	Text-YYY"));
        }

        [Test]
        public void AttrRegex()
        {
            var actual = Build("{abc(A[+]*)}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc!=null?
	Attr-abc(A[+]*)"));
        }

        [Test]
        public void ConditionalTextElseText()
        {
            var actual = Build("{abc=ABC?xxx:yyy}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc=ABC?
	Text-xxx:
	Text-yyy"));
        }

        [Test]
        public void ConditionalTextElseExpr()
        {
            var actual = Build("{abc=ABC?xxx:{def=DEF?yyy:zzz}}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc=ABC?
	Text-xxx:
	Conditional-def=DEF?
		Text-yyy:
		Text-zzz"));
        }

        [Test]
        public void ConditionalExprElseText()
        {
            var actual = Build("{abc=ABC?{def=DEF?yyy:zzz}:xxx}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc=ABC?
	Conditional-def=DEF?
		Text-yyy:
		Text-zzz:
	Text-xxx"));
        }

        [Test]
        public void MultiNestedConditions()
        {
            var actual = Build("{abc=ABC?{def=DEF?uuu:vvv}:{ghi=GHI?www:xxx}:yyy}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc=ABC?
	Conditional-def=DEF?
		Text-uuu:
		Text-vvv:
	Conditional-ghi=GHI?
		Text-www:
		Text-xxx
	Text-yyy"));
        }

        [Test]
        public void MultiNestedConditionsWithAttr()
        {
            var actual = Build("{abc=ABC?{def=DEF?{uuu}:{vvv}}:{ghi=GHI?{www}:{xxx}}:yyy}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc=ABC?
	Conditional-def=DEF?
		Conditional-uuu!=null?
			Attr-uuu:
		Conditional-vvv!=null?
			Attr-vvv:
	Conditional-ghi=GHI?
		Conditional-www!=null?
			Attr-www:
		Conditional-xxx!=null?
			Attr-xxx
	Text-yyy"));
        }

        [Test]
        public void Text_ConditionalPlaceHolder()
        {
            var actual = Build("abc{def=ghi?$}");

            Assert.That(actual, Is.EqualTo(@"
Text-abc
Conditional-def=ghi?
	Conditional-def!=null?
		Attr-def"));
        }

        [Test]
        public void ConditionalTextAndPlaceHolder()
        {
            var actual = Build("{abc=ABC?xxx$yyy}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc=ABC?
	Text-xxx
	Conditional-abc!=null?
		Attr-abc
	Text-yyy"));
        }

        [Test]
        public void Text_ConditionalPlaceHolderRegex()
        {
            var actual = Build("abc{def=ghi?$(A[+]*)}");

            Assert.That(actual, Is.EqualTo(@"
Text-abc
Conditional-def=ghi?
	Conditional-def!=null?
		Attr-def(A[+]*)"));
        }

        [Test]
        public void ConditionalDifferentAttr()
        {
            var actual = Build("abc{def=ghi?{jkl}}");

            Assert.That(actual, Is.EqualTo(@"
Text-abc
Conditional-def=ghi?
	Conditional-jkl!=null?
		Attr-jkl"));
        }

        [Test]
        public void ConditionalDifferentAttrRegex()
        {
            var actual = Build("abc{def=ghi?{jkl(A[+]*)}}");

            Assert.That(actual, Is.EqualTo(@"
Text-abc
Conditional-def=ghi?
	Conditional-jkl!=null?
		Attr-jkl(A[+]*)"));
        }

        [Test]
        public void ConditionaAttrAndText()
        {
            var actual = Build("{abc?xxx{def}yyy}");

            Assert.That(actual, Is.EqualTo(@"
Conditional-abc!=null?
	Text-xxx
	Conditional-def!=null?
		Attr-def
	Text-yyy"));
        }

        [Test]
        public void ConditionalTextAndMultipleAttr()
        {
            var actual = Build("abc{def=ghi?xxx{jkl}yyy{mno}zzz}");

            Assert.That(actual, Is.EqualTo(@"
Text-abc
Conditional-def=ghi?
	Text-xxx
	Conditional-jkl!=null?
		Attr-jkl
	Text-yyy
	Conditional-mno!=null?
		Attr-mno
	Text-zzz"));
        }

        [Test]
        public void Complex()
        {
            var actual = Build("xxx{abc=ABC?$}yy");

            Assert.That(actual, Is.EqualTo(@"
Text-xxx
Conditional-abc=ABC?
	Conditional-abc!=null?
		Attr-abc
Text-yy"));
        }

        private string Build(string text)
        {
            var tokeniser = new Parser();
            var builder = new ExprBuilder();

            var block = builder.Build(tokeniser.Parse(text));

            return ExprToString(block);
        }

        private string ExprToString(BlockExpr block)
        {
            string actual = "";

            ExprToString(block, 0, ref actual);

            return actual;
        }

        private void ExprToString(BlockExpr block, int indent, ref string actual)
        {
            foreach (var expr in block.Exprs)
            {
                TextExpr textExpr = expr as TextExpr;
                ConditionalExpr conditionalExpr = expr as ConditionalExpr;
                AttrExpr attrExpr = expr as AttrExpr;
                BlockExpr blockExpr = expr as BlockExpr;

                if (textExpr != null)
                {
                    actual += Environment.NewLine + new string('\t', indent) + "Text-" + textExpr.Text;
                }
                else if (conditionalExpr != null)
                {
                    actual += Environment.NewLine + new string('\t', indent) + "Conditional-" + conditionalExpr.Attr + conditionalExpr.Operator + String.Join("|", conditionalExpr.Values.Select(x => x ?? "null")) + "?";
                    ExprToString(conditionalExpr.TrueExpr, indent + 1, ref actual);
                    if (conditionalExpr.FalseExpr != null)
                    {
                        actual += ":";
                        ExprToString(conditionalExpr.FalseExpr, indent + 1, ref actual);
                    }
                }
                else if (attrExpr != null)
                {
                    actual += Environment.NewLine + new string('\t', indent) + "Attr-" + attrExpr.Name + (attrExpr.Regex == null ? "" : "(" + attrExpr.Regex + ")");
                }
                else if (blockExpr != null)
                {
                    ExprToString(blockExpr, indent + 1, ref actual);
                }
            }
        }
    }
}