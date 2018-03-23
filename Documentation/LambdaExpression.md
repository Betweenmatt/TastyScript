# Lambda Expression

The `Lambda Expression` is based off of the C# equivalent. It is a simple and easy to read way to include anonymous functions into your scripts. Without the `Lamba Expression` you have to resort to invoking functions via a string name of the function like so:

```
override.Start(){
    Loop("LoopTest");
}
function.LoopTest(i){
    PrintLine("I'm an infinate loop!");
    Sleep(1000);
}
```

Since `Lambda Expressions` have been introduced, you can replace the above code with this, and have the exact same functionality and performance:

```
override.Start(){
    Loop(=>(var i){
        PrintLine("I'm an infinate loop!");
        Sleep(1000);
    });
}
```

Now that's a lot easier to manage than the first example! Note the `var i` declaration in the parameters. Currently the only function that requires this is the `Loop()` function. This is similar to the old way to use `Loop()` where you had to provide a parameter for the current iteration; In `Lamba Expressions` that parameter needs to be defined. Let's take a look at conditional functions to see why...

```
override.Start(){
    var mathExp = [2 * 2];
    If(mathExp >= 2)
        .Then(=>(mathExp){
            PrintLine("Math Expression is: ").Concat(mathExp);
        });
}
```

As you can see from the arbitrary example above, `Lambda Expressions` perform the action of defining *and* providing arguments. To use a value from the parent scope in `Lambda Expression`, you must pass it via the arguments `=>(arg1, arg2, arg3)`. This brings me back to the above point about declaring the variable in the parameters; because a variable can't be used in a function without defining it first. An alternative path would be this, if you so choose:

```
override.Start(){
    var i = 0;
    Loop(=>(i){
        PrintLine("I'm an infinate loop!");
        Sleep(1000);
    });
}
```

This would provide the same result as defining `i` inside the parameters.

`Lambda Expression` can be used anywhere that needs to invoke a function via a string, and can be nested as deeply as needed. Please note that extremely deep nested functions may take a performance hit! So use within reason :).