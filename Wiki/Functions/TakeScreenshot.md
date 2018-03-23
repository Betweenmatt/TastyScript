| [Back]() |

---

# TakeScreenshot
***v1.2.0+***
## Overloads
|   |    | 
| :--- | :--- | 
| **TakeScreenshot()** | Sets the Drivers App Package to app currently in focus. |

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *True* | 
| **Return Value** | *Null* |

---

### Description
Takes a screenshot of the connected device, and saves it to the path provided. *Path string[0] given must use the file extension '.png'*
#### Examples
`TakeScreenshot("SS/testimg.png");`
### Exceptions
|   |   | 
| :--- | :--- | 
| **DriverException** | The current connected device could not be found. Please call `ConnectDevice()` before calling this function | 



| [Back]() |