using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading;

namespace Godot3DInterpreter;

using static Globals;

public class Stack<T>
{
    public int position => data.Count - 1;
    readonly IList<T> data = new List<T>();
    public void Push(T obj) => data.Add(obj);

    public T Pop()
    {
        var ret = data[position];
        data.RemoveAt(position);
        return ret;
    }

    public override string ToString()
    {
        return $"Num of elements: {data.Count}. {data}";
    }

    public int Count()
    { 
        return data.Count; 
    }

}

class ActivationRecord
{
    public string name;
    public int type;
    public int recursionlevel;
    public int idx;
    public readonly Dictionary<string, string> members = new();

    public ActivationRecord(string n, int t, int nl, int i)
    {
        name = n;
        type = t;
        recursionlevel = nl;
        idx = i;
    }

    public bool ExistItem(string key)
    {
        return members.ContainsKey(key);
    }
    public void SetItem(string key, float value)
    {
        if (members.ContainsKey(key))
        {
            members[key] = value.ToString();
        }
        else
        {
            GD.Print("new Setitem: " + key + " value: " + value);
            members.Add(key, value.ToString());
            GD.Print("memberscount: " + members.Count.ToString());
            GD.Print("value:" + members[key]);
        }
    }

    public void SetItemString(string key, string value)
    {
        if (members.ContainsKey(key))
        {
            members[key] = value.ToString();
        }
        else
        {
            GD.Print("new Setitem: " + key + " value: " + value);
            members.Add(key, value.ToString());
            GD.Print("memberscount: " + members.Count.ToString());
            GD.Print("value:" + members[key]);
        }
    }

    public string GetItem(string key)
    {
        //GD.Print("Getitem: " + key );
        return members[key];
    }

    public int Count()
    {
        return (int)members.Count;
    }

    public string StrDump()
    { 
        string temp;
        temp = "ActivationRecord name: "+ name + "\n"; 
        temp = temp +"ActivationRecord type: "+ type.ToString()+ "\n"; 
        temp = temp +"ActivationRecord recursionlevel: "+ recursionlevel.ToString()+ "\n";
        temp = temp + "ActivationRecord idx: " + idx.ToString() + "\n";
        temp = temp + "memberscount " + members.Count + "\n";

        foreach (string key in members.Keys)
        {
            temp += key + "=" + members[key]+"\n";
        }

        return temp;
    }
}

struct G3IProc
{
    public string name;
    public int vidx;
    public int idxstart;
    public string proc;
    public int numberparameter;
    public List<string> formalparameter;
};


class Globals
{
    public static int ARTypeProgram = 1;
    public static int ARTypeProcedure = 2;

    // ActivationRecord for the mainprogram
    public static ActivationRecord AR = new(
            "mainprogram",
       ARTypeProgram,
            1, //recursionlevel
            0 //idx
        );
    //public static Stack myStack = new Stack();
    public static readonly Stack<ActivationRecord> myStack = new();
    public static readonly List<G3IProc> ListProcedures = new();
    public static int recursionlevelnow = 1;
    public static readonly string[] ArgumentArray = new string[42];
    public static bool stoprecursion = false;
    public static bool endrecursion = false;
    public static bool parsestop = false;
    public static bool TestingScanner = false;
    public static bool TestingParser = true;
    //public static bool TurtleMoved = false;
    public static bool NewInput = false;
    public static bool InputRequest = false;
    public static string NewTextInput = "";
    public static string OldTextInput = "";
    public static string inprocedure = "";

    public static int anglex, angley;
    public static float theta, phi;
    public static Godot.Vector3 TurtlePos, TurtlePosOld;
    public static int mix, miy, miz;
    public static bool penup;
    public static float thickness;
    public static Godot.Color pencolor;
    public static int pendensity = 255;
    public static bool turtlevisible = true;
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
        "PENCOLOR",
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
        "MESH",
        "STOP",
        "PENSIZE",
        "CAMERA",
        "PRINTOUT",
        "ERASE",
        "SLEEP",
        "BACKGROUND",
        "HELP",
        "QUIT",
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
        "PENCOLOR",
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
        "MESH",
        "STOP",
        "PENSIZE",
        "CAMERA",
        "PRINTOUT",
        "ERASE",
        "SLEEP",
        "BACKGROUND",
        "HELP",
        "QUIT"
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
        PENCOLOR=12,
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
        MESH=24,
        STOP=25,
        PENSIZE=26,
        CAMERA=27,
        PRINTOUT=28,
        ERASE=29,
        SLEEP=30,
        BACKGROUND=31,
        HELP=32,
        QUIT=33,
        NUMBER = 34, //from here not reserved
        STRING = 35,
        COMMENT = 36,
        LBRACKET = 37,
        RBRACKET = 38,
        LPARENTHESIS = 39,
        RPARENTHESIS = 40,
        LBRACE=41,
        RBRACE=42,
        PLUS=43,
        HYPHEN=44,
        ASTERISK=45,
        SLASH=46,
        EQUALS=47,
        LESS=48,
        GREATER=49,
        COMMA=50,
        COLON=51,
        ITEM=52,
        EOF =53
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
        PENCOLOR = 12,
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
        FOR = 23,
        MESH = 24,
        STOP = 25,
        PENSIZE = 26,
        CAMERA = 27,
        PRINTOUT = 28,
        ERASE = 29,
        SLEEP = 30,
        BACKGROUND = 31,
        HELP=32,
        QUIT=33
    }

}



public class SemanticAnalyser
{
    public Dictionary<string, string> symbols = new();
    G3IScanner scanner;
    public SemanticAnalyser(G3IScanner sc)
    {
        scanner = sc;
    }
    public void Analyse()
    {
        if (TestingParser) GD.Print("SemanticAnalyer: started");

        int nexttoken;

        //variable declarations
        nexttoken = (int)scanner.NextToken();
        while (nexttoken != (int)Tokens.EOF)
        {
            switch (nexttoken)
            {
                case (int)Tokens.MAKE:
                    Match((int)Tokens.MAKE);
                    if (scanner.NextToken() == (int)Tokens.STRING)
                    {
                        Match((int)Tokens.STRING);
                        string foundstring = scanner.scanBuffer;
                        //GD.Print("variablename:" + foundstring);
                        SetSymbol(foundstring, "VARIABLE");
                    }
                    else GD.Print("SemanticAnalyer: ERROR: " + "MAKE: wrong parameter");
                    //nexttoken = (int)scanner.NextToken();
                    //Match(nexttoken);
                    break;
                case (int)Tokens.TO:
                    {
                        Match((int)Tokens.TO);
                        string procedurename;
                        string foundstring;
                        nexttoken = (int)scanner.NextToken();
                        if (nexttoken == (int)Tokens.STRING)
                        {
                            Match((int)Tokens.STRING);
                            procedurename = scanner.scanBuffer;
                            //GD.Print("procedurename:" + foundstring);
                            SetSymbol(procedurename, "PROCEDURE");

                            while ((int)scanner.NextToken() == (int)Tokens.COLON)
                            {

                                //if (TestingParser) GD.Print("SemanticAnalyer: TO: found COLON and variable: " + scanner.scanBuffer);
                                SetSymbol(procedurename + "." + scanner.scanBuffer, "VARIABLE");
                                Match((int)Tokens.COLON);

                            }
                        }
                        else GD.Print("SemanticAnalyer: ERROR: " + "TO: wrong parameter");


                        while ((int)scanner.NextToken() != (int)Tokens.END)
                        {
                            Match(nexttoken);
                            nexttoken = (int)scanner.NextToken();
                        }
                        Match((int)Tokens.END);
                        break;
                    }
                default:
                    Match(nexttoken);
                    break;
            }
            nexttoken = (int)scanner.NextToken();
            //GD.Print("\n");
        }


        //variable references
        scanner.idx = 0;
        nexttoken = (int)scanner.NextToken();
        while (nexttoken != (int)Tokens.EOF)
        {
            inprocedure = "";
            switch (nexttoken)
            {
                case (int)Tokens.COLON:
                    Match((int)Tokens.COLON);

                    string foundstring = scanner.scanBuffer;
                    //GD.Print("variablename:" + foundstring);
                    string result = GetSymbol(foundstring);
                    //nexttoken = (int)scanner.NextToken();
                    //Match(nexttoken);

                    break;

                case (int)Tokens.TO:
                    {
                        Match((int)Tokens.TO);
                        nexttoken = (int)scanner.NextToken();
                        if (nexttoken == (int)Tokens.STRING)
                        {
                            Match((int)Tokens.STRING);
                            inprocedure = scanner.scanBuffer;
                            //GD.Print("procedurename:" + foundstring);
                            //GetSymbol(procedurename, "PROCEDURE");

                            while ((int)scanner.NextToken() == (int)Tokens.COLON)
                            {

                                //if (TestingParser) GD.Print("SemanticAnalyer: TO: found COLON and variable: " + scanner.scanBuffer);
                                //SetSymbol(procedurename + "." + scanner.scanBuffer, "VARIABLE");
                                Match((int)Tokens.COLON);

                            }
                        }
                        else GD.Print("SemanticAnalyer: ERROR: " + "TO: wrong parameter");


                        while ((int)scanner.NextToken() != (int)Tokens.END)
                        {
                            //Match(nexttoken);

                            switch (nexttoken)
                            {
                                case (int)Tokens.COLON:
                                    Match((int)Tokens.COLON);

                                    string foundstring2 = scanner.scanBuffer;
                                    //GD.Print("variablename:" + foundstring);
                                    GetSymbol(inprocedure + "." + foundstring2);
                                    //nexttoken = (int)scanner.NextToken();
                                    //Match(nexttoken);

                                    break;
                                default:
                                    Match(nexttoken);
                                    break;
                            }
                            nexttoken = (int)scanner.NextToken();
                        }
                        Match((int)Tokens.END);
                        break;
                    }


                default:
                    Match(nexttoken);
                    break;
            }
            nexttoken = (int)scanner.NextToken();
            //GD.Print("\n");
        }
    }


