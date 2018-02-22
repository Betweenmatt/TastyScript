#the awake function is called before any other function including
#start. don't put any heavy data here! only use for initializers/notes
#all awake functions in the script are executed!!!!
function.Awake(){
	ConnectDevice("YOUR DEVICE SERIAL NUMBER HERE");
	AppPackage("com.vespainteractive.KingsRaid");
	Sleep(2000);#you dont need this sleep, its just suppsed to slow down the initial touch burst
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
#this is the accept button for the standard popup
function.Accept(){
	Touch(631, 531);
}
