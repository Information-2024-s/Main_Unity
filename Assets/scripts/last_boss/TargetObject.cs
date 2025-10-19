using UnityEngine;

public class TargetObject : MonoBehaviour
{
    // trueの間は、通常のHPダメージを受けない
    public bool isInvulnerable = false;

    // 弾が当たった時に呼び出されることを想定
    public void TakeDamage(int damage, int player_num)
    {
        // Lastboss_Attackの特殊攻撃中かどうかをチェック
        Lastboss_Attack bossAttack = FindObjectOfType<Lastboss_Attack>();
        if (bossAttack != null && bossAttack.IsSpecialAttackTarget(this))
        {
            // 特殊攻撃の対象なので、ヒットしたことを通知する
            bossAttack.OnTargetHitByBullet(this, player_num);
            return; // 通常のダメージ処理は行わない
        }

        // 無敵状態なら何もしない
        if (isInvulnerable)
        {
            Debug.Log(gameObject.name + " は現在無敵です！ダメージは受けません。");
            return;
        }

        // ★★★ ここから修正 ★★★
        // 通常のダメージ処理
        // 同じオブジェクトにアタッチされている BossEnemy コンポーネントを取得
        BossEnemy enemyPart = GetComponent<BossEnemy>();
        if (enemyPart != null)
        {
            // BossEnemy の ApplyDamage を呼び出して、ダメージ処理を親に伝達する
            enemyPart.ApplyDamage(damage, player_num);
        }
        else
        {
            // もしBossEnemyが見つからなかった場合（念のため）
            Debug.LogError("BossEnemy コンポーネントが見つかりません！", gameObject);
        }
    }
}