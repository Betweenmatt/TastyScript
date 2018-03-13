# Welcome!

TastyScript is a very basic language I originally created for rapid prototyping automated gaming on my Android phone, since my computer cannot run Nox. This version of TastyScript connects to your phone via ADB, and allows you to communicate touch locations, ([most](/Wiki/Functions.md#keyevent))hard buttons, and take screenshots and more! There are many more features I'd like to add so I make updates frequently! 

Check the documentation at the [Wiki](/Wiki) to learn about the core concepts of the language, as well as full documentation on functionality.   

If you have any questions or issues, you can DM me on discord `TastyGod#0859` or open a new issue!  

# How to install
* First, download the [latest release](https://github.com/TastyGod/TastyScript/releases) or compile from the source.
* [Download the ADB server](https://developer.android.com/studio/releases/platform-tools.html)
* Turn on USB debugging on your device. Go to `Settings->Developer Options` and you will find a toggle for `USB Debugging`. If you don't have Developer Options menu in your settings google your device model and "Developer Options" and you will find out how to enable it.
* Connect your phone to your computer via usb cable, start the ADB server and connect your device. [Read this documentation to learn how to use ADB, or to learn how to connect to ADB via Wifi](https://developer.android.com/studio/command-line/adb.html)
* Run the TastyScript.exe file you downloaded first

**Note:** ADB must be started and your device connected for TastyScript to work! If you restart your computer, or close the ADB process you will need to start it again.

Now you're good to run any scripts you choose! Take a look at the [Examples](/Examples) folder or the [How To tutorial](https://github.com/TastyGod/TastyScript/blob/master/Wiki/Tutorial-how-to-create-a-functioning-script.md) for ideas/help etc on creating your own script. ~~Note: the touch locations in the examples will most likely fail on your phone! You need to use the pixel locations that work for your phone's aspect ratio/resolution~~ Note: I recently added a helper function in the [tools](/Examples/tools.ts) import script that converts my scripts to whatever resolution you are using! Check out the comment above the function to see how to use it!

For a list of console commands check out the [Wiki](/Wiki/ConsoleCommands.md).

[Connecting to Nox on ADB](https://stackoverflow.com/a/47151050/3496006)

# Requirements
Windows computer with .Net 4.5

[Android ADB which can be found here](https://developer.android.com/studio/releases/platform-tools.html)

Android phone/emulator

[Notepad++](https://notepad-plus-plus.org/) if you want to use the [Notepad++ Extensions](https://github.com/TastyGod/TastyScript/tree/master/Notepad-%20Extensions)

# Source code requirements
[AForge Image Processing](https://github.com/andrewkirillov/AForge.NET)

[SharpADB by quomotion](https://github.com/quamotion/madb)

[Owin](http://owin.org/) - needed for remote, you can just comment out the relevant code in Program.cs if you dont want to use this package.

[Log4Net](https://logging.apache.org/log4net/)

These can be found as Nuget packages

# ToDo List
*Support*
- [ ] Commands sent to an app instead of relying on ADB(wishful thinking!)
- [ ] IPhone Support(not looking so good ;( )
- [ ] MacOS Support

*Functionality*
- [ ] Remote control
- [x] Conditional evaluation
- [x] Nested scopes
- [x] Custom variable support
- [x] Basic math evaluation
- [ ] Better error handling (getting there!)

# Credits
[AForge Image Processing](https://github.com/andrewkirillov/AForge.NET) for their amazing image processing framework!

[SharpADB by quomotion](https://github.com/quamotion/madb) for this beautiful ADB wrapper! Saved me a *ton* of work!

[Log4Net](https://logging.apache.org/log4net/)