    public void SetSymbol(string key, string value)
    {
        if (symbols.ContainsKey(key))
        {
            symbols[key] = value.ToString();
        }
        else
        {
            if (TestingParser) GD.Print("new SetSymbols: " + key + " value: " + value);
            symbols.Add(key, value.ToString());
            if (TestingParser) GD.Print("symbolscount: " + symbols.Count.ToString());
            //GD.Print("value:" + symbols[key]);
        }
    }

    public string GetSymbol(string key)
    {
        if (symbols.ContainsKey(key))
        {
            return symbols[key];
        }
        else
        {
            if (TestingParser) GD.Print("SemanticAnalyer: ERROR: " + "missing variabledeclaraion variable: " + key);
            return null;
        }
    }

    bool Match(int token)
    {
        int nextToken = scanner.Scan();
        if (nextToken != token)
        {
            GD.Print("Parser: " + "Expected " + Token[token] +" but found " + Token[nextToken]);
            return false;
        }
        return true;
    }
}



public class G3IScanner
{
    public string rawContents;
    public string scanBuffer;
    public int idx, lookup;
    public char ch, nextch;

    public G3IScanner(string input)
    {
        rawContents = input;
        idx = 0;
    }


    public int Scan()
    {
        while (idx < rawContents.Length)
        {
            ch = rawContents[idx];
            if (idx+1 < rawContents.Length) nextch = rawContents[idx+1];

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
            else if (ch == '-' && !char.IsDigit(nextch))
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
                if (ch is '[' and not '\n')
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
                    //if (TestingScanner) GD.Print("Scanner: found STRING");
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
                //if (TestingScanner) GD.Print("Scanner: found STRING");
                return (int)Tokens.STRING;
            }
            else if (char.IsDigit(ch) || ch == '-')
            {
                scanBuffer = ch.ToString();
                idx++;
                while (idx < rawContents.Length)
                {
                    ch = rawContents[idx];
                    if (char.IsDigit(ch) || ch == '.')
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
        parsestop = true;
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
        if (TestingScanner) GD.Print("LookupReserved: count " + ListProcedures.Count.ToString());
        for (int j = 0; j < ListProcedures.Count; j++)
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

    private void LexicalError() => 
        GD.Print("Scanner: " + "Lexical error at " + ch + " " + scanBuffer);

    private void SyntaxError(string s) => GD.Print("Scanner: " + "SyntaxError: " + s);
    private void Error(string s) => GD.Print("Scanner: " + "ERROR: " + s);
}


public class G3IParser
{
    public G3IScanner scanner;
    public Godot3DInterpreter IntClass;
    //private ActivationRecord AR;

    public G3IParser(G3IScanner g3iScanner, Godot3DInterpreter IC)
    {
        scanner = g3iScanner;
        IntClass = IC;
    }



    public void VisitProcedureCall(string name, int i)
    {
        //myStack.Push(AR);

        if (TestingParser) GD.Print("Parser: " + "VisitProcedureCall: recursionlevelnow= " + recursionlevelnow);
        recursionlevelnow++;
        //name = name + "_"+ recursionlevelnow.ToString();
        AR.idx = i;
        //name = name + "_idx";
        //setvar(AR.name + "_idx", (float)i);
        if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
        //if (TestingParser) GD.Print("idx=" + i.ToString() + "   raw idx=" + scanner.rawContents.Substring(i));
        myStack.Push(AR);
        AR = null;
        AR = new ActivationRecord(
            name,
            ARTypeProcedure,
            recursionlevelnow, //recursionlevel
            0
        );
        //if (TestingParser) GD.Print("Parser: pushed the stack nr objects:" + myStack.Count);
    }

    public int LeaveProcedure()
    {
        if(parsestop) return scanner.scanBuffer.Length; ;
        if (TestingParser) GD.Print("Parser: " + "LeaveProcedure: recursionlevelnow= " + recursionlevelnow);
 
        //if (myStack.Count == 0)
        //{
        //    if (TestingParser) GD.Print("Parser: LeaveProcedure: myStack empty, leaving");
        //    return scanner.scanBuffer.Length;
        //}

        recursionlevelnow--;
        AR = null;
        if (myStack.Count() == 0) return scanner.scanBuffer.Length;
        AR = (ActivationRecord)myStack.Pop();
        if (TestingParser) GD.Print("Parser: popped the stack nr objects:" + myStack.Count().ToString());

        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
        //int idx = (int)getvar(AR.name + "_idx");
        int idx = AR.idx;
        //if (TestingParser) GD.Print("idx=" + idx.ToString()+ "   raw idx=" + scanner.rawContents.Substring(idx));

        return idx;
    }

    public int LeaveProcedureStop()
    {
        if (TestingParser) GD.Print("Parser: " + "LeaveProcedureStop: recursionlevelnow= " + recursionlevelnow);
        //if (myStack.Count == 0)
        //{
        //    if (TestingParser) GD.Print("Parser: LeaveProcedureStop: myStack empty, leaving");
        //    return scanner.scanBuffer.Length;
        //}

        recursionlevelnow--;
        AR = null;
        AR = (ActivationRecord)myStack.Pop();

        if (TestingParser) GD.Print("Parser: popped the stack nr objects:"+ myStack.Count().ToString());
        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
        //int idx = (int)getvar(AR.name + "_idx");
        int idx = AR.idx;
        //if (TestingParser) GD.Print("idx=" + idx.ToString()+ "   raw idx=" + scanner.rawContents.Substring(idx));

        return idx;
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
        //if (TestingParser) GD.Print("Parser: "+"TurtleForward");
        float x, y, z;

        TurtlePosOld = TurtlePos;

        x = dist * (float)Math.Cos(Deg2Rad(phi)) * (float)Math.Cos(Deg2Rad(theta));
        y = dist * (float)Math.Sin(Deg2Rad(phi)) * (float)Math.Cos(Deg2Rad(theta));
        z = dist * (float)(Math.Sin(Deg2Rad(theta)));

        TurtlePos.X+= x;
        TurtlePos.Y+= y;
        TurtlePos.Z+= z;
        //IntClass.Turtle.Translate(new Godot.Vector3(x/2, y/2, z/2));
        //IntClass.Turtle.Translate(new Godot.Vector3(x, y, z));
        IntClass.turtle.Position = TurtlePos;
        //TurtleMoved = true;
        if (!penup) IntClass.DrawLine3D(TurtlePosOld, TurtlePos, pencolor, thickness);

    }


    public void TurtleBack(float dist)
    {
        //if (TestingParser) GD.Print("Parser: "+"TurtleBack");
        TurtleForward(-dist);
    }

    public void TurtleLeft(float angle)
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleUp");
        phi += angle;
        if (phi < 0) phi = 360 + phi;
        if (phi > 360) phi -= 360;

        
        IntClass.turtle.RotateZ(Deg2Rad(angle));
    }

    public void TurtleRight(float angle)
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleDown");
        phi -= angle;
        if (phi < 0) phi = 360 + phi;
        if (phi > 360) phi -= 360;

        IntClass.turtle.RotateZ(-Deg2Rad(angle));
    }

    public void TurtleUp(float angle)
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleLeft");
        theta += angle;
        if (theta < 0) theta = 360 + theta;
        if (theta > 360) theta -= 360;

        IntClass.turtle.RotateX(Deg2Rad(angle));
    }

