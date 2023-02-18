using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using static Globals;



public struct Logovar
{
    public string name;
    public string value;
}
struct LogoProc
{
    public string name;
    public int vidx;
    public string proc;
};


class Globals
{
    //public static string rawContents;

    public static bool TestingScanner = false;
    public static bool TestingParser = true;
    //public static bool TurtleMoved = false;
    public static bool NewInput = false;
    public static string NewTextInput = "";
    public static string OldTextInput = "";
    public static List<Logovar> Listvar = new List<Logovar>();
    public static List<LogoProc> ListProcedures = new List<LogoProc>();

    public static int anglex, angley;
    public static float theta, phi;
    public static Godot.Vector3 TurtlePos, TurtlePosOld;
    public static int mix, miy, miz;
    public static bool penup;
    public static int pensize;
    public static Godot.Color pencolor;
    public static int pendensity = 255;
    public static bool turtlevisible = true;
    public static string tokenString;
    public static string SelectedFile;


    public static readonly string[] Token = {
        "NONE",
        "REPEAT",
        "FORWARD",
        "BACK",
        "LEFT",
        "RIGHT",
        "UP",
        "DOWN",
        "HOME",
        "CLEAN",
        "PENUP",
        "PENDOWN",
        "SETPENCOLOR",
        "MAKE",
        "LOAD",
        "PRINT",
        "TO",
        "GO",
        "END",
        "RANDOM",
        "RANDOMIZE",
        "IF",
        "ENDIF",
        "FOR",
        "NUMBER",
        "STRING",
        "COMMENT",
        "LBRACKET",
        "RBRACKET",
        "LPARENTHESIS",
        "RPARENTHESIS",
        "LBRACE",
        "RBRACE",
        "PLUS",
        "HYPHEN",
        "ASTERISK",
        "SLASH",
        "EQUALS",
        "LESS",
        "GREATER",
        "COMMA",
        "COLON",
        "ITEM",
        "EOF"
    };

    public static readonly string[] TokenReserved = {
        "NONE",
        "REPEAT",
        "FORWARD",
        "BACK",
        "LEFT",
        "RIGHT",
        "UP",
        "DOWN",
        "HOME",
        "CLEAN",
        "PENUP",
        "PENDOWN",
        "SETPENCOLOR",
        "MAKE",
        "LOAD",
        "PRINT",
        "TO",
        "GO",
        "END",
        "RANDOM",
        "RANDOMIZE",
        "IF",
        "ENDIF",
        "FOR"
    };
    public enum Tokens : int
    {
        NONE = 0,
        REPEAT = 1,
        FORWARD = 2,
        BACK = 3,
        LEFT = 4,
        RIGHT = 5,
        UP=6,
        DOWN=7,
        HOME=8,
        CLEAN=9,
        PENUP=10,
        PENDOWN=11,
        SETPENCOLOR=12,
        MAKE=13,
        LOAD=14,
        PRINT=15,
        TO=16,
        GO=17,
        END=18,
        RANDOM=19,
        RANDOMIZE=20,
        IF=21,
        ENDIF=22,
        FOR=23,
        NUMBER = 24, //from here not reserved
        STRING = 25,
        COMMENT = 26,
        LBRACKET = 27,
        RBRACKET = 28,
        LPARENTHESIS = 29,
        RPARENTHESIS = 30,
        LBRACE=31,
        RBRACE=32,
        PLUS=33,
        HYPHEN=34,
        ASTERISK=35,
        SLASH=36,
        EQUALS=37,
        LESS=38,
        GREATER=39,
        COMMA=40,
        COLON=41,
        ITEM=42,
        EOF =43
    }
    public enum TokensReserved : int
    {
        REPEAT = 1,
        FORWARD = 2,
        BACK = 3,
        LEFT = 4,
        RIGHT = 5,
        UP = 6,
        DOWN = 7,
        HOME=8,
        CLEAN=9,
        PENUP = 10,
        PENDOWN = 11,
        SETPENCOLOR = 12,
        MAKE = 13,
        LOAD = 14,
        PRINT = 15,
        TO = 16,
        GO = 17,
        END = 18,
        RANDOM = 19,
        RANDOMIZE = 20,
        IF = 21,
        ENDIF = 22,
        FOR
    }

}


public class LogoScanner
{
    public string rawContents;
    public string scanBuffer;
    public int idx, lookup;
    public char ch;

    public LogoScanner(string input)
    {
        rawContents = input;
        idx = 0;
    }


