using System;
using System.IO;

namespace V2.Parsing.Core.Tests.Bases
{
    public class TestBase
    {
        protected static string GetDef<T>()
        {
            Type type = typeof (T);
            
            using (Stream stream = type.Assembly.GetManifestResourceStream($"{type.Namespace}.Def.grm"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}