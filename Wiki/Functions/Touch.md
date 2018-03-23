| [Back]() |

---

# Touch
***v1.0.0+***
## Overloads
|   |    | 
| :--- | :--- | 
| **Touch(*number*,*number*)** | Initiates a touch on the device. | 
| **Touch(*number*,*number*,*number*)** | Initiates a touch on the device with a specified sleep timer upon completion | 

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *False* | 
| **Return Value** | *Null* |
| **Extensions** |  | [.For()](../../Extensions.md#for) | 

---

### Description
Initiates a touch on the specified device with X: *number[0]*, Y: *number[1]*, and with the overload - Sleep: *number[3]*. 
### Remarks
Please refer to [SetDefaultSleep](../SetDefaultSleep.md) for information on the default sleep timer.
### Example
```
function.Start(){
	Touch(144,546,300);
}
```



| [Back]() |