# Table of Contents
* [AddParams (Deprecated v1.2.1)](#addparams)
* [Color](#color)
* [Concat](#concat)
* [For](#for)
* [Threshold](#threshold)

---

## AddParams
***Deprecated from use with Print and PrintLine in v1.2.1***

 .AddParams(*Input*) | |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Input*(string,number) | 
*Functions* | [Print()](Functions#print), [PrintLine()](Functions#printline) | 
*Pass to Base()* | true | 
*Allow Multiple* | true | 
#### Description
Adds the *Input* to the Print() or PrintLine() output.

**Note:** Please use `.Concat()` instead, this is deprecated.
#### Examples


---

## Color
 .Color(*Color*) | |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Color*(string) | 
*Functions* | [Print()](Functions#print), [PrintLine()](Functions#printline) | 
*Pass to Base()* | true | 
*Allow Multiple* | false | 
#### Description
Changes the color of the printed text in the console.

Colors available: Red, Green, Blue, Yellow, Magenta, White, Gray, Black, DarkRed, DarkGreen, DarkBlue, DarkYellow, DarkMagenta
#### Examples
`PrintLine("Hello, World!").Color("Red");`

---

## Concat
 .Concat(*Input*) | |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Input*(string,number) | 
*Functions* | [Print()](Functions#print), [PrintLine()](Functions#printline) | 
*Pass to Base()* | true | 
*Allow Multiple* | true | 
#### Description
Adds the *Input* to the Print() or PrintLine() output.
#### Examples
`PrintLine("Hello, World!").Concat(" How are ").Concat("you?");`

```
function.Example(){
    Loop("LoopTest").For(1000);
}
function.LoopTest(i){
    PrintLine("Iteration: ").Concat(i);
}
```

---

## For
 .For(*Iterations*) | |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Iterations*(number) | 
*Functions* | [All](Functions) | 
#### Description
Loops the attached function. *Iterations* is required, but using the value `0` will result in an infinite loop.
#### Examples
`PrintLine("Hello, World!").For(12);`

```
function.Example(){
    EndlessLoop().For(0);
}
function.EndlessLoop(){
    PrintLine("Looping!");
}
```

---

## Threshold
 .Threshold(*Input*) | |  | |
:---:|:---:|:---:|:---:
*Expected Arguments* | *Input*(number) | 
*Functions* | [CheckScreen()](Functions#checkscreen) | 
*Pass to Base()* | true | 
*Allow Multiple* | false | 
#### Description
Changes the pixel recognition threshold in `CheckScreen()` The default is `90`(90%). This value will need to be changed based on your own needs/testing. ***Note:*** using <80% is not advised, because the wrong images may pass the threshold.
#### Examples
```
#using a higher threshold because it's very important that this image matches before continuing
CheckScreen("img.png","SucceedFunction","FailFunction").Threshold(95);
```

---

