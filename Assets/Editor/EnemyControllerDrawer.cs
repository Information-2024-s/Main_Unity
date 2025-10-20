using UnityEngine;
using UnityEditor;

// EnemySpawnInfoクラスのインスペクターでの表示方法をカスタムする
[CustomPropertyDrawer(typeof(EnemySpawnInfo))]
public class EnemySpawnInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 各プロパティ（変数）への参照を取得
        var enemyName = property.FindPropertyRelative("enemyName");
        var enemyPrefab = property.FindPropertyRelative("enemyPrefab");
        var timeSincePreviousSpawn = property.FindPropertyRelative("timeSincePreviousSpawn");
        var spawnPosition = property.FindPropertyRelative("spawnPosition");
        var spawnRotationEuler = property.FindPropertyRelative("spawnRotationEuler");
        var movementType = property.FindPropertyRelative("movementType");
        var moveSpeed = property.FindPropertyRelative("moveSpeed");
        
        // --- ここから各移動タイプごとのプロパティ ---
        var moveDirection = property.FindPropertyRelative("moveDirection");
        var zigzagAmplitude = property.FindPropertyRelative("zigzagAmplitude");
        var zigzagFrequency = property.FindPropertyRelative("zigzagFrequency");
        var sideToSideDistance = property.FindPropertyRelative("sideToSideDistance");
        var waypoints = property.FindPropertyRelative("waypoints");
        var waypointWaitTime = property.FindPropertyRelative("waypointWaitTime");
        var patrolCoordinates = property.FindPropertyRelative("patrolCoordinates");
        var coordinateWaitTime = property.FindPropertyRelative("coordinateWaitTime");
        var circleRadius = property.FindPropertyRelative("circleRadius");
        var circleCenterOffset = property.FindPropertyRelative("circleCenterOffset");
        var circleClockwise = property.FindPropertyRelative("circleClockwise");
        var circlePlacementCount = property.FindPropertyRelative("circlePlacementCount");
        var circlePlacementRadius = property.FindPropertyRelative("circlePlacementRadius");
        // planeプロパティ（観覧車グループ用）
        var plane = property.FindPropertyRelative("plane");
        var floatingAmplitude = property.FindPropertyRelative("floatingAmplitude");
        var floatingSpeed = property.FindPropertyRelative("floatingSpeed");
        var initialMoveDirection = property.FindPropertyRelative("initialMoveDirection");
        var initialMoveDuration = property.FindPropertyRelative("initialMoveDuration");
        var homingWaitDuration = property.FindPropertyRelative("homingWaitDuration");

        // --- リスポーンとレア敵 ---
        var respawns = property.FindPropertyRelative("respawns");
        var respawnTime = property.FindPropertyRelative("respawnTime");
        var rareEnemyPrefab = property.FindPropertyRelative("rareEnemyPrefab");
        var rareEnemyChance = property.FindPropertyRelative("rareEnemyChance");

        // --- GUIの描画 ---
        // Foldoutで見やすくする
        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, $"{enemyName.stringValue} ({((EnemyMovementType)movementType.enumValueIndex).ToString()})");
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(enemyName);
            EditorGUILayout.PropertyField(enemyPrefab);
            EditorGUILayout.PropertyField(timeSincePreviousSpawn);
            EditorGUILayout.PropertyField(spawnPosition);
            EditorGUILayout.PropertyField(spawnRotationEuler);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("移動設定", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(movementType);
            EditorGUILayout.PropertyField(moveSpeed);

            // --- 選択された移動タイプに応じて表示項目を切り替え ---
            switch ((EnemyMovementType)movementType.enumValueIndex)
            {
                case EnemyMovementType.Straight:
                    EditorGUILayout.PropertyField(moveDirection);
                    break;
                case EnemyMovementType.Zigzag:
                    EditorGUILayout.PropertyField(zigzagAmplitude);
                    EditorGUILayout.PropertyField(zigzagFrequency);
                    break;
                case EnemyMovementType.SideToSide:
                    EditorGUILayout.PropertyField(sideToSideDistance);
                    break;
                case EnemyMovementType.WaypointPatrol:
                    EditorGUILayout.PropertyField(waypoints, true);
                    EditorGUILayout.PropertyField(waypointWaitTime);
                    break;
                case EnemyMovementType.RandomCoordinatePatrol:
                    EditorGUILayout.PropertyField(patrolCoordinates, true);
                    EditorGUILayout.PropertyField(coordinateWaitTime);
                    break;
                case EnemyMovementType.Circle:
                    EditorGUILayout.PropertyField(circlePlacementCount);
                    EditorGUILayout.PropertyField(circleClockwise);

                    // 配置数に応じて表示を切り替える
                    if (circlePlacementCount.intValue > 1)
                    {
                        EditorGUILayout.LabelField("観覧車設定 (Group)", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(circlePlacementRadius);
                        // planeプロパティの選択UI
                        if (plane != null)
                        {
                            EditorGUILayout.PropertyField(plane, new GUIContent("回転面"));
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("planeプロパティが見つかりません。EnemySpawnInfoにpublic Vector3 plane = Vector3.up;があるか確認してください。", MessageType.Warning);
                        }
                        EditorGUILayout.HelpBox("指定した数の敵が、SpawnPositionを中心点として半径(Radius)の円周上に配置され、全体が一体となって回転します。", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("単体円運動設定 (Individual)", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(circleRadius);
                        EditorGUILayout.PropertyField(circleCenterOffset);
                    }
                    break;
                case EnemyMovementType.Floating:
                    EditorGUILayout.PropertyField(floatingAmplitude);
                    EditorGUILayout.PropertyField(floatingSpeed);
                    break;
                case EnemyMovementType.InitialMoveThenHoming:
                    EditorGUILayout.PropertyField(initialMoveDirection);
                    EditorGUILayout.PropertyField(initialMoveDuration);
                    EditorGUILayout.PropertyField(homingWaitDuration);
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("リスポーン設定", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(respawns);
            if (respawns.boolValue) EditorGUILayout.PropertyField(respawnTime);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("レア敵設定", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(rareEnemyPrefab);
            if (rareEnemyPrefab.objectReferenceValue != null) EditorGUILayout.PropertyField(rareEnemyChance);


            EditorGUI.indentLevel--;
        }
        
        EditorGUI.EndProperty();
    }
}