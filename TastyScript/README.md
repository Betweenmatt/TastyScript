﻿# ChangeLog

## v1.2.1.15 - 2/24/18
	* More refactoring. Reduced clutter in ExtensionDefinitions.cs by removing unneccessary lines of code.
	* Added `exec`, `shell` command.
	* Added new functions `Swipe()`, `LongTouch()`, `SendText()`, `KeyEvent()`, `AddPackage()` and new extension `.Concat()`, Deprecated `Back()` function, `.AddParams()` extension.
	* Added short console commands for run `-r`, exec `-e`, devices `-d`, loglevel `-ll`, connect `-c`, screenshot `-ss`, shell `-sh`
	* Added focus detecting functionality. To enable this feature, an `AppPackage() needs to be set(alternatively by using `app` in console). Check the documentation for more detail.
	* Better error handling in certain situations such as using `Touch` function, when not including the correct arguments.
	* Kindof fixed a bug when using the focus detection functionality where clicks where still getting through. There is a small chance this will still occur, because the cancelation token cannot be sent in time for the device to stop. I believe this is more of an ADB/hardware limitation.
	* Added `throw` as a new log level. This log level will let all exception messages pass into the console, including CompilerHandledExceptions(). This should only be used for extreme debugging.

## v1.2.0.0 - 2/19/18
	* Major refactoring, reorganizing documents and removing packages that weren't being used.
	* Added Threshold extension for CheckScreen function.
	* Added Color extension for Print and PrintLine functions.