    public int Scan()
    {
        while (idx < rawContents.Length)
        {
            ch = rawContents[idx];
            if (ch == '[')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found Leftbracket");
                return (int)Tokens.LBRACKET;
            }
            else if (ch == ']')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found Rightbracket");
                return (int)Tokens.RBRACKET;
            }
            else if (ch == '(')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found Left Parenthesis");
                return (int)Tokens.LPARENTHESIS;
            }
            else if (ch == ')')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found Right Parenthesis");
                return (int)Tokens.RPARENTHESIS;
            }
            if (ch == '{')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found LBRACE");
                return (int)Tokens.LBRACE;
            }
            else if (ch == '}')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found RBRACE");
                return (int)Tokens.RBRACE;
            }
            else if (ch == '+')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found PLUS");
                return (int)Tokens.PLUS;
            }
            else if (ch == '-')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found HYPHEN");
                return (int)Tokens.HYPHEN;
            }
            else if (ch == '*')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found ASTERISK");
                return (int)Tokens.ASTERISK;
            }
            else if (ch == '/')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found SLASH");
                return (int)Tokens.SLASH;
            }
            else if (ch == '=')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found EQUALS");
                return (int)Tokens.EQUALS;
            }
            else if (ch == '<')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found LESS");
                return (int)Tokens.LESS;
            }
            else if (ch == '>')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found GREATER");
                return (int)Tokens.GREATER;
            }
            else if (ch == ',')
            {
                idx++;
                if (TestingScanner) GD.Print("Scanner: found COMMA");
                return (int)Tokens.COMMA;
            }
            else if (ch == ':')
            {
                scanBuffer = "";
                idx++;
                while (idx < rawContents.Length)
                {
                    ch = rawContents[idx];
                    if (!Char.IsWhiteSpace(ch) && ch != '\n')
                    {
                        scanBuffer += ch;
                        idx++;
                    }
                    else break;
                }
                if (TestingScanner) GD.Print("Scanner: found COLON");
                return (int)Tokens.COLON;
            }
            else if (ch == ';')
            {
                //scanBuffer = ch;
                idx++;
                while (idx < rawContents.Length)
                {
                    ch = rawContents[idx];
                    if (ch != '\n')
                    {
                        scanBuffer += ch;
                        idx++;
                    }
                    else break;
                }
                if (TestingScanner) GD.Print("Scanner: found COMMENT");
                return (int)Tokens.COMMENT;
            }
            else if (ch == '\"')
            {
                scanBuffer = "";
                idx++;
                ch = rawContents[idx];
                if (ch == '[' && ch != '\n')
                {
                    idx++;
                    while (idx < rawContents.Length)
                    {
                        ch = rawContents[idx];
                        if (ch != ']')
                        {
                            scanBuffer += ch;
                            idx++;
                        }
                        else break;
                    }
                    if (idx == rawContents.Length) GD.Print("ERROR - list: closing ] not found\n");
                    idx++;
                    if (TestingScanner) GD.Print("Scanner: found STRING");
                }
                else
                {
                    while (idx < rawContents.Length)
                    {
                        ch = rawContents[idx];
                        if (!Char.IsWhiteSpace(ch) && ch != '\n')
                        {
                            scanBuffer += ch;
                            idx++;
                        }
                        else break;
                    }
                }
                if (TestingScanner) GD.Print("Scanner: found STRING");
                return (int)Tokens.STRING;
            }
            else if (char.IsDigit(ch))
            {
                scanBuffer = ch.ToString();
                idx++;
                while (idx < rawContents.Length)
                {
                    ch = rawContents[idx];
                    if (char.IsDigit(ch))
                    {
                        scanBuffer += ch;
                        idx++;
                    }
                    else break;
                }
                if (TestingScanner) GD.Print("Scanner: found Number");
                return (int)Tokens.NUMBER;
            }
            else if (char.IsLetter(ch))
            {
                scanBuffer = ch.ToString();
                idx++;
                while (idx < rawContents.Length)
                {
                    ch = rawContents[idx];
                    if (char.IsLetter(ch))
                    {
                        scanBuffer += ch;
                        idx++;
                    }
                    else break;
                }
                lookup = LookupReserved(scanBuffer);
                if (lookup > 0)
                {
                    if (TestingScanner) GD.Print("Scanner: found TokenReserved: " + TokenReserved[lookup]);
                    return lookup;
                }
            LexicalError();
            }
            else if (char.IsWhiteSpace(ch))
            {
                idx++;
            }
            else
            {
                LexicalError();
            }
        }
        return (int)Tokens.EOF;
    }

    private int LookupReserved(string s)
    {
        //for (int i =0; i < (int)TokensReserved.NUMBERTOKENSRESERVED; i++)
        for (int i = 0; i < TokenReserved.Length; i++)
        {
            //GD.Print(TokenReserved[i]);
            if (String.Equals(TokenReserved[i], s)) return i;
        }
        if (TestingScanner) GD.Print("LookupReserved: count " + ListProcedures.Count().ToString());
        for (int j = 0; j < ListProcedures.Count(); j++)
        {
            if (TestingScanner) GD.Print("LookupReserved: name "+ ListProcedures[j].name);
            if (String.Equals(ListProcedures[j].name, s)) return 777;
        }
        return 0;
    }

    public int NextToken()
    {
        var oldIdx = idx;
        var result = Scan();
        idx = oldIdx;
        return result;
    }

    public int NextbutoneToken()
    {
        if (TestingScanner) GD.Print("Scanner: " + "Start NextbutoneToken");
        int oldIdx = idx;
        Scan();
        int result = Scan();
        idx = oldIdx;
        if (TestingScanner) GD.Print("Scanner: " + "NextbutoneToken result "+result.ToString());
        return result;
    }

    private void LexicalError()
    {
        GD.Print("Scanner: " + "Lexical error at " + ch +" "+ scanBuffer);
    }

    private void SyntaxError(string s)
    {
        GD.Print("Scanner: " + "SyntaxError: " + s);
    }
    private void Error(string s)
    {
        GD.Print("Scanner: "+"ERROR: "+s);
    }
}


public class LogoParser
{
    public LogoScanner scanner;
    public Godot3DInterpreter IntClass;
    //public List<Logovar> Listvar = new List<Logovar>();
    public LogoParser(LogoScanner logoScanner, Godot3DInterpreter IC)
    {
        scanner = logoScanner;
        IntClass = IC;
    }

    public float Deg2Rad(float deg)
    {
        return 3.14159265358979f * deg / 180.0f;
    }

    public float ReturnASCIISum(string StringX)
    {
        float TotalSum = 0;
        for (int i = 0; i < StringX.Length; i++)
        {
            TotalSum += (int)StringX[i];
        }
        return TotalSum;
    }
    public void TurtleForward(float dist)
    {
        if (TestingParser) GD.Print("Parser: "+"TurtleForward");
        float x, y, z;

        TurtlePosOld = TurtlePos;

        x = dist * (float)Math.Cos(Deg2Rad(phi)) * (float)Math.Cos(Deg2Rad(theta));
        y = dist * (float)Math.Sin(Deg2Rad(phi)) * (float)Math.Cos(Deg2Rad(theta));
        z = dist * (float)(Math.Sin(Deg2Rad(theta)));

        TurtlePos.X= TurtlePos.X + x;
        TurtlePos.Y= TurtlePos.Y + y;
        TurtlePos.Z= TurtlePos.Z + z;

        //TurtleMoved = true;
        if (!penup) IntClass.DrawLine3D(TurtlePosOld, TurtlePos, pencolor);

    }


    public void TurtleBack(float dist)
    {
        if (TestingParser) GD.Print("Parser: "+"TurtleBack");
        TurtleForward(-dist);
    }

