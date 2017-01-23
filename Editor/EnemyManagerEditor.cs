using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;


[CustomEditor(typeof(EnemyManager))]
public class EnemyManagerEditor : Editor {
    private ReorderableList list;
	
    private void OnEnable() {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("enemyTypes"), false, true, true, true);
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            Rect elementRect = new Rect(rect.x, rect.y + 1, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(elementRect, element, GUIContent.none);
        };
        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Enemy types");
        };
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        list.DoLayoutList(); 
        serializedObject.ApplyModifiedProperties();
    }

}
