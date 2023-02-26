using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using static Globals;



class ActivationRecord
{
    public string name;
    public int type;
    public int recursionlevel;
    public int idx;
    public Dictionary<string, string> members = new Dictionary<string, string>();

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
    public static ActivationRecord AR = new ActivationRecord(
            "mainprogram",
       ARTypeProgram,
            1, //recursionlevel
            0 //idx
        );
    public static Stack myStack = new Stack();
    public static List<G3IProc> ListProcedures = new List<G3IProc>();
    public static int recursionlevelnow = 1;
    public static float[] ArgumentArray = new float[42];
    public static bool stoprecursion = false;
    public static bool TestingScanner = false;
    public static bool TestingParser = true;
    //public static bool TurtleMoved = false;
    public static bool NewInput = false;
    public static string NewTextInput = "";
    public static string OldTextInput = "";


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
        "SPHERE",
        "BOX",
        "STOP",
        "PENSIZE",
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
        "SPHERE",
        "BOX",
        "STOP",
        "PENSIZE"
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
        SPHERE=24,
        BOX=25,
        STOP=26,
        PENSIZE=27,
        NUMBER = 28, //from here not reserved
        STRING = 29,
        COMMENT = 30,
        LBRACKET = 31,
        RBRACKET = 32,
        LPARENTHESIS = 33,
        RPARENTHESIS = 34,
        LBRACE=35,
        RBRACE=36,
        PLUS=37,
        HYPHEN=38,
        ASTERISK=39,
        SLASH=40,
        EQUALS=41,
        LESS=42,
        GREATER=43,
        COMMA=44,
        COLON=45,
        ITEM=46,
        EOF =47
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
        SPHERE = 24,
        BOX = 25,
        STOP = 26,
        PENSIZE = 27
    }

}


public class G3IScanner
{
    public string rawContents;
    public string scanBuffer;
    public int idx, lookup;
    public char ch;

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
        if (TestingParser) GD.Print("Parser: " + "VisitProcedureCall: recursionlevelnow= " + recursionlevelnow);
        recursionlevelnow++;
        //name = name + "_"+ recursionlevelnow.ToString();
        AR.idx = i;
        //name = name + "_idx";
        //setvar(AR.name + "_idx", (float)i);
        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
        //if (TestingParser) GD.Print("idx=" + i.ToString() + "   raw idx=" + scanner.rawContents.Substring(i));
        myStack.Push(AR);

