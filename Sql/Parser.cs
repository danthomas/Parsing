using System;
using Parsing.Core;

namespace Sql
{
    public class Parser : ParserBase<TokenType, NodeType>
    {
        public Parser() : base(new Lexer())
        {
        }

        public override Node<NodeType> Root()
        {
            Node<NodeType> root = new Node<NodeType>(null, NodeType.SelectStatement);

            SelectStatement(root);

            return root;
        }

        public void SelectStatement(Node<NodeType> parent)
        {
            Consume(parent, TokenType.Select, NodeType.Select);
            if (IsTokenType(TokenType.Top))
            {
                var topX = Add(parent, NodeType.TopX);
                TopX(topX);
            }
            if (IsTokenType(TokenType.Distinct))
            {
                Consume(parent, TokenType.Distinct, NodeType.Distinct);
            }
            SelectFields(parent);
            Consume(parent, TokenType.From, NodeType.From);
            Table(parent);
            while (IsTokenType(TokenType.Inner, TokenType.Left, TokenType.Right, TokenType.Outer, TokenType.Join))
            {
                Node<NodeType> join = Add(parent, NodeType.JoinDef);
                JoinDef(join);
            }
        }

        private void TopX(Node<NodeType> parent)
        {
            Consume(parent, TokenType.Top, NodeType.Top);
            Consume(parent, TokenType.Integer, NodeType.Integer);
        }

        private void JoinDef(Node<NodeType> parent)
        {
            if (IsTokenType(TokenType.Inner))
            {
                Consume(parent, TokenType.Inner, NodeType.Inner);
            }
            else if (IsTokenType(TokenType.Left))
            {
                Consume(parent, TokenType.Left, NodeType.Left);
            }
            else if (IsTokenType(TokenType.Right))
            {
                Consume(parent, TokenType.Right, NodeType.Right);
            }

            if (IsTokenType(TokenType.Outer))
            {
                Consume(parent, TokenType.Outer, NodeType.Outer);
            }

            Consume(parent, TokenType.Join, NodeType.Join);

            Table(parent);

            Consume(parent, TokenType.On, NodeType.On);
            ObjectRef(parent);
            Consume(parent, TokenType.EqualTo, NodeType.EqualTo);
            ObjectRef(parent);
        }

        private void ObjectRef(Node<NodeType> parent)
        {
            Node<NodeType> objectRef = Add(parent, NodeType.ObjectRef);

            Consume(objectRef, TokenType.Text, NodeType.Text);
            if (IsTokenType(TokenType.Dot))
            {
                Consume(objectRef, TokenType.Dot, NodeType.Dot);
                Consume(objectRef, TokenType.Text, NodeType.Text);
            }
            if (IsTokenType(TokenType.Dot))
            {
                Consume(objectRef, TokenType.Dot, NodeType.Dot);
                Consume(objectRef, TokenType.Text, NodeType.Text);
            }
        }

        private void Table(Node<NodeType> parent)
        {
            var table = Add(parent, NodeType.Table);

            if (IsTokenType(TokenType.Text))
            {
                Consume(table, TokenType.Text, NodeType.Text);

                if (IsTokenType(TokenType.As))
                {
                    Consume(table, TokenType.As, NodeType.As);
                }

                if (IsTokenType(TokenType.Text))
                {
                    Consume(table, TokenType.Text, NodeType.Text);
                }
            }
        }

        private void SelectFields(Node<NodeType> parent)
        {
            var selectFields = Add(parent, NodeType.SelectFields);

            if (IsTokenType(TokenType.Star))
            {
                Consume(selectFields, TokenType.Star, NodeType.Star);
            }
            else if (IsTokenType(TokenType.Text, TokenType.Count, TokenType.Min, TokenType.Max))
            {
                SelectField(selectFields);
                while (IsTokenType(TokenType.Comma))
                {
                    Consume(TokenType.Comma);
                    SelectField(selectFields);
                }
            }
        }

        private void SelectField(Node<NodeType> parent)
        {
            var selectField = Add(parent, NodeType.SelectField);

            if (IsTokenType(TokenType.Text))
            {
                ObjectRef(selectField);
            }
            else if (IsTokenType(TokenType.Count))
            {
                Consume(selectField, TokenType.Count, NodeType.Count);
                Consume(selectField, TokenType.OpenParen, NodeType.OpenParen);
                Field(selectField);
                Consume(selectField, TokenType.CloseParen, NodeType.CloseParen);
            }
            else if (IsTokenType(TokenType.Max, TokenType.Min))
            {
                if (IsTokenType(TokenType.Max))
                {
                    Consume(selectField, TokenType.Max, NodeType.Max);
                }
                else if (IsTokenType(TokenType.Min))
                {
                    Consume(selectField, TokenType.Min, NodeType.Min);
                }
                Consume(selectField, TokenType.OpenParen, NodeType.OpenParen);
                Field(selectField);
                Consume(selectField, TokenType.CloseParen, NodeType.CloseParen);
            }

            if (IsTokenType(TokenType.As))
            {
                Consume(selectField, TokenType.As, NodeType.As);
            }

            if (IsTokenType(TokenType.Text))
            {
                Consume(selectField, TokenType.Text, NodeType.Text);
            }
        }

        private void Field(Node<NodeType> parent)
        {
            var field = Add(parent, NodeType.Field);

            if (IsTokenType(TokenType.Text))
            {
                ObjectRef(field);
            }
            else if (IsTokenType(TokenType.Star))
            {
                Consume(field, TokenType.Star, NodeType.Star);
            }
            else
            {
                throw new Exception($"Unexpected {_currentToken.TokenType}");
            }
        }
    }

    public enum NodeType
    {
        SelectStatement,
        TopX,
        SelectFields,
        SelectField,
        SelectField2,
        Field,
        ObjectRef,
        Table,
        JoinDef,
        Integer,
        Text,
        Select,
        Top,
        Distinct,
        Star,
        Count,
        OpenParen,
        Dot,
        CloseParen,
        Min,
        Max,
        As,
        Comma,
        From,
        Inner,
        Left,
        Right,
        Outer,
        Join,
        On,
        EqualTo,
    }
}
