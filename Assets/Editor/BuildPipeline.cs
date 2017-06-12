// C# example.
using UnityEditor;
using System.Diagnostics;
using UnityEngine;
using System.IO;
public class ScriptBatch 
{
    [MenuItem("MyTools/Windows Build With Postprocess")]
    public static void BuildGame ()
    {
		if (path == null || path.Length == 0)
			path =	PlayerPrefs.GetString ("Build_editor_path");
		if (path == null || path.Length == 0) {
			path = EditorUtility.SaveFolderPanel ("Choose Location of Built Project", "", "");
			PlayerPrefs.SetString ("Build_editor_path", path);
		}

		if (zipToPath == null|| zipToPath.Length == 0)
			zipToPath =	PlayerPrefs.GetString ("Zip_editor_path");
		if (zipToPath == null || zipToPath.Length == 0) {
			zipToPath = EditorUtility.SaveFolderPanel ("Choose Location of zip file", "", "");
			PlayerPrefs.SetString ("Zip_editor_path", zipToPath);
		}
		// Get filename.
        string[] levels = new string[] {"Assets/Menu.unity", "Assets/MainScene.unity"};

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/Editor.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);

		ZipUtil.ZipDirectory (zipToPath + "/Editor.zip", path);

			
    }

	static string path;
	static string zipToPath;
}
