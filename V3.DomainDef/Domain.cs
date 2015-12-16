using System;
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
            Name = node
                .Nodes
                .Single(x => x.NodeType == NodeType.Identifier).Text;

            Entities = node
                .Nodes
                .Where(x => x.NodeType == NodeType.Entity)
                .Select(x => new Entity(x))
                .ToList();
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
                if (!String.IsNullOrWhiteSpace(entity.Group))
                {
                    stringBuilder.Append($"{entity.Group}.");
                }
                stringBuilder.Append(entity.Name);

                if (entity.Enum)
                {
                    stringBuilder.Append(" enum");
                }

                bool first = true;
                foreach (var prop in entity.Props)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append(first ? "      " : "    , ");
                    stringBuilder.Append(prop.Name);
                    stringBuilder.Append($" {prop.Type}");

                    if (prop.Type == "string")
                    {
                        if (prop.MaxLength > 0)
                        {
                            if (prop.MinLength > 0)
                            {
                                stringBuilder.Append($"({prop.MinLength}, {prop.MaxLength})");
                            }
                            else
                            {
                                stringBuilder.Append($"({prop.MaxLength})");
                            }
                        }
                    }

                    if (prop.Null)
                    {
                        stringBuilder.Append(" null");
                    }

                    if (prop.Unique)
                    {
                        stringBuilder.Append(" unique");
                    }

                    if (prop.Readonly)
                    {
                        stringBuilder.Append(" readonly");
                    }

                    first = false;
                }

                if (entity.DataRows != null)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append("data");
                    foreach (var dataRow in entity.DataRows)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.Append($"    [{String.Join(", ", dataRow.Values)}]");
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }

    public class Entity
    {
        public Entity(Node<NodeType> node)
        {
            var identifierNodes = node.Nodes.Where(x => x.NodeType == NodeType.Identifier).ToArray();

            if (identifierNodes.Length == 1)
            {
                Name = identifierNodes[0].Text;
            }
            else if (identifierNodes.Length == 2)
            {
                Group = identifierNodes[0].Text;
                Name = identifierNodes[1].Text;
            }
            else
            {
                throw new Exception("Failed to set name of Entity");
            }

            Enum = node.Nodes.Any(x => x.NodeType == NodeType.Enum);

            Props = node
                .Nodes
                .Where(x => x.NodeType == NodeType.Prop)
                .Select(x => new Prop(x))
                .ToList();

            var data = node.Nodes.SingleOrDefault(x => x.NodeType == NodeType.Data);

            if (data != null)
            {
                DataRows = data
                    .Nodes
                    .Where(x => x.NodeType == NodeType.DataRow)
                    .Select(x => new DataRow(x))
                    .ToList();
            }
        }


        public bool Enum { get; set; }

        public string Group { get; set; }

        public List<Prop> Props { get; set; }

        public string Name { get; set; }

        public List<DataRow> DataRows { get; set; }
    }

    public class Prop
    {
        public Prop(Node<NodeType> node)
        {
            var identifierNodes = node.Nodes.Where(x => x.NodeType == NodeType.Identifier).ToArray();

            Name = identifierNodes[0].Text;

            if (identifierNodes.Length == 1)
            {
                Type = node.Nodes.Single(x => x.NodeType == NodeType.Type).Nodes[0].Text;

                var numberNodes = node.Nodes.Single(x => x.NodeType == NodeType.Type).Nodes.Where(x => x.NodeType == NodeType.Number).ToArray();
                if (numberNodes.Length == 1)
                {
                    MaxLength = Int32.Parse(numberNodes[0].Text);
                }
                else if (numberNodes.Length == 2)
                {
                    MinLength = Int32.Parse(numberNodes[0].Text);
                    MaxLength = Int32.Parse(numberNodes[1].Text);
                }
            }
            else if (identifierNodes.Length == 2)
            {
                Type = identifierNodes[1].Text;
            }
            else
            {
                throw new Exception("Failed to set Type of Entity");
            }

            Null = node.Nodes.Any(x => x.NodeType == NodeType.Null);

            Unique = node.Nodes.Any(x => x.NodeType == NodeType.Unique);

            Readonly = node.Nodes.Any(x => x.NodeType == NodeType.Readonly);
        }

        public bool Readonly { get; set; }

        public bool Unique { get; set; }

        public bool Null { get; set; }

        public int MinLength { get; set; }

        public int MaxLength { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }
    }

    public class DataRow
    {
        public DataRow(Node<NodeType> node)
        {
            Values = node.Nodes.Select(x => x.Text).ToArray();
        }

        public string[] Values { get; set; }
    }
}