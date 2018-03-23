| [Back]() |

---

# ConnectDevice
***v1.0.0+***
## Overloads
|   |    | 
| :--- | :--- | 
| **ConnectDevice()** | Sets the Device to first found device. | 
| **ConnectDevice(*string*)** | Sets the Device to the specified serial number | 

---

## Options
|   |   | 
| :--- | :--- | 
| **IsSealed** | *True* | 
| **Return Value** | *Null* |

---

### Description
Sets the device to the device specified.

### Remarks
Most Driver functions will work without a device connected, instead of performing their actions they will print the command.
### Example
```
function.Awake(){
	ConnectDevice();#connects to the first device
}
```
### Exceptions
|   |   | 
| :--- | :--- | 
| **DriverException** | The device [d] could not be found | 



| [Back]() |