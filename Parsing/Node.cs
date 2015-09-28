using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Parsing
{
    public class Node
    {
        public Node Parent { get; set; }
        public NodeType NodeType { get; set; }
        public string Text { get; set; }

        [DebuggerStepThrough]
        public Node(Node parent, NodeType nodeType, string text = "", params Node[] children)
        {
            Parent = parent;
            NodeType = nodeType;
            Text = text;
            foreach (var child in children)
            {
                child.Parent = this;
            }
            Children = children.ToList();
        }

        public List<Node> Children { get; set; }

        public override string ToString()
        {
            return NodeType + (Text == "" ? "" : ":" + Text);
        }
    }
}
