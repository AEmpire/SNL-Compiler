using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Common;

namespace Compiler.Scanner
{ 
    public class TokenType
    {
        public int Line{ get; set; }
        public int Row { get; set; }
        public LexType lexType { get; set; }
        public string Data { get; set; }
    }
}
