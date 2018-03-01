# Table of Contents
* [AppPackage](#apppackage)
* [Connect](#connect)
* [Devices](#devices)
* [Directory](#directory)
* [Exec](#exec)
* [Help](#help)
* [LogLevel](#loglevel)
* [Remote](#remote)
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

## Directory
`dir 'path'` - Sets the current directory to the path. In essence, all it does is save a prefix for paths, so instead of using the absolute path `C:\TastyScript\Scripts\file.ts` every time you wish to run a script, use the command `dir C:\TastyScript\Scripts\` and then when you call `run`, or any file path command (including imports!) just use `run file.ts` and it will complete the path. This also works for relative paths; if you normally use `Scripts/file.ts` you can use `dir Scripts/` and then proceed to use `run file.ts`

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

## Remote
`remote 'on | off'` - Turns on/off the remote access(defaulted as off). This feature is still experamental, since there is no client side interface yet. The API currently only accepts `run` or `stop`, to run/stop your command without having access to the console. With the remote toggled on, type in a web browser `localhost:8080/run=file.ts` or `localhost:8080/stop`

## Run
`run 'path'` - Runs the script at the designated path. Location begins at the directory the TastyScript.exe was launched from. The script file can be `.ts` or `.txt` or any text format, as long as the contents are correct.

## Screenshot
`screenshot 'path'` - Takes a screenshot of the connected device and saves it at the path specified. Path must end in the `.png` extension. If there is no device connected, this will throw an exception.

## Shell
`shell 'command'` - Sends an adb shell command to the currently connected device. *This is in beta and can sometimes have unexpected behavior!*
