# Welcome!

TastyScript is a very basic language I originally created for rapid prototyping automated gaming on my Android phone, since my computer cannot run Nox. This version of TastyScript connects to your phone via ADB, and allows you to communicate touch locations, (some)hard buttons, and take screenshots. There are many more features I'd like to add so I make updates frequently!

Check the documentation at the [Wiki](../../wiki) to learn about the core concepts of the language, as well as full documentation on functionality.

On a side note, this was my very first language. It does not use any complex parsing frameworks, only regex and and some string manipulation. I plan on replacing my work with ANTLR in the future, when I have time to sit down and learn how to use it!

# How to install
* First, download the [latest release](https://github.com/TastyGod/TastyScript/releases) or compile from the source.
* [Download the ADB server](https://developer.android.com/studio/releases/platform-tools.html)
* Turn on USB debugging on your device. Go to `Settings->Developer Options` and you will find a toggle for `USB Debugging`. If you don't have Developer Options menu in your settings google your device model and "Developer Options" and you will find out how to enable it.
* Connect your phone to your computer via usb cable and start the ADB server. If you wish to use ADB over wifi, [read this post.](https://stackoverflow.com/a/28084202/3496006)
* Run the TastyScript.exe file you downloaded first

Now you're good to run any scripts you choose! Take a look at the [Examples](/Examples) folder for ideas/help etc on creating your own script. Note: the touch locations in the examples will most likely fail on your phone! You need to use the pixel locations that work for your phone's aspect ratio/resolution

For a list of console commands check out the [Wiki](../../wiki/Console-Commands).


# Requirements
Windows computer with .Net 4.5.2

[Android ADB which can be found here](https://developer.android.com/studio/releases/platform-tools.html)

Android phone/emulator

# Source code requirements
[AForge Image Processing](https://github.com/andrewkirillov/AForge.NET)

[SharpADB by quomotion](https://github.com/quamotion/madb)

Both can be found as Nuget packages

# ToDo List
*Support*
- [ ] Commands sent to an app instead of relying on ADB(wishful thinking!)
- [ ] IPhone Support
- [ ] MacOS Support

*Functionality*
- [ ] Remote control
- [ ] Conditional evaluation
- [ ] Nested scopes
- [ ] Custom variable support
- [ ] Basic math evaluation
- [ ] Better error handling

# Credits
[AForge Image Processing](https://github.com/andrewkirillov/AForge.NET) for their amazing image processing framework!

[SharpADB by quomotion](https://github.com/quamotion/madb) for this beautiful ADB wrapper! Saved me a *ton* of work!
