﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Compiler.Common;
using Compiler.Scanner;
using Compiler.Parser;

namespace Compiler
{
    class main
    {
        static void Main(string[] args)
        {
            parser Parser = new parser();

            TreeNode root = Parser.getTree( "../../source.txt" );
            Parser.DislayTokenList();
            Display( root );
            Console.ReadKey();
            
        }
        static public void Display(TreeNode root)
        {
       
            if (root != null)
            {
                Console.WriteLine(root.NonTerminal + ":  ");
                for (int i = 0; i < root.ChildNum; i++)
                {
                    try
                    {
                        if (root.childs[i].IsTerminal) Console.WriteLine(root.childs[i].Data + "    ");
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine("");
                        return;
                        ;
                    }

                }
                for (int i = 0; i < root.ChildNum; i++)
                {
                    if (root.childs[i].IsTerminal == false) Display(root.childs[i]);
                }
            }
            else
            {
                Console.WriteLine("请先修改词法错误");
            }
          
          
        }
    }
}
