# _Core
Version 1

What is _Core?
---
_Core is a collection utilities and libraries to help speed up development in Unity3D by providing the following functionality:
* Asset Bundle loading system
	* Can load assets from the streaming assets folder
	* Can load assets from a web server
	* Can cache downloaded bundles
	* Can simulate asset bundles on editor
* Basic window system
	* Open / Close window
	* Observable events for when a window is opened or closed
	* Transition animations 
* Basic audio system
* Pooler tool
* Base game starting point ([Example Project](https://github.com/nievesj/unity_core_example) )

Purpose
---
The main aspect of this library is loading and unloading asset bundles in a relatively simple way.

    assetService.GetAndLoadAsset<Ball>(bundleRequest)
			.Subscribe(loadedBall =>
			{
				var myBall = GameObject.Instantiate<Ball>(loadedBall);
			});

How to integrate into a project?
---
This project is meant to be added to an existing Unity Project, either by downloading it and placing it in the "Plugins" folder, or by setting it as a submodule to your git repo. Alternatively you can use the [Example Project](https://github.com/nievesj/unity_core_example) as a starting point. 

Dependencies
---
_Core depends on the the following components
* [LeanTween](https://github.com/dentedpixel/LeanTween)
* [UniRx](https://github.com/neuecc/UniRx) (Reactive Extensions for Unity) Most of the _Core functionality is wrapped around Observables. 
* [AssetBundles-Browser](https://github.com/Unity-Technologies/AssetBundles-Browser) Unity's tool for building and organizing asset bundles. This is included in the project as a Git Submodule and it may need to be pulled the first time. 

Which platforms are compatible?
---
Has been tested on iOS, Android, Mac, Windows and [WebGL](http://www.josemnieves.com/unity/core_example_webgl_test/). 

Here is a live WebGL test of the example project: http://www.josemnieves.com/unity/core_example_webgl_test/



