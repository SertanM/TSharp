using System;

namespace Ts
{
    sealed class Program 
    {
        static void Main(string[] args) 
        {
            while (true) 
            {
                Console.Write("> ");
                var line = Console.ReadLine();

                if(string.IsNullOrEmpty(line)) return;


            }
        }
    }

    public class SyntaxToken
    {
        public SyntaxToken(int position, string text)
        {
            
        }
    }

    public sealed class Lexer
    {
        private readonly string _text;
        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }

        //public SyntaxToken NextToken()
        //{

        //}
    }
}