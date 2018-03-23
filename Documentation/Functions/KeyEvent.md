# KeyEvent
***v1.2.2+***
## Overloads
|   |    | 
| :--- | :--- | 
| **KeyEvent(*string*)** | Sends the key event to the device | 

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *False* | 
| **Return Value** | *Null* |

---

### Description
Sends the given key event to the device. The allowed keys are as follows:

Menu, SoftRight, Home, Back, Call, EndCall, Zero, One, Two, Three, Four, Five, Six. Seven, Eight, Nine, Star, Pound, DPadUp, DPadDown, DPadLeft, DPadRight, DPadCenter, VolumeUp, VolumeDown, Power, Camera, Clear, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, Comma, Period, AltLeft, AltRight, ShiftLeft, ShiftRight, Tab, Space, Sym, Explorer, Envelope, Enter, Del, Grave, Minus, Equals, LeftBracket, RightBracket, BackSlash, SemiColon, Apostrophe, Slash, At, Num, HeadSetHook, Focus, Plus, Menu2, Notification, Search

***Note:*** The Menu and Menu2 events are eventcode 1 and 82 respectively. I'm not sure if there is a difference between the two so I included both.

### Example
```
function.Start(){
	KeyEvent("Back");
}
```
### Exceptions
|   |   | 
| :--- | :--- | 
| **CompilerException** | The given key event could not be found | 



| [Back](README.md) |