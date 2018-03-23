| [Back]() |

---

# Sleep
***v1.0.0+***
## Overloads
|   |    | 
| :--- | :--- | 
| **Sleep(*number*)** | Sleeps for the *number[0]* time in milliseconds | 

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *false* | 
| **Return Value** | *Null* |

---

### Description
Sleeps for the *number[0]* time in milliseconds. 
### Remarks
This function is called on all the `Touch` functions, so if you override this, the override will be called
### Example
```
function.Start(){
	Sleep(2000);
}
```



| [Back]() |