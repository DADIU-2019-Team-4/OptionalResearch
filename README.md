# OptionalResearch

Hello lovely programmers!

For those of you not occupied with motion matching, I have project you can do. The purpose of the project is to research and prototype Unity features that may prove useful for the graduation game.

This is 100% optional - Taking your autumn break instead is fine too (please do take some time off). Alternatively we can set off some time in the pre-production cycle as well, so this is just your opportunity have an early look.

This is not an individual exercise - collaboration and communication is encouraged.

---

## Task Ein: Unity version

Download Unity 2019.2.0, released on 30th of July 2019. Copypasta the URL below into your browser.

unityhub://2019.2.0f1/20c1667945cf

This old version contains bugs (hence the later updates), but this version is a technical requirement as listed on Podio. We will use this version for the graduation game.

When adding the module, check off
- Microsoft Visual Studio Community 2019
- Android Build Support
- Android SDK & NDK Tools
It should add up to about ~13 GB.

You can uncheck the documentation.

While you download, please read (or skim) the remainder of this read-me before moving into implementing anything.

## Task Deux: Project Setup and Architecture

Make a Light-Weight Render Pipiline (LWRP) Unity project and upload it to this repository. As you complete sub-tasks, feel free to commit changes.

Switch to building for Android and set the resolution to HD+, which iss `720 x 1480` pixels on the phone. The two previous minigames used WQHD+, which was `1440 x 2960` (twice the dimensions and four times the area size), which no doubt caused performance issues.

* Create an empty scene with a single empty game-object with one script component called "Game".
* Create a second scene with some simple 3D background and lighting. For example, put a plane as a ground object.
* Create a third scene with a 3D object, like a ball (pretend that this scene contains a player character and some gameplay systems - for now keep it simple).

Now, open the Game.cs script and add the following (I typed out this code in Notepad, so typos/mistakes may exist)

```c#
private List<IGameLoop> GameLoops = new List<IGameLoop>();
private bool inErrorState

public Awake() {
	DontDestroyOnLoad(gameObject);
}

void Update() {
	try
	{
		if (!inErrorState)
			foreach (var gameLoop in GameLoops)
				gameLoop.Update();
	}
	catch (System.Exception e)
	{
		HandleGameLoopException(e);
		throw;
	}
}

void HandleGameLoopException(System.Exception e)
{
	Debug.Log("EXCEPTION: " + e.Message + "\n" + e.StackTrace);
	// Time.timeScale = 0; // If certain game objects continue some behaviour, uncomment this line.
	inErrorState = true;
}

public static GameObject GetGame() { return this; }
public void AddGameLoop(IGameLoop gameLoop) { GameLoops.Add(gameLoop); }
```

