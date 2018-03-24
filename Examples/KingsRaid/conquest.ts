@import ../tools.ts;
@end
###
# This script runs through the chh7-ch2 conquests. In the array `areaCompleteArray` set each chapter to false that you do not wish to run
###
function.Start(){
	#define tracking variables
	#an array of bools to define which conquests are done. if you want to skip
	#a conquest for whatever reason just set it to true
	$var areaCompleteArray = 
		array(
			true, #Chapter 7
			true, #Chapter 6
			true, #Chapter 5
			true, #Chapter 4
			true, #Chapter 3
			true  #Chapter 2
			);
	$var currentArea = "ch7";
	##
	SetDefaultSleep(1500);
	var arrlength = areaCompleteArray.Length(); var arrlength--;
	Loop(=>(var i){
		var flag = areaCompleteArray.GetItem(i);
		if(!flag).then(=>(){
			Print("Skipping ");PrintLine(currentArea);
			SetCurrentArea();
			Continue();
		});#skip if true
		var output = "Starting " + currentArea;
		PrintLine(output);
		PortalTo(currentArea);
		Battle();
		SetCurrentArea();
	})
	.For(arrlength);
	
	PrintLine("Conquest complete!");
}
function.Battle(){
	Touch(1190,635,1300);
	Touch(953,645,1500);
	Touch(873,655,1300);#auto repeat
	Accept();
	Sleep(200000);
	Loop(=>(var i){
		CheckScreen(=>(){
				KeyEvent("Back").For(4);
				Touch(1217,596);
				Sleep(15000);
				#exit
				Break();
			},=>(){
				#do nothing
			},"img/outOfTicketsTemplate.png");
	});
}
function.SetCurrentArea(){

	if(currentArea == "ch2").then(=>(){
		$var currentArea = "done";
		Return();
	});
	if(currentArea == "ch3").then(=>(){
		$var currentArea = "ch2";
		Return();
	});
	if(currentArea == "ch4").then(=>(){
		$var currentArea = "ch3";
		Return();
	});
	if(currentArea == "ch5").then(=>(){
		$var currentArea = "ch4";
		Return();
	});
	if(currentArea == "ch6").then(=>(){
		$var currentArea = "ch5";
		Return();
	});
	if(currentArea == "ch7").then(=>(){
		$var currentArea = "ch6";
		Return();
	});
}
override.PortalTo(area){
	Touch(730,635);#portal
	Touch(920,639);#conquest
	Base(area);#touch area
	Accept();#move to
	Sleep(15000);#sleep 15 seconds because loading takes forever on my phone
}
function.PortalTo(area){
	if(area == "ch2").then(=>(){
		Touch(524,276);
		Return();
	});
	if(area == "ch3").then(=>(){
		Touch(852,276);
		Return();
	});
	if(area == "ch4").then(=>(){
		Touch(524,360);
		Return();
	});
	if(area == "ch5").then(=>(){
		Touch(852,360);
		Return();
	});
	if(area == "ch6").then(=>(){
		Touch(524,438);
		Return();
	});
	if(area == "ch7").then(=>(){
		Touch(852,438);
		Return();
	});
}
#bypass the Accept in the tools.ts import
#because it had a chance to spend rubies
#in ud/conquest
function.Accept(){
	Touch(528,532);
}

