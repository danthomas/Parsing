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


            bool firstEntity = true;

            foreach (var entity in Entities)
            {
                stringBuilder.AppendLine();
                if (!firstEntity)
                {
                    stringBuilder.AppendLine();
                }
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
                    else if (prop.Type == "datetime")
                    {
                        if (!String.IsNullOrWhiteSpace(prop.DatetimePrecision))
                        {
                            stringBuilder.Append($"({prop.DatetimePrecision})");
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

                    if (prop.Auto)
                    {
                        stringBuilder.Append(" auto");
                    }

                    first = false;
                }

                if (entity.Indexes.Any())
                {
                    first = true;
                    stringBuilder.AppendLine();
                    stringBuilder.Append("    indexes");

                    foreach (var index in entity.Indexes)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.Append(first ? "      " : "    , ");
                        if (index.Unique)
                        {
                            stringBuilder.Append("unique ");
                        }
                        stringBuilder.Append($"({String.Join(" ", index.Props)})");

                        first = false;
                    }
                }

                first = true;

                if (entity.Procs.Any())
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append("    procs");
                    foreach (var proc in entity.Procs)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.Append(first ? "      " : "    , ");
                        stringBuilder.Append(proc.Name);

                        if (proc.Props != null
                            && proc.Props.Any())
                        {
                            stringBuilder.Append($" ({String.Join(" ", proc.Props)})");
                        }

                        first = false;
                    }
                }

                first = true;

                if (entity.Tasks.Any())
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append("    tasks");
                    foreach (var task in entity.Tasks)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.Append(first ? "      " : "    , ");
                        stringBuilder.Append(task.Name);

                        if (!String.IsNullOrWhiteSpace(task.Text))
                        {
                            stringBuilder.Append($" {task.Text}");
                        }

                        if (task.SelectedIds != SelectedIds.Unspecified)
                        {
                            string selectedIds = task.SelectedIds.ToString();
                            selectedIds = Char.ToLower(selectedIds[0]) + selectedIds.Substring(1);
                            stringBuilder.Append($" {selectedIds}");
                        }

                        first = false;
                    }
                }

                if (entity.DataRows != null)
                {
                    foreach (var dataRow in entity.DataRows)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.Append($"    [{String.Join(", ", dataRow.Values)}]");
                    }
                }
                firstEntity = false;
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

            Indexes = node
                .Nodes
                .Where(x => x.NodeType == NodeType.Index)
                .Select(x => new Index(x))
                .ToList();

            Procs = node
                .Nodes
                .Where(x => x.NodeType == NodeType.Proc)
                .Select(x => new Proc(x))
                .ToList();

            Tasks = node
                .Nodes
                .Where(x => x.NodeType == NodeType.Task)
                .Select(x => new Task(x))
                .ToList();

            var data = node.Nodes.SingleOrDefault(x => x.NodeType == NodeType.DataRows);

            if (data != null)
            {
                DataRows = data
                    .Nodes
                    .Where(x => x.NodeType == NodeType.DataRow)
                    .Select(x => new DataRow(x))
                    .ToList();
            }
        }

        public List<Index> Indexes { get; set; }

        public List<Task> Tasks { get; set; }

        public List<Proc> Procs { get; set; }

        public bool Enum { get; set; }

        public string Group { get; set; }

        public List<Prop> Props { get; set; }

        public string Name { get; set; }

        public List<DataRow> DataRows { get; set; }
    }

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

    public enum SelectedIds
    {
        Unspecified = 0,
        Zero = 1,
        One = 2,
        Two = 4,
        More = 8,
        OneOrMore = One | Two | More,
        TwoOrMore = Two | More,
        Any = Zero | One | Two | More
    }

    public class Prop
    {
        public Prop(Node<NodeType> node)
        {
            var identifierNodes = node.Nodes.Where(x => x.NodeType == NodeType.Identifier).ToArray();

            Name = identifierNodes[0].Text;

            if (identifierNodes.Length == 1)
            {
                var typeNode = node.Nodes.Single(x => x.NodeType == NodeType.Type);

                Type = typeNode.Nodes[0].Text;

                var numberNodes = typeNode.Nodes.Where(x => x.NodeType == NodeType.Number).ToArray();
                if (numberNodes.Length == 1)
                {
                    MaxLength = Int32.Parse(numberNodes[0].Text);
                }
                else if (numberNodes.Length == 2)
                {
                    MinLength = Int32.Parse(numberNodes[0].Text);
                    MaxLength = Int32.Parse(numberNodes[1].Text);
                }

                if (typeNode.Nodes.Any(x => x.NodeType == NodeType.Hour))
                {
                    DatetimePrecision = "h";
                }
                else if (typeNode.Nodes.Any(x => x.NodeType == NodeType.Minute))
                {
                    DatetimePrecision = "m";
                }
                else if (typeNode.Nodes.Any(x => x.NodeType == NodeType.Second))
                {
                    DatetimePrecision = "s";
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

            Auto = node.Nodes.Any(x => x.NodeType == NodeType.Auto);
        }

        public bool Auto { get; set; }

        public string DatetimePrecision { get; set; }

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