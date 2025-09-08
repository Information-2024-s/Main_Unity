using UnityEngine;
using System.Collections.Generic; // Listを使うために必要

// [System.Serializable] をつけることで、インスペクター上に表示・編集できるようになる
[System.Serializable]
public struct TargetTransform
{
    [Tooltip("目標の座標")]
    public Vector3 position;

    [Tooltip("目標の向き（オイラー角）")]
    public Vector3 eulerRotation;
}

public class Object_Move : MonoBehaviour
{
    [Header("移動対象")]
    [Tooltip("移動・回転させたいオブジェクト。未設定の場合は、このスクリプトがアタッチされているオブジェクト自身を対象とします。")]
    public Transform objectToMove;

    [Header("移動先リスト")]
    [Tooltip("移動させたい目標の座標と向きを複数設定します。")]
    public List<TargetTransform> targets = new List<TargetTransform>();

    [Header("移動方法")]
    [Tooltip("チェックを入れると、リストからランダムな目標を選びます。外すと、リストの順番通りに移動します。")]
    public bool randomizeOrder = false;

    [Tooltip("（ランダム選択時のみ）同じ場所を連続で選ばないようにします。")]
    [SerializeField] private bool preventRepetition = true;

    // --- 内部変数 ---
    private int currentIndex = 0; // 順番に移動する際の現在位置
    private int lastRandomIndex = -1; // ランダム移動時に前回選んだ場所を記憶

    void Awake()
    {
        // objectToMoveがインスペクターで設定されていなければ、自身を対象とする
        if (objectToMove == null)
        {
            objectToMove = this.transform;
        }
    }

    /// <summary>
    /// 次の目標へオブジェクトを移動・回転させる公開メソッド
    /// </summary>
    public void MoveToNextTarget()
    {
        // 移動先リストが空の場合は何もしない
        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("移動先の座標・向きが1つも設定されていません！", this.gameObject);
            return;
        }

        TargetTransform nextTarget;

        if (randomizeOrder)
        {
            // --- ランダムに目標を選ぶ ---
            if (preventRepetition && targets.Count > 1)
            {
                int randomIndex;
                // 前回と違うインデックスが選ばれるまで繰り返す
                do
                {
                    randomIndex = Random.Range(0, targets.Count);
                } while (randomIndex == lastRandomIndex);
                
                lastRandomIndex = randomIndex;
                nextTarget = targets[randomIndex];
            }
            else
            {
                // 単純にランダム選択
                int randomIndex = Random.Range(0, targets.Count);
                lastRandomIndex = randomIndex;
                nextTarget = targets[randomIndex];
            }
        }
        else
        {
            // --- リストの順番通りに目標を選ぶ ---
            nextTarget = targets[currentIndex];

            // 次のインデックスへ更新（リストの最後まで行ったら最初に戻る）
            currentIndex = (currentIndex + 1) % targets.Count;
        }

        // 実際にオブジェクトを移動・回転させる
        if (objectToMove != null)
        {
            Debug.Log($"オブジェクト '{objectToMove.name}' を 座標: {nextTarget.position}, 向き: {nextTarget.eulerRotation} へ移動・回転します。");
            objectToMove.position = nextTarget.position;
            objectToMove.rotation = Quaternion.Euler(nextTarget.eulerRotation);
        }
        else
        {
            Debug.LogError("移動対象のオブジェクトが設定されていません！", this.gameObject);
        }
    }
}