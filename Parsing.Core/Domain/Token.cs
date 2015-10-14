namespace Parsing.Core.Domain
{
    public class Token : Thing
    {
        public override ThingType ThingType => ThingType.Token;

        public Token(string text) : base(text, text)
        {
        }

        public Token(string name, string text ) : base(name, text)
        {
        }
    }
}