
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main: MonoBehaviour
{
	public string worldFileName;

	public List<string> modelRootDirectories = new List<string>();
	public List<string> worldRootDirectories = new List<string>();
	public List<string> fileRootDirectories = new List<string>();
	
	private static GameObject worldRoot = null;
	
	#region "SDFParser"
	private SDF.Root sdfRoot = null;
	private SDF.Import.Loader sdfLoader = null;
	#endregion
void Awake()
	{
		// Load Library for Assimp
#if UNITY_EDITOR
#	if UNITY_EDITOR_LINUX
		var assimpLibraryPath = "./Assets/Plugins/AssimpNet.4.1.0/runtimes/linux-x64/native/libassimp";
#	elif UNITY_EDITOR_OSX // TODO: need to be verified,
		var assimpLibraryPath = "./Assets/Plugins/AssimpNet.4.1.0/runtimes/osx-x64/native/libassimp";
#	else // == UNITY_EDITOR_WIN
		var assimpLibraryPath = "./Assets/Plugins/AssimpNet.4.1.0/runtimes/win-x64/native/assimp";
#	endif
#else
#	if UNITY_STANDALONE_WIN
		var assimpLibraryPath = "./CLOiSim_Data/Plugins/x86_64/assimp";
#	elif UNITY_STANDALONE_OSX // TODO: need to be verified,
		var assimpLibraryPath = "./Contents/PlugIns/libassimp";
#	else // == UNITY_STANDALONE_LINUX
		var assimpLibraryPath = "./CLOiSim_Data/Plugins/libassimp";
#	endif
#endif
		Assimp.Unmanaged.AssimpLibrary.Instance.LoadLibrary(assimpLibraryPath);
		
		worldRoot = GameObject.Find("World");
	}

	void Start()
	{
		StartCoroutine(LoadModel(modelRootDirectories[1] , worldFileName));
	}

	private IEnumerator LoadModel(string modelPath, string modelFileName)
	{
		sdfRoot = new SDF.Root();
		sdfRoot.fileDefaultPaths.AddRange(fileRootDirectories);
		sdfRoot.modelDefaultPaths.AddRange(modelRootDirectories);
		sdfRoot.worldDefaultPaths.AddRange(worldRootDirectories);
		sdfRoot.UpdateResourceModelTable();
		
		if (sdfRoot.DoParse(out var model, modelPath, modelFileName))
		{
			sdfLoader = new SDF.Import.Loader();
			sdfLoader.SetRootModels(worldRoot);

			// Debug.Log("Parsed: " + item.Key + ", " + item.Value.Item1 + ", " +  item.Value.Item2);
			model.Name = "Jetbot";

			yield return new WaitForEndOfFrame();
			yield return StartCoroutine(sdfLoader.StartImport(model));
			yield return new WaitForEndOfFrame();
			
		}

		yield return null;
	}
}