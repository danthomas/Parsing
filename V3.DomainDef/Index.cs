using System.Linq;
using V3.Parsing.Core;

namespace V3.DomainDef
{
    public class Index
    {
        public Index(Node<NodeType> node)
        {
            Props = node.Nodes.Where(x => x.NodeType == NodeType.Identifier).Select(x => x.Text).ToArray();

            Unique = node.Nodes.Any(x => x.NodeType == NodeType.Unique);
        }

        public bool Unique { get; set; }

        public string[] Props { get; set; }
    }
}