#the awake function is called before any other function including
#start. don't put any heavy data here! only use for initializers/notes
#all awake functions in the script are executed!!!!
function.Awake(){
	ConnectDevice();#connects to the first available device
	AppPackage();#sets the app that is currently in focus
	SetRes();#set the script resolution to the device resolution
	Print(DateTime);
	PrintLine(" : Starting...");
}
#the halt function is called when the script is ended prematurely
#when pressing the [ENTER] key while executing. this is mainly useful
#for datetime printing(performance analysis) or maybe screen grabbing
override.Halt(){
	Print("Halted at : ");
	PrintLine(DateTime);
}
#this is the accept button for the standard popup in KR
function.Accept(){
	Touch(631, 531);
}
#Gets the device screen size, and sets the touch override to the size.
#PLEASE NOTE: ScreenSize() gets the screen size in PORTRAIT, so we swap them
#on landscape games.
function.SetRes(){
	var size = ScreenSize();
	if(!?size).Then(=>(){
		$var XRes = size.GetItem(1);
		$var YRes = size.GetItem(0);
		var p = "Setting screen size to: " + XRes + " X " + YRes;
		PrintLine(p);
	});
}
###
# This is a helper function which converts the x y coordinates
# of a touch from my resolution(1280x720) to your resolution.
# to use this helper all you have to do is assign global variables
# with your resolution in the beginning of your script like so:
# $var XRes = 1440;
# $var YRes = 900;
# Once those global variables are assigned, all Touch() functions
# called in the script will be relative to the resolution you specified.
###
override.Touch(x,y,sleep){
	var xcoord = x;
	var ycoord = y;
	if(XRes != null).And(YRes != null)
		.then(=>(x,y,sleep){
			var xcoord = [(x / 1280) * XRes];
			var ycoord = [(y / 720) * YRes];
		});
	if(sleep != null)
		.then(=>(sleep){
			Base(xcoord,ycoord,sleep);
		}).else(=>(){
			Base(xcoord,ycoord);
		});
}
