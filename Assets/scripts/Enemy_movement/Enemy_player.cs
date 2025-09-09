using UnityEngine;

/// <summary>
/// プレイヤーに向かって移動するスクリプト
/// </summary>
public class MoveTowardsPlayer : MonoBehaviour
{
    [Header("移動スピード")]
    [Tooltip("オブジェクトがプレイヤーに向かう速さを設定します")]
    [Range(0.1f, 20f)] // インスペクターで0.1から20の範囲でスライダー調整できるようにする
    public float speed = 5f;

    // プレイヤーのTransformをキャッシュするための変数
    private Transform playerTransform;

    // ゲームが開始された時に一度だけ呼ばれる
    void Start()
    {
        // "Player" タグがついたオブジェクトをシーンから探す
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        // プレイヤーが見つかった場合
        if (playerObject != null)
        {
            // プレイヤーのTransformコンポーネントを保存しておく（効率化のため）
            playerTransform = playerObject.transform;
            Debug.Log("プレイヤーを発見しました。追跡を開始します。");
        }
        else
        {
            // プレイヤーが見つからなかった場合は、エラーメッセージをコンソールに表示する
            Debug.LogError("エラー: 'Player' タグを持つオブジェクトが見つかりません。");
            // このスクリプトを無効化して、Updateが呼ばれないようにする
            this.enabled = false;
        }
    }

    // 毎フレーム呼ばれる
    void Update()
    {
        // プレイヤーの方向を計算する
        // (プレイヤーの位置ベクトル - 自分の位置ベクトル) で、自分からプレイヤーへ向かうベクトルが求まる
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        // 計算した方向に、speed と 時間 を考慮して移動させる
        // Time.deltaTimeを掛けることで、フレームレートに依存しない滑らかな移動になる
        transform.position += direction * speed * Time.deltaTime;

        // オプション: プレイヤーの方向を向くようにする
        // transform.LookAt(playerTransform);
    }
}