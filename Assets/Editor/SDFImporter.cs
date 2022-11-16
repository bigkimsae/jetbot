using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.EditorCoroutines.Editor;
using UnityEditorInternal;

public class SDFImporter : EditorWindow
{
    public static string worldFileName;
    private static GameObject worldRoot = null;
    public static string name;
    public static List<string> modelRootDirectories = new List<string>();
    public static List<string> fileRootDirectories = new List<string>();
    private SerializedObject _serializedObject;
    private ReorderableList _reorderableList;
    #region "SDFParser"
    private SDF.Root sdfRoot = null;
    private SDF.Import.Loader sdfLoader = null;
    #endregion
    
    [MenuItem("MyTool/Open Tool Window")]
    static void Open()
    {
	    worldFileName = "modeloriginal.sdf";
	    modelRootDirectories.Add(@"E:\UnityStudyProject\JetbotSim\Assets\Resources\SDF");
	    modelRootDirectories.Add(@"E:\UnityStudyProject\JetbotSim\Assets\Resources\SDF\jetbot");
	    fileRootDirectories.Add(@"E:\UnityStudyProject\JetbotSim\Assets\Resources\SDF");
        var window = GetWindow<SDFImporter>();
        window.titleContent.text = "Tool";
    }
    
    // private void OnGUI()
    // {
	   //  _serializedObject = new SerializedObject(this);
	   //  _reorderableList =
		  //   new ReorderableList(_serializedObject, _serializedObject.FindProperty("modelRootDirectories"), true, true, true, true);
	   //  
	   //  _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
		  //   {
			 //    rect.y += 2f;
			 //    rect.height = EditorGUIUtility.singleLineHeight;
    //
			 //    GUIContent objectLabel = new GUIContent($"");
			 //    EditorGUI.PropertyField(rect, _reorderableList.serializedProperty.GetArrayElementAtIndex(index), objectLabel);
		  //   }
	   //  ;
	   //  _reorderableList.DoList(new Rect(Vector2.zero, Vector2.one * 500));
	   //  if (GUILayout.Button("모든 Scene Object 선택하기"))
	   //  {
		  //   EditorCoroutineUtility.StartCoroutine(LoadModel(modelRootDirectories[1], worldFileName), this);
	   //  }
    // }

    public void Init()
    {
        
    }
    
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

        // Calling this method is required for windows version
        // refer to https://thomas.trocha.com/blog/netmq-on-unity3d/

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        OnDemandRendering.renderFrameInterval = 1;

        var mainCamera = Camera.main;
        mainCamera.depthTextureMode = DepthTextureMode.None;
        mainCamera.allowHDR = true;
        mainCamera.allowMSAA = true;

        worldRoot = GameObject.Find("World");

    }
    
    private IEnumerator LoadModel(string modelPath, string modelFileName)
    {
	    sdfRoot = new SDF.Root();
	    sdfRoot.fileDefaultPaths.AddRange(fileRootDirectories);
	    sdfRoot.modelDefaultPaths.AddRange(modelRootDirectories);
	    sdfRoot.UpdateResourceModelTable();
		
	    if (sdfRoot.DoParse(out var model, modelPath, modelFileName))
	    {
		    sdfLoader = new SDF.Import.Loader();
		    sdfLoader.SetRootModels(worldRoot);

		    // Debug.Log("Parsed: " + item.Key + ", " + item.Value.Item1 + ", " +  item.Value.Item2);
		    model.Name = "Jetbot";

		    yield return new WaitForEndOfFrame();
		    yield return EditorCoroutineUtility.StartCoroutine(sdfLoader.StartImport(model), this);
		    yield return new WaitForEndOfFrame();
			
	    }

	    yield return null;
    }
}