    public void TurtleLeft(float angle)
    {
        if (TestingParser) GD.Print("Parser: " + "TurtleUp");
        phi = phi + angle;
        if (phi < 0) phi = 360 + phi;
        if (phi > 360) phi = phi - 360;
    }
    public void TurtleRight(float angle)
    {
        if (TestingParser) GD.Print("Parser: " + "TurtleDown");
        phi = phi - angle;
        if (phi < 0) phi = 360 + phi;
        if (phi > 360) phi = phi - 360;
    }

    public void TurtleUp(float angle)
    {
        if (TestingParser) GD.Print("Parser: " + "TurtleLeft");
        theta = theta + angle;
        if (theta < 0) theta = 360 + theta;
        if (theta > 360) theta = theta - 360;
    }

    public void TurtleDown(float angle)
    {
        if (TestingParser) GD.Print("Parser: " + "TurtleRight");
        theta = theta - angle;
        if (theta < 0) theta = 360 + theta;
        if (theta > 360) theta = theta - 360;
    }

    public void TurtleHome()
    {
        if (TestingParser) GD.Print("Parser: " + "TurtleHome");
        penup = false;
        pensize = 1;
        pencolor = new Godot.Color(1.0f, 1.0f, 1.0f);
        pendensity = 255;
        anglex = 0;
        angley = 0;
        mix = 0;
        miy = 0;
        miz = 0;
        TurtlePos.X = 0;
        TurtlePos.Y = 0;
        TurtlePos.Z = 0;
        theta = 0;
        phi = 0;
    }

    public void TurtleClean()
    {
        if (TestingParser) GD.Print("Parser: " + "TurtleClean");
        IntClass.Remove3D();
        
    }

    public void TurtlePenUp()
    {
        if (TestingParser) GD.Print("Parser: " + "TurtlePenUp");
        penup = true; 
    }

    public void TurtlePenDown()
    {
        if (TestingParser) GD.Print("Parser: " + "TurtlePenDown");
        penup = false;
    }

    public void TurtleSetPenColor(float c1, float c2, float c3)
    {
        if (TestingParser) GD.Print("Parser: " + "TurtleSetPenColor");
        pencolor = new Godot.Color(c1/255, c2/255, c3/255);
    }

    public void LoadProgram()
    {
        if (TestingParser) GD.Print("Parser: " + "LoadProgram");
        IntClass.FileDia.Visible = true;
    }


    float getvar(string s)
    {
        for (int i = Listvar.Count() - 1; i >= 0; i--)
        {
            if (Listvar[i].name == s)
            {
                return Listvar[i].value.ToFloat();
            }
        }
        ErrorMessage("Parser: getvar: no variable found to get value");
        return -1;
    }

    public string getvarstring(string s)
    {
        for (int i = Listvar.Count - 1; i >= 0; i--)
        {
            //printf("varname varvalue %s %f\n", vec_logovar[i].varname.c_str(),atof(vec_logovar[i].varvalue.c_str()));
            if (Listvar[i].name == s)
            {
                return Listvar[i].value;
            }
        }
        ErrorMessage("Parser: getvarstring: no variable found to get value");
        return "0";
    }

    public string getprocbody(string procedure)
    {
        for (int i = ListProcedures.Count - 1; i >= 0; i--)
        {
            //printf("varname varvalue %s %f\n", vec_logovar[i].varname.c_str(),atof(vec_logovar[i].varvalue.c_str()));
            if (ListProcedures[i].name == procedure)
            {
                return ListProcedures[i].proc;
            }
        }
        ErrorMessage("Parser: getprocbody: no procedure found to get body");
        return "0";
    }
    public float setvar(string s, float val)
    {
        for (int i = Listvar.Count - 1; i >= 0; i--)//stack von hinten durchsuchen
        {
        
            if (Listvar[i].name == s)
            {
                //string myString;
                //GD.Print(myString + val);
                //Listvar[i].value = val.ToString();
                Listvar.RemoveAt(i);
                Logovar tmpvar;
                tmpvar.name = s;
                tmpvar.value = val.ToString();
                Listvar.Add(tmpvar);
                return val;
            }
        }
        ErrorMessage ("Parser: setvar: no variable found to set value");
        return 0;
    }

    public string getstrorvalue()
    {
        int ntok = scanner.NextToken();
        if (ntok == (int)Tokens.STRING || ntok == (int)Tokens.NUMBER)
        {
            Match(ntok);
            return scanner.scanBuffer;
        }
        else if (ntok == (int)Tokens.COLON)
        {
            Match((int)Tokens.COLON);
            //printf("value %s",scanner.scanBuffer.c_str());
            return getvarstring(scanner.scanBuffer);
        }
        //else if (ntok == (int)Tokens.ITEM)
        //{
        //    Match((int)Tokens.ITEM);
        //    return getitem();
        //}
        else ErrorMessage("Parser: getstrorvalue: expected number or variablevalue");
        return "";
    }

    public float numberor()
    {
        if (scanner.NextToken() == (int)Tokens.NUMBER)
        {
            GD.Print("numberor - found number: " + scanner.scanBuffer);
            Match((int)Tokens.NUMBER);
            return float.Parse(scanner.scanBuffer);
        }
        else if (scanner.NextToken() == (int)Tokens.HYPHEN)
        {
            GD.Print("numberor - found -number: " + scanner.scanBuffer);
            Match((int)Tokens.HYPHEN);
            Match((int)Tokens.NUMBER);
            return -scanner.scanBuffer.ToFloat();
        }
        else if (scanner.NextToken() == (int)Tokens.COLON)
        {
            GD.Print("numberor - found COLON and variable: " + scanner.scanBuffer);
            Match((int)Tokens.COLON);
            //printf("value %s",scanner.scanBuffer.c_str());
            float returnvalue;
            string vartmp = getvarstring(scanner.scanBuffer);
            char ch = vartmp[0];
            if (Char.IsLetter(ch)) returnvalue = ReturnASCIISum(vartmp);
            else returnvalue = vartmp.ToFloat();
            return returnvalue;
        }
        else if (scanner.NextToken() == (int)Tokens.STRING)
        {
            GD.Print("numberor - found string: " + scanner.scanBuffer);
            Match((int)Tokens.STRING);
            //printf("value %s",scanner.scanBuffer.c_str());
            return ReturnASCIISum(scanner.scanBuffer);
        }
        else if (scanner.NextToken() == (int)Tokens.RANDOM)
        {
            Random rnd = new Random();
            Match((int)Tokens.RANDOM);
            return (float)rnd.Next(255);
        }
        else ErrorMessage("Parser: numberor: expected number or variable");
        return 0.0f;
    }

