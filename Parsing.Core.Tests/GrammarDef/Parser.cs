using System;
using System.Collections.Generic;
using Parsing.Core;

namespace Xxx
{
    public class Parser : ParserBase<TokenType, NodeType>
    {
        private List<string> _discard;

        public Parser() : base(new Lexer())
        {
            _discard = new List<string>();
        }

        public override List<string> DiscardThings { get { return _discard; } }
        
        public override Node<NodeType> Root()
        {
            Node<NodeType> root = new Node<NodeType>(null, NodeType.Statement);

            Statement(root);

            return root;
        }

        public void Statement(Node<NodeType> parent)
        {
            Consume(parent, TokenType.Select, NodeType.Select);
            StarOrFieldList(parent);
        }

        public void StarOrFieldList(Node<NodeType> parent)
        {
            if (IsTokenType(TokenType.Star))
            {
                Consume(parent, TokenType.Star, NodeType.Star);
            }
            else
            {
                var fieldList = Add(parent, NodeType.FieldList);
                while(IsTokenType(TokenType.Text))
                {
                    FieldList(parent);
                }
            }
        }

        public void FieldList(Node<NodeType> parent)
        {
            Field(parent);
        }

        public void Field(Node<NodeType> parent)
        {
            Consume(parent, TokenType.Text, NodeType.Text);
        }
    }

    public enum NodeType
    {
        Statement,
        StarOrFieldList,
        FieldList,
        Field,
        Text,
        Select,
        Star,
        Comma,
    }

    public class Walker
    {
        public string NodesToString(object node)
        {
            Node<NodeType> parent = node as Node<NodeType>;

            string ret = "";

            NodesToString(parent, ref ret, 0);

            return ret;
        }

        private void NodesToString(Node<NodeType> parent, ref string ret, int indent)
        {
            ret += Environment.NewLine + new string(' ', indent) + parent.NodeType +(String.IsNullOrWhiteSpace(parent.Text) ? "" : " - " + parent.Text);
            foreach (Node<NodeType> child in parent.Children)
            {
                NodesToString(child, ref ret, indent + 1);
            }
        }
    }
}