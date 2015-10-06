using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing.Core.Grammar
{
    public class Parser : ParserBase<TokenType, NodeType>
    {
        /*
        Grammar : Rows
        Rows : Row [newLine Row]*
        Row : text colon Element+ 
        Element : text[star|plus]
        */
        public Parser() : base(new Lexer())
        {
        }

        public override Node<NodeType> Root()
        {
            Node<NodeType> root = new Node<NodeType>(NodeType.Root);

            Grammar(root);

            return root;
        }

        private void Grammar(Node<NodeType> root)
        {
            Rows(root);
        }

        private void Rows(Node<NodeType> root)
        {
            while (IsTokenType(TokenType.Text))
            {
                Row(root);
            }
        }

        private void Row(Node<NodeType> parent)
        {
            var row = Add(parent, NodeType.Row);
            Consume(row, TokenType.Text, NodeType.Text);
            Consume(row, TokenType.Colon, NodeType.Colon);
            Elements(row);
        }

        private void Elements(Node<NodeType> row)
        {
            while (IsTokenType(TokenType.Text))
            {
                var element = Consume(row, TokenType.Element, NodeType.Element);
                Consume(element, TokenType.Text, NodeType.Text);
                if (IsTokenType(TokenType.Plus))
                {
                    Consume(element, TokenType.Plus, NodeType.Plus);
                }
                else if (IsTokenType(TokenType.Star))
                {
                    Consume(element, TokenType.Star, NodeType.Star);
                }
            }
        }
    }

    public enum NodeType
    {
        Root,
        Text,
        Row,
        Colon,
        Plus,
        Star,
        Element
    }
}
