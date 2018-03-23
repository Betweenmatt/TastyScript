| [Back]() |

---

# Timer
***v1.2.1+***
## Overloads
|   |    | 
| :--- | :--- | 
| **Timer()** | A simple static performance timer. | 

---

## Options
|   |   |  |
| :--- | :--- | :--- |
| **IsSealed** | *True* |   |
| **Return Value** | *number* |  |
| **Extensions** | .Start() | .Stop() | .Print()

---

### Description
A simple static performance timer. Use `.Start()` to start the timer, and to print the current value use either the `.Print()` extension or use it as the arguments in a `PrintLine()`. Make sure you call `.Stop()` when you're done with the timer!

Every call, returns the current elapsed time in milliseconds.
#### Examples
```
override.Start(){
    Timer.Start();
    PrintLine(Timer());
    Timer.Stop();    
}
```


| [Back]() |