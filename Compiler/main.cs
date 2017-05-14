using System;
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
            Console.WriteLine("请输入文件路径：");
            string path = Console.ReadLine();
            TreeNode root = Parser.getTree( path);
            Console.ReadKey();
            Parser.DislayTokenList();
            Console.ReadKey();
            Console.WriteLine("\n语法分析完成，输出语法分析树:\n");
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
                        if (root.childs[i].IsTerminal) Console.WriteLine("{0}\n",root.childs[i].Data );
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
