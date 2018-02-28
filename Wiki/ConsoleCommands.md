# Table of Contents
* [AppPackage](#apppackage)
* [Connect](#connect)
* [Devices](#devices)
* [Exec](#exec)
* [Help](#help)
* [LogLevel](#loglevel)
* [Run](#run)
* [Screenshot](#screenshot)
* [Shell](#shell)

This is a list of the current console commands and their functionality.

## AppPackage
`app 'appPackage'` - Sets the App Package. If no device is selected, this will throw an exception. *Note: this setting currently gets reset every time the driver is initiated. It is advised to have the ConnectDevice() function followed by the AppPackage() function in your script; instead of using console commands*

## Connect
`connect 'device'` - Connects to the designated device. If your ADB server is offline, this will throw an exception.

## Devices
`devices` - Lists out the all the devices connected to ADB by serial. If your ADB server is offline, this will throw an exception.

## Exec
`exec 'line'` - Executes the given line of code. This is used for very quickly testing the response of certain functions without building an entire script. This requires the same format as any script(functions and extensions need parameters, line must be ended with `;` etc.). In essence, the line you give the exec command is wrapped in a `Start()` function, and sent through the compiler.

Example: `exec PrintLine("The time is ").Concat(Time).Concat(" right now!");` will print "The time is 7:22 PM right now!"

## Help
`-h` - Prints out a basic explanation of each command.

## LogLevel
`loglevel 'level'` - Sets the level of logging to be done in the console. There are three correct levels, `warn`, `error`, and `none`. 
* `warn` will allow any warnings or errors to be logged in the console 
* `error` will only allow errors to be logged in the console; warnings are ignored
* `none` will ignore both errors and warnings. *This is not advised*

## Run
`run 'path'` - Runs the script at the designated path. Location begins at the directory the TastyScript.exe was launched from. The script file can be `.ts` or `.txt` or any text format, as long as the contents are correct.

## Screenshot
`screenshot 'path'` - Takes a screenshot of the connected device and saves it at the path specified. Path must end in the `.png` extension. If there is no device connected, this will throw an exception.

## Shell
`shell 'command'` - Sends an adb shell command to the currently connected device. *This is in beta and can sometimes have unexpected behavior!*
