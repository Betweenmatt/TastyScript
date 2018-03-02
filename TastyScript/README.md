# ChangeLog

## v1.2.3

### Todo
- [ ] Array get/set functionality
- [ ] Incremental variables( ++ / -- )
- [ ] Seperate functions/commands/drivers to easily adapt to future driver changes/additions etc.
- [x] Fix the visibility. Like none of these classes should be public lol.


## v1.2.2

* Fixed the issue where objects such as DateTime were having their instance referenced, instead of their return data.
* Reduced typeing to objects, functions, and parameters instead of string/number/variable etc.

* Added a sealed, un-callable function `GuaranteedHalt()` which will be used to dispose of some memory leaking objects that may be created by the user. `GuaranteedHalt()` is called after `Halt()` completes.
* Fixed an issue where functions inside `Halt()` were not being passed a blind execute flag, and as a result were not being parsed.
* `CheckScreen()` is now done on another thread, but will still block the main thread until complete or timed out(30 seconds)
* Fixed an issue where `Sleep()` was not being cancelled when halting the script.
* Added support for absolute/relative paths in @imports and ImageChecks, as well as pre-set directories.
* Fixed an issue where variables couldnt be set as other variables. 
* Fixed some performance issues with `CheckScreen()`.
* Fixed a breaking issue with `CheckScreen()` that was setting `.Threshold()` to 0 no matter what
* Added the `Timer()` function, as well as the `.Start()`,`.Stop()`,and `.Print()` extensions to utilize it.
* Added a sealed flag to certain functions, to prevent them from being overridden.
* Full support for nested anonymous functions including with passed parameters. 
* Fixed a numer of misc issues that were hindering performance; as well as readability.
* Added the extensions `.Then()`, `.Else()`, `.And()`, `.Or()`. The first two require invoked functions via string; the second two require conditional evaluations that return "True" or "False" `.Then()` is the only required extension for `If()`;
* Added support for Conditional evaluations using the `If()` function. Currently, `==`, `!=`, `>`, `<`, `>=`, `<=` operators are allowed to compare two objects. `If()` comes with a number of extensions listed above.
* Added mathematical expression support by using `[]`. Wrap any basic math expressions like this `[1 + 1 * (99)]` and it will return the result. Variables can be inside the expression, and the expression can be set as a variable.
* Added local and global variable assignment support for multiple types, including mathematical expressions with `[]`(read above note). 
* Added a toggle for Remote functionality via the `remote` console command. Accepts true/false input.
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
