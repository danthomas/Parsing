using System.Collections.Generic;
using System.Diagnostics;

namespace Parsing
{
    public class Node
    {
        public NodeType NodeType { get; set; }
        public string Text { get; set; }

        [DebuggerStepThrough]
        public Node(NodeType nodeType, string text = "")
        {
            NodeType = nodeType;
            Text = text;
            Children = new List<Node>();
        }

        public List<Node> Children { get; set; }

        public override string ToString()
        {
            return NodeType + (Text == "" ? "" : ":" + Text);
        }
    }
}
