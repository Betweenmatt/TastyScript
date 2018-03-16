# How to install
Welcome! To install the TastyScript Notepad++ Plugin, all you have to do is download the latest [release](https://github.com/TastyGod/TastyScript/releases) and take the `.dll` file in the 
plugins folder and put it in your Notepad++ plugins folder(should be C:\Program Files (x86)\Notepad++ or something similar). Restart your 
Notepad++ and you're good to go!

To use, just load up a `.ts` file you wish to run, and either press the TS button on the toolbar, or go to plugins->TastyScriptNPP->Run/Stop Script

Note: you cannot run any console commands from inside Notepad++. The base directory is set to the directory of the loaded file, so as long as 
your imports are relative to that directory everything loads fine.

## Settings
There is a `Settings` button in plugins->TastyScriptNPP. This opens a dialog which you can set the look of your output panel. The settings are 
pretty straightforward. The large textbox labeled `Color Overrides` should not be messed with. This is a color exchange, from the expected colors 
(whatever color you use with the `.Color()` extension) to a color recognized by .Net which uses the `System.Drawing.Color` struct. I implemented 
this setting because depending on the background color of your output window you may not want certain colors(like light grey on a white background). 
Each color override is split by a `;`, and on the left of the comma is the expected color and the right is the output color.

Note: The left hand color `DarkYellow` has no .Net counterpart so I used `YellowGreen`. The left hahd color `Gray` is not present because that is the 
default input color.

# Changelog
## v1.3.1

* Implementation of the Notepad++ Plugin.