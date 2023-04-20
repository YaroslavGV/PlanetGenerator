using UnityEngine;
using UnityEditor;

namespace Planet
{
    [CustomEditor(typeof(PlanetView))]
    public class PlanetViewEditor : Editor
    {
        private static bool _autoGenerate = true; 
        private PlanetView _target;
        private SerializedProperty _settings;
        private Editor _settingsEditor;

        private void OnEnable ()
        {
            _target = target as PlanetView;
            _settings = serializedObject.FindProperty("_settings");
            if (_settings != null)
                _settingsEditor = CreateEditor(_settings.objectReferenceValue);
            Undo.undoRedoPerformed += OnUndo;
        }

        private void OnDisable ()
        {
            Undo.undoRedoPerformed -= OnUndo;
        }

        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if (_settings.objectReferenceValue != null)
            {
                if (_settingsEditor == null)
                    _settingsEditor = CreateEditor(_settings.objectReferenceValue);

                _autoGenerate = EditorGUILayout.Toggle("Auto Generate", _autoGenerate);
                if (GUILayout.Button("Generate"))
                    _target.Generate();
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                _settingsEditor.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck() && _autoGenerate)
                    _target.Generate();
            }
            else
            {
                _settingsEditor = null;
            }
        }

        private void OnUndo ()
        {
            _target.Generate();
        }
    }

    public static class PlanetViewContext
    {
        [MenuItem("CONTEXT/PlanetView/Log Superficies Gradients")]
        public static void Context (MenuCommand menuCommand)
        {
            PlanetView planetView = menuCommand.context as PlanetView;
            planetView.Settings.Superficies.LogGradients();
        }
    }
}
