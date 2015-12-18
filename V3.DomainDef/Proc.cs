using System.Linq;
using V3.Parsing.Core;

namespace V3.DomainDef
{
    public class Proc
    {
        public Proc(Node<NodeType> node)
        {
            var identifierNodes = node.Nodes.Where(x => x.NodeType == NodeType.Identifier).ToArray();
            var procsNode = node.Nodes.SingleOrDefault(x => x.NodeType == NodeType.Procs);

            Name = identifierNodes[0].Text;

            if (procsNode != null)
            {
                Props = procsNode
                    .Nodes
                    .Where(x => x.NodeType == NodeType.Identifier)
                    .Select(x => x.Text)
                    .ToArray();
            }
        }

        public string[] Props { get; set; }

        public string Name { get; set; }
    }
}