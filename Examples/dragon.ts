@import Examples/tools.ts;
@end
override.Start(){
	#slow down the sleep timer on overnight stam burning
	SetDefaultSleep(1000);
	MainLoop().For(0);
}
function.MainLoop(){
	StartBattle();
	ReturnFromBattle();
}
function.StartBattle(){
	Touch(1132,623,1200);
	Accept();
	Touch(945,167);
}
function.ReturnFromBattle(){
	Touch(852,594);
	Touch(1217,596);
}
