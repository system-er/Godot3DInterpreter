# Godot3DInterpreter

WIP - work in progress

a lowlevel logo-like interpreter to produce 3D-graphics with Godot.    
Working like an old steammachine, that rattles and steams. dont look to the code - some parts are chaos and bugs.         
Its based on a tutorial https://strongminds.dk/artikler/writing-a-small-parser-interpreter-part-1-scanner/   
and from this tutorial https://ruslanspivak.com/lsbasi-part18/ i changed the variable system to AR and stack, procedures with parameter now work.     

- written in C# with Godot 4.0 mono 

example interpreterprogram procedure with PENSIZE 3:    
![Pic1](Godot3DInterpreter/pics/firstpic.JPG)

   
example interpreterprogram spheres:    
![Pic2](Godot3DInterpreter/pics/spheres.JPG)
    
     
example interpreterprogram recursive tree:    
![Pic3](Godot3DInterpreter/pics/tree.jpg)


    
# First start:    
when you start you see the commander - a one line TextEditor. in the middle of the screen you see the "turtle", the 3D-cursor that shows to the direction, you will draw.   

if you enter in the commander for example   
PRINT "[hello world]    
and the string is printed to GODOT-output.    

if you type an expression like 2 * 2, the result will be printed to Godotoutput.

or you type:    
FORWARD 30    
and enter with Return, you see a 3D-line of MeshInstance3D. Then enter   
LEFT 90   
and again FORWARD 30 and you see the next line. If you enter for example:   
REPEAT 4 [ FORWARD 30 LEFT 90 ]   
you get a Cube. you can move camera3D with keys WASD and arrowkeys.       

You can write a procedure if you type TO "X :N   PRINT :N   END    
and can call it from the commander with GO "X 42
and the procedure will write 42 to Godot-output.

If you want interpreterprograms, type LOAD and you can load one of the example-programs in the interpreterprograms-directory. You can write with your editor others and load them with load.   
     
    
   
# Commands 3D (lowercase allowed):   
camera3D movement with Keys WASD, Arrowkeys   
FORWARD number - draw a line forward    
BACK number   
LEFT number - change left in degrees   
RIGHT number   
UP number (cause we are in 3D)   
DOWN number (cause we are in 3D)   
PENUP    
PENDOWN    
PENSIZE number - thickness of 3d lines, normal is 1. now the lines are thin boxes MeshInstance3D.    
HOME   
CLEAN - clear the 3D-lines   
PENCOLOR number number number - the numbers should be 0-255, also you can type RANDOM then a random number is generated    
SPHERE number - draws a sphere (godot MeshInstance3D)   
BOX number - draws a box (godot MeshInstance3D)

   
# Commands interpreter "language" (lowercase allowed):   
; xxx - a comment   
"X - a string   
"[xx xx] - a string with spaces, for example "[hello world]   
MAKE "variablename number - declare a variable, NO GLOBAL variables, get the value with :variablename, change with "X = 7, exampe: MAKE "X 7    PRINT :X   
PRINT string - print   
REPEAT number [ xxx ] - repeat something example: REPEAT 4 [ FORWARD 30 LEFT 90 ]   
FOR varname number number number [ xxx ] - for loop, example: FOR "I 1 3 1 [ PRINT :I ]   
IF xxx ENDIF - if command   
TO procedurename xxx END - a procedure, parameter allowed ( recursion now works, see example ), example TO "PROC :A PRINT :A END    
GO procedurename - start a procedure, parameter allowed, NO nested procedures(proc in prog), example GO "PROC 42     
RANDOM as parameter, if RANDOM alone means 0-255, or RANDOM n for number, for example FORWARD RANDOM 50 means forward 0-49   
math with */+- and parenthesis () behind a = EQUALS TOKEN. or behind procedureparamtervariable example: MAKE "X 0  "X = :X + 7  PRINT :X, now also with float. or math in procedureparameter example: TO "PROC :N   PRINT :N   END ->call-> MAKE "A 2   GO "PROC :A + 1, see also example recursiontest           
      
         
         
# FAQ:   
- why an interpreter, we have gdscript and other languages? for fun and learning c# with godot 4     
- want contact? send me message in godot-forum. you have a nice interpreterprogram with nice results, then please send me?     
  
    
# last changes:       
  01.03.2023: moved to Godot 4.0 RC6, added expressions computed by commander direct for example type in commander 2 + 1   
  02.03.2023: moved to Godot 4.0, added new mesh for turtle that looks to direction    
  04.03.2023: fixed bugs, new interpreterprogram tree.g3i    
  
  
