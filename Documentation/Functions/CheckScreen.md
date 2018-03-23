# CheckScreen
***v1.1.0+***
## Overloads
|   |    | 
| :--- | :--- | 
| **CheckScreen(*function*, *function*, *string*)** | Compares the screen against a given template | 
| **CheckScreen(*function*, *function*, *string*, *string*)** | Compares the screen against a given template. If both templates fail to pass, an exception is thrown | 

---

## Options
|   |   |  |  | 
| :--- | :--- | :--- | :--- | 
| **IsSealed** | *True* |  |  | 
| **Return Value** | *Null* |  |  | 
| **Extensions** |  | [.For()](../Extensions.md#for) | [.Prop()](../Extensions.md#prop) | 



---

### Description
Checks the current device screen against the template at path *string[0]*, executing *function[0]* if it template exists or *function[1]* if it doesn't.

Use the overload with path *string[1]* to ensure if neither templates are a match an exception will be thrown, exiting the script.
### Remarks
For best results, make sure the template is not mono-tone, and has as much pixel to pixel difference as possible. Do not use an inadequate template and just lower the threshold or you will have a lot of false positives.
### Extensions
`.Prop()` extension is used to fine tune the template matching procedure and is not required. Please refer to [SetTemplateDefaultOptions](SetTemplateDefaultOptions.md). **Do Not** give this extension an array like its function counterpart.
### Example
```
function.Start(){
	CheckScreen(=>(){
		RunSuccessFunction();
	},=>(){
		RunFailFunction();
	},"testimg.png").Prop(91,30,"Q1");
}
```
### Exceptions
|   |   | 
| :--- | :--- | 
| **SystemException** | Cannot find the specified path |
| **CompilerException** | Cannot find the function [f] | 
| **NullException** | Arguments cannot be null |
| **DriverException** | Neither the success nor the failure templates were found with the fail-safe overload |



| [Back](README.md) |