using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Node<NodeType> root = new Node<NodeType>(NodeType.Root);

            Select(root);

            return root;
        }

        public void Select(Node<NodeType> root)
        {
            if (IsTokenType(TokenType.Select))
            {
                Consume(root, TokenType.Select, NodeType.Select);

                if (IsTokenType(TokenType.Top))
                {
                    TopX(root);
                }

                if (IsTokenType(TokenType.Distinct))
                {
                    Consume(root, TokenType.Distinct, NodeType.Distinct);
                }

                SelectFields(root);
                Consume(root, TokenType.From, NodeType.From);
                Table(root);
                Join(root);
            }
        }

        private void TopX(Node<NodeType> root)
        {
            var topX = Add(root, NodeType.TopX);
            Consume(topX, TokenType.Top, NodeType.Top);
            Consume(topX, TokenType.Integer, NodeType.Integer);
        }

        private void Join(Node<NodeType> root)
        {
            while (IsTokenType(TokenType.Inner, TokenType.Left, TokenType.Right, TokenType.Outer, TokenType.Join))
            {
                Node<NodeType> join = Add(root, NodeType.Join);
                if (IsTokenType(TokenType.Inner))
                {
                    Consume(join, TokenType.Inner, NodeType.Inner);
                }
                else if (IsTokenType(TokenType.Left))
                {
                    Consume(join, TokenType.Left, NodeType.Left);
                }
                else if (IsTokenType(TokenType.Right))
                {
                    Consume(join, TokenType.Right, NodeType.Right);
                }

                if (IsTokenType(TokenType.Outer))
                {
                    Consume(join, TokenType.Outer, NodeType.Outer);
                }

                Consume(join, TokenType.Join, NodeType.Join);

                Table(join);

                Consume(join, TokenType.On, NodeType.On);
                ObjectRef(join);
                Consume(join, TokenType.EqualTo, NodeType.EqualTo);
                ObjectRef(join);
            }
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

        private void Table(Node<NodeType> root)
        {
            var table = Add(root, NodeType.Table);

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

        private void SelectFields(Node<NodeType> root)
        {
            var selectFields = Add(root, NodeType.SelectFields);

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

        private void SelectField(Node<NodeType> fields)
        {
            var selectField = Add(fields, NodeType.SelectField);

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
        EndOfFile,
        Text,
        Comma,
        Dot,
        OpenParen,
        CloseParen,
        Select,
        Star,
        From,
        Where,
        Order,
        OpenSquare,
        CloseSquare,
        Whitespace,
        String,
        Inner,
        Left,
        Right,
        Outer,
        Cross,
        Join,
        By,
        On,
        As,
        EqualTo,
        With,
        Nolock,
        Like,
        Count,
        Min,
        Max,
        Root,
        SelectFields,
        SelectField,
        Field,
        Table,
        Distinct,
        ObjectRef,
        Top,
        Integer,
        TopX
    }
}