    public void TurtleDown(float angle)
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleRight");
        theta -= angle;
        if (theta < 0) theta = 360 + theta;
        if (theta > 360) theta -= 360;

        IntClass.turtle.RotateX(-Deg2Rad(angle));
    }

    public void Sphere(float s)
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleRight");
        IntClass.DrawSphere(new Godot.Vector3(TurtlePos.X / s, TurtlePos.Y / s, TurtlePos.Z / s), s, pencolor);
    }

    public void Box(float s)
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleRight");
        IntClass.DrawBox(new Godot.Vector3(TurtlePos.X / s, TurtlePos.Y / s, TurtlePos.Z / s), s, pencolor);
    }


    public void TurtleHome()
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleHome");
        penup = false;
        //thickness = 1.0;
        pencolor = new Godot.Color(1.0f, 1.0f, 1.0f);
        //pendensity = 255;
        anglex = 0;
        angley = 0;
        mix = 0;
        miy = 0;
        miz = 0;
        //IntClass.Turtle.Translate(new Godot.Vector3(-TurtlePos.X/2, -TurtlePos.Y/2, -TurtlePos.Z/2));
        TurtlePos.X = 0;
        TurtlePos.Y = 0;
        TurtlePos.Z = 0;
        
        theta = 0;
        phi = 90;

        IntClass.turtle.Position = TurtlePos;
        IntClass.turtle.Rotation = Godot.Vector3.Zero;
    }

    public void TurtleClean()
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleClean");
        IntClass.Remove3D();
        
    }

    public void TurtlePenUp()
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtlePenUp");
        penup = true; 
    }

    public void TurtlePenDown()
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtlePenDown");
        penup = false;
    }

    public void TurtleSetPenColor(float c1, float c2, float c3)
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleSetPenColor");
        pencolor = new Godot.Color(c1/255, c2/255, c3/255);
    }

    public void LoadProgram()
    {
        //if (TestingParser) GD.Print("Parser: " + "LoadProgram");
        IntClass.fileDia.Visible = true;
    }


    public float mathcalc()
    {
        int ntok = scanner.NextToken();
        if (ntok == (int)Tokens.RANDOM)
        {
            Match((int)Tokens.RANDOM);
            int numbertmp = (int)numberor();
            //vec_setvar(stringcalc,rand()%numbertmp);
            //float numbertmp2 = rand() % numbertmp;
            //return numbertmp2;
        }
        return 0.0f;
    }

 

    float getvar(string s)
    {
        //for (int i = Listvar.Count() - 1; i >= 0; i--)
        //for (int i = ARProgram.Count() - 1; i >= 0; i--)
        //{
            //if (Listvar[i].name == s)
        if (AR.ExistItem(s))
        {
            //return Listvar[i].value.ToFloat();
            return AR.GetItem(s).ToFloat();
        }
        //}
        ErrorMessage("Parser: getvar: no variable found to get value");
        return -1;
    }

    public string getvarstring(string s)
    {
        //for (int i = Listvar.Count - 1; i >= 0; i--)
        //{
         
        if (AR.ExistItem(s))
        {
            return AR.GetItem(s);
        }
        //}
        ErrorMessage("Parser: getvarstring: no variable found to get value");
        return "0";
    }

    public string getprocbody(string procedure)
    {
        for (int i = ListProcedures.Count - 1; i >= 0; i--)
        {
            
            if (ListProcedures[i].name == procedure)
            {
                return ListProcedures[i].proc;
            }
        }
        ErrorMessage("Parser: getprocbody: no procedure found to get body");
        return " ";
    }

    public void removeproc(string procedure)
    {
        for (int i = ListProcedures.Count - 1; i >= 0; i--)
        {

            if (ListProcedures[i].name == procedure)
            {
                ListProcedures.RemoveAt(i);
                return;
            }
        }
        ErrorMessage("Parser: removeproc: no procedure found to get body");
    }

    public string getallprocbody()
    {
        string tmp = "";
        for (int i=0; i < ListProcedures.Count; i++)
        {
            tmp=tmp + ListProcedures[i].proc +"\n";
        }
        return tmp;
    }


    public int getidx(string procedure)
    {
        for (int i = ListProcedures.Count - 1; i >= 0; i--)
        {

            if (ListProcedures[i].name == procedure)
            {
                return ListProcedures[i].vidx;
            }
        }
        ErrorMessage("Parser: getidx: no procedure found to get vidx");
        return -1;
    }

    public int getprocparanr(string procedure)
    {
        for (int i = ListProcedures.Count - 1; i >= 0; i--)
        {

            if (ListProcedures[i].name == procedure)
            {
                return ListProcedures[i].numberparameter;
            }
        }
        ErrorMessage("Parser: getprocparanr: no procedure found to get numberparameter");
        return -1;
    }

    public int getstartidx(string procedure)
    {
        for (int i = ListProcedures.Count - 1; i >= 0; i--)
        {

            if (ListProcedures[i].name == procedure)
            {
                return ListProcedures[i].idxstart;
            }
        }
        ErrorMessage("Parser: getstartidx: no procedure found to get idxstart");
        return -1;
    }

    public void setvarproc(string procedure, int nr)
    {
        //procedure= procedure.ToString();
        for (int i = ListProcedures.Count - 1; i >= 0; i--)
        {

            if (ListProcedures[i].name == procedure)
            {
                for (int j = 0; j < nr; j++)
                {
                    setvarstring(ListProcedures[i].formalparameter[j], ArgumentArray[j]);
                }
                return;
            }
        }
        ErrorMessage("Parser: setvarproc: no procedure found to set");
    }
    public void setvar(string item, float val)
    //public void setvar(string s, string val)
    { 
        //AR.SetItem(s, val.ToString());
        AR.SetItem(item, val);
        //GD.Print( AR.StrDump());
    }

    public void setvarstring(string item, string str)
    //public void setvar(string s, string val)
    {
        //AR.SetItem(s, val.ToString());
        AR.SetItemString(item, str);
        //GD.Print( AR.StrDump());
    }

    public string getstrorvalue()
    {
        int ntok = scanner.NextToken();
        if (ntok is (int)Tokens.STRING or (int)Tokens.NUMBER)
        {
            Match(ntok);
            return scanner.scanBuffer;
        }
        else if (ntok == (int)Tokens.COLON)
        {
            Match((int)Tokens.COLON);
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
            //GD.Print("numberor - found number: " + scanner.scanBuffer);
            Match((int)Tokens.NUMBER);
            //GD.Print("scanbuffer:" + scanner.scanBuffer);
            return float.Parse(scanner.scanBuffer, CultureInfo.InvariantCulture);
        }
        else if (scanner.NextToken() == (int)Tokens.HYPHEN)
        {
            //GD.Print("numberor - found -number: " + scanner.scanBuffer);
            Match((int)Tokens.HYPHEN);
            Match((int)Tokens.NUMBER);
            return -scanner.scanBuffer.ToFloat();
        }
        else if (scanner.NextToken() == (int)Tokens.COLON)
        {
            //GD.Print("numberor - found COLON and variable: " + scanner.scanBuffer);
            Match((int)Tokens.COLON);
       
            float returnvalue;
            string vartmp = getvarstring(scanner.scanBuffer);
            char ch = vartmp[0];
            if (Char.IsLetter(ch)) returnvalue = ReturnASCIISum(vartmp);
            else returnvalue = vartmp.ToFloat();
            return returnvalue;
        }
        else if (scanner.NextToken() == (int)Tokens.STRING)
        {
            //GD.Print("numberor - found string: " + scanner.scanBuffer);
            Match((int)Tokens.STRING);
            return ReturnASCIISum(scanner.scanBuffer);
        }
        else if (scanner.NextToken() == (int)Tokens.RANDOM)
        {
            Random rnd = new();
            Match((int)Tokens.RANDOM);
            int nto = scanner.NextToken();
            if (nto == (int)Tokens.NUMBER)
            {
                //if (TestingParser) GD.Print("Parser: " + "numberor RANDOM with NUMBER");
                Match((int)Tokens.NUMBER);
                return (float)rnd.Next(int.Parse(scanner.scanBuffer));
            }
            else
            {
                //if (TestingParser) GD.Print("Parser: " + "numberor RANDOM");
                return (float)rnd.Next(255);
            }
        }
        else ErrorMessage("Parser: numberor: expected number or variable");
        return 0.0f;
    }

    float ParseExpr()
    {
        //if (TestingParser) GD.Print("Parser: " + "Start ParseExpr");
        float op, op1;
        op = ParseFactor();
      
        int nto = scanner.NextToken();
        if (nto != (int)Tokens.EOF)
        {
            if (nto == (int)Tokens.PLUS)
            {
                Match((int)Tokens.PLUS);
                op1 = ParseExpr();
                op += op1;
            }
            else if (nto == (int)Tokens.HYPHEN)
            {
                Match((int)Tokens.HYPHEN);
                op1 = ParseExpr();
                op -= op1;
            }
        }
        return op;
    }

    float ParseFactor()
    {
        //if (TestingParser) GD.Print("Parser: " + "Start ParseFactor");
        float op, op1;
        op = ParseTerm();
       
        int nto = scanner.NextToken();
        if (nto != (int)Tokens.EOF)
        {
            if (nto == (int)Tokens.ASTERISK)
            {
                Match((int)Tokens.ASTERISK);
                op1 = ParseFactor();
                op *= op1;
            }
            else if (nto == (int)Tokens.SLASH)
            {
                Match((int)Tokens.SLASH);
                op1 = ParseFactor();
                op /= op1;
            }
        }
        return op;
    }

    float ParseTerm()
    {
        //if (TestingParser) GD.Print("Parser: " + "Start ParseTerm");
        float returnValue = 0;
        int nto = scanner.NextToken();
        if (nto != (int)Tokens.EOF)
        {
            //if (nto==(int)Tokens.PLUS ||nto==(int)Tokens.HYPHEN)
            //{
            //	Match(nto);
            //	return ParseExpr();
            //}
            if (nto is ((int)Tokens.NUMBER) or ((int)Tokens.COLON))
            {
                return numberor();
            }
            else if (nto == (int)Tokens.RANDOM)
            {
                Random rnd = new();
                Match((int)Tokens.RANDOM);
                int nto2 = scanner.NextToken();
                if (nto2 == (int)Tokens.NUMBER)
                {
                    //if (TestingParser) GD.Print("Parser: " + "numberor RANDOM with NUMBER");
                    Match((int)Tokens.NUMBER);
                    return (float)rnd.Next(int.Parse(scanner.scanBuffer));
                }
                else
                {
                    //if (TestingParser) GD.Print("Parser: " + "numberor RANDOM");
                    return (float)rnd.Next(255);
                }
            }
            else if (nto == (int)Tokens.LPARENTHESIS)
            {
                
                Match((int)Tokens.LPARENTHESIS);
                returnValue = ParseExpr();
               
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
                ErrorMessage("Parser: ParseTerm: math expr parser: not expected token found. scanner.scanBuffer "+ scanner.scanBuffer+ "   raw:"+scanner.rawContents+"   idx:"+scanner.idx.ToString());
            }
        }
        return returnValue;
    }


    public void ParseG3IProgram()
    {
        if (TestingParser) GD.Print("Parser: " + "Start ParseG3IProgram");
        GD.Print(scanner.rawContents);
        //IntClass.PrintLabel("Parser: " + "Start ParseG3IProgram");
        ParseG3ISentence();
        while (true && !parsestop)
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
                case (int)Tokens.PENCOLOR:
                case (int)Tokens.MAKE:
                case (int)Tokens.LOAD:
                case (int)Tokens.PRINT:
                case (int)Tokens.TO:
                case (int)Tokens.GO:
                case (int)Tokens.RANDOMIZE:
                case (int)Tokens.IF:
                case (int)Tokens.FOR:
                case (int)Tokens.MESH:
                case (int)Tokens.STOP:
                case (int)Tokens.END:
                case (int)Tokens.PENSIZE:
                case (int)Tokens.REPEAT:
                case (int)Tokens.CAMERA:
                case (int)Tokens.PRINTOUT:
                case (int)Tokens.ERASE:
                case (int)Tokens.SLEEP:
                case (int)Tokens.BACKGROUND:
                case (int)Tokens.HELP:
                case (int)Tokens.QUIT:
                    ParseG3ISentence();
                    break;

                case (int)Tokens.COMMENT:
                    Match((int)Tokens.COMMENT);
                    break;

                case (int)Tokens.EOF:
                    //if (TestingParser) GD.Print("Parser: found Token EOF");
                    Match((int)Tokens.EOF);
                    parsestop = true;
                    return;
                default:
                    Match((int)Tokens.EOF);
                    parsestop = true;
                    //myStack.Pop();
                    //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                    return;
            }
        }
    }

    private void ParseG3ISentence()
    {
        if (parsestop) return;
        //if (TestingParser) GD.Print("Parser: " + "Start ParseG3ISentence");
        bool nextbutone = false;
        switch (scanner.NextbutoneToken())
        {
            case (int)Tokens.EQUALS:
                {
                    //if (TestingParser) GD.Print("Parser: " + "found Token EQUALS");
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
                //Match((int)Tokens.EOF);
                //return;
                break;
        }

        if (!nextbutone)
        {
            var nextToken = scanner.NextToken();
            float n, n2, n3;
            string scanstring;
            //if (TestingParser) GD.Print("Parser: "+"ParseG3ISentence-nextToken"+nextToken.ToString());
            switch (nextToken)
            {
                case (int)Tokens.NUMBER:
                case (int)Tokens.COLON:
                    //Match((int)Tokens.NUMBER);
                    float result2 = ParseExpr();
                    IntClass.PrintLabel("found Expression Result=" + result2.ToString());
                    break;

                case (int)Tokens.COMMENT:
                    //float result2 = ParseExpr();
                    IntClass.PrintLabel("found Token COMMENT=" + scanner.scanBuffer);
                    Match((int)Tokens.COMMENT);
                    break;

                case (int)Tokens.STRING:
                    IntClass.PrintLabel("found Token STRING=" + scanner.scanBuffer);
                    Match(nextToken);
                    break;

                case (int)Tokens.FORWARD:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    //n = numberor();
                    n = ParseExpr();
                    TurtleForward(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence FORWARD+Number");
                    break;

                case (int)Tokens.BACK:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = ParseExpr();
                    TurtleBack(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence BACK+Number");
                    break;

                case (int)Tokens.LEFT:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = ParseExpr();
                    TurtleLeft(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence LEFT+Number");
                    break;

                case (int)Tokens.RIGHT:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = ParseExpr();
                    TurtleRight(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence RIGHT+Number");
                    break;

                case (int)Tokens.UP:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = ParseExpr();
                    TurtleUp(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence UP+Number");
                    break;

                case (int)Tokens.DOWN:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = ParseExpr();
                    TurtleDown(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence DOWN+Number");
                    break;

                case (int)Tokens.PENSIZE:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    thickness = ParseExpr();

                    //if (TestingParser) GD.Print("Parser: " + "found sentence DOWN+Number");
                    break;

                case (int)Tokens.HOME:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    TurtleHome();
                    //if (TestingParser) GD.Print("Parser: " + "found sentence HOME");
                    break;

                case (int)Tokens.CLEAN:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    TurtleClean();
                    //if (TestingParser) GD.Print("Parser: " + "found sentence CLEAN");
                    break;

                case (int)Tokens.PENUP:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    TurtlePenUp();
                    //if (TestingParser) GD.Print("Parser: " + "found sentence PENUP");
                    break;

                case (int)Tokens.PENDOWN:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    TurtlePenDown();
                    //if (TestingParser) GD.Print("Parser: " + "found sentence PENDOWN");
                    break;
                
                case (int)Tokens.STOP:
                    Match(nextToken);
                    stoprecursion = true;
                    if (TestingParser) GD.Print("Parser: " + "found sentence STOP");
                    break;

                case (int)Tokens.END:
                    Match(nextToken);
                    endrecursion = true;
                    if (TestingParser) GD.Print("Parser: " + "found sentence END");
                    break;
                
                case (int)Tokens.PENCOLOR:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = ParseExpr();
                    n2 = ParseExpr();
                    n3 = ParseExpr();
                    TurtleSetPenColor(n, n2, n3);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence SETPENCOLOR+N1+N2+N3");
                    break;

                case (int)Tokens.BACKGROUND:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = ParseExpr();
                    n2 = ParseExpr();
                    n3 = ParseExpr();
                    IntClass.setbackgroundcolor(Godot.Color.Color8((byte)n, (byte)n2, (byte)n3));
                    //if (TestingParser) GD.Print("Parser: " + "found sentence SETPENCOLOR+N1+N2+N3");
                    break;

                case (int)Tokens.CAMERA:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = ParseExpr();
                    n2 = ParseExpr();
                    n3 = ParseExpr();

                    Godot.Vector3 campos = IntClass.cam.Position;
                    campos.X = n;
                    campos.Y = n2;
                    campos.Z = n3;
                    IntClass.cam.Translate(campos);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence SETPENCOLOR+N1+N2+N3");
                    break;

                case (int)Tokens.PRINTOUT:
                    Match(nextToken);
                    scanstring = "ALL";
                    if (scanner.NextToken() == (int)Tokens.STRING)
                    {
                        Match((int)Tokens.STRING);
                        scanstring = scanner.scanBuffer;
                        //GD.Print("Parser: found string: " + scanner.scanBuffer);
                    }
                    else ErrorMessage("Parser: " + "PRINTOUT: wrong parameter");
                    if (scanstring == "ALL")
                    {
                        IntClass.PrintLabel(getallprocbody());
                    }
                    else
                    {
                        IntClass.PrintLabel(getprocbody(scanstring));
                    }
                    break;

                case (int)Tokens.ERASE:
                    Match(nextToken);
                    scanstring = "ALL";
                    if (scanner.NextToken() == (int)Tokens.STRING)
                    {
                        Match((int)Tokens.STRING);
                        scanstring = scanner.scanBuffer;
                        //GD.Print("Parser: found string: " + scanner.scanBuffer);
                    }
                    else ErrorMessage("Parser: " + "PRINTOUT: wrong parameter");
                    if (scanstring == "ALL")
                    {
                        ListProcedures.Clear();
                        IntClass.PrintLabel("Parser: Erased all procedures.");
                    }
                    else
                    {
                        removeproc(scanstring);
                        IntClass.PrintLabel("Parser: Erased procedure " + scanstring);
                    }
                    break;

                case (int)Tokens.SLEEP:
                    Match(nextToken);
                    n = ParseExpr();
                    Thread.Sleep( Convert.ToInt32(n));
                    break;

                case (int)Tokens.MESH:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    scanstring = "SPHERE";
                    if (scanner.NextToken() == (int)Tokens.STRING)
                    {
                        Match((int)Tokens.STRING);
                        scanstring = scanner.scanBuffer;
                        GD.Print("Parser: found string: "+scanner.scanBuffer);
                    }
                    else ErrorMessage("Parser: " + "MESH: wrong parameter");
                    n = ParseExpr();
                    IntClass.DrawMesh(new Godot.Vector3(TurtlePos.X / n, TurtlePos.Y / n, TurtlePos.Z / n), n, pencolor, scanstring);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence FORWARD+Number");
                    break;

                case (int)Tokens.LOAD:
                    Match(nextToken);
                    //if (!Match((int)Tokens.STRING)) break;
                    LoadProgram();
                    //if (TestingParser) GD.Print("Parser: " + "found sentence LOAD");
                    break;

                case (int)Tokens.FOR:
                    //if (TestingParser) GD.Print("Parser: " + "found sentence FOR");
                    Match(nextToken);
                    Match((int)Tokens.STRING);
                    string varname = scanner.scanBuffer;
                    //Match(TOKEN_NUMBER);
                    int numberstart = (int)ParseExpr();
                    int numberend = (int)ParseExpr();
                    int numberstep = (int)ParseExpr();

                    Match((int)Tokens.LBRACKET);

                    //ParseG3ISentence();
                    int oldidx2 = scanner.idx;

                    for (int i = numberstart; i < numberend + 1; i += numberstep)
                    {
                        setvar(varname, i);
                        scanner.idx = oldidx2;
              
                        while (scanner.NextToken() != (int)Tokens.RBRACKET)
                        {
                    
                            ParseG3ISentence();
                        }
                        //Match(RBRACKET);
                    }
                    Match((int)Tokens.RBRACKET);
                    break;
                    
                case (int)Tokens.IF:
                    //if (TestingParser) GD.Print("Parser: " + "found sentence IF");
                    {
                        int countif = 1;
                        int matchif = 0;
                        Match(nextToken);
                
                        float vecvaltmp = ParseExpr();
                        //int found=vecvaltmp.find(".000000");
                        //if(found !=std::string::npos)
                        //{
                        //	vecvaltmp=vecvaltmp.substr(0,found);
                        //}
                        if (scanner.NextToken() == (int)Tokens.LESS)
                        {
                    
                            Match((int)Tokens.LESS);
                            //float vecvaltmp2=numberorvalue();
                            float vecvaltmp2 = ParseExpr();
                            //int found=vecvaltmp2.find(".000000");
                            //if(found !=std::string::npos)
                            //{
                            //	vecvaltmp2=vecvaltmp2.substr(0,found);
                            //}
                            if (vecvaltmp < vecvaltmp2)
                            {
                                //Match(TOKEN_LBRACKET);
                                ParseG3ISentence();
                                while (scanner.NextToken() != (int)Tokens.ENDIF)
                                {
                                    ParseG3ISentence();
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
                         
                            Match((int)Tokens.GREATER);
                            //float vecvaltmp2=numberorvalue();
                            float vecvaltmp2 = numberor();
                      
                            if (vecvaltmp > vecvaltmp2)
                            {
               
                                ParseG3ISentence();
                                while (scanner.NextToken() != (int)Tokens.ENDIF)
                                {
                                    ParseG3ISentence();
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
                        
                            Match((int)Tokens.EQUALS);
                            //float vecvaltmp2=numberorvalue();
                            float vecvaltmp2 = numberor();
                      
                            if (vecvaltmp == vecvaltmp2)
                            {
                              
                                //Match(TOKEN_LBRACKET);
                                ParseG3ISentence();
                                while (scanner.NextToken() != (int)Tokens.ENDIF)
                                {
                                    ParseG3ISentence();
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
                 
                        //Match(TOKEN_RBRACKET);
                        break;
                    }

                case (int)Tokens.HELP:
                    Match(nextToken);
                    //if (!Match((int)Tokens.STRING)) break;
                    IntClass.PrintLabel("HELP: commander-commands:");
                    IntClass.PrintLabel("HELP: LOAD //load an interpreterprogram *.g3i");
                    IntClass.PrintLabel("HELP: PRINTOUT string //prints procedure, with \"ALL prints all");
                    IntClass.PrintLabel("HELP: ERASE string //erases procedure, with \"ALL erases all");
                    IntClass.PrintLabel("HELP: HELP //show this helpmessages");
                    IntClass.PrintLabel("HELP: QUIT //quit program");
                    //IntClass.PrintLabel("HELP: Godot3Dinterpretercommands");
                    
                    //if (TestingParser) GD.Print("Parser: " + "found sentence LOAD");
                    break;

                case (int)Tokens.QUIT:
                    Match(nextToken);
                    //if (!Match((int)Tokens.STRING)) break;
                    IntClass.GetTree().Quit();
                    //if (TestingParser) GD.Print("Parser: " + "found sentence LOAD");
                    break;

                case (int)Tokens.PRINT:
              
                    Match(nextToken);

                    var nextt = scanner.NextToken();
                    while (nextt is ((int)Tokens.COMMA) or ((int)Tokens.COLON) or ((int)Tokens.STRING)) //|| nextt == (int)Tokens.ITEM
                    {
                        if (nextt == (int)Tokens.COMMA)
                        {
                            Match((int)Tokens.COMMA);
                            nextt = scanner.NextToken();
                        }
                        if (nextt == (int)Tokens.NUMBER) // || nextt == TOKEN_ITEM)
                        {
                      
                            float erg = numberor();
                            //if (erg == floor(erg))
                            //{
                            IntClass.PrintLabel(erg.ToString());
                            //}
                            //else
                            //{
                            //    GD.Print(erg);
                            //}
                        }
                        else if (nextt == (int)Tokens.COLON)
                        {
                            Match((int)Tokens.COLON);
                            //GD.Print(AR.StrDump());
                            IntClass.PrintLabel("PRINT COLON: " + scanner.scanBuffer);
                            string stringvar = getvarstring(scanner.scanBuffer);

                            IntClass.PrintLabel(stringvar);
                    
                        }
                        else if (scanner.NextToken() == (int)Tokens.STRING)
                        {
                        
                            Match((int)Tokens.STRING);
                            //GD.Print(scanner.scanBuffer);
                            IntClass.PrintLabel(scanner.scanBuffer);
                      
                        }
                        else ErrorMessage("Parser: "+"PRINT: wrong parameter");
                        nextt = scanner.NextToken();
                    }
                    //GD.Print("\n");
                    break;

                case (int)Tokens.MAKE:
                    Match(nextToken);
                    Match((int)Tokens.STRING);
                    string arrayname = scanner.scanBuffer;
               
                    int nextto = scanner.NextToken();
                    if (nextto == (int)Tokens.LBRACE)//array
                    {
          
                    }
                    else if (nextto == (int)Tokens.NUMBER)
                    {
                        Match((int)Tokens.NUMBER);

                        if (TestingParser) GD.Print("Parser: " + "found sentence MAKE+NUMBER "+ arrayname+" "+scanner.scanBuffer);
                        //GD.Print("scanbuffer:" + scanner.scanBuffer);
                        //setvar(arrayname, float.Parse(scanner.scanBuffer));
                        setvar(arrayname, float.Parse(scanner.scanBuffer, CultureInfo.InvariantCulture));
                        //setvar(arrayname, scanner.scanBuffer.ToFloat());

                        break;
                    }
                    else if (nextto == (int)Tokens.STRING)
                    {
                        Match((int)Tokens.STRING);

                        if (TestingParser) GD.Print("Parser: " + "found sentence MAKE+STRING " + arrayname + " " + scanner.scanBuffer);
                        //GD.Print("scanbuffer:" + scanner.scanBuffer);
                        //setvar(arrayname, float.Parse(scanner.scanBuffer));
                        setvarstring(arrayname, scanner.scanBuffer);
                        //setvar(arrayname, scanner.scanBuffer.ToFloat());

                        break;
                    }
                    break;

                case (int)Tokens.TO:
                    {
                        if (TestingParser) GD.Print("Parser: " + "start TO");
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                        int idxbegin = scanner.idx;
                        Match(nextToken);
                        Match((int)Tokens.STRING);
                        G3IProc tmpproc;
                        tmpproc.name = scanner.scanBuffer;
                        tmpproc.vidx = scanner.idx;
                        tmpproc.numberparameter = 0;
                        tmpproc.formalparameter = new List<string>();
                        while ((int)scanner.NextToken() == (int)Tokens.COLON)
                        {
                            //GD.Print("Parser: TO: found COLON "+scanner.scanBuffer);

                            //while ((int)scanner.NextToken() == (int)Tokens.STRING)
                            //{
                            //Match((int)Tokens.STRING);
                            if (TestingParser) GD.Print("Parser: TO: found COLON and variable: " + scanner.scanBuffer);
                            tmpproc.numberparameter++;
                            tmpproc.formalparameter.Add(scanner.scanBuffer);
                            //}
                            Match((int)Tokens.COLON);

                            //nextt = scanner.NextToken();
                        }
                        if (TestingParser && tmpproc.numberparameter>0)
                        {
                            for (int i = 0; i < tmpproc.numberparameter; i++)
                            {
                                GD.Print("parameterlist " + i.ToString() + " " + tmpproc.formalparameter[i]);
                            }
                        }
                        tmpproc.idxstart = scanner.idx;
                        GD.Print("procedurename:"+tmpproc.name+ "   idxstart " + tmpproc.idxstart.ToString());
                        while (scanner.NextToken() is not (int)Tokens.END and not (int)Tokens.EOF)
                        {
                            Match(scanner.NextToken());
                            //ParseG3ISentence();
                        }

                        //Match(RBRACKET);
                        //} 

                        Match((int)Tokens.END);
                        int idxend = scanner.idx;
                        //if (TestingParser) GD.Print("Parser: " + "proc= " + scanner.rawContents.Substring(idxbegin, idxend));
                        //GD.Print("idxbegin:"+idxbegin.ToString()+"   idxend:"+idxend.ToString());
                        tmpproc.proc = scanner.rawContents.Substring(idxbegin, idxend-idxbegin)+" ";
                        GD.Print("procedure:" + tmpproc.proc);
                        ListProcedures.Add(tmpproc);
                        
                        if (TestingParser) GD.Print("Parser: " + "found sentence TO name " + tmpproc.name);
                        //if (TestingParser) GD.Print("Parser: Procedure: " + getprocbody(tmpproc.name));
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());

                        break;
                    }
                case (int)Tokens.GO:
                    {
                        if (TestingParser) GD.Print("Parser: " + "start GO");
                        if (parsestop) return;
                        //myStack.Push(AR);
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                        Match(nextToken);
                        Match((int)Tokens.STRING);
                        string procedurename = scanner.scanBuffer;

                        
                        int argumentnr = 0;
                        nextt = (int)scanner.NextToken();
                        while (nextt is ((int)Tokens.NUMBER) or ((int)Tokens.COLON) or ((int)Tokens.STRING))
                        {
                            if (nextt == (int)Tokens.NUMBER)
                            {
                                float erg = ParseExpr();
                                ArgumentArray[argumentnr] = erg.ToString();
                                GD.Print("Parser: GO: argument " + argumentnr.ToString() + " = " + erg.ToString());
                                //Match(nextt);
                                nextt = (int)scanner.NextToken();
                                argumentnr++;
                            
                            }
                            else
                            {
                                if (nextt == (int)Tokens.COLON)
                                {
                                    float erg = ParseExpr();
                                    ArgumentArray[argumentnr] = erg.ToString();
                                    GD.Print("Parser: GO: argument " + argumentnr.ToString() + " = " + erg.ToString());
                                    //Match(nextt);
                                    nextt = (int)scanner.NextToken();
                                    argumentnr++;
                                }
                                else
                                {
                                    if (nextt == (int)Tokens.STRING)
                                    {
                                        Match((int)Tokens.STRING);
                                        string stri = scanner.scanBuffer;
                                        ArgumentArray[argumentnr] = stri;
                                        GD.Print("Parser: GO: argument " + argumentnr.ToString() + " = " + stri);
                                        //Match(nextt);
                                        nextt = (int)scanner.NextToken();
                                        argumentnr++;
                                    }
                                }
                            }
                        
                        }
                        

                        if (TestingParser) GD.Print("Parser: " + "found sentence GO name " + procedurename);
                        if ( argumentnr != getprocparanr(procedurename))
                        {
                            ErrorMessage("Parser: GO procedure - number formalparameter to arguments not equal");
                            break;
                        }
                        int idxold = scanner.idx;

                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                        // --- now old AR is on stack -----------------------------------
                        //myStack.Push(AR);
                        //VisitProcedureCall(procedurename);
                        //setvarproc(procedurename, argumentnr);
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                          
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());

                        //if (TestingParser) GD.Print("Parser: raw: "+ scanner.rawContents);
                        
                        string regpattern = @"\bTO\s*""";
                        regpattern += procedurename;
                        //if (TestingParser) GD.Print("Parser: regpattern: " +regpattern);
                        string regresult = Regex.Match(scanner.rawContents, regpattern).ToString();
                        //if (TestingParser) GD.Print("Parser: regresult: " + regresult+ "   length: "+regresult.Length.ToString());
                        //if (TestingParser) GD.Print("regexsearch: " + Regex.Match(scanner.rawContents, regpattern).Index.ToString());


                        if (regresult.Length > 0 )
                        {
                            if (TestingParser) GD.Print("Parser: " + "found procedure in raw " + procedurename);
                            //scanner.idx = Regex.Match(scanner.rawContents, regpattern).Index + regresult.Length;
                            scanner.idx = getidx(procedurename);
                        }
                        else
                        {
                            if (TestingParser) GD.Print("Parser: " + "found no procedure in raw " + procedurename);
                            string p = getprocbody(procedurename);
                            if (TestingParser) GD.Print("Parser: procbody: " + p);
                            if (p != " ")
                            {
                                scanner.rawContents = scanner.rawContents.Insert(0, p);
                                idxold += p.Length;
                                //scanner.idx = procedurename.Length+4;
                                scanner.idx = getstartidx(procedurename);
                                //scanner.idx = getidx(procedurename);
                                if (TestingParser) GD.Print("Parser: idx=" + scanner.idx);
                                
                                //if (TestingParser) GD.Print("scanner.rawcontents: " + scanner.rawContents);
                            }
                            else
                            {
                                ErrorMessage("Parser: GO procedure - no procedure with name "+procedurename+ " found.");
                                //scanner.idx = oldidx3;
                                //AR = null;
                                //AR = (ActivationRecord)myStack.Pop();
                                //scanner.idx = LeaveProcedure();
                                break;
                            }
                        }
                        parsestop = false;

                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                        //myStack.Push(AR);

                        // --- now old AR is pushed on stack -----------------------------------
                        VisitProcedureCall(procedurename, idxold);
                        setvarproc(procedurename, argumentnr);
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());

                        /*
                        if( AR.recursionlevel > 3)
                        {
                            if (TestingParser) GD.Print("Parser: STOP procedure");
                            scanner.idx = LeaveProcedure();
                            if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                            stopprocedure= false;
                            return;
                        }
                        */

                        //if (TestingParser) GD.Print("Parser: starting procedure");
                        nextt = scanner.NextToken();
                        //if (TestingParser) GD.Print("Parser: nextt: "+nextt);
                        //if (TestingParser) GD.Print("Parser: "+stoprecursion.ToString()+" "+endrecursion.ToString()+" "+parsestop.ToString());
                        while (//scanner.NextToken() != (int)Tokens.END &&
                            nextt != (int)Tokens.EOF
                            && stoprecursion != true
                            && endrecursion != true
                            && !parsestop
                            )
                            //&& AR.recursionlevel < 4)
                        {
                            //if (TestingParser) GD.Print("Parser: starting ParseG3ISentence");
                            ParseG3ISentence();
                            nextt = scanner.NextToken();
                        }
                        //if (TestingParser) GD.Print("Parser: ending procedure");
                        
                        
                        //if (TestingParser) GD.Print("Parser: procedure ended");
                        //Match(TOKEN_END);
                        //scanner.idx = oldidx3;
                        //AR = null;
                        //AR=(ActivationRecord)myStack.Pop();
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
    
                        if (stoprecursion)
                        {
                            scanner.idx = LeaveProcedureStop();
                            //scanner.idx = LeaveProcedure();
                            //if (TestingParser) GD.Print("Parser: procedure STOP");
                        }
                        else
                        {
                            scanner.idx = LeaveProcedure();
                            //myStack.Clear();
                            //if (TestingParser) GD.Print("Parser: procedure ended");
                        }
                        
                        stoprecursion = false;
                        endrecursion = false;
                        // --- now old AR is poped back from stack -----------------------------------
                        return;
                        //break;
                    }

                case (int)Tokens.REPEAT:
                    Match(nextToken);
                    float numberrecord = numberor();
                    //Match((int)Tokens.NUMBER);
                    Match((int)Tokens.LBRACKET);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence REPEAT+LBr+Number");

                    int oldidx = scanner.idx;
                    for (int i = 0; i < numberrecord; i++)
                    {
                        scanner.idx = oldidx;
                       
                        ParseG3ISentence();
              
                        while ((int)scanner.NextToken() != (int)Tokens.RBRACKET)
                        {
                            ParseG3ISentence();
                        }
                        //Match(RBRACKET);
                    }

                    Match((int)Tokens.RBRACKET);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence REPEAT+LBr+Number+Sentence+RBr");
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
    private Window win1;
    private Window winoutput;
    private LineEdit line;
    private RichTextLabel outputlabel;
    public MeshInstance3D turtle;
    private MeshInstance3D parentN;
    private MeshInstance3D lineMeshInstance;
    private string input;
    public FileDialog fileDia;
    public Camera3D cam;
    public Godot.Vector3 camDir;



    public float Deg2Rad(float deg)
    {
        return 3.14159265358979f * deg / 180.0f;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
  
        TurtleInit();
        winoutput = GetNode<Window>("Output");
        win1 = GetNode<Window>("Window");
        outputlabel = winoutput.GetNode<RichTextLabel>("RichTextLabel");
        line = win1.GetNode<LineEdit>("TextLineEdit");
        parentN = GetNode<MeshInstance3D>("parent");
        turtle = GetNode<MeshInstance3D>("Turtle");
        fileDia = GetNode<FileDialog>("FileDialog");
        cam = GetNode<Camera3D>("Camera3D");
        
        //GD.Print("\nWELCOME TO GODOT3DINTERPRETER\nPlease type a command in the Commander\nFor example type PRINT \"[HELLO WORLD]\nmove camera3d with ASWD and arrowkeys\n");
        PrintLabel( "\nWELCOME TO GODOT3DINTERPRETER\nPlease type a command in the Commander\nFor example type PRINT \"[HELLO WORLD]\nmove camera3d with ASWD and arrowkeys\n");
        line.GrabFocus();
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        line.GrabFocus();
        if (NewInput)
        {
            NewInput = false;
            //GD.Print("New Line Input");
            //PrintLabel("New Line Input");

            NewTextInput = NewTextInput.ToUpper();

            var scanner = new G3IScanner(NewTextInput);
            var analyser = new SemanticAnalyser(scanner);
            analyser.Analyse();

            recursionlevelnow = 1;
            stoprecursion = false;
            endrecursion = false;
            parsestop = false;
            var parser = new G3IParser(new G3IScanner(NewTextInput), this);
            try
            {
                var t = new Thread(delegate () { parser.ParseG3IProgram(); });
                t.Start();
                //parser.ParseG3IProgram();
            }
            catch (Exception e)
            {
                GD.Print("exception caught: "+ e.ToString());
            }
            
            line.GrabFocus();
        }

        //if (Input.IsActionPressed("Up"))
        //{
        //    GD.Print("Up Arrow pressed");
        //    Line.Text = OldTextInput;
        //}

    }

    public override void _Input(InputEvent Inp)
    {
        if (Godot.Input.IsKeyPressed(Key.W))
        {
            camDir -= Transform.Basis.Z;
            camDir = camDir.Normalized();
            cam.Translate(camDir);
        }
        if (Godot.Input.IsKeyPressed(Key.S))
        {
            camDir += Transform.Basis.Z;
            camDir = camDir.Normalized();
            cam.Translate(camDir);
        }
        if (Godot.Input.IsKeyPressed(Key.A))
        {
            camDir -= Transform.Basis.X;
            camDir = camDir.Normalized();
            cam.Translate(camDir);
        }
        if (Godot.Input.IsKeyPressed(Key.D))
        {
            camDir += Transform.Basis.X;
            camDir = camDir.Normalized();
            cam.Translate(camDir);
        }
        if (Godot.Input.IsKeyPressed(Key.Up))
        {
            camDir += Transform.Basis.Y;
            camDir = camDir.Normalized();
            cam.Translate(camDir);
        }
        if (Godot.Input.IsKeyPressed(Key.Down))
        {
            camDir -= Transform.Basis.Y;
            camDir = camDir.Normalized();
            cam.Translate(camDir);
        }
        if (Godot.Input.IsKeyPressed(Key.Left))
        {
            camDir -= Transform.Basis.Y;
            camDir = camDir.Normalized();
            cam.RotateY(Deg2Rad(3));
        }
        if (Godot.Input.IsKeyPressed(Key.Right))
        {
            camDir -= Transform.Basis.Y;
            camDir = camDir.Normalized();
            cam.RotateY(-Deg2Rad(3));
        }
        if (Godot.Input.IsMouseButtonPressed(MouseButton.Right))
        {
            camDir -= Transform.Basis.Y;
            camDir = camDir.Normalized();
            cam.RotateY(Deg2Rad(3));
        }
        if (Godot.Input.IsMouseButtonPressed(MouseButton.Left))
        {
            camDir -= Transform.Basis.Y;
            camDir = camDir.Normalized();
            cam.RotateY(Deg2Rad(3));
        }
        if (Godot.Input.IsMouseButtonPressed(MouseButton.WheelUp))
        {
            camDir -= Transform.Basis.Z;
            camDir = camDir.Normalized();
            cam.Translate(camDir);
        }
        if (Godot.Input.IsMouseButtonPressed(MouseButton.WheelDown))
        {
            camDir += Transform.Basis.Z;
            camDir = camDir.Normalized();
            cam.Translate(camDir);
        }
    }

    //public override void _PhysicsProcess(double delta)
    //{
    //}


    public void TurtleInit()
    {
        penup = false;
        thickness = 1.0f;
        pencolor = new Godot.Color(1.0f, 1.0f, 1.0f);
        //pendensity = 255;
        anglex = 0;
        angley = 0;
        mix = 0;
        miy = 0;
        miz = 0;
        TurtlePos.X = 0;
        TurtlePos.Y = 0;
        TurtlePos.Z = 0;
        //Turtle.Translate(new Godot.Vector3(TurtlePos.X, TurtlePos.Y, TurtlePos.Z));
        theta = 0;
        phi = 90;
 
    }

    public void DrawLine3DThin(Godot.Vector3 begin, Godot.Vector3 end, Godot.Color c)
    {
        // old code for "normal" thin 3d lines:
        MeshInstance3D mi = new();
        ImmediateMesh me = new();
        StandardMaterial3D mat = new()
        {
            NoDepthTest = true,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            VertexColorUseAsAlbedo = true,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha
        };
        mi.MaterialOverride= mat;
        mi.RotationEditMode = Node3D.RotationEditModeEnum.Quaternion;
        me.SurfaceBegin(Mesh.PrimitiveType.Lines);
        me.SurfaceSetColor(c);
        me.SurfaceAddVertex(begin);
        me.SurfaceAddVertex(end);
        me.SurfaceEnd();
        mi.Mesh = me;
        parentN.AddChild(mi);
    }

    public void DrawLine3D(Godot.Vector3 begin, Godot.Vector3 end, Godot.Color c, float thickness)
    {
        MeshInstance3D mi = new()
        {
            Mesh = new BoxMesh()
        };
        StandardMaterial3D mat = new()
        {
            NoDepthTest = true,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = c
            //VertexColorUseAsAlbedo = true,
            //Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            //Metallic = 0.85f,
            //Roughness = 0.4f
    };

        //mat.
        //mat.
        //mat.
        //mat.
        mi.MaterialOverride = mat;
        mi.RotationEditMode = Node3D.RotationEditModeEnum.Quaternion;
        //var newscale = new Godot.Vector3(scale, scale, scale);
        var newscale = new Godot.Vector3(0.3f * thickness, 0.3f * thickness, begin.DistanceTo(end));
        mi.Scale = newscale;
        end.X += 0.1f; //workaround error from LookAtFromPosition
        mi.LookAtFromPosition((begin + end) / 2, end, Godot.Vector3.Up);
        parentN.AddChild(mi);
    }


    public void DrawSphere(Godot.Vector3 pos, float scale , Godot.Color c)
    {
        MeshInstance3D mi = new()
        {
            Mesh = new SphereMesh()
        };
        StandardMaterial3D mat = new();
        //mat.NoDepthTest = true;
        //mat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        //mat.VertexColorUseAsAlbedo = true;
        //mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        mat.AlbedoColor = c;
        mat.Metallic = 0.85f;
        mat.Roughness = 0.4f;
        mi.MaterialOverride = mat;
        mi.RotationEditMode = Node3D.RotationEditModeEnum.Quaternion;
        var newscale = new Godot.Vector3(scale, scale, scale);
        mi.Scale = newscale;
        mi.Translate(pos);
        parentN.AddChild(mi);
    }


    public void DrawBox(Godot.Vector3 pos, float scale, Godot.Color c)
    {
        MeshInstance3D mi = new();
        mi.Mesh = new BoxMesh();
        StandardMaterial3D mat = new();
        //mat.NoDepthTest = true;
        //mat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        //mat.VertexColorUseAsAlbedo = true;
        //mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        mat.AlbedoColor = c;
        mat.Metallic = 0.85f;
        mat.Roughness = 0.4f;
        mi.MaterialOverride = mat;
        mi.RotationEditMode = Node3D.RotationEditModeEnum.Quaternion;
        var newscale = new Godot.Vector3(scale, scale, scale);
        mi.Scale = newscale;
        mi.Translate(pos);
        parentN.AddChild(mi);
    }

    public void DrawMesh(Godot.Vector3 pos, float scale, Godot.Color c, string meshstring)
    {
        MeshInstance3D mi = new();
        meshstring = meshstring.ToUpper();
        meshstring = meshstring.Trim();
        GD.Print("DrawMesh:" + meshstring);
        mi.Mesh = meshstring switch
        {
            "CAPSULE" => new CapsuleMesh(),
            "CYLINDER" => new CylinderMesh(),
            "BOX" => new BoxMesh(),
            "QUAD" => new QuadMesh(),
            "PLANE" => new PlaneMesh(),
            "PRISM" => new PrismMesh(),
            "SPHERE" => new SphereMesh(),
            "TORUS" => new TorusMesh(),
            _ => new SphereMesh(),
        };
        StandardMaterial3D mat = new();
        //mat.NoDepthTest = true;
        //mat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        //mat.VertexColorUseAsAlbedo = true;
        //mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        mat.AlbedoColor = c;
        mat.Metallic = 0.85f;
        mat.Roughness = 0.4f;
        mi.MaterialOverride = mat;
        mi.RotationEditMode = Node3D.RotationEditModeEnum.Quaternion;
        var newscale = new Godot.Vector3(scale, scale, scale);
        mi.Scale = newscale;
        mi.Translate(pos);
        mi.RotateX(Deg2Rad(theta));
        mi.RotateZ(Deg2Rad(phi));
        parentN.AddChild(mi);
    }


    public void Remove3D()
    {
        var Childs = parentN.GetChildren();
        foreach (var c in Childs)
        {
            c.QueueFree();
        }
    }


    public void _on_line_edit_text_submitted(string newtext)
    {
        OldTextInput = NewTextInput;
        input = newtext;
        //GD.Print(input);
        PrintLabel(input);

        NewInput = true;
        NewTextInput = input;
        //NewTextInput = Input.ToUpper();
        line.Text = "";
    }

    public void _on_file_dialog_file_selected(string file)
    {
        //GD.Print("selected files is: " + file);
        PrintLabel("selected files is: " + file+ "\n");
        SelectedFile = file;
        fileDia.Visible = false;

        string textoffile = System.IO.File.ReadAllText(file);
        //GD.Print(textoffile);
        //rawContents = "";
        textoffile = textoffile.Replace(System.Environment.NewLine, " \n ");
        //GD.Print(textoffile);
        PrintLabel(textoffile);
        NewTextInput = textoffile;
        NewInput = true;
    }

    public void setbackgroundcolor(Godot.Color color)
    {
        RenderingServer.SetDefaultClearColor(color);
    }

    public void PrintLabel(string s)
    {
        outputlabel.Text = outputlabel.Text + s +"\n";
    }
}
