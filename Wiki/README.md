# Table of Contents
* [Introduction](#introduction)
* [Extensions](#extensions)
* [Imports](#imports)
* [Overriding](#overriding)
* [Looping](#looping)
* [Variables](#variables)
* [Order of Operations](#order-of-operations)

# Introduction
Welcome!

TastyScript is a very simple command based language. Every script comprises of *Functions* which execute in their own scope on command. Every **main** script **must override Start()!** (not imports)

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

Extensions are a very basic way to add optional functionality to some predefined *Functions* You can find the list of Extensions [Here](/Wiki/Extensions.md). Extensions are prepended with a period\(.\) and always require arguments.

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

You can override pre-defined functions with the `override` tag. Overriding is useful for adding additional functionality to the pre-defined functions.

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

There are a couple ways to have a recursive loop, depending on the results needed. **Note:** *DO NOT* create uncontrolled loops by calling a function into another function! This will cause a stack overflow exception at around 500 iterations.
```
*WRONG!!!:*

function.Test1(){
    PrintLine("Test1 is looping");
    Test1();
}
```

Instead either use the `Loop()` function with the `.For()` extension, or just use the `.For()` extension. Use the `Loop()` function if you want to get the current iteration at runtime. The performance difference between the two functions is the same.

```
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
```

# Variables
Variable functionality is extremely limited at this point. You can define a variable with a string or number, like so: `var MyNumberVar = 12;`

`var MyStringVar = "Hello, World!";`

Variables are local to the scope they were assigned, and at this time cannot be reassigned.

# Order of Operations
The Order of Operations, or what I like to call the OoO, works in a predictable and sensible way. The compiler parses scripts with a multi-pass system so functions *don't* have to be created before they're called or vise versa. The position of a function in a script *does* make a difference though, but only when there are multiple functions with the same name.

* The main script is parsed from top to bottom
   1) All Functions are put into the stack
   2) All Overrides are put into the stack
   3) All imports are parsed from top to bottom, following the above two points
   4) All the pre-defined functions are put into the stack
* When a function is put into the stack it is parsed from top to bottom, on a line by line basis.
   1) All strings are found and a token is created
   2) All numbers are found and a token is created
   3) All parameters are found and a token is created
   4) All variables are assigned and a token is created
   5) All functions are found and a token is created. The parameters are added to the function token.
   6) All extensions are found and a token is created. The parameters are added to the extension token, and then the extension is added to the function token it is attached to.
* The compiler walks the function stack
   1) The Awake function(s) are executed in the order of first in the stack
   2) The Start override is executed
   3) Every function token that is found is executed by searching the stack for the First function by name, so if there are two `Print()` functions, the one that is added to the stack first will always execute.

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
