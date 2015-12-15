using System.Collections.Generic;
using System.Linq;
using System.Text;
using V3.Parsing.Core;

namespace V3.DomainDef
{
    public class Domain
    {
        public Domain(Node<NodeType> node)
        {
            Name = node.Nodes.Single(x => x.NodeType == NodeType.Identifier).Text;
            Entities = node.Nodes.Where(x => x.NodeType == NodeType.Entity).Select(x => new Entity(x)).ToList();
        }
        
        public string Name { get; set; }

        public List<Entity> Entities { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("domain ");
            stringBuilder.Append(Name);

            foreach (var entity in Entities)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("entity ");
                stringBuilder.Append(entity.Name);
            }

            return stringBuilder.ToString();
        }
    }

    public class Entity
    {
        public Entity(Node<NodeType> node)
        {
            Name = node.Nodes.Single(x => x.NodeType == NodeType.Identifier).Text;
        }

        public string Name { get; set; }
    }
}