using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class Enemy_chuboss : MonoBehaviour
{
    [Header("基本設定")]
    public int maxHP = 10; // HP追加
    public int scoreValue = 10;
    public int scoreValue_damaged = 5; // ダメージ時のスコア
    public AudioClip explodeSound; // 爆発音
    public AudioClip damageSound; // ダメージ時の効果音を追加
    public GameObject breakEffect;
    public Vector3 breakEffectOffset = new Vector3(0, 1f, 0); // オフセット追加

    [Header("突進関連")]
    // Inspector で最小/最大を指定してその範囲から乱数で待機時間を決める
    public float waitBeforeCharge_min = 1.0f; // 突進前の最小待機秒数
    public float waitBeforeCharge_max = 3.0f; // 突進前の最大待機秒数
    private float chargeDistance = -1.0f; // 突進する距離
    private float chargeSpeed = 30.0f; // 突進速度
    private float returnSpeed = 30.0f; // 戻る速度
    public int scorePenaltyOnCharge = 50; // 突進時に減らすスコア
    public float shakeDuration = 0.3f; // カメラを揺らす時間
    public float shakeMagnitude = 0.1f; // カメラを揺らす強さ

    public event Action OnDeath; // 死亡時に呼ばれるイベント

    private Vector3 originalPosition; // 元の座標
    private bool isCharging = false;
    private bool isPreparing = false; // 突進の準備中フラグ
    private bool chargeCanceled = false; // 準備中にキャンセルされたか
    public float prepareDuration = 2.0f; // 突進開始前の予備時間（プレイヤーが攻撃してキャンセルできる）
    private int currentHP;
    private MonoBehaviour[] otherScripts; // 自分以外のスクリプト
    [Header("Cancel Effects")]
    [Tooltip("突進準備のときに再生するサウンド")]
    public AudioClip prepareSound;
    [Tooltip("突進がキャンセルされたときに再生するエフェクト（プレハブ）")]
    public GameObject cancelEffectPrefab;
    [Tooltip("突進がキャンセルされたときに再生するサウンド")]
    public AudioClip cancelSound;

    [Header("突進演出エフェクト")]
    [Tooltip("突進時に再生するエフェクト（プレハブ）")]
    public GameObject chargeEffectPrefab;
    [Tooltip("突進時に再生するサウンド")]
    public AudioClip chargeSound;
    [Tooltip("突進エフェクトの表示時間（カメラシェイクもこれに合わせる）")]
    public float chargeEffectDuration = 0.5f;

    private AudioSource loopingAudioSource;

    void Start()
    {
        // 準備サウンド用のAudioSourceを取得
        loopingAudioSource = GetComponent<AudioSource>();
        if (loopingAudioSource == null)
        {
            // もしAudioSourceが付いていなければ、新しく追加する
            loopingAudioSource = gameObject.AddComponent<AudioSource>();
        }
        // 準備サウンドはループ再生させ、勝手に再生されないように設定
        loopingAudioSource.playOnAwake = false;
        loopingAudioSource.loop = true;

        currentHP = maxHP * Mathf.Max(1, player_manager.player_count);
        originalPosition = transform.position;

        // 自分以外の有効なMonoBehaviourを取得
        otherScripts = GetComponents<MonoBehaviour>().Where(script => script != this && script.enabled).ToArray();

        // min/max の整合性チェック（min が max より大きければ入れ替える）
        if (waitBeforeCharge_min > waitBeforeCharge_max)
        {
            float tmp = waitBeforeCharge_min;
            waitBeforeCharge_min = waitBeforeCharge_max;
            waitBeforeCharge_max = tmp;
        }

        // 突進コルーチンを開始
        StartCoroutine(ChargeRoutine());
    }

    // ダメージを受ける
    public void TakeDamage(int damage, int player_num)
    {
        currentHP -= damage;

        // 準備中にダメージを受けたら突進をキャンセル
        if (isPreparing)
        {
            CancelCharge();
        }

        if (currentHP <= 0)
        {
            // ★★★ 修正箇所 ★★★
            // DestroyEnemy(player_num); // <-- 即時破棄をやめる
            StartCoroutine(DestroyEnemyRoutine(player_num)); // <-- コルーチンで遅延破棄する
        }
        else
        {
            // HPがまだ残っている場合、ダメージ効果音を鳴らす
            if (damageSound != null && Camera.main != null) // Camera.main が null でないことも確認
            {
                Debug.Log("Play damageSound!");
                AudioSource.PlayClipAtPoint(damageSound, Camera.main.transform.position, 1.0f);

                if (ScoreManager.instance != null)
                {
                    Debug.Log(player_num);
                    ScoreManager.instance.AddScore(player_num, scoreValue_damaged);
                }
            }
        }
    }

    // 突進挙動コルーチン
    private IEnumerator ChargeRoutine()
    {
        while (true)
        {
            // 待機（min〜max の範囲から乱数）
            float waitTime = UnityEngine.Random.Range(waitBeforeCharge_min, waitBeforeCharge_max);
            yield return new WaitForSeconds(waitTime);

            // 突進前の座標を保存 (突進移動がないので、意味は薄れるが残しておく)
            originalPosition = transform.position;

            // 突進の準備フェーズ: この間にダメージを受けると突進がキャンセルされる
            isPreparing = true;
            chargeCanceled = false;
            if (prepareSound != null && loopingAudioSource != null)
            {
                // PlayClipAtPoint の代わりに、コンポーネントで再生
                loopingAudioSource.clip = prepareSound;
                loopingAudioSource.Play();
            }
            float t = 0f;
            while (t < prepareDuration)
            {
                if (chargeCanceled) break;
                t += Time.deltaTime;
                yield return null;
            }

            // 準備ループが終わったので、サウンドを停止
            // (キャンセルされても、成功しても、ここで止まる)
            if (loopingAudioSource != null)
            {
                loopingAudioSource.Stop();
            }

            // 準備フラグを下げる
            isPreparing = false;
            if (chargeCanceled)
            {
                // キャンセルされた場合、必要ならエフェクトやリアクションを入れてから次のループへ
                Debug.Log("Charge cancelled by damage.");
                yield return new WaitForSeconds(0.1f);
                continue; // 次の待機へ戻る
            }

            // 準備が完了したので、突進に入る前に他スクリプトを無効化する
            foreach (var script in otherScripts)
            {
                script.enabled = false;
            }

            // スコアを減らす (これは突進開始時に起こるペナルティなので残す)
            if (ScoreManager.instance != null)
            {
                for (int i = 0; i < ScoreManager.scores.Length; i++)
                {
                    ScoreManager.instance.DecreaseScore(i, scorePenaltyOnCharge);
                }
            }

            // ★★★ ここから突進演出エフェクトに変更する部分 ★★★
            Debug.Log(">> Charge Effect Started!");

            // 突進エフェクトの再生
            if (chargeEffectPrefab != null)
            {
                // 現在位置でエフェクトを生成
                GameObject effect = Instantiate(chargeEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, chargeEffectDuration); // 指定時間後に自動破棄
            }

            // 突進サウンドの再生
            if (chargeSound != null && Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(chargeSound, Camera.main.transform.position, 1.0f);
            }

            // カメラを揺らす（エフェクトの終了に合わせて調整）
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(chargeEffectDuration, shakeMagnitude);
            }

            // エフェクトの再生時間だけ待機
            yield return new WaitForSeconds(chargeEffectDuration);

            Debug.Log(">> Charge Effect Ended.");
            // ★★★ 突進演出エフェクトに変更する部分ここまで ★★★

            // 元の座標へ戻る (エフェクトだけで移動していないので、単に位置をリセットするだけ)
            // transform.position = originalPosition; // 必要であれば、元の場所に戻す処理。エフェクトだけなら不要かも
            isCharging = false; // 突進状態を終了

            // 他のスクリプトを再度有効化
            foreach (var script in otherScripts)
            {
                script.enabled = true;
            }
        }

    }

    private IEnumerator DestroyEnemyRoutine(int player_num)
    {
        if (!Application.isPlaying) 
        {
            Destroy(gameObject); // エディタ実行停止時などは即時破棄
            yield break;
        }

        OnDeath?.Invoke(); // 死亡イベントを呼び出す

        float waitTime = 0f; // 待機時間

        if (explodeSound != null)
        {
            Debug.Log("Play explodeSound!");
            AudioSource.PlayClipAtPoint(explodeSound, Camera.main.transform.position, 1.0f);
            waitTime = Mathf.Max(waitTime, explodeSound.length); // サウンドの長さを待機時間にする
        }

        if (breakEffect != null)
        {
            GameObject effect = Instantiate(breakEffect, transform.position + breakEffectOffset, Quaternion.identity);
            float effectDuration = 1.0f; // エフェクトの持続時間 (Destroyタイマーと合わせる)
            Destroy(effect, effectDuration);
            waitTime = Mathf.Max(waitTime, effectDuration); // エフェクトの持続時間も考慮
        }

        if (ScoreManager.instance != null)
        {
            Debug.Log(player_num);
            ScoreManager.instance.AddScore(player_num, scoreValue);
        }

        // ★★★重要★★★
        // 他のスクリプトを無効化し、当たり判定なども消す
        foreach (var script in GetComponents<MonoBehaviour>())
        {
            script.enabled = false;
        }
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        // メッシュを非表示にする（任意）
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        // エフェクトやサウンドが終わるまで待機
        yield return new WaitForSeconds(waitTime);

        // 待機後にオブジェクトを破棄
        Destroy(gameObject);
    }

    // 突進をキャンセルする（準備中にダメージを受けたときなど）
    private void CancelCharge()
    {
        if (!isPreparing) return;
        chargeCanceled = true;
        isPreparing = false;

        // キャンセルが呼ばれた瞬間に準備サウンドを停止
        if (loopingAudioSource != null)
        {
            loopingAudioSource.Stop();
        }

        // キャンセル時のエフェクトを再生
        if (cancelEffectPrefab != null)
        {
            GameObject effect = Instantiate(cancelEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1.0f); // 1秒後に自動破棄
        }
        // キャンセル時の音を再生
        if (cancelSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(cancelSound, Camera.main.transform.position, 1.0f);
        }
        Debug.Log("CancelCharge called: charge will be cancelled.");
    }
}