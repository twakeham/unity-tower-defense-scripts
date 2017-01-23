using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

/// GUI editor extension for creating a nicer enemy wave editor

[CustomEditor(typeof(Wave))]
public class WaveComponentEditor : Editor {

    private ReorderableList list;

    private void OnEnable() {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("waveComponents"), true, true, true, true);

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("enemyPrefab"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("path"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 120, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("enemiesInWave"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 150, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("startTimeDelay"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 180, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("timeBetweenEnemies"), GUIContent.none);

        };

        list.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Enemy Wave Components");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

}
