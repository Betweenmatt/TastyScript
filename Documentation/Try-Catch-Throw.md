# Try/Catch/Throw

The `Try()` and `Throw()` function as well as the `.Catch()` extension are new to version 1.3.3. By utilizing these functions you can build safer, and ulitimately more reliable scripts in a variety of situations.

* The `Throw()` function prints a verbose exception to the console in bright red, with the **Exception Type**(currently cannot be changed, and is defaulted to `UserThrownException`), the **Exception Message** which is defined as the argument in the function(ex:`Throw("Value cannot be null.");`), the File and line number of the exception, and a snippet of the code surrounding the exception.
* The `Try()` function accepts a function to invoke(usually anonymous), and if an exception occurs inside the function, it will be passed to the `.Catch()` function.
* The `.Catch()` extension accepts a function to invoke(usually anonymous), and if an exception occurs inside the `Try()` function this is attached to, the function invoked in the extension will be parsed.

Under normal circumstances, an exception would halt your script in its tracks, but with a `Try().Catch()` function you can handle the exception correctly and continue on. Here are some examples:

```
function.ThrowExample(initVar){
	#if initVar is null we need to throw an exception
	#to alert the user.
	If(?initVar).then(=>(){
		Throw("initVar value cannot be null!");
	});
	var dosomething = [initVar * 100];
}
```

For the function which is invoked by `.Catch()` you can supply a parameter which will output the exception thrown as an array. 

```
Try(=>(){
		var testar = array(1,2,3);
		var t = testar.GetItem(4);
	}).Catch(=>(e){
		#print the exception as an array
		PrintLine(e);
		#get the exception elements and print them individually
		var type = e.GetItem(0);
		var msg = e.GetItem(1);
		var line = e.GetItem(2);
		var snippet = e.GetItem(3);
		PrintLine(type);
		PrintLine(msg);
		PrintLine(line);
		PrintLine(snippet);
	});
```