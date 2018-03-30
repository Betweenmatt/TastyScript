# ReadLine
***v1.3.3+***
## Overloads
|   |    | 
| :--- | :--- | 
| **ReadLine()** | Sets the Drivers App Package to app currently in focus. | 

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *False* | 
| **Return Value** | *string* |

---

### Description
This function waits for user input and returns the string that is entered.
### Remarks
`ReadLine()` will block the thread until the enter key has been pressed, or on Notepad++ plugin, until the `send` button has been pressed.
### Example
```
function.Awake(){
	var line = ReadLine();
	PrintLine(line);
}
```


| [Back](README.md) |