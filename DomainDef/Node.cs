using System.Collections.Generic;

namespace DomainDef
{
    public class Node
    {
        public Node Parent { get; set; }
        public TokenType TokenType { get; set; }
        public string Text { get; set; }
        public List<Node> Children { get; set; }

        private Node(Node parent, TokenType tokenType, string text = "")
        {
            Parent = parent;
            TokenType = tokenType;
            Text = text ?? "";
            Children = new List<Node>();
        }

        public Node(TokenType tokenType)
        {
            TokenType = tokenType;
            Children = new List<Node>();
        }

        public Node AddChild(TokenType tokenType, string text = "")
        {
            Node child = new Node(this, tokenType, text);
            Children.Add(child);
            return child;
        }

        public override string ToString()
        {
            return TokenType + (Text == "" ? "" : ":" + Text);
        }
    }
}