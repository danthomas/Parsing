using V3.Parsing.Core;

namespace V3.Templates
{
    /*
    Expr : TextOrSubExpr*
    TextOrSubExpr : text | SubExpr
    SubExpr : openCurly whitespace identifier whitespace [equals Values] [Then] closeCurly
    
    Then : then SubExprOrValues [Else]
    Else : else SubExprOrValues
    SubExprOrValues : SubExpr | ValueOrDollar+
    ValueOrDollar : value | dollar
    Values : value [or value]*

    whitespace : '^[\ ]+$'
    text : '^[^{}]+$'
    identifier : ''
    value : ''
    openCurly : '{'
    closeCurly : '}'
    equals : '='
    or : '|'
    dollar : '$'
    */
    public class Parser : ParserBase<NodeType>
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
            if (AreNodeTypes(NodeType.Whitespace))
            {
                Whitespace(child);
            }
            if (AreNodeTypes(NodeType.Equals))
            {
                Consume(NodeType.Equals);
                Values(child);
            }
            if (AreNodeTypes(NodeType.Then))
            {
                Then(child);
            }
            Consume(NodeType.CloseCurly);
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

            SubExprOrValues(child);

            if (AreNodeTypes(NodeType.Else))
            {
                Else(child);
            }
        }

        private void Else(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Else);

            Consume(NodeType.Else);

            SubExprOrValues(child);
        }

        private void SubExprOrValues(Node<NodeType> parent)
        {
            if (AreNodeTypes(NodeType.OpenCurly))
            {
                SubExpr(parent);
            }
            else if (AreNodeTypes(NodeType.Value)
                || AreNodeTypes(NodeType.Dollar))
            {
                do
                {
                    ValueOrDollar(parent);
                } while (AreNodeTypes(NodeType.Value)
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

        private void ValueOrDollar(Node<NodeType> parent)
        {
            if (AreNodeTypes(NodeType.Value))
            {
                Consume(NodeType.Value, parent);
            }
            else if (AreNodeTypes(NodeType.Dollar))
            {
                Consume(NodeType.Dollar, parent);
            }
        }

        public override bool CaseSensitive => false;
    }
}
