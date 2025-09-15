using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// シリアライズ可能なクラスとして定義し、インスペクターに表示できるようにする
[System.Serializable]
public class SpawnEvent
{
    [Tooltip("この時間（秒）から生成を開始します")]
    public float startTime;
    [Tooltip("この時間（秒）の間、生成を継続します")]
    public float duration = 5.0f;
}

public class TimedAreaSpawner : MonoBehaviour
{
    [Header("生成するプレハブ")]
    [Tooltip("ここに生成したいオブジェクトのプレハブを設定します")]
    public GameObject prefabToSpawn;

    [Header("生成時間の設定")]
    [Tooltip("プレハブを生成する時間帯のリスト")]
    public List<SpawnEvent> spawnEvents = new List<SpawnEvent>();

    [Header("生成間隔")]
    [Tooltip("次のプレハブが生成されるまでの時間")]
    public float spawnInterval = 0.5f;

    [Header("生成範囲")]
    [Tooltip("この範囲の中心座標")]
    public Vector3 spawnAreaCenter;
    [Tooltip("XYZそれぞれの方向への広がり")]
    public Vector3 spawnAreaSize = new Vector3(10, 0, 10);

    private Coroutine spawnCoroutine;
    private float gameTimer = 0f;

    void Start()
    {
        // プレハブが設定されているか確認
        if (prefabToSpawn == null)
        {
            Debug.LogError("生成するプレハブが設定されていません！", this);
            this.enabled = false; // スクリプトを無効化
            return;
        }

        // 開始時間順にリストをソートしておく（任意）
        spawnEvents.Sort((a, b) => a.startTime.CompareTo(b.startTime));
    }

    void Update()
    {
        // ゲーム内の時間を計測
        gameTimer += Time.deltaTime;

        // 現在の時間が次のイベント開始時間と一致するかチェック
        foreach (var spawnEvent in spawnEvents)
        {
            // 開始時間を過ぎており、まだ終了していない時間帯か
            if (gameTimer >= spawnEvent.startTime && gameTimer < (spawnEvent.startTime + spawnEvent.duration))
            {
                // もし生成コルーチンが動いていなければ、新しく開始する
                if (spawnCoroutine == null)
                {
                    spawnCoroutine = StartCoroutine(SpawnRandomlyInArea(spawnEvent.duration));
                }
                // 一致するイベントを見つけたらループを抜ける
                return;
            }
        }
    }

    /// <summary>
    /// 指定された時間、範囲内にプレハブを生成し続けるコルーチン
    /// </summary>
    private IEnumerator SpawnRandomlyInArea(float duration)
    {
        float elapsedTime = 0f;
        
        // 指定された継続時間の間、ループし続ける
        while (elapsedTime < duration)
        {
            // --- ランダムな座標を計算 ---
            // 中心座標から、指定されたサイズの半分の範囲でランダムな値を取得
            float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
            float randomZ = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);

            Vector3 randomPosition = spawnAreaCenter + new Vector3(randomX, randomY, randomZ);

            // --- プレハブを生成 ---
            // 親オブジェクト（このスクリプトがアタッチされたオブジェクト）のワールド座標を基準に生成
            Instantiate(prefabToSpawn, transform.position + randomPosition, Quaternion.identity);

            // 次の生成まで待機
            yield return new WaitForSeconds(spawnInterval);
            elapsedTime += spawnInterval;
        }

        // コルーチンが終了したら参照をnullに戻す
        spawnCoroutine = null;
    }

    /// <summary>
    /// Sceneビューに生成範囲を視覚的に表示するためのメソッド
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f); // 半透明の緑色
        // このオブジェクトの位置を基準に、生成範囲のボックスを描画
        Gizmos.DrawCube(transform.position + spawnAreaCenter, spawnAreaSize);
    }
}