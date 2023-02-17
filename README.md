# Godot3DInterpreter

WIP - work in progress

a little logo-like interpreter, very buggy, but working (working like an old steammachine, that rattles and steams).
- written in C# with Godot 4.0 rc1

when you start you see the commander - a one line TextEditor. if you enter for example
FORWARD 30
and enter with Return, you see a 3D-line of MeshInstance3D. Then enter
LEFT 90
and again FORWARD 30 and you see the next line. If you enter for example:
REPEAT 4 [ FORWARD 30 LEFT 90 ]
you get a Cube.
If you like to get real programs, type
LOAD
and you can load one of the example-programs in the interpreterprograms-directory. You can take your editor you like and write others and load them with load.




