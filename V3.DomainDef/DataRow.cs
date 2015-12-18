using System.Linq;
using V3.Parsing.Core;

namespace V3.DomainDef
{
    public class DataRow
    {
        public DataRow(Node<NodeType> node)
        {
            Values = node.Nodes.Select(x => x.Text).ToArray();
        }

        public string[] Values { get; set; }
    }
}