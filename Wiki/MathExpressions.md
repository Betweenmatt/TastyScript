# Mathematical Expressions

Mathematical expressions are new with 1.2.2! By using `[]` you can perform basic math functions like addition, subtraction, multiplication, and division. You can also nest your math expressions in `()` like this `var x = [(12 * 12) / 100]`. Variables can also be used, as long as their value can be parsed to a number.

A more realistic example:

```
override.Start(){

    Loop(=>(var i){
        #the modulus operator (%) is an operator that is meant
        #to find the remainder after dividing the first
        #operand (the first number) by the second.
        var even = [i % 2];
        If(even == 0)
            .Then(=>(){
                PrintLine("Even!");
            })
            .Else(=>(){
                PrintLine("Odd!");
            });
    });

}
```

The above example will print `Even!` or `Odd!` depending on the current iteration.