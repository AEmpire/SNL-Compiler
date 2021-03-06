﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Scanner;
using Compiler.Common;

namespace Compiler.Parser
{
    public class parser
    {
        public parser()
        {
            SNLScanner = new scanner() ;
            SNLPredict = new Predict("../../Predict.txt") ;
            SNLProduct = new Product("../../Product.txt") ;
            error      = new ErrorType() ;
            cur = 0;
        }

        Predict  SNLPredict;
        Product  SNLProduct;
        scanner  SNLScanner;
        List<TokenType> TokenList;
        int cur ;

        public ErrorType error { get; set; }

        public void DislayTokenList()
        {
            string line = "行";
            string token = "token";
            string data = "数据";
            Console.WriteLine("词法分析完成，输出Token表:");
            Console.WriteLine();
            Console.WriteLine("{0,-2}|{1,-10}|{2,-8}|", line, token, data);
            for (int i = 0; i < TokenList.Count; i++)
            {
                Console.WriteLine("{0,-3}|{1,-10}|{2,-10}|",TokenList[i].Line,TokenList[i].lexType,TokenList[i].Data);
            }
        }
        public TreeNode getTree(String filePath)
        {
            TreeNode root = null ;
            TokenList = SNLScanner.getTokenList(filePath) ;
            if (SNLScanner.isError == true)
            {
                error.isError = true;
                return root;
            }

            root = match(nonTerminals.Program, null );

            return root;
        }

        public TreeNode match( nonTerminals NonTerminal , TreeNode father )
        {
            TreeNode root = new TreeNode();
            int  choose = -1;

            root.IsTerminal = false;
            root.NonTerminal = NonTerminal;
            root.father = father;
            root.Line = TokenList[cur].Line;
            root.Row  = TokenList[cur].Row ;

            for (int i = 0; i < SNLPredict.predicts.Count; i++)
            {
                bool flag = false;
                for (int j = 0; j < SNLPredict.predicts[i].Count; j++)
                {
                    if (TokenList[cur].lexType == SNLPredict.predicts[i][j])
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag == true && SNLProduct.product[i][0].NonTerminal == NonTerminal)
                {
                    choose = i;
                    break;
                }
            }

            if (choose == -1)
            {
                error.isError = true;
                error.Type = ErrorType.errorType.SyntaxError;
                error.Line = TokenList[cur].Line;
                error.Row = TokenList[cur].Row;
                error.output();
                return null;
            }
            else
            {
                for (int i = 1 ; i < SNLProduct.product[choose].Count; i++)
                {
                    if (SNLProduct.product[choose][i].IsTerminal)
                    {

                        if (SNLProduct.product[choose][i].Terminal == TokenList[cur].lexType)
                        {
                            TreeNode leaf = new TreeNode();
                            leaf.IsTerminal = true;
                            leaf.father = root;
                            leaf.Terminal = TokenList[cur].lexType;
                            leaf.Data = TokenList[cur].Data;
                            leaf.Line = TokenList[cur].Line;
                            leaf.Row = TokenList[cur].Row;
                            root.childs.Add(leaf);
                            cur++;
                        }
                        else
                        {
                            error.isError = true;
                            error.Type = ErrorType.errorType.SyntaxError;
                            error.Line = TokenList[cur].Line;
                            error.Row = TokenList[cur].Row;
                            error.output();
                            cur++;
                            return null;
                        }
                        
                    }
                    else
                    {
                        TreeNode child;
                        child = match( SNLProduct.product[choose][i].NonTerminal , root );
                        root.childs.Add( child );
                    }
                }
            }
            return root;
        }
    }
}