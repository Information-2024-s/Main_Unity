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
        var rotProp = property.FindPropertyRelative("spawnRotationEuler");
        var moveTypeProp = property.FindPropertyRelative("movementType");
        var speedProp = property.FindPropertyRelative("moveSpeed");
        var zigzagAmpProp = property.FindPropertyRelative("zigzagAmplitude");
        var zigzagFreqProp = property.FindPropertyRelative("zigzagFrequency");

        // リストの要素を折りたたみで表示
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

        if (property.isExpanded)
        {
            // インデントを一段深くする
            EditorGUI.indentLevel++;

            // 各プロパティを描画
            float y = position.y + EditorGUIUtility.singleLineHeight;
            Rect currentRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(currentRect, prefabProp);
            currentRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(currentRect, timeProp, new GUIContent("Time Since Previous"));
            currentRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(currentRect, posProp, new GUIContent("Position"));
            currentRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(currentRect, rotProp, new GUIContent("Rotation (Euler)"));
            currentRect.y += EditorGUIUtility.singleLineHeight * 1.5f; // 少しスペースを空ける

            EditorGUI.PropertyField(currentRect, moveTypeProp);
            currentRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(currentRect, speedProp);
            currentRect.y += EditorGUIUtility.singleLineHeight;
            
            // movementType の値に応じて表示を切り替え
            if (moveTypeProp.enumValueIndex == (int)EnemyMovementType.Zigzag)
            {
                EditorGUI.PropertyField(currentRect, zigzagAmpProp, new GUIContent("Zigzag Amplitude"));
                currentRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(currentRect, zigzagFreqProp, new GUIContent("Zigzag Frequency"));
            }

            // インデントを元に戻す
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    // プロパティの高さに応じてGUI全体の高さを計算するメソッド (非常に重要！)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded)
        {
            totalHeight += EditorGUIUtility.singleLineHeight * 7.5f; // 基本項目の高さ

            var moveTypeProp = property.FindPropertyRelative("movementType");
            if (moveTypeProp.enumValueIndex == (int)EnemyMovementType.Zigzag)
            {
                totalHeight += EditorGUIUtility.singleLineHeight * 2; // Zigzag選択時の追加項目の高さ
            }
        }
        
        return totalHeight;
    }
}