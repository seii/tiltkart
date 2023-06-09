# TiltKart

This Unity-based game is intended to fully develop the [Karting Microgame](https://learn.unity.com/project/karting-template) sample provided as a tutorial by Unity while also making it compatible with the [TiltFive](https://www.tiltfive.com/) augmented reality system. The Unity example project provides many assets like multiple tracks and game modes which are not actually covered by the tutorial but can be fully implemented without adding any additional assets.

## Added Features
- NPC racers
- TiltFive headset support
- Game mode selection
   - Time Trial
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

## Planned Features
- Game mode selection
   - Total Laps
   - Ghost Race

# Contributing

This repository will continue to track the latest LTS version of Unity. (Currently, this version is `2021.3.26f1`.)

Contributors must refrain from adding new Unity assets or packages to this project, as doing so would complicate the current stated goal. However, contributors are welcome to fork this repository in order to pursue alternative goals which may include new Unity assets or packages.

# Building
Currently TiltFive supports Linux and Windows. Android support is also offered, but it is considered experimental. This repository will continue to be tested against Windows and Android builds.

To build this project, simply use Unity's "Build Settings" menu. By default the project will be set to build for Android as this is the slightly more complex setup. If you wish to build for Windows, switching to build for Windows instead should be sufficient with no further steps required.

# Licensing
There are multiple licenses to keep in mind:
- This project is released under an Apache 2 license
- The Unity-provided Karting Microgame is (according to its Unity Asset Store [entry](https://assetstore.unity.com/packages/3d/vehicles/karting-microgame-urp-150956)) released under the [Standard Unity Asset Store EULA](https://unity.com/legal/as-terms) license.
- TiltFive releases their SDK, including their Unity plugin, under the Apache 2 license.
