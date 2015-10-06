using System.Collections.Generic;

namespace Parsing.Core
{
    public class Node<N>
    {
        public N NodeType { get; set; }
        public string Text { get; set; }
        public List<Node<N>> Children { get; set; }

        public Node(N nodeType, string text = "")
        {
            NodeType = nodeType;
            Text = text;
            Children = new List<Node<N>>();
        }

        public Node<N> AddNode(N nodeType, string text)
        {
            var node = new Node<N>(nodeType, text);
            Children.Add(node);
            return node;
        }

        public Node<N> AddNode(N nodeType)
        {
            var node = new Node<N>(nodeType);
            Children.Add(node);
            return node;
        }
    }
}