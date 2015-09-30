using System.Collections.Generic;

namespace DomainDef
{
    public class Node
    {
        public Node Parent { get; set; }
        public NodeType NodeType { get; set; }
        public string Text { get; set; }
        public List<Node> Children { get; set; }

        public Node(Node parent, NodeType nodeType, string text = "")
        {
            Parent = parent;
            NodeType = nodeType;
            Text = text ?? "";
            Children = new List<Node>();
        }

        public Node(NodeType nodeType)
        {
            NodeType = nodeType;
        }

        public Node AddChild(NodeType nodeType, string text = "")
        {
            Node child = new Node(this, nodeType, text);
            Children.Add(child);
            return child;
        }

        public override string ToString()
        {
            return NodeType + (Text == "" ? "" : ":" + Text);
        }
    }
}