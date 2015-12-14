using System.Collections.Generic;

namespace V3.Parsing.Core
{
    public class Node<N>
    {
        public Node(N nodeType, string text = null)
        {
            NodeType = nodeType;
            Text = text;
            Nodes = new List<Node<N>>();
        }

        public N NodeType { get; set; }
        public string Text { get; set; }
        public List<Node<N>> Nodes { get; set; }
    }
}