using System;
using System.Text;
using NUnit.Framework;
using V3.Parsing.Core;

namespace V3.Templates.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Complex()
        {
          var root =  Parse("xxx {  abc  =def|ghi|jkl?{ab=cd?de$fg}:{hi=jk?lm$:no$pq}} yyy", @"
Expr
    Text : xxx 
    SubExpr
        Identifier : abc
        Values
            Value : def
            Value : ghi
            Value : jkl
        Then
            SubExpr
                Identifier : ab
                Values
                    Value : cd
                Then
                    Value : de
                    Dollar : $
                    Value : fg
            Else
                SubExpr
                    Identifier : hi
                    Values
                        Value : jk
                    Then
                        Value : lm
                        Dollar : $
                        Else
                            Value : no
                            Dollar : $
                            Value : pq
    Text :  yyy");

            var expr = new ExprBuilder().Build(root);
        }

        [Test]
        public void AttrThenSubExp()
        {
            Parse("{abc?{def}}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            SubExpr
                Identifier : def");
        }


        [Test]
        public void SimpleText()
        {
            Parse("abc", @"
Expr
    Text : abc");
        }

        [Test]
        public void AttrEqualsValue()
        {
            Parse("{abc=def ghi}", @"
Expr
    SubExpr
        Identifier : abc
        Values
            Value : def ghi");
        }

        [Test]
        public void AttrEqualsMultiValue()
        {
            Parse("{abc=def|ghi|jkl}", @"
Expr
    SubExpr
        Identifier : abc
        Values
            Value : def
            Value : ghi
            Value : jkl");
        }

        [Test]
        public void AttrThenValue()
        {
            Parse("{abc?def ghi}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            Value : def ghi");
        }

        [Test]
        public void AttrThenValueElseValue()
        {
            Parse("{abc?def def:ghi ghi}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            Value : def def
            Else
                Value : ghi ghi");
        }

        [Test]
        public void AttrThenDollarValue()
        {
            Parse("{abc?$def}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            Dollar : $
            Value : def");
        }

        [Test]
        public void AttrThenValueDollar()
        {
            Parse("{abc?def$}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            Value : def
            Dollar : $");
        }

        [Test]
        public void AttrThenValueDollarValue()
        {
            Parse("{abc?def$ghi}", @"
Expr
    SubExpr
        Identifier : abc
        Then
            Value : def
            Dollar : $
            Value : ghi");
        }

        [Test]
        public void AttrEqualsValueThenValueElseValue()
        {
            Parse("{abc=abc def?def$def:ghi$ghi}", @"
Expr
    SubExpr
        Identifier : abc
        Values
            Value : abc def
        Then
            Value : def
            Dollar : $
            Value : def
            Else
                Value : ghi
                Dollar : $
                Value : ghi");
        }

        [Test]
        public void SimpleSubExpr()
        {
            Parse("{abc}", @"
Expr
    SubExpr
        Identifier : abc");
        }

        [Test]
        public void TextWithWhiteSpace()
        {
            Parse("abc def", @"
Expr
    Text : abc def");
        }

        [Test]
        public void SimpleTextSubExprText()
        {
            Parse("abc{def}ghi", @"
Expr
    Text : abc
    SubExpr
        Identifier : def
    Text : ghi");
        }

        private Node<NodeType> Parse(string text, string expected)
        {
            Parser parser = new Parser();

            var root = parser.Parse(text);

            var actual = NodeToString(root);

            Assert.That(actual, Is.EqualTo(expected));

            return root;
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
