using UnityEngine;
using System;
using TMPro; 
using System.Linq;
public class player_manager : MonoBehaviour
{
    public static int[] players_id = new int[4];
    public int player_count = 0;
    public TextMeshProUGUI [] player_state = new TextMeshProUGUI [4];
    void Start()
    {

    }

    void Update()
    {

    }

    public void add_player(int player_id)
    {

        player_count++;
        players_id[player_count - 1] = player_id;
        Array.Sort(players_id, 0, player_count);//先頭から今はデータが入ってるとこまでで並び替え
        for (int i = 0; i < players_id.Count(x => x != 0); i++)
        {
            player_state[i].text = players_id[i].ToString();
        }
    }
}
