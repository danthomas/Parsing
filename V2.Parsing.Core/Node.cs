using System;
using System.Collections.Generic;

namespace V2.Parsing.Core
{
    public class Node<N>
    {
        public Node<N> Parent { get; set; }
        public N NodeType { get; set; }
        public string Text { get; set; }
        public List<Node<N>> Children { get; set; }

        public Node(Node<N> parent, N nodeType, string text = "")
        {
            Parent = parent;
            NodeType = nodeType;
            Text = text;
            Children = new List<Node<N>>();
        }

        public Node<N> AddNode(N nodeType, string text)
        {
            var node = new Node<N>(this, nodeType, text);
            Children.Add(node);
            return node;
        }

        public Node<N> AddNode(N nodeType)
        {
            var node = new Node<N>(this, nodeType);
            Children.Add(node);
            return node;
        }

        public override string ToString()
        {
            return NodeType + (String.IsNullOrWhiteSpace(Text) ? "" : " - " + Text);
        }

        public Node<N> AddNode(Node<N> node)
        {
            node.Parent = this;
            Children.Add(node);
            return node;
        }

        public void RemoveNode(Node<N> node)
        {
            node.Parent = null;
            Children.Remove(node);
        }
    }
}