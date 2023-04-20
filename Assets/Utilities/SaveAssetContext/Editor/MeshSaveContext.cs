using UnityEngine;
using UnityEditor;

public static class MeshSaveContext
{
	[MenuItem("CONTEXT/MeshFilter/Save Mesh Asset")]
	public static void MeshFilterContext (MenuCommand menuCommand)
	{
		MeshFilter meshFilter = menuCommand.context as MeshFilter;
		Mesh mesh = meshFilter.sharedMesh;
		SaveDialog(mesh);
	}

	[MenuItem("CONTEXT/Mesh/Save Mesh Asset")]
	public static void Context (MenuCommand menuCommand)
	{
		Mesh mesh = menuCommand.context as Mesh;
		SaveDialog(mesh);
	}

	private static void SaveDialog (Mesh mesh)
	{
		string title = "Save Mesh Asset";
		string path = EditorUtility.SaveFilePanel(title, "Assets/", mesh.name, "asset");
		if (string.IsNullOrEmpty(path))
			return;
		path = FileUtil.GetProjectRelativePath(path);
		bool isInstance = EditorUtility.DisplayDialog(title, "Save mesh as instance?", "Yes", "No");
		bool optimizeMesh = EditorUtility.DisplayDialog(title, "Optimize mesh?", "Yes", "No");
		Save(mesh, path, isInstance, optimizeMesh);
	}

	private static void Save (Mesh mesh, string path, bool isInstance, bool optimizeMesh)
    {
		Mesh target = isInstance ? Object.Instantiate(mesh) : mesh;
		if (optimizeMesh)
			MeshUtility.Optimize(target);
		AssetDatabase.CreateAsset(target, path);
		AssetDatabase.SaveAssets();
	}
}
