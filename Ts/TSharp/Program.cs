using System;
using System.Text;

using TSharp.CodeAnalysis;

namespace TSharp{
    internal static class Program{
        private static void Main(){
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.InputEncoding = System.Text.Encoding.Unicode;
            bool showTree = false;
            while(true){
                Console.Write("T#>> ");
                var line = Console.ReadLine();
                if(line == "#çık"){
                    return;
                }
                if(line == "#ağacıGöster"){
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Syntaxtree(sözdizimi ağacı) açıldı." : "Syntaxtree(sözdizimi ağacı) kapandı.");
                    continue;
                }
                if(line == "#sil"){
                    Console.Clear();
                    continue;
                }
                if(line == "#silÇık"){
                    Console.Clear();
                    return;
                }
                var syntaxTree = SyntaxTree.Parse(line);
                var color = Console.ForegroundColor;
                if(showTree){
                    Console.ForegroundColor = ConsoleColor.Green;
                    PrettyPrint(syntaxTree.Root);
                    Console.ForegroundColor = color;
                }
                
                if(syntaxTree.Diagnostics.Any()){
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach(var di in syntaxTree.Diagnostics){
                        Console.WriteLine(di.ToString());
                    }
                    Console.ForegroundColor = color;
                }else{
                    var e = new Evalutor(syntaxTree.Root);
                    var result = e.Evalute();
                    Console.WriteLine(result);
                }
            }
        }
        
        static void PrettyPrint(Node node,string indent="", bool islast = true){
           
            var marker = islast ? "│___" : "│―――";
            
            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);
            if(node is Token t && t.Value != null){
                Console.Write(" ");
                Console.Write(t.Value);
            }
            Console.WriteLine();

            indent+= islast ? "    " :  "│   ";
            
            var last = node.GetChildren().LastOrDefault();

            foreach(var child in node.GetChildren()){
                
                PrettyPrint(child,indent, child == last);

            }
        }
    }

    
}