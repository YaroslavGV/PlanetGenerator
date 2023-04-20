using UnityEngine;
using UnityEditor;

public static class Texture2DSaveContext
{
	[MenuItem("CONTEXT/Texture2D/Save Texture Asset")]
	public static void Context (MenuCommand menuCommand)
	{
		Texture2D texture = menuCommand.context as Texture2D;
		SaveDialog(texture);
	}

	private static void SaveDialog (Texture2D texture)
	{
		string title = "Save Mesh Asset";
		string path = EditorUtility.SaveFilePanel(title, "Assets/", texture.name, "asset");
		if (string.IsNullOrEmpty(path))
			return;
		path = FileUtil.GetProjectRelativePath(path);
		bool isInstance = EditorUtility.DisplayDialog(title, "Save mesh as instance?", "Yes", "No");
		Save(texture, path, isInstance);
	}

	private static void Save (Texture2D texture, string path, bool isInstance)
	{
		Texture2D target = isInstance ? Object.Instantiate(texture) : texture;
		AssetDatabase.CreateAsset(target, path);
		AssetDatabase.SaveAssets();
	}
}
