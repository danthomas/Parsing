﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Parsing.Core.GrammarDef;

namespace Sql.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            File.WriteAllText(@"C:\temp\parser.cs", new Generator().GenerateParser(new SqlGrammar()));
            File.WriteAllText(@"C:\temp\lexer.cs", new Generator().GenerateLexer(new SqlGrammar()));
            File.WriteAllText(@"C:\temp\grammar.txt", new Generator().GenerateGrammar(new SqlGrammar()));

            return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
