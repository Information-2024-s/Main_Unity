using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int scoreValue = 10;

    private void OnDestroy()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);
        }
    }
}