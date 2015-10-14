namespace Parsing.Core.Domain
{
    public class Optional : Thing
    {
        public override ThingType ThingType => ThingType.Optional;

        public Optional(params Thing[] children) : base(null, null, children)
        {
        }
    }
}