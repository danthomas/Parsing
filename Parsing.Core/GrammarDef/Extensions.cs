namespace Parsing.Core.GrammarDef
{
    public static class Extensions
    {
        public static string ToCamelCase(this string thisString)
        {
            return thisString.Substring(0, 1).ToLower() + thisString.Substring(1);
        }
        public static string ToIdentifier(this string thisString)
        {
            return thisString.Substring(0, 1).ToUpper() + thisString.Substring(1);
        }
    }
}