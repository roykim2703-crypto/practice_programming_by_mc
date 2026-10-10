using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MinecraftSkinCanvas))]
[CanEditMultipleObjects]
public sealed class MinecraftSkinCanvasEditor : Editor
{
    private bool appearanceOpen = true;
    private bool controlsOpen = true;
    private bool motionOpen = true;
    private bool breathingOpen = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawGroup("Appearance", ref appearanceOpen,
            "uiFont", "defaultSkin", "initialPosition", "avatarScale");
        DrawGroup("Controls", ref controlsOpen,
            "showControls", "turnSpeedDegrees", "initialYaw");
        DrawGroup("Motion", ref motionOpen,
            "attackDuration", "attackAngle", "jumpDuration", "defaultJumpHeight");
        DrawGroup("Breathing", ref breathingOpen,
            "breathUpdown", "breathOffset");

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGroup(string label, ref bool isOpen, params string[] propertyNames)
    {
        isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, label);
        if (isOpen)
        {
            EditorGUI.indentLevel++;
            foreach (string propertyName in propertyNames)
                EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), true);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
