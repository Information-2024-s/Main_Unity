using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class EnemyControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the component we are inspecting
        EnemyController controller = (EnemyController)target;

        // --- Use the correct public variable names ---
        // Change controller._movementType to controller.movementType
        controller.movementType = (EnemyMovementType)EditorGUILayout.EnumPopup("Movement Type", controller.movementType);
        
        // Change controller._moveSpeed to controller.moveSpeed
        controller.moveSpeed = EditorGUILayout.FloatField("Move Speed", controller.moveSpeed);

        // Check the value of the enum and show/hide fields accordingly
        switch (controller.movementType)
        {
            case EnemyMovementType.Straight:
                // No extra fields needed
                break;

            case EnemyMovementType.Zigzag:
                // Show fields for Zigzag settings
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Zigzag Settings", EditorStyles.boldLabel);
                controller.zigzagAmplitude = EditorGUILayout.FloatField("Amplitude", controller.zigzagAmplitude);
                controller.zigzagFrequency = EditorGUILayout.FloatField("Frequency", controller.zigzagFrequency);
                break;

            case EnemyMovementType.Homing:
                // You could add fields for Homing settings here later
                break;
        }

        // Apply changes so they are saved
        if (GUI.changed)
        {
            EditorUtility.SetDirty(controller);
        }
    }
}