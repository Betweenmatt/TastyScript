# How To Create A Functioning Scripts

Hey!

I wrote this tutorial to help out the people who aren't used to programming! This should be much easier to understand than looking through endless files of code ;)

# Creating the script
The actual creation of the script is the easy part. First you create a new txt file(you know how to do that right?) and giving it a descriptive name. While you're at it, change the extension to `.ts` so you can find it easier later. We will use the name `example.ts` in this tutorial.

Once you've created your script, the first thing you need to do is open it and add this block of code

```
override.Start(){

}
```

This is the entry point into your script which means when the program reads the script, it will execute the `Start()` function first.

All of your code should remain within the function blocks `{ }`, and any code outside the blocks will either be ignored or give you an error and fail to continue.

Lets add a couple functions to your `Start()` function to get you started. First you need to get your device serial number(I'm assuming you already installed everything and got it hooked up by following the How To here*). Type in the console `devices` and your device serial number should be printed in the console. Once you have the serial number, change your `Start()` function to look like this, where `DEVICE SERIAL` is the serial that was printed in the console.:

```
override.Start(){
   ConnectDevice("DEVICE SERIAL");
}
```

Now when you run the script, you will see a dark green prompt saying `Device xxx has been connected`

![Connect Device](/Images/connectdevice.png)

# Adding functions to your script
Now that we got the creation out of the way, how about we create a new function in your `example.ts`! Add this block of code after the `Start()` function you already added:

```
function.MyFunction(){

}
```

Great! Now we need to make your `Start()` function call your `MyFunction()`. Do this by adding the line `MyFunction();` inside the code block of your `Start()` function like this

```
override.Start(){
   ConnectDevice("DEVICE SERIAL");
   MyFunction();
}
```

It's as simple as that! Now if you were to run your `example.ts`, nothing would actually happen because `MyFunction()` has nothing inside it. To solve that; let's add a `PrintLine()` function, so that you know that `MyFunction()` is being called correctly, like so:

```
function.MyFunction(){
   PrintLine("Hello, World! I'm being called from MyFunction!");
}
```

Now if you were to run your `example.ts`, `Hello, World! I'm being called from MyFunction!` would be printed in the console!

# Touch Functions
So now that you know how to create a script and add functions, let's get down to the actual work your script is supposed to do.

The `Touch()` function does exactly what it says; it touches at the pixel location you give it. Let's add the function to your `MyFunction()` block: `Touch(150,250);`. If you run this script, The pixel at X Position 150 and the Y Position 250 will be clicked.

# Applying what you've learned
Hopefully you learned enough so far to continue! I'm going to be adding pictures to help describe some of the examples. Also, the following examples I'm using are from the game Kings Raid, but any Android game will work.

So first, open up your TastyScript console. Then open up Kings Raid and create a new dragon room, lets say BD70. Now on your TastyScript console type the command `connect 'DEVICE SERIAL'` where `'DEVICE SERIAL'` is the serial of your device. The console should prompt you in dark green of your success. Then type in the command `screenshot DragonRoom.png` and press enter and wait for it to finish. Once it finishes, you will find the image `DragonRoom.png` in your TastyScript directory!

![Console taking screenshot](/Images/console_dragonroom.png)

Now right click the image, and click `open with` and select Paint. *Note: You don't need to use paint, you can use any image software that gives pixel locations(I think photoshop does this); or you could just use the Android developers option "Show Touches". I prefer using images though, because I can very easily see if touches will interact with different buttons on other screens*

Now put your mouse over the "Start Battle" button, and look to the bottom left-hand side of the Paint window; You will see something similar `1056,645px`. That number is the location of that pixel!

![Showing location](/Images/dragonroom.png)

Now in your `example.ts` file, create a new function called `StartBattle()`:
```
function.StartBattle(){
   Touch(1056,645);#enter the location that you see, not what i've typed in!
}
```

If you were to run this script, it would touch the "Start Battle" button. Now assuming you're doing this for auto runs, you're also going to want to touch the accept button when that popup shows up saying you don't have 9 heroes in there. I'm also going to add a safety touch, which will close the stamina popup if you run out of stamina. This safety touch just closes the stamina box immediately after it opens so no other touch would potentially do unwanted things. *You will need to get the actual locations relevant to your device*

```
function.StartBattle(){
   Touch(1056,645);#enter the location that you see, not what i've typed in!
   Touch(631, 531);
   Touch(945,167);#saftey touch!
}
```

Great! Now we need to give the script an ending battle function. This function basically touches the Abandon button and the the Exit button. Now you may ask yourself "what about the claim rewards button?", well this script uses a more brute force method to execute. The entire loop runs 5 or so times per minute, clicking on all these areas. Now as long as you pay attention where your clicks happen, there will be no overlap causing unwanted actions. Also keep in mind this script is very basic, and requires occasional intervention for more stamina/inventory clearing. If you want more complex scripts take a look at the Examples folder!

```
function.EndBattle(){
   Touch(852,594);
   Touch(1217,596);
}
```

So now we have to connect it all together! we're going to create *another* new function, called `MainLoop()`, this function will be responsible for being looped:

```
function.MainLoop(){
   StartBattle();
   EndBattle();
}
```

As you can see, there's not much going on there. Now, finally change your `Start()` function to look like this: 

```
override.Start(){
   MainLoop().For(0);#endless loop!
}
```
While you're at it, lets add a `ConnectDevice()` function and a `AppPackage()` function. To get the `DEVICE NAME` type `devices` in the console, and pick your device. To get the `PACKAGE NAME`, If you're playing Kings Raid, then just use `com.vespainteractive.KingsRaid` or if you're playing a different game you have to open the game or app on your phone and make sure its in focus, then type in the TastyScript console `shell dumpsys window windows | grep -E 'mCurrentFocus'` and press enter. You will see an output similar to the following image:

![shell get focus example](/Images/shell_getpackage.png)

Find the app package where it says `com.xxx.xxx` followed by a slash and another `com.xxx.xxx`; the section before the slash is the App Package. As you can see in the image above, `com.vespainteractive.KingsRaid` is before the slash, because that is the app I had opened when I took the picture. ***Note:*** You can opt-out of using an App Package name, but I *highly* suggest that you don't. If you don't use one, the bot will continue to run, touch, press buttons no matter what is the current focus; even if you get a phone call, or your boss walks in and you press the home button really fast - the bot will keep going. With setting the correct App Package, the bot checks before it makes any action to see if its on the right focus. Occasionally a touch will get through when dropping focus due to lag between ADB and the device, but with v1.2.1 its pretty rare.

So now your `Start()` function will look something like this:

```
override.Start(){
   ConnectDevice("192.168.4.10:5555");
   AppPackage("com.vespainteractive.KingsRaid");
   MainLoop().For(0);#endless loop!
}
```

And that's that! Running this script will touch "Start Battle", "Accept", "Popup X", "Abandon", "Exit" over and over again until you stop it. So create a new BD Room, set your heroes and try it out!

Here is the full script, so you can see how it's supposed to look:

```
override.Start(){
   ConnectDevice("192.168.4.10:5555");
   AppPackage("com.vespainteractive.KingsRaid");
   MainLoop().For(0);#endless loop!
}
function.MainLoop(){
   StartBattle();
   EndBattle();
}
function.StartBattle(){
   Touch(1056,645);#enter the location that you see, not what i've typed in!
   Touch(631, 531);
   Touch(945,167);#saftey touch!
}
function.EndBattle(){
   Touch(852,594);
   Touch(1217,596);
}
```

# Conclusion
Hopefully this was a good in-depth explanation on building scripts in TastyScript! With this How To combined with the documentation and Examples I've provided you should be on your way to creating all sorts of automated scripts.
