namespace Parsing.Core.Domain
{
    public class OneOf : Thing
    {
        public override ThingType ThingType => ThingType.OneOf;

        public OneOf(params Thing[] children) : base(null, null, children)
        {
        }
    }
}