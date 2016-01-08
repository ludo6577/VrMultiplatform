#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using System.Linq;

// Sources:
// http://jonathanpeppers.com/Blog/automating-unity3d-builds-with-fake
// http://www.thegameengineer.com/blog/2013/07/23/unity-3-building-the-project/
// http://ralphbarbagallo.com/2015/05/26/how-to-support-gear-vr-and-google-cardboard-in-one-unity3d-project/
public class BuildScript : MonoBehaviour {

	// Generated file info
	private static string productName = "TestScene";
	private static string outputFolder = "bin/";
	private static string outputFilenameAndroidCardboard = outputFolder + BuildScript.productName + "Cardboard.apk";
	private static string outputFilenameAndroidGearVR = outputFolder + BuildScript.productName + "GearVR.apk";

	// Folders that contains the manifest, GearVR keys, ...
	// Copy the content of imported files from the Cardboard and GearVR SDK in this news folders (manifest, phone keys, ...)
	private static string androidFolderDestination = Application.dataPath + "/Plugins/Android/";
	private static string androidFolderCardboard = Application.dataPath + "/Plugins/AndroidCardboard/";
	private static string androidFolderGearVR = Application.dataPath + "/Plugins/AndroidGearVR/";

	// Keystore path, username and password (set it in Player Settings)
	private static string keystorePath;
	private static string username;
	private static string password;

	// Generated version and build number
	private static string versionNumber;
	private static string buildNumber;


	[MenuItem("Build/Cardboard")]
	public static void BuildCardboard()
	{
		Init ();
		SwapAndroidPluginFolder (androidFolderCardboard);
		PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, "");
		Build(BuildTarget.Android, outputFilenameAndroidCardboard);
	}
	
	[MenuItem("Build/GearVR")]
	public static void BuildGearVR()
	{
		Init ();
		SwapAndroidPluginFolder (androidFolderGearVR);
		PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, "VR_SUPPORTED");	// Add a precompilation sympol to disable the cardboard SDK
		Build(BuildTarget.Android, outputFilenameAndroidGearVR);
	}
	
	
	
	
	private static void Init()
	{
		versionNumber = Environment.GetEnvironmentVariable("VERSION_NUMBER");
		if (string.IsNullOrEmpty(versionNumber))
			versionNumber = "1.0.0.0";
		
		buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");
		if (string.IsNullOrEmpty(buildNumber))
			buildNumber = "1";
		
		PlayerSettings.productName = productName;
		PlayerSettings.bundleVersion = versionNumber;

		keystorePath = PlayerSettings.Android.keystoreName;
		username = PlayerSettings.Android.keyaliasName;
		password = PlayerSettings.Android.keyaliasPass;
	}


	static void SwapAndroidPluginFolder(string folderSource) {

		if (Directory.Exists (androidFolderDestination)) 
			Directory.Delete (androidFolderDestination);

		if (Directory.Exists (folderSource)) {
			FileUtil.CopyFileOrDirectory (folderSource, androidFolderDestination);		
			AssetDatabase.Refresh ();
		} else {
			Debug.LogError("Source folder: " + folderSource + " Doesn't exist");
		}
	}


	private static void Build(BuildTarget target, string output){		
		var outputDirectory = output.Remove (output.LastIndexOf ("/"));
		if (!Directory.Exists(outputDirectory))
			Directory.CreateDirectory(outputDirectory);

		// Get all the scenes
		string[] levelList = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
		string scenesNames = "(";
		foreach( string s in levelList)
			scenesNames += s.Remove( s.IndexOf(".unity") ) + ", ";
		if (scenesNames.Length <= 1) {
			Debug.LogError("No scenes found! Please add scenes (Files -> Build Settings -> Scenes in build");
			return;
		}
		scenesNames = scenesNames.Remove (scenesNames.Length - 2) + ")";
		
		Debug.Log("Building Platform: " + target.ToString() );
		Debug.Log("Building Target: " + output);
		Debug.Log("Scenes Processed: " + levelList.Length );		
		Debug.Log("Scenes Names: " + scenesNames);

		// Build the project
		string results = BuildPipeline.BuildPlayer( levelList, output, target, BuildOptions.None );		
		if ( results.Length == 0 )
			Debug.Log("No Build Errors" );
		else
			Debug.LogError("Build Error:" + results);
	}
}

#endif