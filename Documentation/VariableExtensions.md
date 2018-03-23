# Variable Extensions

Variable extensions are new to v1.3.0, and are used to maniuplate various variable types such as strings and arrays.

## .GetIndex(*item*)
Returns the index of the item *item* in the attached array. Example: 

```
var arr = array("first", "second", "third");
var index = arr.GetIndex("second");
PrintLine(index); #will print the number 2
```

## .GetItem(*index*)
Returns the item at the index *index* in the attached array. Example:

```
var arr = array("first", "second", "third");
var thirdItem = arr.GetIndex(3);
PrintLine(thirdItem); #will print "third"
```

## .Length()
Returns the length of the array, or the number of characters in a string.

## .Replace(*in,out*)
Replaces *in* with *out* in the string.

## .Split(*in*)
Splits the attached string into an array by the value of *in*

## .SetItem(*item,index*)
If *index* is null, add's *item* to the attached array. If *index* is not null, it replaces the item in the attached array at index *index* with *item*.