# Swipe
***v1.2.1+***
## Overloads
|   |    | 
| :--- | :--- | 
| **Swipe(*number*,*number*,*number*,*number*,*number*)** | Initiates a swipe on the device. | 
| **Swipe(*number*,*number*,*number*,*number*,*number*,*number*)** | Initiates a swipe on the device with a specified sleep timer upon completion | 

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *False* | 
| **Return Value** | *Null* |
| **Extensions** |  | [.For()](../Extensions.md#for) | 

---

### Description
Initiates a long touch on the specified device with X1: *number[0]*, Y1: *number[1]*, X2: *number[2]*, Y2: *number[3]*, and with the overload - Sleep: *number[4]*. 
### Remarks
Please refer to [SetDefaultSleep](SetDefaultSleep.md) for information on the default sleep timer.
### Example
```
function.Start(){
	Swipe(144,546,300,150,300);
}
```



| [Back](README.md) |