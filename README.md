# Core Framework for Unity

Current Version 2

Supported Unity versions: 2018.1.0f2. or latest.

If you need to use the old scripting runtime (.net 3.5) then use the [release-1.3 branch](https://github.com/nievesj/unity-core-project/tree/release-1.3).

What is Core Framework?
---
The purpose of Core Framework is to help speed up development in Unity3D by providing the following basic systems every game needs to have:
* Asset Bundle loading system that can load assets from:
	* Streaming assets folder
	* Web server or cloud service
	* Simulate asset bundles on editor
* UI System
	* Basic implementation of Widgets, Dialogs and Panels 
	* Transition animations by using DOTween, configurable on inspector
	* Can trigger sounds when a transition plays
* Basic audio system
* Mouse / Touch input control
* Factory tool
* Console window colors! Colorize your debug messages with colors so they are easier to read.
* Base game starting point ([Example Project](https://github.com/nievesj/unity_core_example))

Requirements
---
* .Net 4.5.
* .Net Standard 2.0
* Incremental Compiler

Demo
---
[Example Project](https://github.com/nievesj/unity_core_example)

Purpose
---
The main aspect of this library is loading and unloading asset bundles in a relatively simple way.

     _assetService.GetAndLoadAsset<Ball>(bundleNeeded)
                .TaskToObservable()
                .Subscribe(ball =>
                {
                    var myBall = Instantiate<Ball>(ball);
                });

How to integrate into a project?
---
This project is meant to be added to an existing Unity Project, either by downloading it and placing it in the "Plugins" folder, or by setting it as a subtree to your git repo. Alternatively you can use the [Example Project](https://github.com/nievesj/unity_core_example) as a starting point. 

Dependencies
---
Core Framework depends on the the following components
* [UniRx](https://github.com/neuecc/UniRx): And UniAsync. 
* [Zenject](https://github.com/modesttree/Zenject): Core Framework libraries are loaded and used with Dependency Injectiion.
* [AssetBundles-Browser](https://github.com/Unity-Technologies/AssetBundles-Browser): Unity's tool for building and organizing asset bundles. 
* [DOTween](https://github.com/Demigiant/dotween): Used in UI transitions.

Which platforms are compatible?
---
Has been tested on iOS, Android, Mac, Windows and WebGL.

Asset Bundles
---

For simplicity, the current asset bundle strategy for this tool is that each prefab is its own asset bundle, and asset bundles are organized by categories or directories. Image below is Unity's [AssetBundles-Browser](https://github.com/Unity-Technologies/AssetBundles-Browser).

![Asset Bundle Organization](http://www.josemnieves.com/unity/images/aborg.PNG)

 These directories are mapped to the enum [AssetCategoryRoot](https://github.com/nievesj/unity-core-project/blob/master/Services/AssetService/BundleRequest.cs#L97-L107) as shown below.

    public enum AssetCategoryRoot
	{
		None,
		Configuration,
		Services,
		Levels,
		SceneContent,
		GameContent,
		Windows,
		Audio,
		Prefabs
	}


The service also detects the platform it's running on, and uses that to get the asset bundles from the web in the following order: 

![Cloud Asset Bundle Structure](http://www.josemnieves.com/unity/images/webab.png)

This functionality is entirely seamless to the developer, thus requesting an asset is now as easy as:

       _assetService.GetAndLoadAsset<Ball>(bundleNeeded)
                .TaskToObservable()
                .Subscribe(ball =>
                {
                    var myBall = Instantiate<Ball>(ball);
                });

Simulating Asset Bundles
---
Asset Bundle simulation is enabled by default. If you wish to disable it go to menu Core Framework -> Disable Simulate Asset Bundles.

![Core Framework Preferences](http://www.josemnieves.com/unity/images/preferences.png)

Alternatively, there's also a _Core menu to enable/disable simulation mode

![Core Menu](http://www.josemnieves.com/unity/images/coremenu.png)

Asset Service Options
---
* Use Streaming Assets
	* Toggling this will load the asset bundles from the streaming assets folder
* Asset Bundles URL
	* Location where the asset bundles are stored on the cloud
* Cache Asset Bundles?
	* Toggle this if you want to cache the asset bundles on device. The file [UnityCloudBuildManifest.json](https://docs.unity3d.com/Manual/UnityCloudBuildManifest.html) needs to be present in order to cache bundles. 

Console window colors!
---

This feature allows you to easily colorize debug messages so you can keep track of related events by colors on editor. This functionality is disabled on builds so the console log doesn't become cluttered with color tags. 

![Asset Service Options](http://www.josemnieves.com/unity/images/consolecolors.png)


       Debug.Log(("My very awesome lime colored text!").Colored(Colors.Lime));
