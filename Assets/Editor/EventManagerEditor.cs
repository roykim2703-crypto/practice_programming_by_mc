using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(이벤트매니저))]
[CanEditMultipleObjects]
public sealed class EventManagerEditor : Editor
{
    private bool heartEffectOpen = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("effectCanvas"));
        heartEffectOpen = EditorGUILayout.BeginFoldoutHeaderGroup(heartEffectOpen, "하트 이펙트 생성");
        if (heartEffectOpen)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("heartSprite"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("heartCount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("heartLifetime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("heartRise"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("heartSpread"));
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
