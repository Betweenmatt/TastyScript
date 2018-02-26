# ChangeLog

## v1.2.2
* Added `dir` console command, which sets the directory to call from. can either be a full path, or local path.
* Fixed an issue where Properties.Settings werent being saved on change.
* Fixed script exit thread issue by rewriting how the exit thread/menu listener was written.
* Added remote functionality - now the TCPListener is opened when the program is open, and can listen for the run/stop commands on localhost:8080.

## v1.2.1
* More refactoring. Reduced clutter in ExtensionDefinitions.cs by removing unneccessary lines of code.
* Added `exec`, `shell` command.
* Added new functions `Swipe()`, `LongTouch()`, `SendText()`, `KeyEvent()`, `AddPackage()` and new extension `.Concat()`, Deprecated `Back()` function, `.AddParams()` extension.
* Added short console commands for run `-r`, exec `-e`, devices `-d`, loglevel `-ll`, connect `-c`, screenshot `-ss`, shell `-sh`
* Added focus detecting functionality. To enable this feature, an `AppPackage() needs to be set(alternatively by using `app` in console). Check the documentation for more detail.
* Better error handling in certain situations such as using `Touch` function, when not including the correct arguments.
* Kindof fixed a bug when using the focus detection functionality where clicks where still getting through. There is a small chance this will still occur, because the cancelation token cannot be sent in time for the device to stop. I believe this is more of an ADB/hardware limitation.
* Added `throw` as a new log level. This log level will let all exception messages pass into the console, including CompilerHandledExceptions(). This should only be used for extreme debugging.
* Fixed an error that was being caused by a null `line` in ExceptionHandler
* Cleaned up the switch/case command format for better readability

## v1.2.0
* Major refactoring, reorganizing documents and removing packages that weren't being used.
* Added Threshold extension for CheckScreen function.
* Added Color extension for Print and PrintLine functions.