    float ParseExpr()
    {
        float op, op1;
        op = ParseFactor();
        //printf("expr %f\n",op);
        int nto = scanner.NextToken();
        if (nto != (int)Tokens.EOF)
        {
            if (nto == (int)Tokens.PLUS)
            {
                Match((int)Tokens.PLUS);
                op1 = ParseExpr();
                op = op + op1;
            }
            else if (nto == (int)Tokens.HYPHEN)
            {
                Match((int)Tokens.HYPHEN);
                op1 = ParseExpr();
                op = op - op1;
            }
        }
        return op;
    }

    float ParseFactor()
    {
        float op, op1;
        op = ParseTerm();
        //printf("fact %f\n",op);
        int nto = scanner.NextToken();
        if (nto != (int)Tokens.EOF)
        {
            if (nto == (int)Tokens.ASTERISK)
            {
                Match((int)Tokens.ASTERISK);
                op1 = ParseFactor();
                op = op * op1;
            }
            else if (nto == (int)Tokens.SLASH)
            {
                Match((int)Tokens.SLASH);
                op1 = ParseFactor();
                op = op / op1;
            }
        }
        return op;
    }

    float ParseTerm()
    {
        float returnValue = 0;
        int nto = scanner.NextToken();
        if (nto != (int)Tokens.EOF)
        {
            if (nto==(int)Tokens.PLUS ||nto==(int)Tokens.HYPHEN)
            {
            	Match(nto);
            	return ParseExpr();
            }
            if (nto == (int)Tokens.NUMBER ) // || nto == (int)Tokens.VALUE)
            {
                return numberor();
            }
            
            else if (nto == (int)Tokens.RANDOM /*||
                     nto == (int)Tokens.ABS ||
                     nto == (int)Tokens.NEG ||
                     nto == (int)Tokens.ASIN ||
                     nto == (int)Tokens.ACOS ||
                     nto == (int)Tokens.ATAN ||
                     nto == (int)Tokens.ATANTWO ||
                     nto == (int)Tokens.SIN ||
                     nto == (int)Tokens.COS ||
                     nto == (int)Tokens.TAN ||
                     nto == (int)Tokens.LOG ||
                     nto == (int)Tokens.POW ||
                     nto == (int)Tokens.SQRT ||
                     nto == (int)Tokens.MOUSEPOSITIONX ||
                     nto == (int)Tokens.MOUSEPOSITIONY ||
                     nto == (int)Tokens.GETVOXEL ||
                     nto == (int)Tokens.DSGET ||
                     nto == (int)Tokens.ITEM*/
                )
            {
                return 0; // mathcalc();
            }
            else if (nto == (int)Tokens.LPARENTHESIS)
            {
                //printf("lp found\n");
                Match((int)Tokens.LPARENTHESIS);
                returnValue = ParseExpr();
                //printf("returnvalue %f\n",returnValue);
                Match((int)Tokens.RPARENTHESIS);
                return returnValue;
            }
            else if (nto == (int)Tokens.RPARENTHESIS)
            {
                Match((int)Tokens.RPARENTHESIS);
                return returnValue;
            }
            else
            {
                ErrorMessage("math expr parser: not expected token found");
            }
        }
        return returnValue;
    }

    // <logo-program>  ::= <logo-sentence> { <logo-sentence> } <EOF>
    public void ParseLogoProgram()
    {
        if (TestingParser) GD.Print("Parser: " + "Start ParseLogoProgram");
        ParseLogoSentence();
        while (true)
        {
            switch (scanner.NextToken())
            {
                case (int)Tokens.STRING:
                case (int)Tokens.FORWARD:
                case (int)Tokens.BACK:
                case (int)Tokens.LEFT:
                case (int)Tokens.RIGHT:
                case (int)Tokens.UP:
                case (int)Tokens.DOWN:
                case (int)Tokens.HOME:
                case (int)Tokens.CLEAN:
                case (int)Tokens.PENUP:
                case (int)Tokens.PENDOWN:
                case (int)Tokens.SETPENCOLOR:
                case (int)Tokens.MAKE:
                case (int)Tokens.LOAD:
                case (int)Tokens.PRINT:
                case (int)Tokens.TO:
                case (int)Tokens.GO:
                case (int)Tokens.RANDOMIZE:
                case (int)Tokens.IF:
                case (int)Tokens.FOR:
                case (int)Tokens.REPEAT:
                    ParseLogoSentence();
                    break;
                case (int)Tokens.COMMENT:
                    Match((int)Tokens.COMMENT);
                    break;

                default:
                    Match((int)Tokens.EOF);
                    return;
            }
        }
    }