Should we ever need it, we can extend the file with FixedUpdate and LateUpdate as well (but don't implement the code below).

```c#
void FixedUpdate()
{
	foreach (var gameLoop in GameLoops)
		gameLoop.FixedUpdate();
}

void LateUpdate() {
	try
	{
		// Update all other game objects.
		if (!inErrorState)
			foreach (var gameLoop in GameLoops)
				gameLoop.LateUpdate();
	}
	catch (System.Exception e)
	{
		inErrorState = true;
		throw;
	}
}
```

You may also need to implement the interface IGameLoop. In the same file, add
```c#
public interface IGameLoop
{
	void Update();
	// void FixedUpdate();
	// void LateUpdate();
}
```

### Discussion
The biggest advantage of this system is faster performance, a single call stack, and proper exception handling. We can also selectively choose which game loops to add and which to skip.

The disadvantage is that it is trickier to add references between objects, and that each game loop needs to be managed in the Game.cs file.

You may wonder: Since we override MonoBehaviour's Update function with our own update function, isn't the interface IGameLoop incompatible with MonoBehaviour? And if we cannot inherit MonoBehaviour, we lose access to standard Unity functions and cannot show the script in the inspector?

It turns out that this can work. An example:

```c#
// This is some other script file you want visible in the inspector for some other game object.

public class SomeOtherScriptFile: MonoBehaviour, IGameLoop
{
    // Update is called once per frame
    void IGameLoop.Update()
    {
        
    }

}
```

Notice the **explicit interface implementation**. You have to do this for every component to avoid using MonoBehaviour base functions.

If you do not want the script file to show up in the inspector, you can simply remove the MonoBehaviour inheritance. You will not lose access to running the game loop and pure C# code, but you do lose access to the basic Unity functions.

## Task Trois: Additive scenes.

https://docs.unity3d.com/Manual/MultiSceneEditing.html

For the next project, instead of using prefabs, we will be using additive scenes (Do an internet search if you're unsure what this means).

* In the Game.cs file, implement adding the two other scenes to the current one.

* You also need to check that the interaction between objects work. For example, the background scene can have a plane, and the game-system scene can have a ball that drops from the sky. If the ball lands on the plane (using physics and gravity) despite being different scenes, then it works.

Finally, finding object references can be tricker with this setup.

* Add a script component to the plane and another script component to the ball. When (and only when) both scenes are loaded, both objects needs to find the other object as reference. For testing purposes, serialize the GameObject field in each of the scripts into the inspector, but do not set the reference in the inspector. At runtime, the inspector should update when the scripts assign the GameObject references.

Both scripts need to use IGameLoop, and both instances need to be added to the Game.cs GameLoop data structure - otherwise, IGameLoop.Update() is never called. Try to complete this task without using `MonoBehaviour.Start()` or `MonoBehaviour.Update()`.

## Task Quatre: Asset Bundles

https://docs.unity3d.com/Manual/AssetBundlesIntro.html

Asset bundles allows us to select which resources to load at runtime
https://docs.unity3d.com/Manual/LoadingResourcesatRuntime.html

More importantly, we can package assets in bundles and download the bundles, which is often how patching works in Unity games.
https://docs.unity3d.com/Manual/AssetBundles-Patching.html

Below is an example of a game that has a minimal .apk file size compared to how much data and cache is used.

![ok](https://i.imgur.com/4FodLrc.jpg)

To access the menu above for Samsung S8 phones, go to the Settings app -> Apps -> [Unity project on the app list] -> Storage. To my knowledge, an empty 2D/3D Unity project is ~16 MB. An unedited LWRP Template project is 32.6 MB.

(If you are retrieving the Samsung S8 phones and have yet to set the resolution, you can do so at Settings app -> Display -> Screen resolution).

The task is to place all LRWP assets into bundles, then build the asset bundles. Place the asset bundles at some URL where you can programmatically retrieve them (maybe this repo?), then load the assets at run-time and enter the SampleScene from the LWRP Template.

https://docs.unity3d.com/ScriptReference/Networking.DownloadHandler.html

You may have to do some internet searches to accomplish this task. The asset bundle should ideally be downloaded to the [data folder](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) and be loaded.

If you succeeded, the built .apk file should now be 16-20 MB and the data/cache should contain the remaining ~16MB from the LWRP template.

## Task Cinq: Other useful features.

* Other teams will not set the resolution on their phones before playing, so use [this](https://docs.unity3d.com/ScriptReference/Screen.SetResolution.html) to forcibly set the application resolution to HD+ (720 x 1480). Set the Samsung S8 phone resolutions back to WQHD+. In the project, add an UI text displaying the current resolution (ironcally, UI is usually what screws up when resolutions mis-match, if you recall from our mini-game productions).
* Test that the game pauses when any exception is thrown.
* Implement a FPS counter. The first value is calculated by `1/Time.deltaTime`. Any following frames should take the average between the previous value and the next `1/Time.deltaTime` value to avoid too much fluctuation.
* Make multiple asset bundles that sum up to 100+ MB data/cache usage.
* Implement a progress bar when downloading the bundles. The previously linked [Networking.DownloadHandler](https://docs.unity3d.com/ScriptReference/Networking.DownloadHandler.html) has a function for this.
