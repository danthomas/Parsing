using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using V3.Parsing.Core;

namespace V3.DomainDef
{
    public class Domain
    {
        private readonly string[] _types = { "bool", "byte", "short", "int", "long", "datetime", "date", "decimal", "string" };
        private readonly string[] _idTypes = { "byte", "short", "int", "long" };

        public Domain(Node<NodeType> domainNode)
        {
            Name = domainNode
                .Nodes
                .Single(x => x.NodeType == NodeType.Identifier).Text;

            var entityNodes = domainNode
                .Nodes
                .Where(x => x.NodeType == NodeType.Entity)
                .ToArray();

            Entities = new List<Entity>();

            foreach (var entityNode in entityNodes)
            {
                GroupAndName(entityNode, (group, name) =>
                {
                    bool @enum = entityNode.Nodes.Any(x => x.NodeType == NodeType.Enum);

                    Entities.Add(new Entity(group, name, @enum));
                });
            }

            foreach (var entityNode in entityNodes)
            {
                GroupAndName(entityNode, (group, entityName) =>
                {
                    var entity = Entities.Single(x => x.Group == group && x.Name == entityName);

                    var propNode = entityNode
                        .Nodes
                        .First(x => x.NodeType == NodeType.Prop);

                    var identifierNodes = propNode.Nodes.Where(x => x.NodeType == NodeType.Identifier).ToArray();

                    string name = identifierNodes[0].Text;

                    var typeNode = propNode.Nodes.Single(x => x.NodeType == NodeType.Type);

                    var type = typeNode.Nodes[0].Text;

                    var auto = propNode.Nodes.Any(x => x.NodeType == NodeType.Auto);

                    var @readonly = propNode.Nodes.Any(x => x.NodeType == NodeType.Readonly);

                    if (_idTypes.Contains(type))
                    {
                        entity.Props.Add(new ValueProp(name, type, auto, @readonly));
                    }
                    else
                    {
                        throw new Exception($"Failed to find Entity {type}");
                    }
                });
            }

            foreach (var entityNode in entityNodes)
            {
                GroupAndName(entityNode, (group, entityName) =>
                {
                    var entity = Entities.Single(x => x.Group == group && x.Name == entityName);

                    foreach (var propNode in entityNode
                        .Nodes
                        .Where(x => x.NodeType == NodeType.Prop)
                        .Skip(1))
                    {
                        var identifierNodes = propNode.Nodes.Where(x => x.NodeType == NodeType.Identifier).ToArray();

                        string name = identifierNodes[0].Text;
                        string type;
                        int minLength = 0;
                        int maxLength = 0;
                        string dateTimePrecision = "";

                        if (identifierNodes.Length == 1)
                        {
                            var typeNode = propNode.Nodes.Single(x => x.NodeType == NodeType.Type);

                            type = typeNode.Nodes[0].Text;

                            var numberNodes = typeNode.Nodes.Where(x => x.NodeType == NodeType.Number).ToArray();
                            if (numberNodes.Length == 1)
                            {
                                maxLength = Int32.Parse(numberNodes[0].Text);
                            }
                            else if (numberNodes.Length == 2)
                            {
                                minLength = Int32.Parse(numberNodes[0].Text);
                                maxLength = Int32.Parse(numberNodes[1].Text);
                            }

                            if (typeNode.Nodes.Any(x => x.NodeType == NodeType.Hour))
                            {
                                dateTimePrecision = "h";
                            }
                            else if (typeNode.Nodes.Any(x => x.NodeType == NodeType.Minute))
                            {
                                dateTimePrecision = "m";
                            }
                            else if (typeNode.Nodes.Any(x => x.NodeType == NodeType.Second))
                            {
                                dateTimePrecision = "s";
                            }
                        }
                        else if (identifierNodes.Length == 2)
                        {
                            type = identifierNodes[1].Text;
                        }
                        else
                        {
                            throw new Exception("Failed to set Type of Entity");
                        }

                        var @null = propNode.Nodes.Any(x => x.NodeType == NodeType.Null);

                        var unique = propNode.Nodes.Any(x => x.NodeType == NodeType.Unique);

                        var @readonly = propNode.Nodes.Any(x => x.NodeType == NodeType.Readonly);

                        var auto = propNode.Nodes.Any(x => x.NodeType == NodeType.Auto);

                        if (_types.Contains(type))
                        {
                            entity.Props.Add(new ValueProp(name, type, dateTimePrecision, @null, unique, @readonly, auto, minLength, maxLength));
                        }
                        else
                        {
                            var refEntity = Entities.SingleOrDefault(x => x.Name == type);

                            if (refEntity == null)
                            {
                                throw new Exception($"Failed to find Entity {type}");
                            }

                            entity.Props.Add(new RefProp(name, refEntity, @null, unique, @readonly));
                        }
                    }
                });
            }

            foreach (var entityNode in entityNodes)
            {
                GroupAndName(entityNode, (group, name) =>
                {
                    var entity = Entities.Single(x => x.Group == group && x.Name == name);

                    entity.Indexes = entityNode
                        .Nodes
                        .Where(x => x.NodeType == NodeType.Index)
                        .Select(x => new Index(x))
                        .ToList();

                    entity.Procs = entityNode
                        .Nodes
                        .Where(x => x.NodeType == NodeType.Proc)
                        .Select(x => new Proc(x))
                        .ToList();

                    entity.Tasks = entityNode
                        .Nodes
                        .Where(x => x.NodeType == NodeType.Task)
                        .Select(x => new Task(x))
                        .ToList();

                    entity.DataRows = entityNode
                        .Nodes.Single(x => x.NodeType == NodeType.DataRows)
                        .Nodes
                        .Where(x => x.NodeType == NodeType.DataRow)
                        .Select(x => new DataRow(x))
                        .ToList();
                });
            }
        }

        private void GroupAndName(Node<NodeType> entityNode, Action<string, string> action)
        {
            string group;
            string name;

            Node<NodeType>[] identifierNodes = entityNode.Nodes.Where(x => x.NodeType == NodeType.Identifier).ToArray();

            if (identifierNodes.Length == 1)
            {
                group = "";
                name = identifierNodes[0].Text;
            }
            else if (identifierNodes.Length == 2)
            {
                group = identifierNodes[0].Text;
                name = identifierNodes[1].Text;
            }
            else
            {
                throw new Exception("Failed to set name of Entity");
            }

            action(group, name);
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
                    var valueProp = prop as ValueProp;
                    var refProp = prop as RefProp;

                    stringBuilder.AppendLine();
                    stringBuilder.Append(first ? "      " : "    , ");
                    stringBuilder.Append(prop.Name);
                    if (valueProp != null)
                    {
                        stringBuilder.Append($" {prop.Type}");
                    }
                    else if (refProp != null)
                    {
                        stringBuilder.Append($" {refProp.Entity.Name}");
                    }

                    if (prop.Type == "string")
                    {
                        if (valueProp != null && valueProp.MaxLength > 0)
                        {
                            if (valueProp.MinLength > 0)
                            {
                                stringBuilder.Append($"({valueProp.MinLength}, {valueProp.MaxLength})");
                            }
                            else
                            {
                                stringBuilder.Append($"({valueProp.MaxLength})");
                            }
                        }
                    }
                    else if (valueProp != null && prop.Type == "datetime")
                    {
                        if (!String.IsNullOrWhiteSpace(valueProp.DatetimePrecision))
                        {
                            stringBuilder.Append($"({valueProp.DatetimePrecision})");
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

                    if (valueProp != null && valueProp.Auto)
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
}