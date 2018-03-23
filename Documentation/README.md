# Table of Contents
* [Introduction](#introduction)
* [Extensions](#extensions)
* [Imports](#imports)
* [Overriding](#overriding)
* [Looping](#looping)
* [Variables](#variables)
* [Order of Operations](#order-of-operations)
 
# Introduction
Welcome to **1.3.2**! Please note the documentation is currently being developed. I am trying my best to keep up with each change and new feature so please bear with me!

TastyScript is a simple programming language which uses top level *functions* to execute lower level *commands*. Functions can call other functions, and you can *override* functions to build on top of their functionality with a modular approach.

Every script must have exactly one `Start()` function which is the entry point into your script.

```
function.Start(){
	PrintLine("Hello, World!");
}
```

# Extensions

Extensions are a very basic way to add optional functionality to some [Predefined Functions](/Functions.md) You can find the list of Extensions [Here](/Extensions.md). Extensions are prepended with a period\(.\) and, like *Functions*, always require arguments.

```
function.Start(){
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
override.PrintLine(args){
   Print(DateTime).Concat(": ");
   Base(args);
}
```

The example above would print `2/20/18 4:46:44: Hello, World!`. Another example:

```
override.Touch(x,y){
	var out = "X:" + x + " Y:" + y;
	PrintLine(out);
	Base(x,y);
}
```

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

the `Loop()` function requires a `string`, which is the name of the function to Invoke. ***NEW:*** With version 1.2.2+ you can alternatively use the [Lambda Expression](/Wiki/LambdaExpressions.md) to create an anonymous function to be invoked. Check out example2:

```
#example1
override.Start(){
   Loop("LoopExample").For(1000);#loops 1000 times
   ForExample().For(0);#infinite loop
   Loop("LoopExample");#infinite loop
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

With v1.3.0 you can now use `Break()` and `Continue()` inside of loops, to skip iterations or break from the loop completely.

# Variables
***NEW:*** Variables are now much more flexible in 1.2.2+! You can now assign both local *and* global variables, as well as reassignment in both scopes. 

To assign a local variable use the keyword `var` like so:

`var Variable = "I am a variable";`

To assign a global variable use the `$var` keyword:

`$var GlobalVariable = "I am a global variable!";`

With 1.3.0 you can also assign a functions return value, as well as concatenation via the `+` operand!

```
override.Start(){
    var version = VersionString();
    PrintLine(version);
    #alternatively you can input the function directly in the PrintLine parameters!
    PrintLine(VersionString());
}
function.VersionString(){
    var version = "V" + GetVersion.GetItem(0) + "."
		+ GetVersion.GetItem(1) + "."
		+ GetVersion.GetItem(2) + "."
		+ GetVersion.GetItem(3);
	Return(version);
}
```

Both assignment and reassignment of variables must be prepended with the `var` or `$var` keyword. Available types are `string`, `number`, another variable, or a [Mathematical Expression](/Wiki/MathExpressions.md).

Variables can be called by just their name. `PrintLine(GlobalVariable)` would print `I am a global variable!`.

# Return
Like many other programming languages, TastyScript supports returning. `Return()` has many uses; the main use is to return a value:

```
function.Start(){
	PrintLine(TestReturn());
}
function.TestReturn(){
	Return("Hello, World!");
}
```

As you can see, the value gets passed back to where the function was called. You can call `Return()` with no arguments, and `null` will be returned. You don't even need to use the return value!

```
function.Start(){
	Loop(=>(var i){
		If(i > 12).Then(=>(){
			Return();
		});
		PrintLine("i is less than 12");
	});
}
```

# Order of Operations
The Order of Operations, or what I like to call the OoO, works in a predictable and sensible way. The compiler first gets all the functions from your script and assigns them for later reference. Then it proceeds with the `Awake()` and then the `Start()` function, going line by line evaluating your commands. 

The order a line of code is parsed to find the correct token:

1) Mathematical expressions\*
1) Arrays\*\*
2) Parameters\*\*
4) Followed by strings then numbers
5) Extensions and last, functions.

*\* Mathematical Expressions search for any variables before evaluating the expression*

*\*\*Arrays and Parameters parse somewhat recursively, allowing you to put functions and extensions directly in parameters instead of assigning variables for each one*

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