    // <logo-sentence> ::= FORWARD <integer>
    //                   | BACK <integer>
    //                   | LEFT <integer>
    //                   | RIGHT <integer>
    //                   | REPEAT <integer> [ <logo-sentence> { <logo-sentence> } ]
    private void ParseLogoSentence()
    {
        if (TestingParser) GD.Print("Parser: " + "Start ParseLogoSentence");
        bool nextbutone = false;
        switch (scanner.NextbutoneToken())
        {
            case (int)Tokens.EQUALS:
                {
                    //printf("nextbutone equal found\n");
                    Match((int)Tokens.STRING);
                    string varname = scanner.scanBuffer;
                    Match((int)Tokens.EQUALS);
                    int nto = scanner.NextToken();

                    //if (nto == (int)Tokens.ISKEYDOWN ||
                    //   nto == (int)Tokens.MOUSELEFTBUTTONDOWN)
                    //{
                    //    calc(varname);
                    //}
                    //else
                    //{
                        float result = ParseExpr();
                        setvar(varname, result);
                    //}
                    nextbutone = true;

                    break;
                }
            default:
                //Match(TOKEN_EOF);
                // return;
                break;
        }

        if (!nextbutone)
        {
            var nextToken = scanner.NextToken();
            float n, n2, n3;
            //if (TestingParser) GD.Print("Parser: "+"ParseLogoSentence-nextToken"+nextToken.ToString());
            switch (nextToken)
            {
                case (int)Tokens.STRING:
                    Match(nextToken);
                    break;

                case (int)Tokens.FORWARD:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = numberor();
                    TurtleForward(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence FORWARD+Number");
                    break;

                case (int)Tokens.BACK:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = numberor();
                    TurtleBack(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence BACK+Number");
                    break;

                case (int)Tokens.LEFT:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = numberor();
                    TurtleLeft(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence LEFT+Number");
                    break;

                case (int)Tokens.RIGHT:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = numberor();
                    TurtleRight(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence RIGHT+Number");
                    break;

                case (int)Tokens.UP:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = numberor();
                    TurtleUp(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence UP+Number");
                    break;

                case (int)Tokens.DOWN:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = numberor();
                    TurtleDown(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence DOWN+Number");
                    break;

                case (int)Tokens.HOME:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    TurtleHome();
                    if (TestingParser) GD.Print("Parser: " + "found sentence HOME");
                    break;

                case (int)Tokens.CLEAN:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    TurtleClean();
                    if (TestingParser) GD.Print("Parser: " + "found sentence CLEAN");
                    break;

                case (int)Tokens.PENUP:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    TurtlePenUp();
                    if (TestingParser) GD.Print("Parser: " + "found sentence PENUP");
                    break;

                case (int)Tokens.PENDOWN:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    TurtlePenDown();
                    if (TestingParser) GD.Print("Parser: " + "found sentence PENDOWN");
                    break;

                case (int)Tokens.SETPENCOLOR:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = numberor();
                    n2 = numberor();
                    n3 = numberor();
                    TurtleSetPenColor(n, n2, n3);
                    if (TestingParser) GD.Print("Parser: " + "found sentence SETPENCOLOR+N1+N2+N3");
                    break;

                case (int)Tokens.LOAD:
                    Match(nextToken);
                    //if (!Match((int)Tokens.STRING)) break;
                    LoadProgram();
                    if (TestingParser) GD.Print("Parser: " + "found sentence LOAD");
                    break;

                case (int)Tokens.FOR:
                    if (TestingParser) GD.Print("Parser: " + "found sentence FOR");
                    Match(nextToken);
                    Match((int)Tokens.STRING);
                    string varname = scanner.scanBuffer;
                    //Match(TOKEN_NUMBER);
                    int numberstart = (int)numberor();
                    int numberend = (int)numberor();
                    int numberstep = (int)numberor();
                    //printf("nexttoken %i \n",scanner.NextToken());
                    Match((int)Tokens.LBRACKET);
                    //printf("FOR command %i %i %i\n", numberstart, numberend, numberstep);
                    //ParseLogoSentence();
                    int oldidx2 = scanner.idx;

                    for (int i = numberstart; i < numberend + 1; i = i + numberstep)
                    {
                        setvar(varname, i);
                        scanner.idx = oldidx2;
                        //printf("idx %i\n", scanner.idx);
                        //ParseLogoProgram();
                        //printf("idx %i\n", scanner.idx);
                        while (scanner.NextToken() != (int)Tokens.RBRACKET)
                        {
                            //printf("nexttoken %i \n",scanner.NextToken());
                            ParseLogoSentence();
                        }
                        //Match(RBRACKET);
                    }
                    Match((int)Tokens.RBRACKET);
                    break;
                    
                case (int)Tokens.IF:
                    if (TestingParser) GD.Print("Parser: " + "found sentence IF");
                    {
                        int countif = 1;
                        int matchif = 0;
                        Match(nextToken);
                        //Match(TOKEN_VALUE);
                        //printf("value %s",scanner.scanBuffer.c_str());
                        //float vecvaltmp= vec_getvar(scanner.scanBuffer);
                        float vecvaltmp = numberor();
                        //int found=vecvaltmp.find(".000000");
                        //if(found !=std::string::npos)
                        //{
                        //	vecvaltmp=vecvaltmp.substr(0,found);
                        //}
                        if (scanner.NextToken() == (int)Tokens.LESS)
                        {
                            //printf("if less %s\n",scanner.scanBuffer.c_str());
                            Match((int)Tokens.LESS);
                            //float vecvaltmp2=numberorvalue();
                            float vecvaltmp2 = numberor();
                            //int found=vecvaltmp2.find(".000000");
                            //if(found !=std::string::npos)
                            //{
                            //	vecvaltmp2=vecvaltmp2.substr(0,found);
                            //}
                            if (vecvaltmp < vecvaltmp2)
                            {
                                //Match(TOKEN_LBRACKET);
                                ParseLogoSentence();
                                while (scanner.NextToken() != (int)Tokens.ENDIF)
                                {
                                    ParseLogoSentence();
                                }
                                Match((int)Tokens.ENDIF);
                                break;
                            }
                            else
                            {
                                while (countif > matchif)
                                {
                                    while (scanner.NextToken() != (int)Tokens.ENDIF)
                                    {
                                        if (scanner.NextToken() == (int)Tokens.IF) countif++;
                                        Match(scanner.NextToken());
                                    }
                                    Match((int)Tokens.ENDIF);
                                    matchif++;
                                }
                            }
                        }
                        else if (scanner.NextToken() == (int)Tokens.GREATER)
                        {
                            //printf("if greater %s\n",scanner.scanBuffer.c_str());
                            Match((int)Tokens.GREATER);
                            //float vecvaltmp2=numberorvalue();
                            float vecvaltmp2 = numberor();
                            //int found=vecvaltmp2.find(".000000");
                            //if(found !=std::string::npos)
                            //{
                            //	vecvaltmp2=vecvaltmp2.substr(0,found);
                            //}
                            //printf("if greater %s %s\n",vecvaltmp.c_str(),vecvaltmp2.c_str());
                            if (vecvaltmp > vecvaltmp2)
                            {
                                //printf("if greater %s %s\n",vecvaltmp.c_str(),vecvaltmp2.c_str());
                                //Match(TOKEN_LBRACKET);
                                ParseLogoSentence();
                                while (scanner.NextToken() != (int)Tokens.ENDIF)
                                {
                                    ParseLogoSentence();
                                }
                                Match((int)Tokens.ENDIF);
                                break;
                            }
                            else
                            {
                                while (countif > matchif)
                                {
                                    while (scanner.NextToken() != (int)Tokens.ENDIF)
                                    {
                                        if (scanner.NextToken() == (int)Tokens.IF) countif++;
                                        Match(scanner.NextToken());
                                    }
                                    Match((int)Tokens.ENDIF);
                                    matchif++;
                                }
                            }
                        }
                        else if (scanner.NextToken() == (int)Tokens.EQUALS)
                        {
                            //printf("if equal %s\n",scanner.scanBuffer.c_str());
                            Match((int)Tokens.EQUALS);
                            //float vecvaltmp2=numberorvalue();
                            float vecvaltmp2 = numberor();
                            //int found=vecvaltmp2.find(".000000");
                            //if(found !=std::string::npos)
                            //{
                            //	vecvaltmp2=vecvaltmp2.substr(0,found);
                            //}
                            //printf("if equal %f %f\n",vecvaltmp,vecvaltmp2);
                            if (vecvaltmp == vecvaltmp2)
                            {
                                //printf("if equal %f %f\n",vecvaltmp,vecvaltmp2);
                                //Match(TOKEN_LBRACKET);
                                ParseLogoSentence();
                                while (scanner.NextToken() != (int)Tokens.ENDIF)
                                {
                                    ParseLogoSentence();
                                }
                                Match((int)Tokens.ENDIF);
                                break;
                            }
                            else
                            {
                                while (countif > matchif)
                                {
                                    while (scanner.NextToken() != (int)Tokens.ENDIF)
                                    {
                                        if (scanner.NextToken() == (int)Tokens.IF) countif++;
                                        Match(scanner.NextToken());
                                    }
                                    Match((int)Tokens.ENDIF);
                                    matchif++;
                                }
                            }
                        }
                        //Match(TOKEN_LBRACKET);
                        //printf("REPEAT command %i\n", numberrecord);
                        //ParseLogoSentence();

                        //Match(TOKEN_RBRACKET);
                        break;
                    }

                case (int)Tokens.PRINT:
                    //printf("print \n");
                    Match(nextToken);

                    int nextt = scanner.NextToken();
                    while (nextt == (int)Tokens.COMMA || nextt == (int)Tokens.COLON || nextt == (int)Tokens.STRING) //|| nextt == (int)Tokens.ITEM
                    {
                        if (nextt == (int)Tokens.COMMA)
                        {
                            Match((int)Tokens.COMMA);
                            nextt = scanner.NextToken();
                        }
                        if (nextt == (int)Tokens.NUMBER) // || nextt == TOKEN_ITEM)
                        {
                            //Match(TOKEN_VALUE);
                            //std::string stringmake = scanner.scanBuffer;
                            //printf("print string %s\n", scanner.scanBuffer.c_str());
                            float erg = numberor();
                            //if (erg == floor(erg))
                            //{
                                GD.Print(erg);
                            //}
                            //else
                            //{
                            //    GD.Print(erg);
                            //}
                        }
                        else if (nextt == (int)Tokens.COLON)
                        {
                            Match((int)Tokens.COLON);
                            string stringvar = getvarstring(scanner.scanBuffer);
                            //for (int i = vec_logovar.size()-1; i >=0; i--)
                            //{
                            //printf("varname varvalue %s %i\n", vec_logovar[i].varname.c_str(),vec_logovar[i].varvalue);
                            //if(vec_logovar[i].varname==stringvar)
                            //{
                            //printf("%s\n", vec_logovar[i].varvalue.c_str());
                            ////int found = stringvar.find(".000000");
                            ////if (found != string::npos)
						    ////{
                            ////    stringvar = stringvar.Substr(0, found);
                            ////}
                            GD.Print(stringvar);
                            //}
                            //}
                        }
                        else if (scanner.NextToken() == (int)Tokens.STRING)
                        {
                            //printf("print string %i\n", scanner.idx);
                            Match((int)Tokens.STRING);
                            GD.Print(scanner.scanBuffer);
                            //printf("print string %i\n", scanner.idx);
                        }
                        else ErrorMessage("Parser: "+"PRINT: wrong parameter");
                        nextt = scanner.NextToken();
                    }
                    GD.Print("\n");
                    break;

                case (int)Tokens.MAKE:
                    Match(nextToken);
                    Match((int)Tokens.STRING);
                    string arrayname = scanner.scanBuffer;
                    string varvalascii;
                    //std::string stringmake = scanner.scanBuffer;
                    ////if (inprocedure) setvar(procedurename + "_c_o_u_n_t", getvar(procedurename + "_c_o_u_n_t") + 1);
                    //printf("make procedurename %s \n", procedurename.c_str());
                    //printf("variable %s \n", arrayname.c_str());
                    int nextto = scanner.NextToken();
                    if (nextto == (int)Tokens.LBRACE)//array
                    {
                        //Match((int)Tokens.LBRACE);
                        //Logoarray vec_tmparray;
                        //float numberrecord = numberorvalue();
                        //printf("make array %i\n", numberrecord);
                        //string Result;
                        //for (int i = 0; i < numberrecord; i++)
                        //{
                            //sprintf(Result, "%d", i);
                            //varvalascii = Result;
                            //tmparray.name = arrayname + varvalascii;
                            //printf("make arrayname %s\n", vec_tmparray.varname.c_str());
                            //tmparray.varvalue = "";
                            //logoarray.push_back(vec_tmparray);
                        //}
                        //Match((int)Tokens.RBRACE);
                        //break;
                    }
                    else if (nextto == (int)Tokens.NUMBER)
                    {
                        Logovar tmpvar;
                        tmpvar.name = arrayname;
                   
                        Match((int)Tokens.NUMBER);
                        //int numberrecord = atoi(scanner.scanBuffer.c_str());
                        //float numberrecord=numberorvalue();

                        //char myString[30];

                        tmpvar.value = scanner.scanBuffer;
                        if (TestingParser) GD.Print("Parser: " + "found sentence MAKE+NUMBER "+ scanner.scanBuffer);
                        Listvar.Add(tmpvar);
                        
                        break;
                    }
                    break;

                case (int)Tokens.TO:
                    {
                        int idxbegin = scanner.idx;
                        Match(nextToken);
                        //Match(TOKEN_NUMBER);
                        Match((int)Tokens.STRING);
                        //std::string stringmake = scanner.scanBuffer;
                        LogoProc tmpproc;
                        tmpproc.name = scanner.scanBuffer;
                        //tmpproc.proc = "";
                        //printf("to string %s\n", scanner.scanBuffer.c_str());
                        //Match(TOKEN_NUMBER);
                        //int numberrecord = atoi(scanner.scanBuffer.c_str());
                        //printf("make number %i\n", numberrecord);
                        //vec_tmpvar.varvalue=numberrecord;
                        //vec_logovar.push_back(vec_tmpvar);
                        //Match(TOKEN_LBRACKET);
                        //printf("REPEAT command %i\n", numberrecord);
                        //ParseLogoSentence();
                        //int oldidx=scanner.idx;
                        tmpproc.vidx = scanner.idx;
                        //GD.Print("raw= " + scanner.rawContents + "---idx: " + scanner.idx.ToString());
                        //for (int i = 0; i < numberrecord; i++)
                        //{
                        //scanner.idx=oldidx;
                        //printf("idx %i\n", scanner.idx);
                        //ParseLogoSentence();
                        //printf("to vo while %i\n");

                        while (scanner.NextToken() != (int)Tokens.END)
                        {
                            Match(scanner.NextToken());
                            //ParseLogoSentence();
                        }

                        //Match(RBRACKET);
                        //} 
                        //printf("to END found %i\n");
                        Match((int)Tokens.END);
                        int idxend = scanner.idx;
                        if (TestingParser) GD.Print("Parser: " + "proc= " + scanner.rawContents.Substring(idxbegin, idxend));
                        tmpproc.proc = scanner.rawContents.Substring(idxbegin, idxend)+" ";
                        ListProcedures.Add(tmpproc);
                        if (TestingParser) GD.Print("Parser: " + "found sentence TO name " + tmpproc.name);
                        break;
                    }
                case (int)Tokens.GO:
                    {
                        if (TestingParser) GD.Print("Parser: " + "start GO");
                        //printf("start \n");
                        Match(nextToken);
                        Match((int)Tokens.STRING);
                        string procedurename = scanner.scanBuffer;
                        if (TestingParser) GD.Print("Parser: " + "found sentence GO name " + procedurename);
                        if (scanner.rawContents.Count("TO \""+procedurename)>0 )
                        {
                            if (TestingParser) GD.Print("Parser: " + "found procedure " + procedurename);
                        }
                        else
                        {
                            if (TestingParser) GD.Print("Parser: " + "found not procedure " + procedurename);
                            string p = getprocbody(procedurename);
                            if (p != "0")
                            {
                                scanner.rawContents = scanner.rawContents.Insert(0, p);
                                scanner.idx = procedurename.Length+4;
                                if (TestingParser) GD.Print("scanner.rawcontents: " + scanner.rawContents);
                                break;
                            }
                            else
                            {
                                ErrorMessage("GO procedure - no procedure with name "+procedurename+ " found.");
                                break;
                            }
                        }
                        int nextt2 = scanner.NextToken();
                        string[] parameter = new string[42];
                        
                        Logovar tmpvar;
                        //int pcounter=0;
                        tmpvar.name = procedurename + "_p_a_r_a";
                        //printf("go varname %s\n",vec_tmpvar.varname.c_str());
                        //char myString[30];
                        //sprintf(myString, "%f", 0);
                        //vec_logovar[i].varvalue=myString;
                        tmpvar.value = "0";
                        Listvar.Add(tmpvar);
                        //procedurecounter=0;
                        int parametercount = 0;
                        while (nextt2 == (int)Tokens.NUMBER || nextt2 == (int)Tokens.COLON)
                        {
                            parameter[(int)getvar(procedurename + "_p_a_r_a")] = getstrorvalue();
                            //printf("parameter %i value %i\n", pcounter, parameter[pcounter]);
                            //procedurecounter++;
                            setvar(procedurename + "_p_a_r_a", getvar(procedurename + "_p_a_r_a") + 1);
                            //Logovar vec_tmpvar;
                            //vec_tmpvar.varname=scanner.scanBuffer;
                            //int numberrecord=numberorvalue();
                            //vec_tmpvar.varvalue=numberrecord;
                            //vec_logovar.push_back(vec_tmpvar);
                            nextt2 = scanner.NextToken();
                            parametercount++;
                        }
                        //if (TestingParser) GD.Print("Parser: " + "found sentence GO parametercounter " + parametercount.ToString());
                        //printf("START %s\n", scanner.scanBuffer.c_str());
                        //std::string stringmake = scanner.scanBuffer;
                        //vec_startfunc(procedurename);

                        int oldidx3 = scanner.idx;
                        int result = 0;
                        //std::string s;
                        for (int i = 0; i < ListProcedures.Count; i++)
                        {
                            //printf("varname varvalue %s %i\n", vec_logovar[i].varname.c_str(),vec_logovar[i].varvalue);
                            if (ListProcedures[i].name == procedurename)
                            {
                                scanner.idx = ListProcedures[i].vidx;
                                int pcount2 = 0;
                                while (scanner.NextToken() == (int)Tokens.COLON)
                                {
                                    Match((int)Tokens.COLON);
                                    Logovar vec_tmpvar;
                                    tmpvar.name = scanner.scanBuffer;
                                    //char myString[30];
                                    //sprintf(myString, "%f", parameter[pcount2]);
                                    tmpvar.value = parameter[pcount2];
                                    Listvar.Add(tmpvar);
                                    //printf("varname %s value %i\n", scanner.scanBuffer.c_str(), parameter[pcount2]);
                                    pcount2++;
                                    //printf("getint %i\n", vec_getint(scanner.scanBuffer));
                                }
                                //ParseLogoProgram();
                                //printf("idx %i\n", scanner.idx);
                                //ParseLogoSentence();
                                bool inprocedure = true;
                                while (scanner.NextToken() != (int)Tokens.END)
                                {
                                    ParseLogoSentence();
                                }
                                inprocedure = false;
                                //Match(TOKEN_END);
                                scanner.idx = oldidx3;
                                for (int i2 = 0; i2 < getvar(procedurename + "_p_a_r_a"); i2++)
                                {
                                    //Listvar.RemoveAt(Listvar.Count);
                                    //printf("pop\n");
                                }
                                //and the counter
                                //Listvar.RemoveAt(Listvar.Count);

                                result = 1;
                                break;
                            }
                        }
                        if (result == 0) ErrorMessage("Parser: GO: no procedure found");


                        //printf("maketest number %i\n", vec_logovar[vec_logovar.size()-1].varvalue);
                        break;
                    }

                case (int)Tokens.REPEAT:
                    Match(nextToken);
                    float numberrecord = numberor();
                    //Match((int)Tokens.NUMBER);
                    Match((int)Tokens.LBRACKET);
                    if (TestingParser) GD.Print("Parser: " + "found sentence REPEAT+LBr+Number");

                    int oldidx = scanner.idx;
                    for (int i = 0; i < numberrecord; i++)
                    {
                        scanner.idx = oldidx;
                       
                        ParseLogoSentence();
              
                        while ((int)scanner.NextToken() != (int)Tokens.RBRACKET)
                        {
                            ParseLogoSentence();
                        }
                        //Match(RBRACKET);
                    }

                    Match((int)Tokens.RBRACKET);
                    if (TestingParser) GD.Print("Parser: " + "found sentence REPEAT+LBr+Number+Sentence+RBr");
                    break;

                default:
                    ErrorMessage("Parser: " + "Expected one of TOKENS but found other...");
                    break;
            }
        }
    }

    bool Match(int token)
    {
        int nextToken = scanner.Scan();
        if (nextToken != token)
        {
            ErrorMessage("Parser: " + "Expected " + Token[token] +" but found " + Token[nextToken]);
            return false;
        }
        return true;
    }
    private void ErrorMessage(string errorMessage)
    {
        GD.Print("Parser: " + "ERROR: " +errorMessage);
    }
}


public partial class Godot3DInterpreter : Node3D
{
    private Window Win1;
	private LineEdit Line;
    private MeshInstance3D Turtle;
    private MeshInstance3D ParentN;
    private MeshInstance3D LineMeshInstance;
    private string Input;
    public FileDialog FileDia;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
  
        TurtleInit();
        Win1 = GetNode<Window>("Window");
		Line = Win1.GetNode<LineEdit>("TextLineEdit");
        ParentN = GetNode<MeshInstance3D>("parent");
        FileDia = GetNode<FileDialog>("FileDialog");

        //GD.Print(Token.REPEAT);
        //string input = "REPEAT 4    [ FORWARD 100    LEFT 90 ] ";

        //DrawLine3D(new Godot.Vector3(0, 0, 0), new Godot.Vector3(10, 0, 0), 
        //    new Godot.Color(1.0f,1.0f,1.0f));

        //Test LogoScanner - Success
        /*
        GD.Print("LogoScannerTest Programm: "+input);
        var scanner = new LogoScanner(input);
        var token = scanner.Scan();
        token = scanner.Scan();
        token = scanner.Scan();
        token = scanner.Scan();
        token = scanner.Scan();
        token = scanner.Scan();
        token = scanner.Scan();
        token = scanner.Scan();
        token = scanner.Scan();
        */

        //Test LogoParser - Success
        //string input = "FORWARD 10   LEFT 45    UP 45   FORWARD 10 ";
        //GD.Print("LogoParserTest Programm: " + input);
        //var parser = new LogoParser(new LogoScanner(input));
        //parser.ParseLogoProgram();
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        if (NewInput)
        {
            NewInput = false;
            GD.Print("New Line Input");

            var parser = new LogoParser(new LogoScanner(NewTextInput), this);
            parser.ParseLogoProgram();
        }

        //if (Input.IsActionPressed("Up"))
        //{
        //    GD.Print("Up Arrow pressed");
        //    Line.Text = OldTextInput;
        //}

    }


    public void TurtleInit()
    {
        penup = false;
        pensize = 1;
        pencolor = new Godot.Color(1.0f, 1.0f, 1.0f);
        pendensity = 255;
        anglex = 0;
        angley = 0;
        mix = 0;
        miy = 0;
        miz = 0;
        TurtlePos.X = 0;
        TurtlePos.Y = 0;
        TurtlePos.Z = 0;
        theta = 0;
        phi = 0;
 
    }

    public void DrawLine3D(Godot.Vector3 begin, Godot.Vector3 end, Godot.Color c)
    {
        MeshInstance3D mi = new MeshInstance3D();
        ImmediateMesh me = new ImmediateMesh();
        StandardMaterial3D mat = new StandardMaterial3D();
        mat.NoDepthTest= true;
        mat.ShadingMode=BaseMaterial3D.ShadingModeEnum.Unshaded;
        mat.VertexColorUseAsAlbedo= true;
        mat.Transparency=BaseMaterial3D.TransparencyEnum.Alpha;
        mi.MaterialOverride= mat;
        mi.RotationEditMode = Node3D.RotationEditModeEnum.Quaternion;
        me.SurfaceBegin(Mesh.PrimitiveType.Lines);
        me.SurfaceSetColor(c);
        me.SurfaceAddVertex(begin);
        me.SurfaceAddVertex(end);
        me.SurfaceEnd();
        mi.Mesh = me;
        ParentN.AddChild(mi);
    }

    public void Remove3D()
    {
        var Childs = ParentN.GetChildren();
        foreach (var c in Childs)
        {
            c.QueueFree();
        }
    }

    public void _on_line_edit_text_submitted(string newtext)
    {
        OldTextInput = NewTextInput;
        Input = newtext;
        GD.Print(Input);

        NewInput = true;
        NewTextInput = Input.ToUpper();
        Line.Text = "";
    }

    public void _on_file_dialog_file_selected(string file)
    {
        GD.Print("selected files is: " + file);
        SelectedFile = file;
        FileDia.Visible = false;

        string textoffile = System.IO.File.ReadAllText(file);
        GD.Print(textoffile);
        //rawContents = "";
        NewTextInput = textoffile;
        NewInput = true;
    }

}
