using System.Linq;

namespace V2.Parsing.Core
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

        public static Node<T> FirstChild<T>(this Node<T> node, T t)
        {
            return node.Children.FirstOrDefault(x => x.NodeType.Equals(t));
        }
    }
}