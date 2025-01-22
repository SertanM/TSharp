using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSharp.CodeAnalysis
{
    public sealed class SyntaxTree{
        public Expression Root;
        public Token EOF;
        public IReadOnlyList<string> Diagnostics; //I dont know what the fuck is IReadOnlyList but it is work
        public SyntaxTree(IEnumerable<string> diagnostics,Expression root, Token eOF){
            Root = root;
            EOF = eOF;
            Diagnostics = diagnostics.ToArray();
        }

        public static SyntaxTree Parse(string text){
            var parser = new Parser(text);
            return parser.Parse();
        }

    }
    
}