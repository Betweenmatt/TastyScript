@import Examples/tools.ts;
@end
override.Start(){
	# set this loop to a large number, or the number of times the buy/grind loop
	# can be run before you run out of raid points (currentRaidPoints / (buyLoopIterations * 450))
	Loop("RaidScreen").For(1000);
}
function.RaidScreen(i){
	Print(DateTime);
	Print(": Iteration number: ");
	PrintLine(i);
	#touch the weapon
	Touch(548, 463);
	#on buy loop, do less iterations than space in inventory
	Loop("BuyLoop").For(42);
	GoToInventoryAndGrind();
}
function.GoToInventoryAndGrind(){
	#touch back arrow twice
	BackArrow();
	BackArrow();
	#touch inventory button
	Touch(169,654,3000);
	#touch grind
	Touch(845,625,3000);
	#touch grind all
	Touch(729,629,3000);
	#touch grind
	Touch(631,594,3000);
	#touch yes;
	Touch(631, 531,3000);
	#touch middle of screen
	Touch(675,625,3000);
	BackArrow();
	#touch forge
	Touch(1195,638,3000);
	#touch shop
	Touch(632,563,3000);
}
function.BackArrow(){
	Touch(134,26,3000);
}
function.BuyLoop(i){
	#touch buy button
	Touch(1008, 648);
	#confirm buy button
	#Touch(631, 531);
}
