# TastyScript
Welcome!

TastyScript is a very basic language I originally created for rapid prototyping automation on my Android phone. This version of TastyScript connects to your phone via ADB, and allows you to communicate touch locations, (some)hard buttons, and take screenshots. There are many more features I'd like to add so I make updates frequently!

Check the documentation at the [Wiki](../../Wiki) to learn about the core concepts of the language, as well as full documentation on functionality.

# tl;dr Lets get started
Either download the latest release here* or compile from the source.

Take a look at the [Examples](/Examples) folder for ideas/help etc on creating your own script. Note: the touch locations in the examples will most likely fail on your phone! You need to use the pixel locations that work for your phone's aspect ratio/resolution

Enter `run 'script path'` in the TastyScript console and profit!

# Requirements
Windows computer with .Net 4.5.2

[Android ADB which can be found here](https://developer.android.com/studio/releases/platform-tools.html)

Android phone/emulator

# Source code requirements
[AForge Image Processing](https://github.com/andrewkirillov/AForge.NET)

[SharpADB by quomotion](https://github.com/quamotion/madb)

Both can be found as Nuget packages

# Credits
[AForge Image Processing](https://github.com/andrewkirillov/AForge.NET) for their amazing image processing framework!

[SharpADB by quomotion](https://github.com/quamotion/madb) for this beautiful ADB wrapper! Saved me a *ton* of work!
