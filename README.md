# TastyScript
Welcome!

TastyScript is a very basic language I originally created for rapid prototyping automated gaming on my Android phone, since my computer cannot run Nox. This version of TastyScript connects to your phone via ADB, and allows you to communicate touch locations, (some)hard buttons, and take screenshots. There are many more features I'd like to add so I make updates frequently!

Check the documentation at the [Wiki](../../wiki) to learn about the core concepts of the language, as well as full documentation on functionality.

# tl;dr Lets get started
Either download the latest release here* or compile from the source. Also you need ADB from either [this link](https://developer.android.com/studio/releases/platform-tools.html) or from the Android SDK. The ADB server needs to be started or the driver will not connect.

Take a look at the [Examples](/Examples) folder for ideas/help etc on creating your own script. Note: the touch locations in the examples will most likely fail on your phone! You need to use the pixel locations that work for your phone's aspect ratio/resolution

In the TastyScript console:
* Enter `devices` to see the current list of devices connected to your computer.
* Enter `connect 'device'` to connect to the device.
* Enter `run 'script path'` and profit!

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
- [ ] Conditional evaluation
- [ ] Nested scopes
- [ ] Custom variable support
- [ ] Basic math evaluation
- [ ] Better error handling
# Credits
[AForge Image Processing](https://github.com/andrewkirillov/AForge.NET) for their amazing image processing framework!

[SharpADB by quomotion](https://github.com/quamotion/madb) for this beautiful ADB wrapper! Saved me a *ton* of work!
