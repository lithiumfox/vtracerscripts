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
- Saving the location of the sim rig
- Manual adjustment of any 3D objects
- Any system to modify or attempt to modify the sim rig or shaders of any compontent shown

It's simply a script to control both the sim rig, the world lighting, and other such variables SOLELY around the in-game systems. But, I could never get myself to sit down and find out what I could include in the source code or not, because I'm NOT a programmer. I'm just a person who got hyper focused for like a year and then was "content" with what I built for myself. I'm sorry. I wanted to do so much more. But I don't even know... I just think I'll leave it at that.

I hope this at least can inspire you to do something better than me. If anything, the big thing I'd focus on is the conversion of the data itself. Most of the data is relatively easy to get. inputs are just inputs, iRacing telemetry provides most example data and can be either cached or pulled from again just to do an update (for example, trying not to update the lighting constantly is important when you're dealing with 8x speed in iRacing)

Regardless, I hope this is useful to someone.
