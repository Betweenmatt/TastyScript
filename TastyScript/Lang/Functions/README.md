# Function API

 Functions exist in the `TastyScript.Lang.Functions` namespace and are prefixed with `Function`.

 To create a new function, inherit `FDefinition` in the same namespace, and be sure to override `public override string CallBase()`. 
 The return string is not used atm, and will probably be replaced at a later time so you can just `return null;` or `return "";`. The 
 `CallBase()` is your point of ingress into the function.

 There are a variety of properties which can be accessed to perform whatever functionality needed:

 * `ProvidedArgs` Type: List<Token>. Returns the list of provided arguments as tokens. You can cast a token as TArray if you're expecting an array.
 * `Extensions` Type: List<EDefinition>. Returns the list of extensions attached to this function call. An extensions arguments can be accessed by the `Extend()` method.
 * wip lol