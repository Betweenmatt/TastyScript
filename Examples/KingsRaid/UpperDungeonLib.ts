
#set location constants
function.Awake(){
	#chapter touch locations
	#the first item in the array is the base 
	#screen position for the coordinates to work.
	#BL=bottom left, BR=bottom right,
	#TL=top left, TR=top right
	$var ch7 = array(
		array("BL",904,510),  #7-1
		array("BR",579,583),  #7-2
		array("BR",785,495),  #7-3
		array("BR",937,316),  #7-4
		array("BL",1192,225), #7-5
		array("BL",919,282),  #7-6
		array("BL",676,170)   #7-7
	);
	$var ch6 = array(
		array("BL",589,466),  #6-1
		array("BL",871,466),  #6-2
		array("BL",647,344),  #6-3
		array("BL",949,296),  #6-4
		array("BR",592,400),  #6-5
		array("BR",936,355),  #6-6
		array("BR",623,85)    #6-7
	);
	$var ch5 = array(
		array("BR",243,529),  #5-1
		array("BR",452,489),  #5-2
		array("BR",642,440),  #5-3
		array("BR",840,372),  #5-4
		array("BR",630,300),  #5-5
		array("BR",378,235),  #5-6
		array("BR",613,171)   #5-7
	);
	$var ch4 = array(
		array("BR",696,455),  #4-1 
		array("BR",423,503),  #4-2
		array("BR",154,495),  #4-3
		array("BL",513,403),  #4-4
		array("BL",455,176),  #4-5
		array("BL",733,111),  #4-6
		array("BR",500,155)   #4-7
	);
	$var ch3 = array(
		array("BR",468,502),  #3-1
		array("BR",710,492),  #3-2
		array("BR",940,444),  #3-3
		array("BR",780,290),  #3-4
		array("BR",580,282),  #3-5
		array("BR",236,245),  #3-6
		array("TL",594,455)   #3-7
	);
	$var ch2 = array(
		array("BR",764,425),  #2-1
		array("BR",757,292),  #2-2
		array("BR",650,188),  #2-3
		array("BR",486,124),  #2-4
		array("BR",277,175),  #2-5
		array("BL",690,240),  #2-6
		array("BL",530,335)   #2-7
	);
	$var ch1 = array(
		array("BL",473,529),  #1-1
		array("BL",627,437),  #1-2
		array("BL",758,380),  #1-3
		array("BL",930,317),  #1-4
		array("BL",1115,280), #1-5
		array("TR",675,424),  #1-6
		array("TR",540,321)   #1-7
	);
	$var currentArea = "ch7";
}

function.SelectArea(arr){
	var dir = arr.GetItem(0);
	var x = arr.GetItem(1);
	var y = arr.GetItem(2);
	if(dir == "BL").then(=>(){
		SetBaseBL();
	});
	if(dir == "BR").then(=>(){
		SetBaseBR();
	});
	if(dir == "TL").then(=>(){
		SetBaseTL();
	});
	if(dir == "TR").then(=>(){
		SetBaseTR();
	});
	Touch(x,y);
	Sleep(4000);
	Touch(1145,615,1500);#touch bottom right button
}
#swipe the screen to the far bottom left/right,
#perform 3 times to ensure that the correct base
#position is achieved
function.SetBaseBL(){
	Swipe(485,520,1160,90,500).For(3);
}
function.SetBaseBR(){
	Swipe(1000,535,200,220,500).For(3);
}
function.SetBaseTL(){
	Swipe(292,197,1086,526,500).For(3);
}
function.SetBaseTR(){
	Swipe(1206,196,387,552,500).For(3);
}
override.PortalTo(area){
	Touch(730,635);#portal
	Touch(1051,662);#conquest
	Base(area);#touch area
	Accept();#move to
	Sleep(8000);#sleep 8 seconds because loading takes forever on my phone
}
function.PortalTo(area){
	if(area == "ch7").then(=>(){
		Touch(524,525);
		Return();
	});
	if(area == "ch6").then(=>(){
		Touch(852,438);
		Return();
	});
	if(area == "ch5").then(=>(){
		Touch(524,438);
		Return();
	});
	if(area == "ch4").then(=>(){
		Touch(852,360);
		Return();
	});
	if(area == "ch3").then(=>(){
		Touch(524,360);
		Return();
	});
	if(area == "ch2").then(=>(){
		Touch(852,276);
		Return();
	});
	if(area == "ch1").then(=>(){
		Touch(524,276);
		Return();
	});
}
function.SetCurrentArea(){
	if(currentArea == "ch7").then(=>(){
		$var currentArea = "ch6";
		Return();
	});
	if(currentArea == "ch6").then(=>(){
		$var currentArea = "ch5";
		Return();
	});
	if(currentArea == "ch5").then(=>(){
		$var currentArea = "ch4";
		Return();
	});
	if(currentArea == "ch4").then(=>(){
		$var currentArea = "ch3";
		Return();
	});
	if(currentArea == "ch3").then(=>(){
		$var currentArea = "ch2";
		Return();
	});
	if(currentArea == "ch2").then(=>(){
		$var currentArea = "ch1";
		Return();
	});
}
function.GetArrayFromString(str){
	if(str == "ch7").then(=>(){
		Return(ch7);
	});
	if(str == "ch6").then(=>(){
		Return(ch6);
	});
	if(str == "ch5").then(=>(){
		Return(ch5);
	});
	if(str == "ch4").then(=>(){
		Return(ch4);
	});
	if(str == "ch3").then(=>(){
		Return(ch3);
	});
	if(str == "ch2").then(=>(){
		Return(ch2);
	});
	if(str == "ch1").then(=>(){
		Return(ch1);
	});
}