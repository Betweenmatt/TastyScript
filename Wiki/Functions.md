# Table of Contents
* [AppPackage](#apppackage)
* [CheckScreen](#checkscreen)
* [ConnectDevice](#connectdevice)
* [KeyEvent](#keyevent)
* [If](#if)
* [Loop](#loop)
* [LongTouch](#longtouch)
* [Print](#print)
* [PrintLine](#printline)
* [SendText](#sendtext)
* [SetDefaultSleep](#setdefaultsleep)
* [Sleep](#sleep)
* [Swipe](#swipe)
* [TakeScreenshot](#takescreenshot)
* [Touch](#touch)

---

## AppPackage
 AppPackage(*PackageName*) | |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *PackageName*(string) | 
*Extensions* |  | 
*Override* | sealed | 
#### Description
Sets the current App Package. When an App Package has been set, the Driver commands will not be sent if the App Package is not the current focus. This must be called after the `ConnectDevice()` function.

**Side Note:** Technically you don't need the full package name, or you could even use the package name plus the activity name; whichever floats your boat. The focus check works by calling the adb shell command `dumpsys window windows | grep -E 'mCurrentFocus'` and checks if the result contains the `AppPackage()` you have set. If it does, it continues to call the command; if it doesn't, it stops all commands and re-checks for the package every 5 seconds.
#### Examples
`AppPackage("com.vespainteractive.KingsRad");`

---

## CheckScreen
CheckScreen( *SuccessFunction*, *FailFunction*,*SuccessPath* ) | CheckScreen(*SuccessFunction*, *FailFunction*,*SuccessPath*, *SuccessPath* ) | | | |
:---:|:---:|:---:|:---:|:---:
*Expected Arguments* | *SuccessFunction*(string) | *FailFunction*(string) | *SuccessPath*(string) |
*Overload* | *SuccessFunction*(string) | *FailFunction*(string) | *SuccessPath*(string) | *FailPath*(string) 
*Extensions* | [.For()](/Wiki/Extensions.md#for) | [.Threshold()](/Wiki/Extensions.md#threshold) | |
*Override* | sealed | | |

#### Description
Takes a screenshot of the current device, and compares it to the image at the *SuccessPath*. If the screenshot passes the threshold, Invoke *SuccessFunction*; else Invoke *FailFunction*.

Use the overload to include a fail-check image to compare against. If the image at *SuccessPath* passes the threshold, Invoke *SuccessFunction*; then if the image at *FailPath* passes the threshold, Invoke *FailFunction*; else throws DriverException.
#### Examples
```
function.Example(){
    CheckScreen("SuccessFunction", "FailFunction", "img/test1.png");
}
function.SuccessFunction(){
    PrintLine("Succeeded!");
}
function.FailFunction(){
    PrintLine("Failed!");
}

#lambda expression example
function.Example(){
    CheckScreen(=>(){
            PrintLine("Succeeded!");
        },=>(){
            PrintLine("Failed!");
        }, "img/test1.png");
}

```

---

## ConnectDevice
ConnectDevice(*SerialNumber*) | | | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *SerialNumber*(string) |  |
*Extensions* |  |  | 
*Override* | sealed | |
#### Description
Connects to the first device found with the given *SerialNumber*.
#### Examples
`ConnectDevice("DEVICE");`


---

## If
If(*Conditional*) | | | | |
:---:|:---:|:---:|:---:|:---:
*Expected Arguments* | *Conditional*(string) |  | |
*Extensions* | [.And()](/Wiki/Extensions.md#and) | [.Or()](/Wiki/Extensions.md#or) | [.Then()](/Wiki/Extensions.md#then) | [.Else()](/Wiki/Extensions.md#else)  |
*Override* | sealed | |
#### Description
Evaluates the conditional statement, and chooses the invoking extension `.Then()` or `.Else()` based on the result. `.Then()` is the only required extension for this function.

You can use the extensions `.And()` and `.Or()` to narrow down your conditional.

The conditional statement evaluates an expression based on the operator, and results in a bool `True` or `False`, which are represented to the compiler as a string. If you wish to create an always passing conditional, pass `"True"` or `"False"` as your conditional. The currently allowed operators are `==` `!=` `>=` `<=` `<` `>`
#### Examples
```
override.Start(){
    Loop(=>(var i){
        var even = [i % 2];
        If(even == 0)
            .Then(=>(){
                PrintLine("This iteration is even");
            });     
    });

    Loop(=>(var i){
        var even = [i % 2];
        If(even != 0).And(i > 100)
            .Then(=>(){
                PrintLine("This iteration is odd");
                PrintLine("And the iteration is above 100");
            });     
    });

    Loop(=>(var i){
        var even = [i % 2];
        If(even != 0).Or(i < 9)
            .Then(=>(){
                PrintLine("This iteration is odd");
                PrintLine("Or the iteration is less than 9");
            }).Else(=>(){
                PrintLine("This iteration is even");
                PrintLine("Or the iteration is greater than 9");
            });     
    });
}
```

---

## KeyEvent
 ***Introduced in v1.2.1***
 
 KeyEvent(*Key*) |  |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Key*(string) |  | 
*Extensions* | [.For()](/Wiki/Extensions.md#for) | 
*Override* | true | 
#### Description
Sends the given key event to the currently connected device. Key Events are case sensitive, must be as a string, and must be one of the following:

Menu, SoftRight, Home, Back, Call, EndCall, Zero, One, Two, Three, Four, Five, Six. Seven, Eight, Nine, Star, Pound, DPadUp, DPadDown, DPadLeft, DPadRight, DPadCenter, VolumeUp, VolumeDown, Power, Camera, Clear, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, Comma, Period, AltLeft, AltRight, ShiftLeft, ShiftRight, Tab, Space, Sym, Explorer, Envelope, Enter, Del, Grave, Minus, Equals, LeftBracket, RightBracket, BackSlash, SemiColon, Apostrophe, Slash, At, Num, HeadSetHook, Focus, Plus, Menu2, Notification, Search

***Note:*** The Menu and Menu2 events are eventcode 1 and 82 respectively. I'm not sure if there is a difference between the two so I included both.

#### Examples
`KeyEvent("Back");`

---

## Loop
Loop(*Function*) | | | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Function*(string) |  |
*Extensions* | [.For()](/Wiki/Extensions.md#for) |  | 
*Override* | sealed | |
#### Description
Loops the invoked *Function*. For extension is required. The invoked function *must* have at least one parameter to use this function!

**NEW:** in 1.2.2, you can use the functions `Break()` to break away from the current loop, or `Continue()` to skip the rest of the current iteration; moving on to the next.

#### Examples
```
function.Example(){
    Loop("LoopFunction").For(12);
}
function.LoopFunction(i){
    PrintLine("Iteration: ").Concat(i);
}

#lambda example
function.Example(){
    Loop(=>(var i){
        PrintLine("Iteration: ").Concat(i);
        If(i > 10)
            .Then(=>(){
                #breaks the loop on the 11th iteration
                Break();
            });
    });
}
```

---

## LongTouch
***Introduced in v1.2.1***

 LongTouch(*XPos*, *YPos*, *Duration*) | LongTouch(*XPos*, *YPos*, *Duration*, *Sleep*) |  | ||
:---:|:---:|:---:|:---:|:---:
*Expected Arguments* | *XPos*(number) | *YPos*(number) | *Duration*|
*Overload* | *XPos*(number) | *YPos*(number) | *Duration* | *Sleep*(number)
*Extensions* | [.For()](/Wiki/Extensions.md#for) | 
*Override* | true | 
#### Description
Touches the currently connected device at the given location for *Duration* amount of time. Overload the function with a 3rd argument *Sleep* to bypass the DefaultSleep timer. *Note: The DefaultSleepTimer is defaulted at 1200ms. Also every ADB command currently executes asynchronously, but has a 300ms sleep timer in between commands to prevent event bursting due to user error or lag spikes. The DefaultSleepTimer can be changed, but the 300ms command sleep can not.*
#### Examples
`LongTouch(500,500,100);`

`LongTouch(1546,645,100,500);#only sleep for half a second`

---

## Print
Print(*Input*) | | | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Input*(string, number) |  |
*Extensions* | [.For()](/Wiki/Extensions.md#for) | [.Concat()](/Wiki/Extensions.md#concat) | [.Color()](/Wiki/Extensions.md#color)
*Override* | true | |
#### Description
Prints the given argument in the console.

*Edit:* The extension `.AddParams()` is deprecated from use with this function. Please use `.Concat()` instead [.Concat()](/Wiki/Extensions.md#concat)
#### Examples
`Print("Hello,");`

`Print(" World!");`

---

## PrintLine
 PrintLine(*Input*) |  |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Input*(string, number) |  |
*Extensions* | [.For()](/Wiki/Extensions.md#for) | [.Concat()](/Wiki/Extensions.md#concat) | [.Color()](/Wiki/Extensions.md#color)
*Override* | true | |
#### Description
Prints the given argument in the console followed by a new line.

*Edit:* The extension `.AddParams()` is deprecated from use with this function. Please use `.Concat()` instead [.Concat()](/Wiki/Extensions.md#concat)
#### Examples
`PrintLine("Hello, World!");`

---

## SendText
 ***Introduced in v1.2.1***
 
 SendText(*Text*) |  |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Text*(string) |  | 
*Extensions* | [.For()](/Wiki/Extensions.md#for) | 
*Override* | true | 
#### Description
Sends the given text string to the connected device which will be typed into any Android Text Box. 

#### Examples
`SendText("Hello, World!");`

---

## SetDefaultSleep
SetDefaultSleep(*Sleep*) | | | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Sleep*(number) |  |
*Extensions* |  |  | 
*Override* | true | |
#### Description
Sets the default sleep timer to the number (in milliseconds) provided *Sleep*. Script default is set at 1200.
#### Examples
`SetDefaultSleep(3000);#3 seconds`

---

## Sleep
Sleep(*Sleep*) | | | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Sleep*(number) |  |
*Extensions* | [.For()](/Wiki/Extensions.md#for) |  | 
*Override* | true | |
#### Description
Sleeps for the number of miliseconds provided in *Sleep*. Defaults to **SetDefaultSleep**.
#### Examples
`Sleep(3000);#3 seconds`

---

## Swipe
***Introduced in v1.2.1***

|Swipe(*XPos1*, *YPos1*, *XPos2*, *YPos2*, *Duration*) | Swipe(*XPos1*, *YPos1*, *XPos2*, *YPos2*, *Duration*, *Sleep*)| | | | | |
|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
|*Expected Arguments* | *XPos1*(number) | *YPos1*(number) | *XPos2*(number) | *YPos2*(number) | *Duration* | |
|*Overload* | *XPos1*(number) | *YPos1*(number) | *XPos2*(number) | *YPos2*(number) | *Duration* | *Sleep*(number) |
|*Extensions* | [.For()](/Wiki/Extensions.md#for) | | | | | |
|*Override* | true | | | | | |

#### Description
Swipes the currently connected device at the given location for *Duration* amount of time. *XPos1* and *YPos1* are the start locations, *XPos2* and *YPos2* are the end locations. Overload the function with a 3rd argument *Sleep* to bypass the DefaultSleep timer. *Note: The DefaultSleepTimer is defaulted at 1200ms. Also every ADB command currently executes asynchronously, but has a 300ms sleep timer in between commands to prevent event bursting due to user error or lag spikes. The DefaultSleepTimer can be changed, but the 300ms command sleep can not.*
#### Examples
`Swipe(100,200,100,1200,100);#swipes down the y axis over a 100ms duration`

---

## TakeScreenshot
 TakeScreenshot(*Path*) |  |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Path*(string) |
*Extensions* | [.For()](/Wiki/Extensions.md#for) | 
*Override* | sealed | 
#### Description
Takes a screenshot of the connected device, and saves it to the path provided. *Path given must use the file extension '.png'*
#### Examples
`TakeScreenshot("SS/testimg.png");`

---

## Touch
 Touch(*XPos*, *YPos*) | Touch(*XPos*, *YPos*, *Sleep*) |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *XPos*(number) | *YPos*(number)
*Overload* | *XPos*(number) | *YPos*(number) | *Sleep*(number)
*Extensions* | [.For()](/Wiki/Extensions.md#for) | 
*Override* | true | 
#### Description
Touches the currently connected device at the given location. Overload the function with a 3rd argument *Sleep* to bypass the DefaultSleep timer. *Note: The DefaultSleepTimer is defaulted at 1200ms. Also every ADB command currently executes asynchronously, but has a 300ms sleep timer in between commands to prevent event bursting due to user error or lag spikes. The DefaultSleepTimer can be changed, but the 300ms command sleep can not.*
#### Examples
`Touch(12,24);`

`Touch(1546,645,500);#only sleep for half a second`

---

