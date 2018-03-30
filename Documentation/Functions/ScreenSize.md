# ScreenSize
***v1.2.0+***
## Overloads
|   |    | 
| :--- | :--- | 
| **ScreenSize()** | Gets the current device's screen size in pixels. | 

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *False* | 
| **Return Value** | *Array* |

---

### Description
Gets the currently connected device's screen size in pixels. This function calls the command `dumpsys display | grep -E 'mDisplayWidth'` for width and height, and parses the results. If your screen size is overrided, it will return the overrided size.

Note the first item in the array is the width in portrait, and the second item is the height in portrait.

### Example
```
function.Awake(){
	var screenSize = ScreenSize();
	var x = screenSize.GetItem(0);
	var y = screenSize.GetItem(1);
}
```
### Exceptions
|   |   | 
| :--- | :--- | 
| **DriverException** | The current connected device could not be found. Please call `ConnectDevice()` before calling this function | 



| [Back](README.md) |