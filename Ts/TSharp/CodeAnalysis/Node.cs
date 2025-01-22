using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSharp.CodeAnalysis
{
    public abstract class Node{
        public abstract TokenKind Kind { get;}

        public abstract IEnumerable<Node> GetChildren();
    }
}