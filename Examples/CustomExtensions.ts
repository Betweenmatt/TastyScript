#returns true or false if the `item` is in the extended array.
#add an additional argument true for parameter index to have the
#return value be the index of the item in the array
extension.Contains(this variable, item, index){
	var length = variable.Length(); var length--;
	Loop(=>(var i){
		var x = variable.GetItem(i);
		if(x == item).then(=>()
		{
			if(!?index).And(index).then(=>(){ return(i); });
			return(true); 
		});
	}).For(length);
	if(!?index).And(index).then(=>(){ return(-1); });
	return(false);
}
#Gets an item in an array where `index` index is equal to `equals`
#used for dictionary like 3d arrays
#returns an array
extension.Where(this variable, index, equals){
	if(?index).Or(?equals).then(=>(){
		Return(null);
	});
	var length = variable.Length(); var length--;
	Loop(=>(var i){
		var x = variable.GetItem(i);
		var thisItem = x.GetItem(index);
		if(thisItem == equals).then(=>(){
			return(x);
		});
	})
}

#this is flawed and will be made into a predefined extension with v1.3.3
#returns the array after shifting all items after the given index down one spot.
#known issue: does not effect the length of the array.
extension.Remove(this variable, index){
	var length = variable.Length(); var length--;
	var arr = variable;
	Loop(=>(var i){
		if(i <= index).then(=>(){ Continue(); });
		var thisItem = variable.GetItem(i);
		var newIndex = [i - 1];
		var arr = arr.SetItem(thisItem,newIndex);
	}).For(length);
	return(arr);
}