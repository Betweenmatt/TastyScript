# Table of Contents
* [Introduction](#introduction)
* [Extensions](#extensions)
* [Imports](#imports)
* [Overriding](#overriding)
* [Looping](#looping)
* [Variables](#variables)
* [Order of Operations](#order-of-operations)
 
# Introduction
Welcome to **1.2.2**!

TastyScript is a very simple command based language. Every script comprises of *Functions* which execute in their own scope on command. Functions can be extended with *Extensions* for even more customization. **Every main script(not imported!) must override the Start() function!** Like this:

```
#comments are prepended with the hash symbol!
#this is your entry point into the script
override.Start(){
   PrintLine("Script is starting....");
   TestFunction();
}
function.TestFunction(){
   PrintLine("Hello, World!");
}
```

The example above can be run and would print:

`Script is starting....`

`Hello, World!`

And then complete.

A list of all the functions available to you can be found [Here](/Wiki/Functions.md).

Custom *Functions* can also have parameters to make reusing code much easier!

```
override.Start(){
   Select(12,24);
}
function.Select(x,y){
   Touch(x,y,3000);
   Touch(223,654,3000);
}
```

As you see in the above example, the custom function required an *x* and a *y* parameter, which it then used in the *Touch()* function.

# Extensions

Extensions are a very basic way to add optional functionality to some predefined *Functions* You can find the list of Extensions [Here](/Wiki/Extensions.md). Extensions are prepended with a period\(.\) and, like *Functions*, always require arguments.

```
override.Start(){
   PrintLine("Hello, World").Color("Red");
}
```
The above example would run, and print `Hello, World` in the color red.

# Imports

Reusing code is easier than ever with imports! See the example below
```
Main.ts:

@import Test.ts;
@end
override.Start(){
   Hello("World!");
}
Test.ts:

function.Hello(s){
   PrintLine("Hello ").Concat(s);
}
```

The above example would run and print `Hello World!`. **Note:** The import section *must* be at the beginning of the file, and must end with `@end`. Each import line must begin with `@import` and end with `;`

# Overriding

You can override pre-defined functions with the `override` tag. Overriding is useful for adding additional functionality to the pre-defined functions. Note some functions are `Sealed` and cannot be overriden! If you take a look at the [Functions](/Wiki/Functions.md) page you can see which functions are sealed.

```
override.Start(){
   PrintLine("Hello, World!");
}
override.PrintLine(args[]){
   Print(DateTime).Concat(": ");
   Base(args[]);
}
```

The example above would print `2/20/18 4:46:44: Hello, World!`. **Note:** the override parameters must be `args[]` because the compiler evaluates the arguments differently. Currently you can not change the number of parameters needed in a pre-defined function via overrides.

# Looping

There are a couple ways to have a recursive loop, depending on the results needed. **Note:** *DO NOT* create uncontrolled loops by calling a function into another function! This will cause a stack overflow exception very quickly.
```
*WRONG!!!:*

function.Test1(){
    PrintLine("Test1 is looping");
    Test1();
}
```

Instead either use the `Loop()` function or use the `.For()` extension. Use the `Loop()` function if you want to get the current iteration at runtime. The performance difference between the two functions is the same.

the `Loop()` function requires a `string`, which is the name of the function to Invoke. ***NEW:*** With version 1.2.2 you can alternatively use the [Lambda Expression](/Wiki/LambdaExpressions.md) to create an anonymous function to be invoked. Check out example2:

```
#example1
override.Start(){
   Loop("LoopExample").For(1000);
   ForExample().For(0);
}
function.LoopExample(i){
   PrintLine("I'm looping!").Concat(i).Concat(" Times");
}
function.ForExample(){
   PrintLine("I'm Looping!");
}
#example2, with lambda expression
override.Start(){
    #this performs the same function as the above Loop() example!
    Loop(=>(var i){
        PrintLine("I'm looping!").Concat(i).Concat(" Times");
    }).For(1000);
}
```

# Variables
***NEW:*** Variables are now much more flexible in 1.2.2! You can now assign both local *and* global variables, as well as reassignment in both scopes. 

To assign a local variable use the keyword `var` like so:

`var Variable = "I am a variable";`

To assign a global variable use the `$var` keyword:

`$var GlobalVariable = "I am a global variable!";`

Both assignment and reassignment of variables must be prepended with the `var` or `$var` keyword. Available types are `string`, `number`, another variable, or a [Mathematical Expression](/Wiki/MathExpressions.md).

Variables can be called by just their name. `PrintLine(GlobalVariable)` would print `I am a global variable!`.

# Order of Operations
The Order of Operations, or what I like to call the OoO, works in a predictable and sensible way. The compiler parses scripts with a multi-pass system so functions *don't* have to be created before they're called or vise versa. The position of a function in a script *does* make a difference though, but only when there are multiple functions with the same name.

* The main script is parsed from top to bottom
   1) All Functions are put into the stack
   2) All Overrides are put into the stack
   3) All imports are parsed from top to bottom, following the above two points
   4) All the pre-defined functions are put into the stack
   1) The Awake function(s) are executed in the order of first in the stack
   2) The Start override is executed\
* When a function is called from the stack it is parsed from top to bottom, on a line by line basis.
   1) All strings are found and a token is created
   2) All numbers are found and a token is created
   3) All Mathematical Expressions are determined
   3) All parameters are found and a token is created
   4) All variables are assigned and a token is created
   5) All functions are found and a token is created. The parameters are added to the function token.
   6) All extensions are found and a token is created. The parameters are added to the extension token, and then the extension is added to the function token it is attached to.

Because of the way that the parser finds things, Syntax errors are generally found either immediately, or once the function has started. Compiler errors generally aren't found until they're trying to be called.
   

Here are some examples: 

```
#in this example, First would be printed and Second would be ignored
override.Start(){
    DoubleFunctionExample();
}
function.DoubleFunctionExample(){
    PrintLine("First");
}
function.DoubleFunctionExample(){
    PrintLine("Second");
}
#in this example, Second would be printed before First.
#this is because functions are added to the stack before overrides regardless of
#the order they were created.
override.Start(){
   Print("Hello, World!");
}
override.Print(args[]){
    PrintLine("First");
    PrintLine(args[]);
}
function.Print(arg){
    PrintLine("Second");
    PrintLine(arg);
}
#this example shows the Awake function, and if you were to run it it would print Hello.
function.Awake(){
   Print("H");
}
function.Awake(){
   Print("e");
}
function.Awake(){
   Print("l");
}
function.Awake(){
   Print("l");
}
function.Awake(){
   Print("o");
   PrintLine("");
}
```
