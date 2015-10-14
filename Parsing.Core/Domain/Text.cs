namespace Parsing.Core.Domain
{
    public class Text : Thing
    {
        public override ThingType ThingType => ThingType.Text;

        public Text(string name, string text) : base(name, text)
        {
        }
    }
}