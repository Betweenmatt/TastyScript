@import Examples/tools.ts;
@end
override.Start(){
	# set this loop to a large arbitrary number, or the number of times the buy/grind loop
	# can be run before you run out of raid points (currentRaidPoints / (buyLoopIterations * 450))
	Loop("RaidScreen").For(1000);
}
function.RaidScreen(i){
	PrintLine(DateTime).Concat(": Iteration number: ").Concat(i);
	#touch the weapon
	Touch(548, 463);
	#on buy loop, do less iterations than space in inventory
	#so if your inventory is 114/200, then use .For(85);
	Loop("BuyLoop").For(85);
	GoToInventoryAndGrind();
}
function.GoToInventoryAndGrind(){
	#Some of these touches use longer sleep timers because
	#sometimes screen lag would cause issues.
	#if you have issues still, try increasing the sleep times,
	#or adding sleep timers to the overloaded touches
	
	#touch back arrow twice
	BackArrow();
	BackArrow();
	#touch inventory button
	Touch(169,654,2000);
	#touch grind
	Touch(845,625);
	#touch grind all
	Touch(729,629);
	#touch grind
	Touch(631,594);
	#touch yes;
	Touch(631, 531,3000);#sleep longer
	#touch middle of screen
	Touch(675,625);
	BackArrow();
	#touch forge
	Touch(1195,638,3000);
	#touch shop
	Touch(632,563);
}
function.BackArrow(){
	Touch(134,26, 2000);
}
function.BuyLoop(i){
	#touch buy button
	Touch(1008, 648,300);
	#confirm buy button
	#removed with most recent patch
	#Touch(631, 531);
}
