# Tustan AR

Tustan AR is Augmented Reality App. It takes back wooden castle existed in IX-XVI centuries. You can reconstruct outdoor landmark of Your choose with the source.

How we made it: [Article](./Tustan%20AR%20Matching.md)

Links: [Play Market](https://play.google.com/store/apps/details?id=com.softserve.tustanar), [App Store](https://itunes.apple.com/ua/app/tustanar/id1283043687?mt=8)

## Demos

[![Tustan AR Demo @ WARP 1 Event @ UCU](https://img.youtube.com/vi/OBeoiyIr75M/0.jpg)](https://www.youtube.com/watch?v=OBeoiyIr75M)

[![warp tustan sxsw cut1](https://img.youtube.com/vi/ph_UmJngLQ8/0.jpg)](https://www.youtube.com/watch?v=ph_UmJngLQ8)

## Requirements

#### App tested on
- Unity 5.6.0f3 and higher;
- Vuforia 6.2.10+;
- Android Build Tools 23+ (latest may have not been supported)

#### Operational Systems
- Windows 10 x64;
- Mac OS X Sierra and higher;

#### Plugins
- Android Studio 2.3+;
- Xcode 9+.

#### Minimal software requirements
- iOS 10+;
- Android 4.4+ (Unity may not support latest Android versions).

## Configuration

Main scene is `Scenes/Scene`.

### Enable Vuforia

If you have Unity 2017.2 or later, you must have Vuforia component installed (if you haven't installed it, get it [here](https://docs.unity3d.com/Manual/InstallingUnity.html)). 
1. Go to `GameObject -> Vuforia -> ARCamera` menu, accept installing Vuforia components. After that delete newly created ARCamera gameobject from scene.
2. Go to `Resources/VuforiaConfiguration` in Project hierarchy and set checkboxes `Load TustanARStableTargets Database` and `Activate` on.

For previous Unity versions you must download Vuforia manually from Vuforia [website](https://developer.vuforia.com/downloads/sdk) and activate `TustanARStableTargets` Database.

Now you can build and run the project!

<img src="./article-src/Point_1.jpeg" style="max-width: 768px" alt="Two big stones on the top of hill.">
(By the way, you can test the app, pointing phone camera to the image above. :))

### Enable iOS plugin

This is a hack which enables native behavior on iOS platform. Fix is impossible due to Objective-C language features.

#### Unity 2017.2+:

1. Go to `Applications/Unity/PlaybackEngines/VuforiaSupport/iOS` folder.
2. Copy file `VuforiaNativeRendererController.mm` to other folder on your drive.
3. Сomment last line in the file (add to slashes in the beginning of `IMPL_APP_CONTROLLER_SUBCLASS(VuforiaNativeRendererController)`)
4. Save file and replace old file in `VuforiaSupport/iOS` with modified.
5. You're done!

#### Legacy Unity versions:
1. Go to `Assets/Plugins/iOS/` folder in your project.
2. Сomment last line in the file `VuforiaNativeRendererController.mm`(add to slashes in the beginning of `IMPL_APP_CONTROLLER_SUBCLASS(VuforiaNativeRendererController)`)
3. You're done!

### Rebuild Android plugin

If you want to rebuild Android plugin (`/plugin` folder in repository root), you must find `classes.jar` file, provided with Unity ([how to find](https://docs.unity3d.com/Manual/AndroidUnityPlayerActivity.html)), and copy that file to the Android project.

### Texture quality

Unity lightmaps has different formats in different versions. Current textures are baked with Unity 2017.2.0f3. You may want to rebake them. [How to bake?](https://docs.unity3d.com/Manual/GlobalIllumination.html)
