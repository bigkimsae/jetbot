
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