using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloatingPlatforms))]
public class MyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Update the serialized object's representation
        serializedObject.Update();

        // Get references to the serialized properties
        #region Properties delcaration
        SerializedProperty moveOnTouch = serializedObject.FindProperty("moveOnTouch");
        SerializedProperty platformProp = serializedObject.FindProperty("platform");
        SerializedProperty verticalChangeProp = serializedObject.FindProperty("verticalChange");
        SerializedProperty horizontalChangeProp = serializedObject.FindProperty("horizontalChange");
        SerializedProperty verticalSpeedProp = serializedObject.FindProperty("verticalSpeed");
        SerializedProperty horizontalSpeedProp = serializedObject.FindProperty("horizontalSpeed");
        SerializedProperty horizontalMovementProp = serializedObject.FindProperty("horizontalMovement");
        SerializedProperty verticalMovementProp = serializedObject.FindProperty("verticalMovement");
        SerializedProperty duration = serializedObject.FindProperty("duration");
        SerializedProperty IsMoving = serializedObject.FindProperty("IsMoving");
        SerializedProperty canMove = serializedObject.FindProperty("canMove");
        SerializedProperty isContinous = serializedObject.FindProperty("isContinous");
        SerializedProperty fallAfterTouch = serializedObject.FindProperty("fallAfterTouch");
        SerializedProperty _waitTime = serializedObject.FindProperty("_waitTime");
        SerializedProperty _dontReturn = serializedObject.FindProperty("_dontReturn");
        #endregion

        // Draw all properties
        EditorGUILayout.PropertyField(platformProp);
        EditorGUILayout.PropertyField(horizontalMovementProp);


        if (horizontalMovementProp.boolValue)
        {
            EditorGUILayout.PropertyField(horizontalChangeProp);

            EditorGUILayout.PropertyField(isContinous);
            EditorGUILayout.PropertyField(_dontReturn);
            if (isContinous.boolValue)
                EditorGUILayout.PropertyField(horizontalSpeedProp);


        }

        EditorGUILayout.PropertyField(verticalMovementProp);

        if (verticalMovementProp.boolValue) 
        {
            EditorGUILayout.PropertyField(verticalChangeProp);
            EditorGUILayout.PropertyField(isContinous);
            EditorGUILayout.PropertyField(_dontReturn);
            if (isContinous.boolValue)
                EditorGUILayout.PropertyField(verticalSpeedProp);

        }

        EditorGUILayout.PropertyField(duration);

        EditorGUILayout.PropertyField(moveOnTouch);

        EditorGUILayout.PropertyField(IsMoving);
        EditorGUILayout.PropertyField(canMove);
        // Apply changes to the serialized object

        EditorGUILayout.PropertyField(fallAfterTouch);
        EditorGUILayout.PropertyField(_waitTime);

        serializedObject.ApplyModifiedProperties();

    }
}
