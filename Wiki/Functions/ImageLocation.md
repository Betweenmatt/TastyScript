| [Back]() |

---

# ImageLocation
***v1.3.2+***
## Overloads
|   |    | 
| :--- | :--- | 
| **ImageLocation(*string*)** | Gets the center-point location of the given template | 

---

## Options
|   |   |  |
| :--- | :--- | :--- |
| **IsSealed** | *False* |  |
| **Return Value** | *array(number,number)* | *null*
| **Extensions** |  | [.For()](../../Extensions.md#for) | [.Prop()](../../Extensions.md#prop) | 

---

### Description
Gets the center-point location of the given template. If the template does not exist on the screen, *null* is returned.
### Remarks
While optional, you should consider using the `.Prop()` extension to maximize the efficiency of this function based on the needs of your template.
### Extensions
`.Prop()` extension is used to fine tune the template matching procedure and is not required. Please refer to [SetTemplateDefaultOptions](../SetTemplateDefaultOptions.md). **Do Not** give this extension an array like its function counterpart.
### Example
```
function.Awake(){
	var coords = ImageLocation("testimg.png").Prop(90,40,"Q3");
	PrintLine(coords);
}
```
### Exceptions
|   |   | 
| :--- | :--- | 
| **DriverException** | The current connected device could not be found. Please call `ConnectDevice()` before calling this function | 
| **SystemException** | The specified path [p] could not be found |



| [Back]() |