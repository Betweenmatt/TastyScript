# SetDefaultSleep
***v1.1.0+***
## Overloads
|   |    | 
| :--- | :--- | 
| **SetDefaultSleep(*number*)** | Sets the default sleep timer for touch commands | 

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *True* | 
| **Return Value** | *Null* |

---

### Description
Sets the default sleep timer for touch commands in milliseconds. Default is set at 1200.
### Example
```
function.Awake(){
	SetDefaultSleep(1500);
}
```

| [Back](README.md) |