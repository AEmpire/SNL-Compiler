﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Compiler.Common;
//using System.Collections.Generic;

namespace Compiler.Scanner
{
    class scanner
    {
        public int       Line   { get; set; }  //源程序字符所在行
        public int       Row    { get; set; }  //源程序字符所在列
        public int       Cur    { get; set; }  //当前字符
        public ErrorType error  { get; set; }  //报错错误对象声明
        public String    Buffer { get; set; }
        public bool      isError { get; set; }
        public scanner()
        {
            Line = 1;
            Row  = 1;
            Cur  = 0;
            error = new ErrorType();
        }

        public Char getNextChar()
        {
            if (Cur >= Buffer.Length ) return '\0';

            if (Buffer[Cur] == '{')  //忽略注释内容
            {
                Cur ++;
                int level = 1;
                while (level != 0)
                {
                    Row ++;
                    //windows下换行不仅仅只有\n,还有\r
                    if (Buffer[Cur] == '\n' )
                    {
                        Row = 0;
                        Line ++;
                    }
                    if (Buffer[Cur] == '\t') Row += 3;
                    if (Buffer[Cur] == '{') level ++;
                    if (Buffer[Cur] == '}') level --;
                    if (Cur == Buffer.Length - 1 && level != 0)
                    {
                        // 错误处理
                        error.isError = true;
                        error.Line    = Line;
                        error.Row     = Row ;
                        error.Type    = ErrorType.errorType.LexicalError;
                        return '\0';
                    }
                    Cur ++;
                }
                return getNextChar();
            }
            //忽略空格，换行符，回车符
            if (Buffer[Cur] == ' ' || Buffer[Cur] == '\n' || Buffer[Cur] == '\r' || Buffer[Cur] == '\t' )
            {
                while (Cur<Buffer.Length)
                {
                    if (Buffer[Cur] == '\r' || Buffer[Cur] == ' ') Row++;
                    else if (Buffer[Cur] == '\n')
                    {
                        Line++;
                        Row = 1;
                    }
                    else if (Buffer[Cur] == '\t') Row += 4;
                    else break;
                    Cur ++;
                }
                return getNextChar();
            }
            Char character = Buffer[Cur];
            return character;
        }


        public String recoNumber()
        {
            String Number = "";
            while (true)
            {
                if (Buffer[Cur] >= '0' && Buffer[Cur] <= '9')
                {
                    Number += Buffer[Cur];
                    Cur++;
                    Row++;
                }
                else break;
            }
            if ((Buffer[Cur] >= 'a' && Buffer[Cur] <= 'z') || (Buffer[Cur] >= 'A' && Buffer[Cur] <= 'Z'))
            {
                error.Line = Line;
                error.Row = Row;
                error.isError = true;
                error.Type = ErrorType.errorType.LexicalError;
                return null;
            }
            return Number;
        }


        public String recoName()
        {
            String Name = "";
            while (true)
            {
                if ((Buffer[Cur] >= 'a' && Buffer[Cur] <= 'z')
                    || (Buffer[Cur] >= 'A' && Buffer[Cur] <= 'Z')
                    || (Buffer[Cur] >= '0' && Buffer[Cur] <= '9'))
                {
                    Name += Buffer[Cur];
                    Cur++;
                    Row++;
                }
                else break;
            }
            return Name;
        }


    
        public LexType isReserved(String name)
        {
            switch (name)
            {
                case "program":
                    return LexType.PROGRAM;
                case "type":
                    return LexType.TYPE;
                case "var":
                    return LexType.VAR;
                case "procedure":
                    return LexType.PROCEDURE;
                case "begin":
                    return LexType.BEGIN;
                case "end":
                    return LexType.END;
                case "array":
                    return LexType.ARRAY;
                case "of":
                    return LexType.OF;
                case "record":
                    return LexType.RECORD;
                case "if":
                    return LexType.IF;
                case "then":
                    return LexType.THEN;
                case "else":
                    return LexType.ELSE;
                case "fi":
                    return LexType.FI;
                case "while":
                    return LexType.WHILE;
                case "do":
                    return LexType.DO;
                case "endwh":
                    return LexType.ENDWH;
                case "read":
                    return LexType.READ;
                case "write":
                    return LexType.WRITE;
                case "return":
                    return LexType.RETURN;
                case "integer":
                    return LexType.INTEGER;
                case "char":
                    return LexType.CHAR;

            }
            return LexType.ID;
        }

        public LexType recoSymbol(char symbol)
        {
            switch (symbol)
            {
                case '+':  return LexType.PLUS;
                case '-':  return LexType.MINUS;
                case '*':  return LexType.TIMES;
                case '/':  return LexType.OVER;
                case '(':  return LexType.LPAREN;
                case ')':  return LexType.RPAREN;
                case '.':  return LexType.DOT;
                case '[':  return LexType.LMIDPAREN;
                case ']':  return LexType.RMIDPAREN;
                case ';':  return LexType.SEMI;
                case ':':  return LexType.COLON;
                case ',':  return LexType.COMMA;
                case '<':  return LexType.LT;
                case '=':  return LexType.EQ;
                case '\0': return LexType.ENDFILE;
            }
            error.Line = Line;
            error.Row  = Row;
            error.Type = ErrorType.errorType.LexicalError;
            error.isError = true;
            return LexType.ERROR;
        }
        

        public TokenType getNextToken()
        {
            char entrance;
            TokenType Token = new TokenType() ;


            entrance = getNextChar();

            Token.Line = Line;
            Token.Row = Row;

            if ((entrance >= 'a' && entrance <='z') || (entrance >= 'A' && entrance <= 'Z'))
            {
                Token.Data = recoName();
                Token.lexType = isReserved(Token.Data);
                
            }
            else if (entrance >= '0' && entrance <= '9')
            {
                Token.Data = recoNumber();
                Token.lexType = LexType.INTC;
            }
            else if (entrance == ':' && Buffer[Cur + 1] == '=' )
            {
                Cur += 2; Row += 2; 
                Token.Data = ":=" ;
                Token.lexType = LexType.ASSIGN;
            }
            else if (entrance == '.' && Cur < Buffer.Length - 1)
            {
                if (Buffer[Cur + 1] == '.')
                {
                    Cur += 2;
                    Row += 2;
                    Token.Data = "..";
                    Token.lexType = LexType.UNDERANGE;
                }
                else
                {
                    Token.lexType = recoSymbol(entrance);
                    Cur++;
                    Row++;
                }
            }
            else
            {
                Token.lexType = recoSymbol(entrance);
                if (Token.lexType != LexType.ENDFILE) Token.Data = "" + Buffer[Cur];
                Cur++;
                Row++;
            }

            return Token;
        }

        public List<TokenType> getTokenList( String filePath )
        {
            List<TokenType> tokenList = new List<TokenType>();
            TokenType word;

            StreamReader reader = new StreamReader(filePath); 
            Buffer = reader.ReadToEnd();
            reader.Close();

            while (true)
            {
                word = getNextToken();
                if (error.isError)
                {
                    isError = error.isError;
                    error.output();
                }
                error.isError = false;
                tokenList.Add(word);
                if (word.lexType == LexType.ENDFILE) break;
            }
            return tokenList;
        }

    }

    /*class Program
    {
        static void Main( string[] args ) 
        {
            scanner scan = new scanner();
            List<TokenType> TokenList = scan.getTokenList( "../../test.txt" );
            for (int i = 0; i < TokenList.Count; i++)348
            {
                Console.WriteLine( TokenList[i].Data + " " + TokenList[i].lexType);
            } 
            Console.ReadKey();
        }
    }*/
}
