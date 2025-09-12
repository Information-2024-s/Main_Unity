using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// プレハブと座標をペアで管理するためのクラス
/// [System.Serializable] を付けることで、インスペクターに表示されるようになります。
/// </summary>
[System.Serializable]
public class SpawnData
{
    [Tooltip("この座標に生成するプレハブ")]
    public GameObject prefabToSpawn;
    [Tooltip("プレハブを生成するワールド座標")]
    public Vector3 spawnPosition;
}

/// <summary>
/// 指定したリストに従ってプレハブを生成し、それらがすべて破壊されたら自身も破壊するコンポーネント
/// </summary>
public class PrefabSpawner : MonoBehaviour
{
    [Header("生成するプレハブと座標のリスト")]
    [Tooltip("このリストに従って、指定されたプレハブが指定された座標に生成されます。")]
    public List<SpawnData> spawnList;

    // 生成したプレハブを追跡するためのリスト
    private List<GameObject> spawnedPrefabs = new List<GameObject>();

    void Start()
    {
        // リストが設定されているか確認
        if (spawnList == null || spawnList.Count == 0)
        {
            Debug.LogWarning("生成するプレハブのリストが設定されていません。スポナーを削除します。", this);
            Destroy(gameObject);
            return;
        }

        SpawnAllPrefabs();
    }

    /// <summary>
    /// リストに従ってすべてのプレハブを生成します。
    /// </summary>
    private void SpawnAllPrefabs()
    {
        Debug.Log($"{spawnList.Count}個のプレハブを生成します。", this);
        foreach (SpawnData data in spawnList)
        {
            // リスト内のプレハブが未設定の場合は警告を出してスキップ
            if (data.prefabToSpawn == null)
            {
                Debug.LogWarning("リスト内の一部のプレハブが設定されていません。この項目はスキップします。", this);
                continue;
            }

            Debug.Log($"プレハブ '{data.prefabToSpawn.name}' を座標 {data.spawnPosition} に生成しようとしています。", this);
            
            // 指定されたプレハブを、指定された座標に生成
            GameObject spawnedObject = Instantiate(data.prefabToSpawn, data.spawnPosition, Quaternion.identity);
            spawnedPrefabs.Add(spawnedObject);
        }
    }

    void Update()
    {
        if (spawnedPrefabs.Count > 0)
        {
            spawnedPrefabs.RemoveAll(item => item == null);

            if (spawnedPrefabs.Count == 0)
            {
                Debug.Log("生成した全てのプレハブが破壊されました。スポナーを削除します。", this);
                Destroy(gameObject);
            }
        }
    }
}