@import ../tools.ts;
@end
####
#This dragon script is much more complex than the others,
#it checks that it's in the raid room before doing any
#start battle/sell gear functions. It sells and grinds the gear
#in your inventory every even iteration.
#it also skips the screen check on the first iteration to save time
####
override.Start(){
	#set the global iteration count
	#we use this for the run count instead of the loop
	#iterations because we have more control of the increment
	$var iterations = 0;
	#set the start time for halt stats
	$var startTime = DateTime;
	
	Loop("MainLoop");
}
function.MainLoop(var i){
	#skip the screen check on the first pass
	If(i == 0)
		.Then(=>(i){
			PrintLine("Run number: ").Concat(i);
			StartBattle();
			#skips the rest of the loop this iteration
			Continue();
		});
		
	CheckScreen(=>(){
		#increment the iteration count
		$var iterations = [iterations + 1];
		PrintLine("Run number: ").Concat(iterations);
		#do the even check just to speed up things a little
		#since selling/grinding doesnt need to be done after
		#every run.
		var even = [iterations % 2];
		If(even == 0)
			.Then(=>(){
				SellCrap();
				GrindCrap();
				BackArrow();
				});
		StartBattle();
	},=>(){
		#do this a few times since its short and can help
		#speed things up
		ReturnFromBattle().For(5);
	},"/startbattle.png");
}
#tries to start the battle, uses a pot if needed
#and then tries again, and then sleeps for 2 minutes
function.StartBattle(){
	Touch(1132,623,1200);
	Accept();
	#click the pot on the right
	Touch(652,381);
	Accept();
	#touch yes
	Touch(839,565);
	Touch(1132,623,1200);
	Accept();
	Touch(945,167);
	Sleep(120000);#sleep 2 minutes
}
function.ReturnFromBattle(){
	Touch(852,594);
	Touch(1217,596);
}
function.SellCrap(){
	#open inventory
	Touch(124,443);
	#sell
	Touch(1075,624);
	#sell all
	Touch(729,629);
	#big sell button
	Touch(638,579);
	#are u sure?
	Accept();
	#click the x on the sellall box in case nothing to sell
	Touch(1074,102);
	#click in whitespace to clear anything that pops up
	Touch(421,659);
}
function.GrindCrap(){
	#make sure `Gear` tab is selected
	Touch(208,98);
	#touch grind
	Touch(845,625);
	#touch grind all
	Touch(729,629);
	#touch grind
	Touch(631,594);
	#touch yes;
	Touch(631, 531,3000);#sleep longer
	#make sure grind box isnt open
	Touch(1176,108);
	#touch middle of screen
	Touch(675,625);
}
#gives run stats when halted
override.Halt(){
	PrintLine("Total Runs: ").Concat(iterations);
	PrintLine("Start Time: ").Concat(startTime);
	PrintLine("End Time: ").Concat(DateTime);
}
