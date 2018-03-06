@import tools.ts;
@end

###
# This script is just a tool for evaluating your template image in CheckScreen()
# against other screens. It checks the screen every loop for the template, and reports
# 'succeed' or 'fail'. It also measures the time in milliseconds that each CheckScreen()
# takes, so you can more easily determine sleep timers in your loop
###

override.Start(){
  	#store the start time for the stats displayed when halted
	$var startTime = DateTime;
  
	Loop("Check");
}
function.Check(var i){
	#start the performance timer
  	Timer().Start();
	PrintLine(DateTime).Color("DarkYellow");
	PrintLine("Iteration number: ").Concat(i).Color("Yellow");
	#check the screen against "/yourimage.png" and print the result 
  	CheckScreen(
		=>(){
			PrintLine("\n\t\t\tSucceed\n").Color("DarkGreen");
		},
		=>(){
			PrintLine("\n\t\t\tFail\n").Color("DarkRed");
		},
		"/yourimage.png");
	#add one to the total iterations since the loop is zero-index
	$var totalIterations = [i + 1];
  	#print the performance timer results for this iteration
	Print("Elapsed MS: ");
	Timer().Print().Color("Magenta");
	PrintLine("\n");
  	#stop the timer to prevent memory leaks!
  	#this probably isnt needed since .Start() is called at the beginning of the next loop
  	#and .Stop() is called at the halt function, but it doesnt hurt to make sure!
	Timer().Stop();
}
override.Halt(){
  	#stop the timer to prevent memory leaks!
	Timer().Stop();
  	#end run stats
	PrintLine("Total iterations: ").Concat(totalIterations);
	PrintLine("Start: ").Concat(startTime);
	PrintLine("End: ").Concat(DateTime);
}
