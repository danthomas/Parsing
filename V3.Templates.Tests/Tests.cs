using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using V3.Parsing.Core;

namespace V3.Templates.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void SimpleRegex()
        {
            Test("{abc([+])}", @"
Expr
    SubExpr
        Identifier : abc
        Regex
            Text : [+]", @"
BlockExpr : 
    ConditionalExpr : abc != 
         Then BlockExpr : 
            AttrExpr : abc");
        }

        [Test]
        public void SimpleText()
        {
            Test("abc", @"
Expr
    Text : abc", @"
BlockExpr : 
    TextExpr : abc", @"abc");
        }

        [Test]
        public void Complex()
        {
            Test("xxx {  abc  =def|ghi|jkl?{ab=cd?de$fg}:{hi=jk?lm$:no$pq}} yyy", @"
Expr
    Text : xxx 
    SubExpr
        Identifier : abc
        Equals : =
        Values
            Value : def
            Value : ghi
            Value : jkl
        Then
            SubExpr
                Identifier : ab
                Equals : =
                Values
                    Value : cd
                Then
                    Text : de
                    Dollar : $
                    Text : fg
            Else
                SubExpr
                    Identifier : hi
                    Equals : =
                    Values
                        Value : jk
                    Then
                        Text : lm
                        Dollar : $
                        Else
                            Text : no
                            Dollar : $
                            Text : pq
    Text :  yyy", @"
BlockExpr : 
    TextExpr : xxx 
    ConditionalExpr : abc = def|ghi|jkl
         Then BlockExpr : 
            ConditionalExpr : ab = cd
                 Then BlockExpr : 
                    TextExpr : de
                    AttrExpr : ab
                    TextExpr : fg
         Else BlockExpr : 
            ConditionalExpr : hi = jk
                 Then BlockExpr : 
                    TextExpr : lm
                    AttrExpr : hi
                 Else BlockExpr : 
                    TextExpr : no
                    AttrExpr : hi
                    TextExpr : pq
    TextExpr :  yyy");
        }

        [Test]
        public void AttrThenSubExp()
        {
            Test("{abc?{def}}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            SubExpr
                Identifier : def", @"
BlockExpr : 
    ConditionalExpr : abc != 
         Then BlockExpr : 
            ConditionalExpr : def != 
                 Then BlockExpr : 
                    AttrExpr : def", "DEF");
        }

        [Test]
        public void AttrEqualsValue()
        {
            Test("{abc=def ghi}", @"
Expr
    SubExpr
        Identifier : abc
        Equals : =
        Values
            Value : def ghi", @"
BlockExpr : 
    ConditionalExpr : abc = def ghi
         Then BlockExpr : 
            AttrExpr : abc");
        }

        [Test]
        public void AttrEqualsMultiValue()
        {
            Test("{abc=def|ghi|jkl}", @"
Expr
    SubExpr
        Identifier : abc
        Equals : =
        Values
            Value : def
            Value : ghi
            Value : jkl", @"
BlockExpr : 
    ConditionalExpr : abc = def|ghi|jkl
         Then BlockExpr : 
            AttrExpr : abc");
        }

        [Test]
        public void AttrThenValue()
        {
            Test("{abc?def ghi}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            Text : def ghi", @"
BlockExpr : 
    ConditionalExpr : abc != 
         Then BlockExpr : 
            TextExpr : def ghi");
        }

        [Test]
        public void AttrThenValueElseValue()
        {
            Test("{abc?def def:ghi ghi}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            Text : def def
            Else
                Text : ghi ghi", @"
BlockExpr : 
    ConditionalExpr : abc != 
         Then BlockExpr : 
            TextExpr : def def
         Else BlockExpr : 
            TextExpr : ghi ghi");
        }

        [Test]
        public void AttrThenDollarValue()
        {
            Test("{abc?$def}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            Dollar : $
            Text : def", @"
BlockExpr : 
    ConditionalExpr : abc != 
         Then BlockExpr : 
            AttrExpr : abc
            TextExpr : def");
        }

        [Test]
        public void AttrThenValueDollar()
        {
            Test("{abc?def$}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            Text : def
            Dollar : $", @"
BlockExpr : 
    ConditionalExpr : abc != 
         Then BlockExpr : 
            TextExpr : def
            AttrExpr : abc");
        }

        [Test]
        public void AttrThenValueDollarValue()
        {
            Test("{abc?def$ghi}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            Text : def
            Dollar : $
            Text : ghi", @"
BlockExpr : 
    ConditionalExpr : abc != 
         Then BlockExpr : 
            TextExpr : def
            AttrExpr : abc
            TextExpr : ghi");
        }

        [Test]
        public void AttrEqualsValueThenValueElseValue()
        {
            Test("{abc=abc def?def$def:ghi$ghi}", @"
Expr
    SubExpr
        Identifier : abc
        Equals : =
        Values
            Value : abc def
        Then
            Text : def
            Dollar : $
            Text : def
            Else
                Text : ghi
                Dollar : $
                Text : ghi", @"
BlockExpr : 
    ConditionalExpr : abc = abc def
         Then BlockExpr : 
            TextExpr : def
            AttrExpr : abc
            TextExpr : def
         Else BlockExpr : 
            TextExpr : ghi
            AttrExpr : abc
            TextExpr : ghi");
        }

        [Test]
        public void SimpleSubExpr()
        {
            Test("{abc}", @"
Expr
    SubExpr
        Identifier : abc", @"
BlockExpr : 
    ConditionalExpr : abc != 
         Then BlockExpr : 
            AttrExpr : abc");
        }

        [Test]
        public void TextWithWhiteSpace()
        {
            Test("abc def", @"
Expr
    Text : abc def", @"
BlockExpr : 
    TextExpr : abc def");
        }

        [Test]
        public void SimpleTextSubExprText()
        {
            Test("abc{def}ghi", @"
Expr
    Text : abc
    SubExpr
        Identifier : def
    Text : ghi", @"
BlockExpr : 
    TextExpr : abc
    ConditionalExpr : def != 
         Then BlockExpr : 
            AttrExpr : def
    TextExpr : ghi");
        }

        private void Test(string text, string expectedNode, string expectedExpr, string expected = null)
        {
            Parser parser = new Parser();

            var root = parser.Parse(text);

            var actualNode = NodeToString(root);

            Assert.That(actualNode, Is.EqualTo(expectedNode));


            var expr = new ExprBuilder().Build(root);

            var actualExpr = ExprToString(expr);

            Assert.That(actualExpr, Is.EqualTo(expectedExpr));

            Dictionary<string, string> attribs = new Dictionary<string, string>();

            attribs.Add("abc", "ABC");
            attribs.Add("def", "DEF");
            attribs.Add("ghi", "GHI");
            attribs.Add("jkl", "JKL");
            attribs.Add("mno", "MNO");

            string actual = new FuncBuilder().Build(text)(attribs);

            if (expected!= null)
            {
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        private string ExprToString(BlockExpr expr)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ExprToString(expr, stringBuilder);
            return stringBuilder.ToString();
        }

        private void ExprToString(ExprBase expr, StringBuilder stringBuilder, int indent = 0, string prefix = "")
        {
            var textExpr = expr as TextExpr;
            var attrExpr = expr as AttrExpr;
            var blockExpr = expr as BlockExpr;
            var conditionalExpr = expr as ConditionalExpr;

            stringBuilder.AppendLine();
            stringBuilder.Append(new string(' ', indent * 4));
            stringBuilder.Append(prefix);
            stringBuilder.Append(expr.GetType().Name);
            stringBuilder.Append(" : ");

            if (textExpr != null)
            {
                stringBuilder.Append(textExpr.Text);
            }
            else if (attrExpr != null)
            {
                stringBuilder.Append(attrExpr.Name);
            }
            else if (conditionalExpr != null)
            {
                stringBuilder.Append(conditionalExpr.Attr);
                stringBuilder.Append(" ");
                stringBuilder.Append(conditionalExpr.Operator);
                stringBuilder.Append(" ");
                stringBuilder.Append(String.Join("|", conditionalExpr.Values));

                ExprToString(conditionalExpr.TrueExpr, stringBuilder, indent + 1, " Then ");
                if (conditionalExpr.FalseExpr != null)
                {
                    ExprToString(conditionalExpr.FalseExpr, stringBuilder, indent + 1, " Else ");
                }
            }
            else if (blockExpr != null)
            {
                foreach (var exprBase in blockExpr.Exprs)
                {
                    ExprToString(exprBase, stringBuilder, indent + 1);
                }
            }
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
            stringBuilder.Append(new string(' ', indent * 4));
            stringBuilder.Append(parent.NodeType);
            if (!String.IsNullOrWhiteSpace(parent.Text))
            {
                stringBuilder.Append(" : ");
                stringBuilder.Append(parent.Text);
            }
            foreach (Node<NodeType> child in parent.Nodes)
            {
                NodeToString(child, stringBuilder, indent + 1);
            }
        }

    }
}
