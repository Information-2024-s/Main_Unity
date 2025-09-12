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
            case EnemyMovementType.None:
                // 追加項目なし
                break;
            case EnemyMovementType.Straight:
                // No extra fields needed
                break;
            case EnemyMovementType.Zigzag:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Zigzag Settings", EditorStyles.boldLabel);
                controller.zigzagAmplitude = EditorGUILayout.FloatField("Amplitude", controller.zigzagAmplitude);
                controller.zigzagFrequency = EditorGUILayout.FloatField("Frequency", controller.zigzagFrequency);
                break;
            case EnemyMovementType.Homing:
                // You could add fields for Homing settings here later
                break;
            case EnemyMovementType.LastBoss1:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("LastBoss1 Settings", EditorStyles.boldLabel);
                controller.rectangleWidth = EditorGUILayout.FloatField("Rectangle Width", controller.rectangleWidth);
                controller.rectangleHeight = EditorGUILayout.FloatField("Rectangle Height", controller.rectangleHeight);
                controller.sideDuration = EditorGUILayout.FloatField("Side Duration", controller.sideDuration);
                controller.initialDirection = (BossInitialDirection)EditorGUILayout.EnumPopup("Initial Direction", controller.initialDirection);
                break;
            case EnemyMovementType.InitialMoveThenHoming:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Initial Move Then Homing Settings", EditorStyles.boldLabel);
                controller.initialMoveDirection = EditorGUILayout.Vector3Field("Initial Move Direction", controller.initialMoveDirection);
                controller.initialMoveDuration = EditorGUILayout.FloatField("Initial Move Duration", controller.initialMoveDuration);
                controller.waitDuration = EditorGUILayout.FloatField("Wait Duration", controller.waitDuration);
                break;
            case EnemyMovementType.Circle:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Circle Settings", EditorStyles.boldLabel);
                controller.circleRadius = EditorGUILayout.FloatField("Circle Radius", controller.circleRadius);
                controller.circleCenterOffset = EditorGUILayout.Vector3Field("Circle Center Offset", controller.circleCenterOffset);
                break;
        }

        // If any value was changed, mark the object as "dirty" so Unity knows to save the changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(controller);
        }
    }
}