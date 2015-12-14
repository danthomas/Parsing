using System.Collections.Generic;
using V2.Parsing.Core;

namespace Titles
{
    public class Parser : ParserBase<TokenType, NodeType>
    {
        public Parser() : base(new Lexer())
        {
            base.Discard = new List<string>
            {
            };
        }

        public override Node<NodeType> Root()
        {
            Node<NodeType> root = new Node<NodeType>(null, NodeType.Root);

            return Expr(root);
        }

        public Node<NodeType> Expr(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.Expr);


            while (IsTokenType(TokenType.Text, TokenType.OpenCurly))
            {
                TextOrSubExpr(child);
            }

            return child;
        }

        public Node<NodeType> TextOrSubExpr(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.TextOrSubExpr);


            if (AreTokenTypes(TokenType.Text))
            {
                Consume(child, TokenType.Text, NodeType.Text);
            }
            else if (AreTokenTypes(TokenType.OpenCurly))
            {
                SubExpr(child);
            }

            return child;
        }

        public Node<NodeType> SubExpr(Node<NodeType> parent)
        {
            var child = Add(parent, NodeType.SubExpr);

            Consume(child, TokenType.OpenCurly, NodeType.OpenCurly);
            Consume(child, TokenType.Text, NodeType.Text);
            Consume(child, TokenType.CloseCurly, NodeType.CloseCurly);

            return child;
        }
    }
}