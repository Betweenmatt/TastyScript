@import ../tools.ts;
@import UpperDungeonLib.ts;
@end
###
# This script will run ch7-ch1 upper dungeons. The global array `data` is how you tell
# the script which chapters to run/which area to run. If you have to stop in the middle,
# before you restart just set the chapters you've already done to false.
###
function.Start(){
	
	$var data = array(
	#dothis(bool),area(number)
		array(true,1), #chapter7
		array(true,1), #chapter6
		array(true,1), #chapter5
		array(true,2), #chapter4
		array(true,1), #chapter3
		array(true,1), #chapter2
		array(true,1)  #chapter1
	);
	### Area to Fragment
	# 1 = Justice
	# 2 = Force
	# 3 = Wizdom
	# 4 = Strategy
	# 5 = Patience
	# 6 = Harmony
	# 7 = Luck
	###
	DoLoop();
	PrintLine("Upper Dungeons Complete!");
}
function.DoLoop(){
	Loop(=>(var i){
		var current = data.GetItem(i);
		var flag = current.GetItem(0);
		if(!flag).then(=>(){
			Print("Skipping ");PrintLine(currentArea);
			SetCurrentArea();
			Continue();
		}).else(=>(){
			var a = current.GetItem(1);
			var output = "Starting " + currentArea + "-" + a;
			PrintLine(output);
			PortalTo(currentArea);
			var offset = [a - 1];#offset for zero index
			var arr = GetArrayFromString(currentArea);
			SelectArea(arr.GetItem(offset));
			Battle();
			SetCurrentArea();
		});
	}).For(6);
}
function.Battle(){
	#Touch(1190,635);
	Touch(953,645,1500);
	Touch(873,655,1500);#auto repeat
	Accept();
	Sleep(220000);
	Loop(=>(){
		CheckScreen(=>(){
			KeyEvent("Back").For(5);
			Touch(1217,596);
			Sleep(20000);
			#exit
			Break();
		},=>(){
			#do nothing
		},"img/haveUdKeysTemplate.png");#since i have 10 extra keys. 
										#Use outOfTicketsTemplate.png if 
										#you don't have any extra keys
	});
}
#bypass the Accept in the tools.ts import
#because it had a chance to spend rubies
#in ud/conquest
function.Accept(){
	Touch(528,532);
}


