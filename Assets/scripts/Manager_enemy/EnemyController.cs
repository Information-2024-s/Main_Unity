using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // --- 変数宣言 ---
    // これらがクラスのどこからでも使える変数です。
    // 名前と大文字・小文字が完全に一致している必要があります。
    public EnemyMovementType movementType;
    public float moveSpeed;
    
    // ジグザグ移動用の設定
    public float zigzagAmplitude = 2f;
    public float zigzagFrequency = 5f;

    // この変数は内部でのみ使うのでprivateのまま
    private Transform playerTransform;

    /// <summary>
    /// スポナーから呼び出される初期化メソッド
    /// </summary>
    public void Setup(EnemySpawnInfo spawnInfo)
    {
        // ここでクラス変数に値を設定しています
        movementType = spawnInfo.movementType;
        moveSpeed = spawnInfo.moveSpeed;
        zigzagAmplitude = spawnInfo.zigzagAmplitude;
        zigzagFrequency = spawnInfo.zigzagFrequency;

        if (movementType == EnemyMovementType.Homing)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("追尾対象のプレイヤーが見つかりません。Playerタグを確認してください。");
                movementType = EnemyMovementType.Straight; // 見つからない場合は直進に変更
            }
        }
    }

    /// <summary>
    /// 毎フレーム呼ばれる更新処理
    /// </summary>
    void Update()
    {
        // エラーが発生した箇所。
        // 上で宣言した `movementType` を使います。
        switch (movementType)
        {
            case EnemyMovementType.Straight:
                MoveStraight();
                break;
            case EnemyMovementType.Zigzag:
                MoveZigzag();
                break;
            case EnemyMovementType.Homing:
                MoveHoming();
                break;
        }
    }

    private void MoveStraight()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void MoveZigzag()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        float zigzagAmount = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
        transform.Translate(Vector3.right * zigzagAmount * Time.deltaTime);
    }

    private void MoveHoming()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}