using System;
using System.Linq;

namespace Parsing
{
    public class Expander
    {
        public Node Expand(Node node)
        {
            Walk(node, ExpandDollar);
            Walk(node, ExpandIdentifier);
            Walk(node, ExpandIdentifierQuestion);
            Walk(node, ExpandIdentifierEqualToValue);

            return node;
        }

        private void ExpandDollar(Node node)
        {
            if (node.NodeType == NodeType.Dollar)
            {
                node.NodeType = NodeType.Attrib;

                Node parent = node.Parent;
                string text = "";

                while (parent != null)
                {
                    Node identifier = parent.Children.FirstOrDefault(x => x.NodeType == NodeType.Identifier);

                    if (identifier != null)
                    {
                        text = identifier.Text;
                        break;
                    }

                    parent = parent.Parent;
                }
                
                node.Text = text;
            }
        }

        private void ExpandIdentifier(Node node)
        {
            if (node.Children.Count == 1 && node.Children[0].NodeType == NodeType.Identifier)
            {
                //identifier -> identifier != null ? attrib
                node.Children.Add(new Node(node, NodeType.NotEqualTo));
                node.Children.Add(new Node(node, NodeType.Null));
                node.Children.Add(new Node(node, NodeType.Question));
                node.Children.Add(new Node(node, NodeType.Expressions, "",
                    new Node(null, NodeType.Expression, "",
                    new Node(null, NodeType.Attrib, node.Children[0].Text))));
            }
        }

        private void ExpandIdentifierQuestion(Node node)
        {
            if (node.Children.Count > 2
                && node.Children[0].NodeType == NodeType.Identifier
                && node.Children[1].NodeType == NodeType.Question)
            {
                //identifier?expression -> identifier != null ? expression

                node.Children.Insert(1, new Node(node, NodeType.NotEqualTo));
                node.Children.Insert(2, new Node(node, NodeType.Null));
            }
        }

        private void ExpandIdentifierEqualToValue(Node node)
        {
            if (node.Children.Count ==3
                && node.Children[0].NodeType == NodeType.Identifier
                && (node.Children[1].NodeType == NodeType.EqualTo || node.Children[1].NodeType == NodeType.NotEqualTo)
                && node.Children[2].NodeType == NodeType.Values)
            {
                //identifier=text -> identifier=text?{identifier}
                node.Children.Add(new Node(node, NodeType.Question));
                
                node.Children.Add(new Node(node, NodeType.Expressions, "", 
                    new Node(null, NodeType.Expression, "", 
                    new Node(null, NodeType.Attrib, node.Children[0].Text))));
            }
        }

        private void Walk(Node parent, Action<Node> action)
        {
            foreach (Node child in parent.Children)
            {
                action(child);
                Walk(child, action);
            }
        }
    }
}
