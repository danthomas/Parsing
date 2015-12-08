using V3.Parsing.Core;

namespace V3.Templates
{
    /*
    Expr : TextOrSubExpr*
    TextOrSubExpr : text | SubExpr
    SubExpr : openCurly identifier [equals Values] [Then] closeCurly
    Then : then SubExprOrValues [Else]
    Else : else SubExprOrValues
    SubExprOrValues : SubExpr | ValueOrDollar+
    ValueOrDollar : value | dollar
    Values : value [or value]*

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
                Consume(parent, NodeType.Text);
            }
            else if (AreNodeTypes(NodeType.OpenCurly))
            {
                SubExpr(parent);
            }
        }

        private void SubExpr(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.SubExpr);

            Consume(child, NodeType.OpenCurly, false);
            Consume(child, NodeType.Identifier);
            if (AreNodeTypes(NodeType.Equals))
            {
                Consume(child, NodeType.Equals);
                Values(child);
            }
            if (AreNodeTypes(NodeType.Then))
            {
                Then(child);
            }
            Consume(child, NodeType.CloseCurly, false);
        }

        private void Then(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Then);

            Consume(child, NodeType.Then, false);

            SubExprOrValues(child);

            if (AreNodeTypes(NodeType.Else))
            {
                Else(child);
            }
        }

        private void Else(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Else);

            Consume(child, NodeType.Else, false);

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

            Consume(child, NodeType.Value);
            while (AreNodeTypes(NodeType.Or))
            {
                Consume(child, NodeType.Or, false);
                Consume(child, NodeType.Value);
            }
        }

        private void ValueOrDollar(Node<NodeType> parent)
        {
            if (AreNodeTypes(NodeType.Value))
            {
                Consume(parent, NodeType.Value);
            }
            else if (AreNodeTypes(NodeType.Dollar))
            {
                Consume(parent, NodeType.Dollar);
            }
        }

        public override bool CaseSensitive => false;
    }
}
