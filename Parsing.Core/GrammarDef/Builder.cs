using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using static System.String;

namespace Parsing.Core.GrammarDef
{
    public class Builder
    {
        public Assembly Build(params string[] sources)
        {
            CodeDomProvider codeDomProvider = CSharpCodeProvider.CreateProvider("C#", new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            
            CompilerParameters compilerParameters = new CompilerParameters { GenerateInMemory = true };
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add("Parsing.Core.dll");

            compilerParameters.IncludeDebugInformation = false;

            CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, sources);

            if (compilerResults.Errors.HasErrors)
            {
                var errors =
                    compilerResults.Errors.Cast<CompilerError>().Select(item => item.Line + ": " + item.ErrorText).ToList();
                throw new Exception(Join(Environment.NewLine, errors));
            }
            return compilerResults.CompiledAssembly;
        }
    }
}
