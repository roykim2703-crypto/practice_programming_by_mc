using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkinController))]
[CanEditMultipleObjects]
public sealed class SkinControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("Attack"))
            {
                foreach (Object selected in targets)
                    ((SkinController)selected).Attack();
            }

            if (GUILayout.Button("Jump"))
            {
                foreach (Object selected in targets)
                    ((SkinController)selected).Jump();
            }
        }

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("Attack and Jump can be tested in Play mode.", MessageType.Info);
    }
}
