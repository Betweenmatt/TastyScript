# Custom Extensions

With v1.3.2 you can now add custom extensions to your scripts. This is a great way to reduce the amount of code needed to achieve a goal. Please note that currently you arent allowed to `override` an extension.

**Note:** *If you try to name an extension with an extension that already exists, then the first one found will be called. With pre-defined extensions, they will always be called first because they are added to the stack immediately. You are advised to use strong unique names for your extensions to prevent accidental overwrites which are tough to debug.*

There are two types of extensions in TastyScript - *Function* extensions, and *Variable* extensions. Variable extensions can be used on functions, **but Function extensions CAN NOT be used on variables**

To define an extension use the `extension` tag, with a provided parameter `this function` or `this variable` depending on the type you need. **THIS IS REQUIRED**

```
extension.MyExtension(this function, param){
	Return(param);
}
extension.MyVariableExtension(this variable, param){
	Return(param);
}
```

The two extensions above, if run, would return the first argument provided.

```
function.Start(){
	var test = "this will not print";
	PrintLine(test.MyVariableExtension("because this is all thats returned"));
	
	#this will print:
	#because this is all thats returned
}
```

# Variable Extensions

With variable extensions, using the `variable` parameter you can get the object that is being extended:

```
extension.MyVariableExtension(this variable, param){
	if(variable == param).then(=>(){
		Return(true);
	});
	Return(false);
}
function.Start(){
	var test = "hi";
	PrintLine(test.MyVariableExtension("hi"));
	
	#this will print:
	#true
	
	PrintLine(test.MyVariableExtension("hii"));
	
	#this will print:
	#false
}
```

Here is a more complex example, an implementation for checking if an array contains a specified item:

```
## [item] is the item we are looking for, [index], if exists,
## returns the index of the item. If it doesnt exist, returns a bool. 
extension.Contains(this variable, item, index){
	
	#get the length of the array. Make sure to 'var length--' because
	#arrays are zero index!
	
	var length = variable.Length(); var length--;
	
	#loop through each item in the array
	
	Loop(=>(var i){
		var x = variable.GetItem(i);
		if(x == item).then(=>()
		{
			#if index exists, then return the index of the item
			
			if(!?index).then(=>(){ return(i); });
			
			#else we return the bool 'true'
			
			return(true); 
		});
	}).For(length);#make sure you set the loop length to the length of
					#array or you'll get an error!
	
	#if we make it this far, then the item doesnt exist
	#so if index exists, we return a -1 index
	
	if(!?index).then(=>(){ return(-1); });
	
	#or we return the bool 'false'
	
	return(false);
}
```

# Function Extensions

Function extensions work quite different than variable extensions under the hood. First, the required parameter `function` only returns the name of the function. This is only useful for ensuring the extension is only used on certain functions, but it is not really needed. 

The bulk of the work is up to the function to take care of. I introduced the internal function `This()` with v1.3.2, which returns some meta-data useful for debugging, as well as an interface for getting an extension from inside a function. Calling the function `This("Extension").Prop("Args","{extension name}");` will return the arguments passed in the extension `{extension name}`. This means you *can* get the arguments supplied by other extensions like so:

```
override.PrintLine(args){
	var colorExt = This("Extension").Prop("Args","Color");
	if(colorExt != null).then(=>(){
		Base(args).Color(colorExt);
		Return();
	});
	Base(args);
}
```

The example above is a simple override for `PrintLine()`, but since overrides do not pass extensions to their base function, this implementation accounts for that.

When creating your custom function extension make sure that you return the parameters that you wish to be extended. The `This("Extension").Prop("Args","{extension name}");` only provides you with the return value from your extension definition.

```
extension.MyExtension(this function, arg1, arg2, arg3){
	var arr = array();
	if(!?arg1).then(=>(){ var arr = arr.SetItem(arg1); });
	if(!?arg2).then(=>(){ var arr = arr.SetItem(arg2); });
	if(!?arg3).then(=>(){ var arr = arr.SetItem(arg3); });
	Return(arr);
}
function.TestFunction(){
	var ext = This("Extension").Prop("Args","MyExtension");
	Return(ext);
}
function.Start(){
	var test = TestFunction().MyExtension("Good","Day","Sir");
	PrintLine(test);
}
```

The above example takes the 3 arguments and adds them to an array *if they are not null* which is then returned to the TestFunction. The example above would print `[Good, Day, Sir]` in the console.

In addition to the `.Prop()` getter above, you can also call `This("Extension");` which will return an array of the available extensions. This provides you with the ability to check for certain use cases, and will ultimately make code much safer to run.