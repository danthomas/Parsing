using System.Collections.Generic;

namespace V3.DomainDef
{
    public class Entity
    {
        public Entity(string group, string name, bool @enum)
        {
            Group = @group;
            Name = name;
            Enum = @enum;
            Props = new List<Prop>();
        }

        public string Group { get; set; }

        public string Name { get; set; }

        public bool Enum { get; set; }

        public List<Index> Indexes { get; set; }

        public List<Task> Tasks { get; set; }

        public List<Proc> Procs { get; set; }

        public List<Prop> Props { get; set; }

        public List<DataRow> DataRows { get; set; }
    }
}