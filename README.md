# TiltKart

This Unity-based game is intended to flesh out the [Karting Microgame](https://learn.unity.com/project/karting-template) sample provided as a tutorial by Unity as an example of making a pre-existing game compatible with the [TiltFive](https://www.tiltfive.com/) augmented reality system. The Unity example project provides many assets like multiple tracks and game modes which are not actually covered by the original tutorial. These items will be used more fully in this project.

## Added Features
- NPC racers
- TiltFive headset support
- Game mode selection
   - Time Limit
   - Total Laps
   - Crash
- Kart model selection
   - Default kart
   - Roadster
   - 4x4
- Track selection
   - Country
   - Mountain
   - Oval
   - Winding
- Non-Keyboard Input
   - TiltFive Wand Controller (only in "controller mode")
   - Android touchscreen
   - Gamepad

# Contributing

This repository was initially built using Unity version `2021.3.26f1`. Later Unity versions should be compatible, but this is not guaranteed.

As the goal of this repository is to be a starting point with a basic TiltFive integration, any contributions which add new Unity assets or packages to this project will be rejected as adding unnecessary complexity. However, contributors are welcome to fork this repository in order to pursue alternative goals which may include new Unity assets or packages.

# Building
Currently TiltFive supports Linux and Windows. Android support is also offered, but it is considered experimental. This repository will continue to be tested against Windows and Android builds only.

To build this project, simply use Unity's "Build Settings" menu. By default the project will be set to build for Android as this is the slightly more complex setup. If you wish to build for Windows, switching to build for Windows instead should be sufficient with no further steps required.

# Licensing
There are multiple licenses to keep in mind:
- Any code created for this repository is released under an Apache 2 license
- The Unity-provided Karting Microgame is (according to its Unity Asset Store [entry](https://assetstore.unity.com/packages/3d/vehicles/karting-microgame-urp-150956)) released under the [Standard Unity Asset Store EULA](https://unity.com/legal/as-terms) license.
   - Notably, this appears to mean that distribution of the "4x4" and "Roadster" assets is completely permissible when included with this game but developing with those assets may require either unlocking a license to the asset(s) yourself for free (by going through the Unity tutorial) or purchasing them outright from the Unity Asset Store. (This should not, however, be construed as official legal advice.)
- TiltFive releases their SDK, including their Unity plugin, under the Apache 2 license.
