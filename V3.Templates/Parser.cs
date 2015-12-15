using System;
using V3.Parsing.Core;

namespace V3.Templates
{
    /*
    Expr : TextOrSubExpr*
    TextOrSubExpr : text | SubExpr
    SubExpr : openCurly whitespace identifier [Regex] whitespace [EqualsOrNotEquals Values] [Then] closeCurly
    Regex : openBrace text closeBrace
    EqualsOrNotEquals : equals | notEquals
    Then : then SubExprOrTexts* [Else]
    Else : else SubExprOrTexts*
    SubExprOrTexts : SubExpr | TextOrDollar+
    TextOrDollar : text | dollar
    Values : value [or value]*

    whitespace : '^[\ ]+$'
    text : '^[^{}]+$'
    identifier : ''
    value : ''
    openCurly : '{'
    closeCurly : '}'
    openBrace : '('
    closeBrace : ')'
    equals : '='
    notEquals : '!='
    or : '|'
    dollar : '$'
    */
    public class Parser : ParserBase<NodeType>
    {
        public Parser()
            : base(new Lexer())
        {
            IgnoreChars = new char[0];
            CaseSensitive = false;
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
                Consume(NodeType.Text, parent);
            }
            else if (AreNodeTypes(NodeType.OpenCurly))
            {
                SubExpr(parent);
            }
        }

        private void SubExpr(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.SubExpr);

            Consume(NodeType.OpenCurly);
            if (AreNodeTypes(NodeType.Whitespace))
            {
                Whitespace(child);
            }
            Consume(NodeType.Identifier, child);
            if (AreNodeTypes(NodeType.OpenBrace))
            {
                Regex(child);
            }
            if (AreNodeTypes(NodeType.Whitespace))
            {
                Whitespace(child);
            }
            if (AreNodeTypes(NodeType.Equals) 
                || AreNodeTypes(NodeType.NotEquals))
            {
                EqualsOrNotEquals(child);
                Values(child);
            }
            if (AreNodeTypes(NodeType.Then))
            {
                Then(child);
            }
            Consume(NodeType.CloseCurly);
        }

        private void Regex(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Regex);

            Consume(NodeType.OpenBrace);
            Consume(NodeType.Text, child);
            Consume(NodeType.CloseBrace);
        }

        private void EqualsOrNotEquals(Node<NodeType> parent)
        {
            if (AreNodeTypes(NodeType.Equals))
            {
                Consume(NodeType.Equals, parent);
            }
            else if (AreNodeTypes(NodeType.NotEquals))
            {
                Consume(NodeType.NotEquals, parent);
            }
        }

        private void Whitespace(Node<NodeType> child)
        {
            if (AreNodeTypes(NodeType.Whitespace))
            {
                Consume(NodeType.Whitespace);
            }
        }

        private void Then(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Then);

            Consume(NodeType.Then);

            while (AreNodeTypes(NodeType.OpenCurly)
                || AreNodeTypes(NodeType.Text)
                || AreNodeTypes(NodeType.Dollar))
            {
                SubExprOrTexts(child);
            }

            if (AreNodeTypes(NodeType.Else))
            {
                Else(child);
            }
        }

        private void Else(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Else);

            Consume(NodeType.Else);

            SubExprOrTexts(child);
        }

        private void SubExprOrTexts(Node<NodeType> parent)
        {
            if (AreNodeTypes(NodeType.OpenCurly))
            {
                SubExpr(parent);
            }
            else if (AreNodeTypes(NodeType.Text)
                || AreNodeTypes(NodeType.Dollar))
            {
                do
                {
                    TextOrDollar(parent);
                } while (AreNodeTypes(NodeType.Text)
                         || AreNodeTypes(NodeType.Dollar));
            }
        }

        private void Values(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Values);

            Consume(NodeType.Value, child);
            while (AreNodeTypes(NodeType.Or))
            {
                Consume(NodeType.Or);
                Consume(NodeType.Value, child);
            }
        }

        private void TextOrDollar(Node<NodeType> parent)
        {
            if (AreNodeTypes(NodeType.Text))
            {
                Consume(NodeType.Text, parent);
            }
            else if (AreNodeTypes(NodeType.Dollar))
            {
                Consume(NodeType.Dollar, parent);
            }
        }

        public override char[] IgnoreChars { get; }
        public override bool CaseSensitive { get; }
    }
}
