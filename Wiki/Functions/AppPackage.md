| [Back]() |

---

# AppPackage
***v1.2.0+***
## Overloads
|   |    | 
| :--- | :--- | 
| **AppPackage()** | Sets the Drivers App Package to app currently in focus. | 
| **AppPackage(*string*)** | Sets the Drivers App Package to the package specified. | 

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *True* | 
| **Return Value** | *Null* |

---

### Description
Sets the App Package for the driver, for the current instance. When this function is set, the driver will not send commands to your device if the specified package is not currently in focus.

It is advised to use this feature with every script that utilizes the device driver.
### Remarks
Technically you don't need the full package name, or you could even use the package name plus the activity name; whichever floats your boat. The focus check works by calling the adb shell command `dumpsys window windows | grep -E 'mCurrentFocus'` and checks if the result contains the `AppPackage()` you have set. If it does, it continues to call the command; if it doesn't, it stops all commands and re-checks for the package every 5 seconds.
### Example
```
function.Awake(){
	ConnectDevice();#connects to the first device
	AppPackage();#sets the app package to the current focused app
}
```
### Exceptions
|   |   | 
| :--- | :--- | 
| **DriverException** | The current connected device could not be found. Please call `ConnectDevice()` before calling this function | 



| [Back]() |