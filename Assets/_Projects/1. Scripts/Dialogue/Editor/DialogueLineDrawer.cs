using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueLine))]
public class DialogueLineDrawer : PropertyDrawer
{
    private const float Pad = 2f;
    private const float Line = 18f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
        float h = 0f;

        h += Line + Pad;
        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text"), true) + Pad;
        h += Line + Pad;
        h += Line + Pad;
        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("onShownAction"), true) + Pad;
        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("onAdvancedAction"), true) + Pad;

        if(!property.FindPropertyRelative("useAction").boolValue){
            h += Line + Pad;
            h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("onShown"), true) + Pad;
            h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("onAdvanced"), true) + Pad;
        }

        return h;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        EditorGUI.BeginProperty(position, label, property);

        var speaker = property.FindPropertyRelative("speaker");
        var text = property.FindPropertyRelative("text");
        var useAction = property.FindPropertyRelative("useAction");
        var onShownAction = property.FindPropertyRelative("onShownAction");
        var onAdvancedAction = property.FindPropertyRelative("onAdvancedAction");
        var onShown = property.FindPropertyRelative("onShown");
        var onAdvanced = property.FindPropertyRelative("onAdvanced");

        float y = position.y;

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, Line), speaker);
        y += Line + Pad;

        float textH = EditorGUI.GetPropertyHeight(text, true);
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, textH), text, true);
        y += textH + Pad;

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, Line), useAction);
        y += Line + Pad;

        bool use = useAction.boolValue;

        if(use){
            EditorGUI.LabelField(new Rect(position.x, y, position.width, Line), "Actions", EditorStyles.boldLabel);
            y += Line + Pad;

            float shownH = EditorGUI.GetPropertyHeight(onShownAction, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, shownH), onShownAction, true);
            y += shownH + Pad;

            float advancedH = EditorGUI.GetPropertyHeight(onAdvancedAction, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, advancedH), onAdvancedAction, true);
        }else{
            EditorGUI.LabelField(new Rect(position.x, y, position.width, Line), "Events", EditorStyles.boldLabel);
            y += Line + Pad;

            float shownH = EditorGUI.GetPropertyHeight(onShown, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, shownH), onShown, true);
            y += shownH + Pad;

            float advancedH = EditorGUI.GetPropertyHeight(onAdvanced, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, advancedH), onAdvanced, true);
        }

        EditorGUI.EndProperty();
    }
}