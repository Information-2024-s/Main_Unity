using UnityEngine;
using UnityEditor;

// どのクラスのプロパティ描画をカスタマイズするか指定
[CustomPropertyDrawer(typeof(EnemySpawnInfo))]
public class EnemySpawnInfoDrawer : PropertyDrawer
{
    // インスペクターにGUIを描画するメソッド
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // このプロパティの描画を開始することを示す
        EditorGUI.BeginProperty(position, label, property);

        // 各フィールドへの参照(SerializedProperty)を取得
        var prefabProp = property.FindPropertyRelative("enemyPrefab");
        var timeProp = property.FindPropertyRelative("timeSincePreviousSpawn");
        var posProp = property.FindPropertyRelative("spawnPosition");
        var useMovePosProp = property.FindPropertyRelative("useMovePosition");
        var movePosProp = property.FindPropertyRelative("movePosition");
        var rotProp = property.FindPropertyRelative("spawnRotationEuler");
        var onInitialMoveCompleteProp = property.FindPropertyRelative("onInitialMoveComplete"); // ★追加
        var moveTypeProp = property.FindPropertyRelative("movementType");
        var speedProp = property.FindPropertyRelative("moveSpeed");
        var zigzagAmpProp = property.FindPropertyRelative("zigzagAmplitude");
        var zigzagFreqProp = property.FindPropertyRelative("zigzagFrequency");
        var rectWidthProp = property.FindPropertyRelative("rectangleWidth");
        var rectHeightProp = property.FindPropertyRelative("rectangleHeight");
        var sideDurationProp = property.FindPropertyRelative("sideDuration");
        var initialDirProp = property.FindPropertyRelative("initialDirection");
        var initialMoveDirProp = property.FindPropertyRelative("initialMoveDirection");
        var initialMoveDurationProp = property.FindPropertyRelative("initialMoveDuration");
        var waitDurationProp = property.FindPropertyRelative("waitDuration");
        var circleRadiusProp = property.FindPropertyRelative("circleRadius");
        var circleCenterOffsetProp = property.FindPropertyRelative("circleCenterOffset");

        // --- プロパティの描画 ---

        // isExpandedプロパティを使って、折りたたみ表示を可能にする
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

        if (property.isExpanded)
        {
            // インデントを1レベル下げる
            EditorGUI.indentLevel++;

            // 各プロパティを一行ずつ描画
            var currentPos = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(currentPos, prefabProp);
            currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(currentPos, timeProp);
            currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(currentPos, posProp);
            currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(currentPos, rotProp);
            currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // useMovePositionのチェックボックスを描画
            EditorGUI.PropertyField(currentPos, useMovePosProp);
            currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // useMovePositionがtrueの場合のみmovePositionを描画
            if (useMovePosProp.boolValue)
            {
                EditorGUI.PropertyField(currentPos, movePosProp);
                currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // ★追加：初期移動を使うときだけイベント設定欄を表示
                EditorGUI.PropertyField(currentPos, onInitialMoveCompleteProp);
                currentPos.y += EditorGUI.GetPropertyHeight(onInitialMoveCompleteProp) + EditorGUIUtility.standardVerticalSpacing;
            }

            EditorGUI.PropertyField(currentPos, moveTypeProp);
            currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(currentPos, speedProp);
            currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // movementTypeの値に応じて表示するフィールドを切り替える
            EnemyMovementType moveType = (EnemyMovementType)moveTypeProp.enumValueIndex;

            if (moveType == EnemyMovementType.Zigzag)
            {
                EditorGUI.PropertyField(currentPos, zigzagAmpProp);
                currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(currentPos, zigzagFreqProp);
            }
            else if (moveType == EnemyMovementType.LastBoss1)
            {
                EditorGUI.PropertyField(currentPos, rectWidthProp);
                currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(currentPos, rectHeightProp);
                currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(currentPos, sideDurationProp);
                currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(currentPos, initialDirProp);
            }
            else if (moveType == EnemyMovementType.InitialMoveThenHoming)
            {
                EditorGUI.PropertyField(currentPos, initialMoveDirProp);
                currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(currentPos, initialMoveDurationProp);
                currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(currentPos, waitDurationProp);
            }
            else if (moveType == EnemyMovementType.Circle)
            {
                EditorGUI.PropertyField(currentPos, circleRadiusProp);
                currentPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(currentPos, circleCenterOffsetProp);
            }
            // Noneの場合は何も追加項目を表示しない

            // インデントを元に戻す
            EditorGUI.indentLevel--;
        }

        // このプロパティの描画を終了
        EditorGUI.EndProperty();
    }

    // プロパティの高さに応じてGUI全体の高さを計算するメソッド (非常に重要！)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 折りたたまれている場合は1行分の高さ
        if (!property.isExpanded)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        // 基本のプロパティ数 (Foldout + prefab + time + pos + rot + useMovePos + moveType + speed)
        float totalHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 8;

        // useMovePositionがtrueなら高さを追加
        var useMovePosProp = property.FindPropertyRelative("useMovePosition");
        if (useMovePosProp != null && useMovePosProp.boolValue)
        {
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // ★追加：イベントフィールドの高さを加算
            var onInitialMoveCompleteProp = property.FindPropertyRelative("onInitialMoveComplete");
            if (onInitialMoveCompleteProp != null)
            {
                totalHeight += EditorGUI.GetPropertyHeight(onInitialMoveCompleteProp) + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        // movementTypeに応じて高さを追加
        var moveTypeProp = property.FindPropertyRelative("movementType");
        if (moveTypeProp != null)
        {
            EnemyMovementType moveType = (EnemyMovementType)moveTypeProp.enumValueIndex;

            if (moveType == EnemyMovementType.Zigzag)
            {
                totalHeight += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
            }
            else if (moveType == EnemyMovementType.LastBoss1)
            {
                totalHeight += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4;
            }
            else if (moveType == EnemyMovementType.InitialMoveThenHoming)
            {
                totalHeight += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3;
            }
            else if (moveType == EnemyMovementType.Circle)
            {
                totalHeight += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
            }
        }


        return totalHeight;
    }
}