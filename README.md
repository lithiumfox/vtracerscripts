These are the scripts that I use for vtracer.

It's not a full repo, and you'll probably have to rework them to make them work, but some hard requirements:


- Virtual Motion Capture Source Code AND it's requirements (https://github.com/sh-akira/VirtualMotionCapture)
- iRSDK (https://github.com/NickThissen/iRacingSdkWrapper) [You may need to modify it to remove some C# that doesn't work with mono. I think I have a version of it somewhere if you need it.)
- YamlDotNet (requirement of iRSDK)
- Hessburg - Sunlight (https://assetstore.unity.com/packages/tools/particles-effects/sunlight-location-based-time-of-day-66399?clickref=1101lyIek3Vf&utm_source=partnerize&utm_medium=affiliate&utm_campaign=unity_affiliate) | Needed to control the lighting system for day/night cycles
- NodaTime (Needed to convert date-time values for iRacing to properly usable date time systems)
- TzLookup (Needed for database of timezones to convert UTC offset correctly based on iRacing's provided track coordinates)
- Rewired (Controller Input)

Now, these scripts mostly serves as a "proof of concept" to show how one could setup a VR Room Scale environment to correctly adjust based on in-game and real world systems. It's not fully fleshed out, in that it does not have:
- Cannot save the location of the sim rig
- Cannot provide Manual adjustment of any 3D objects
- Cannot handle any system to modify or attempt to modify the sim rig or shaders of any compontent shown

It's simply a script to control both the sim rig, the world lighting, and other such variables SOLELY around the in-game systems. But, I could never get myself to sit down and find out what I could include in the source code or not, because I'm NOT a programmer. I'm just a person who got hyper focused for like a year and then was "content" with what I built for myself. I'm sorry. I wanted to do so much more. But I don't even know... I just think I'll leave it at that.

I hope this at least can inspire you to do something better than me. If anything, the big thing I'd focus on is the conversion of the data itself. Most of the data is relatively easy to get. inputs are just inputs, iRacing telemetry provides most example data and can be either cached or pulled from again just to do an update (for example, trying not to update the lighting constantly is important when you're dealing with 8x speed in iRacing)

Regardless, I hope this is useful to someone.

I included the modified rig I used for this exmaple as well. It's jank, but if anything, hopefully the "concept" of it makes sense. I wanted to control it via bones so that someone could easily make a custom sim rig and just "plop" it in.

Explanation of files:

- "Rig" folder - Materials and textures for Unity
- RigUpdate.fbx - the 3D model of the sim rig
- iRacingSDKLoader.cs - (Just a basic loader CS)
- iRacingConnector.cs - Direct Integration with iRacing : Gathers Location, generates date time, and pulls car telemetry updates for North direction in iRacing, and converts it to Unity space, and then offsets all of that based on the current rotation of the sim rig (allowing for correct placement of lighting regardless of calibration changes !IMPORTANT)
- vtracerController.cs - Just rewired detection scripts and finds the rotation of the sim rig so that iRacingConnector can properly assess it's current position. Also adds shifter stuff. First script I ever did, lol. (Also calibrates the sim rig to the head position + right hand/Wheel position. NOTE: rightHand = GameObject.Find("Hand_R"); (Bone must be named Hand_R for it to function. This can be improved, I just suck!)


Some random quirks I can remember off the top of my head.

iRacing uses Radians. Sunlight uses Degrees. Conversion is needed.
Pretty sure iRacing and Unity are like flipped. I did a bunch of different methods, whatever math I did got it sorted out.
Do not move the avatar, move the sun. Problem with moving the avatar is PhysBones/SpringBones/DynamicBones. I mean you could... but.. probably a better method would be preferred.

Anyway, thank you for... idk, reading? I wish I did more with this. I just... don't think I'll ever be able to.
