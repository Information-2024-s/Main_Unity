using UnityEngine;

public class player_manager : MonoBehaviour
{
    private face face;
    public int[] players_id = { 32411 };
    void Start()
    {
        face = GetComponent<face>();
        face.set_textures(players_id);
    }

    void Update()
    {
    }
}
