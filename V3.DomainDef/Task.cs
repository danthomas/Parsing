using System.Linq;
using V3.Parsing.Core;

namespace V3.DomainDef
{
    public class Task
    {
        public Task(Node<NodeType> node)
        {
            Name = node.Nodes.Single(x => x.NodeType == NodeType.Identifier).Text;

            Text = node.Nodes.SingleOrDefault(x => x.NodeType == NodeType.Literal) == null
                ? ""
                : node.Nodes.Single(x => x.NodeType == NodeType.Literal).Text;

            if (node.Nodes.SingleOrDefault(x => x.NodeType == NodeType.One) != null)
            {
                SelectedIds = SelectedIds.One;
            }
            else if (node.Nodes.SingleOrDefault(x => x.NodeType == NodeType.OneOrMore) != null)
            {
                SelectedIds = SelectedIds.OneOrMore;
            }
        }

        public SelectedIds SelectedIds { get; set; }

        public string Text { get; set; }

        public string Name { get; set; }
    }
}