        AR = new ActivationRecord(
            name,
            ARTypeProcedure,
            recursionlevelnow, //recursionlevel
            0
        );
    }

    public int LeaveProcedure()
    {
        if (TestingParser) GD.Print("Parser: " + "LeaveProcedure: recursionlevelnow= " + recursionlevelnow);
        recursionlevelnow--;
        AR = null;
        AR = (ActivationRecord)myStack.Pop();
        //while(AR.name != "mainprogram")
        //{
        //    AR = null;
        //    AR = (ActivationRecord)myStack.Pop();
        //}
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

        TurtlePos.X= TurtlePos.X + x;
        TurtlePos.Y= TurtlePos.Y + y;
        TurtlePos.Z= TurtlePos.Z + z;
        IntClass.Turtle.Translate(new Godot.Vector3(x/2, y/2, z/2));
        

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
        phi = phi + angle;
        if (phi < 0) phi = 360 + phi;
        if (phi > 360) phi = phi - 360;
    }
    public void TurtleRight(float angle)
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleDown");
        phi = phi - angle;
        if (phi < 0) phi = 360 + phi;
        if (phi > 360) phi = phi - 360;
    }

    public void TurtleUp(float angle)
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleLeft");
        theta = theta + angle;
        if (theta < 0) theta = 360 + theta;
        if (theta > 360) theta = theta - 360;
    }

    public void TurtleDown(float angle)
    {
        //if (TestingParser) GD.Print("Parser: " + "TurtleRight");
        theta = theta - angle;
        if (theta < 0) theta = 360 + theta;
        if (theta > 360) theta = theta - 360;
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
        IntClass.Turtle.Translate(new Godot.Vector3(-TurtlePos.X/2, -TurtlePos.Y/2, -TurtlePos.Z/2));
        TurtlePos.X = 0;
        TurtlePos.Y = 0;
        TurtlePos.Z = 0;
        
        theta = 0;
        phi = 0;
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
        IntClass.FileDia.Visible = true;
    }


    public float mathcalc()
    {
        int ntok = scanner.NextToken();
        if (ntok == (int)Tokens.RANDOM)
        {
            Match((int)Tokens.RANDOM);
            //printf("in if %s\n", scanner.scanBuffer.c_str());
            int numbertmp = (int)numberor();
            //printf("calc value %i\n", numbertmp);
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

    public int getidx(string procedure)
    {
        for (int i = ListProcedures.Count - 1; i >= 0; i--)
        {

            if (ListProcedures[i].name == procedure)
            {
                return ListProcedures[i].idxstart;
            }
        }
        ErrorMessage("Parser: getidx: no procedure found to get idx");
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
        ErrorMessage("Parser: getstartidx: no procedure found to get startidx");
        return -1;
    }

    public void setvarproc(string procedure, int nr)
    {
        procedure= procedure.ToString();
        for (int i = ListProcedures.Count - 1; i >= 0; i--)
        {

            if (ListProcedures[i].name == procedure)
            {
                for (int j = 0; j < nr; j++)
                {
                    setvar(ListProcedures[i].formalparameter[j], ArgumentArray[j]);
                }
                return;
            }
        }
        ErrorMessage("Parser: setvarproc: no procedure found to set");
    }
    public void setvar(string s, float val)
    //public void setvar(string s, string val)
    { 
        //AR.SetItem(s, val.ToString());
        AR.SetItem(s, val);
        //GD.Print( AR.StrDump());
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
            Random rnd = new Random();
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
            if (nto == (int)Tokens.NUMBER  || nto == (int)Tokens.COLON)
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
        //if (TestingParser) GD.Print("Parser: " + "Start ParseG3IProgram");
        ParseG3ISentence();
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
                case (int)Tokens.PENCOLOR:
                case (int)Tokens.MAKE:
                case (int)Tokens.LOAD:
                case (int)Tokens.PRINT:
                case (int)Tokens.TO:
                case (int)Tokens.GO:
                case (int)Tokens.RANDOMIZE:
                case (int)Tokens.IF:
                case (int)Tokens.FOR:
                case (int)Tokens.SPHERE:
                case (int)Tokens.BOX:
                case (int)Tokens.STOP:
                case (int)Tokens.PENSIZE:
                case (int)Tokens.REPEAT:
                    ParseG3ISentence();
                    break;

                case (int)Tokens.COMMENT:
                    Match((int)Tokens.COMMENT);
                    break;

                case (int)Tokens.END:
                    Match((int)Tokens.END);
                    stoprecursion = true;
                    break;
                case (int)Tokens.EOF:
                default:
                    Match((int)Tokens.EOF);
                    //myStack.Pop();
                    //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                    return;
            }
        }
    }



    private void ParseG3ISentence()
    {
        //if (TestingParser) GD.Print("Parser: " + "Start ParseG3ISentence");
        bool nextbutone = false;
        switch (scanner.NextbutoneToken())
        {
            case (int)Tokens.EQUALS:
                {
                    if (TestingParser) GD.Print("Parser: " + "found Token EQUALS");
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
            //if (TestingParser) GD.Print("Parser: "+"ParseG3ISentence-nextToken"+nextToken.ToString());
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

                case (int)Tokens.PENSIZE:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    thickness = numberor();
                    
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
                
                case (int)Tokens.STOP:
                case (int)Tokens.END:
                    Match(nextToken);
                    stoprecursion = true;
                    if (TestingParser) GD.Print("Parser: " + "found sentence STOP");
                    break;
                
                case (int)Tokens.PENCOLOR:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = numberor();
                    n2 = numberor();
                    n3 = numberor();
                    TurtleSetPenColor(n, n2, n3);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence SETPENCOLOR+N1+N2+N3");
                    break;

                case (int)Tokens.SPHERE:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = numberor();
                    Sphere(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence FORWARD+Number");
                    break;

                case (int)Tokens.BOX:
                    Match(nextToken);
                    //if (!Match((int)Tokens.NUMBER))break;
                    n = numberor();
                    Box(n);
                    //if (TestingParser) GD.Print("Parser: " + "found sentence FORWARD+Number");
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
                
                    Match((int)Tokens.LBRACKET);

                    //ParseG3ISentence();
                    int oldidx2 = scanner.idx;

                    for (int i = numberstart; i < numberend + 1; i = i + numberstep)
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
                    if (TestingParser) GD.Print("Parser: " + "found sentence IF");
                    {
                        int countif = 1;
                        int matchif = 0;
                        Match(nextToken);
                
                        float vecvaltmp = numberor();
                        //int found=vecvaltmp.find(".000000");
                        //if(found !=std::string::npos)
                        //{
                        //	vecvaltmp=vecvaltmp.substr(0,found);
                        //}
                        if (scanner.NextToken() == (int)Tokens.LESS)
                        {
                    
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

                case (int)Tokens.PRINT:
              
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
                            //GD.Print(AR.StrDump());
                            GD.Print("PRINT COLON: " + scanner.scanBuffer);
                            string stringvar = getvarstring(scanner.scanBuffer);
                 
                            GD.Print(stringvar);
                    
                        }
                        else if (scanner.NextToken() == (int)Tokens.STRING)
                        {
                        
                            Match((int)Tokens.STRING);
                            GD.Print(scanner.scanBuffer);
                      
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
                            tmpproc.numberparameter = tmpproc.numberparameter + 1;
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
                        GD.Print("idxstart " + tmpproc.idxstart.ToString());
                        while (scanner.NextToken() != (int)Tokens.END && scanner.NextToken() != (int)Tokens.EOF)
                        {
                            Match(scanner.NextToken());
                            //ParseG3ISentence();
                        }

                        //Match(RBRACKET);
                        //} 

                        Match((int)Tokens.END);
                        int idxend = scanner.idx;
                        //if (TestingParser) GD.Print("Parser: " + "proc= " + scanner.rawContents.Substring(idxbegin, idxend));
                        tmpproc.proc = scanner.rawContents.Substring(idxbegin, idxend)+" ";
                        ListProcedures.Add(tmpproc);
                        
                        if (TestingParser) GD.Print("Parser: " + "found sentence TO name " + tmpproc.name);
                        if (TestingParser) GD.Print("Parser: Procedure: " + getprocbody(tmpproc.name));
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());

                        break;
                    }
                case (int)Tokens.GO:
                    {
                        if (TestingParser) GD.Print("Parser: " + "start GO");
                        
                        //myStack.Push(AR);
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                        Match(nextToken);
                        Match((int)Tokens.STRING);
                        string procedurename = scanner.scanBuffer;

                        
                        int argumentnr = 0;
                        nextt = (int)scanner.NextToken();
                        while (nextt == (int)Tokens.NUMBER || nextt == (int)Tokens.COLON)
                        {
                            if (nextt == (int)Tokens.NUMBER)
                            {
                                float erg = numberor();
                                ArgumentArray[argumentnr] = erg;
                                GD.Print("Parser: GO: argument " + argumentnr.ToString() + " = " + erg);
                                //Match(nextt);
                                nextt = (int)scanner.NextToken();
                                argumentnr++;
                            }
                            else
                            {
                                if (nextt == (int)Tokens.COLON)
                                {
                                    float erg = ParseExpr();
                                    ArgumentArray[argumentnr] = erg;
                                    GD.Print("Parser: GO: argument " + argumentnr.ToString() + " = " + erg);
                                    //Match(nextt);
                                    nextt = (int)scanner.NextToken();
                                    argumentnr++;
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
                        regpattern = regpattern + procedurename;
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
                            if (p != " ")
                            {
                                scanner.rawContents = scanner.rawContents.Insert(0, p);
                                idxold = idxold + p.Length;
                                //scanner.idx = procedurename.Length+4;
                                scanner.idx = getstartidx(procedurename);
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


                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                        myStack.Push(AR);
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

                        if (TestingParser) GD.Print("Parser: starting procedure");
                        bool inprocedure = true;
                        while (scanner.NextToken() != (int)Tokens.END
                            && scanner.NextToken() != (int)Tokens.EOF
                            && stoprecursion != true)
                            //&& AR.recursionlevel < 4)
                        {
                            ParseG3ISentence();
                        }
                        stoprecursion= false;
                        inprocedure = false;
                        if (TestingParser) GD.Print("Parser: procedure ended");
                        //Match(TOKEN_END);
                        //scanner.idx = oldidx3;
                        //AR = null;
                        //AR=(ActivationRecord)myStack.Pop();
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                        //if (TestingParser) GD.Print("Parser: ARdump: " + AR.StrDump());
                        scanner.idx=LeaveProcedure();
                        // --- now old AR is poped back from stack -----------------------------------
                        return;
                
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
    private Window Win1;
	private LineEdit Line;
    public MeshInstance3D Turtle;
    private MeshInstance3D ParentN;
    private MeshInstance3D LineMeshInstance;
    private string Input;
    public FileDialog FileDia;
    public Camera3D Cam;

    public Godot.Vector3 CamDir;


    public float Deg2Rad(float deg)
    {
        return 3.14159265358979f * deg / 180.0f;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
  
        TurtleInit();
        Win1 = GetNode<Window>("Window");
		Line = Win1.GetNode<LineEdit>("TextLineEdit");
        ParentN = GetNode<MeshInstance3D>("parent");
        Turtle = GetNode<MeshInstance3D>("Turtle");
        FileDia = GetNode<FileDialog>("FileDialog");
        Cam = GetNode<Camera3D>("Camera3D");

        Line.GrabFocus();
        GD.Print("\nWELCOME TO GODOT3DINTERPRETER\nPlease type a command in the Commander\nFor example type PRINT \"[HELLO WORLD]");
        //GD.Print(Token.REPEAT);
        //string input = "REPEAT 4    [ FORWARD 100    LEFT 90 ] ";

        //DrawLine3D(new Godot.Vector3(0, 0, 0), new Godot.Vector3(10, 0, 0), 
        //    new Godot.Color(1.0f,1.0f,1.0f));

        //Test G3IScanner - Success
        /*
        GD.Print("G3IScannerTest Programm: "+input);
        var scanner = new G3IScanner(input);
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

        //Test G3IParser - Success
        //string input = "FORWARD 10   LEFT 45    UP 45   FORWARD 10 ";
        //GD.Print("G3IParserTest Programm: " + input);
        //var parser = new G3IParser(new G3IScanner(input));
        //parser.ParseG3IProgram();


    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        if (NewInput)
        {
            NewInput = false;
            GD.Print("New Line Input");

            var parser = new G3IParser(new G3IScanner(NewTextInput), this);
            parser.ParseG3IProgram();
            Line.GrabFocus();
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
            CamDir -= Transform.Basis.Z;
            CamDir = CamDir.Normalized();
            Cam.Translate(CamDir);
        }
        if (Godot.Input.IsKeyPressed(Key.S))
        {
            CamDir += Transform.Basis.Z;
            CamDir = CamDir.Normalized();
            Cam.Translate(CamDir);
        }
        if (Godot.Input.IsKeyPressed(Key.A))
        {
            CamDir -= Transform.Basis.X;
            CamDir = CamDir.Normalized();
            Cam.Translate(CamDir);
        }
        if (Godot.Input.IsKeyPressed(Key.D))
        {
            CamDir += Transform.Basis.X;
            CamDir = CamDir.Normalized();
            Cam.Translate(CamDir);
        }
        if (Godot.Input.IsKeyPressed(Key.Up))
        {
            CamDir += Transform.Basis.Y;
            CamDir = CamDir.Normalized();
            Cam.Translate(CamDir);
        }
        if (Godot.Input.IsKeyPressed(Key.Down))
        {
            CamDir -= Transform.Basis.Y;
            CamDir = CamDir.Normalized();
            Cam.Translate(CamDir);
        }
        if (Godot.Input.IsKeyPressed(Key.Left))
        {
            CamDir -= Transform.Basis.Y;
            CamDir = CamDir.Normalized();
            Cam.RotateY(Deg2Rad(3));
        }
        if (Godot.Input.IsKeyPressed(Key.Right))
        {
            CamDir -= Transform.Basis.Y;
            CamDir = CamDir.Normalized();
            Cam.RotateY(-Deg2Rad(3));
        }
        if (Godot.Input.IsMouseButtonPressed(MouseButton.Right))
        {
            CamDir -= Transform.Basis.Y;
            CamDir = CamDir.Normalized();
            Cam.RotateY(Deg2Rad(3));
        }
        if (Godot.Input.IsMouseButtonPressed(MouseButton.Left))
        {
            CamDir -= Transform.Basis.Y;
            CamDir = CamDir.Normalized();
            Cam.RotateY(Deg2Rad(3));
        }
        if (Godot.Input.IsMouseButtonPressed(MouseButton.WheelUp))
        {
            CamDir = CamDir - Transform.Basis.Z;
            CamDir = CamDir.Normalized();
            Cam.Translate(CamDir);
        }
        if (Godot.Input.IsMouseButtonPressed(MouseButton.WheelDown))
        {
            CamDir = CamDir + Transform.Basis.Z;
            CamDir = CamDir.Normalized();
            Cam.Translate(CamDir);
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
        phi = 0;
 
    }

    public void DrawLine3DThin(Godot.Vector3 begin, Godot.Vector3 end, Godot.Color c)
    {
        // old code for "normal" thin 3d lines:
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

    public void DrawLine3D(Godot.Vector3 begin, Godot.Vector3 end, Godot.Color c, float thickness)
    {
        /* old code for "normal" thin 3d lines:
        MeshInstance3D mi = new MeshInstance3D();
        ImmediateMesh me = new ImmediateMesh();
        StandardMaterial3D mat = new StandardMaterial3D();
        mat.Grow = true;
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
        */
        MeshInstance3D mi = new MeshInstance3D();
        mi.Mesh = new BoxMesh();
        StandardMaterial3D mat = new StandardMaterial3D();
        mat.NoDepthTest = true;
        mat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        //mat.VertexColorUseAsAlbedo = true;
        //mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        mat.AlbedoColor = c;
        //mat.Metallic = 0.85f;
        //mat.Roughness = 0.4f;
        mi.MaterialOverride = mat;
        mi.RotationEditMode = Node3D.RotationEditModeEnum.Quaternion;
        //var newscale = new Godot.Vector3(scale, scale, scale);
        var newscale = new Godot.Vector3(0.3f * thickness, 0.3f * thickness, begin.DistanceTo(end));
        mi.Scale = newscale;
        end.X = end.X + 0.1f; //workaround error from LookAtFromPosition
        mi.LookAtFromPosition((begin + end) / 2, end, Godot.Vector3.Up);
        ParentN.AddChild(mi);
    }


    public void DrawSphere(Godot.Vector3 pos, float scale , Godot.Color c)
    {
        MeshInstance3D mi = new MeshInstance3D();
        mi.Mesh = new SphereMesh();
        StandardMaterial3D mat = new StandardMaterial3D();
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
        ParentN.AddChild(mi);
    }


    public void DrawBox(Godot.Vector3 pos, float scale, Godot.Color c)
    {
        MeshInstance3D mi = new MeshInstance3D();
        mi.Mesh = new BoxMesh();
        StandardMaterial3D mat = new StandardMaterial3D();
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
        //GD.Print(textoffile);
        //rawContents = "";
        textoffile = textoffile.Replace(System.Environment.NewLine, " \n ");
        GD.Print(textoffile);
        NewTextInput = textoffile;
        NewInput = true;
    }

}
