namespace V3.DomainDef
{
    public abstract class Prop
    {
        protected Prop(string name, string type, bool @null, bool unique, bool @readonly)
        {
            Name = name;
            Type = type;
            Null = @null;
            Unique = unique;
            Readonly = @readonly;
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool Readonly { get; set; }

        public bool Unique { get; set; }

        public bool Null { get; set; }
    }

    class RefProp : Prop
    {
        public Entity Entity { get; set; }

        public RefProp(string name, Entity entity, bool @null, bool unique, bool @readonly) 
            : base(name, entity.Props[0].Type, @null, unique, @readonly)
        {
            Entity = entity;
        }
    }

    class ValueProp : Prop
    {
        public ValueProp(string name, string type, bool auto, bool @readonly) 
            : base(name, type, false, false, @readonly)
        {
            Auto = auto;
        }

        public ValueProp(string name, string type, string datetimePrecision, bool @null, bool unique, bool @readonly, bool auto, int minLength, int maxLength) 
            : base(name, type, @null, unique, @readonly)
        {
            DatetimePrecision = datetimePrecision;
            Auto = auto;
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public string DatetimePrecision { get; set; }

        public bool Auto { get; set; }

        public int MinLength { get; set; }

        public int MaxLength { get; set; }
    }
}