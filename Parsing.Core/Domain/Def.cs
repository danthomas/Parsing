namespace Parsing.Core.Domain
{
    public class Def : Thing
    {
        public override ThingType ThingType => ThingType.Def;

        public Def(string name, params Thing[] children) : base(name, null, children)
        {
        }
    }
}