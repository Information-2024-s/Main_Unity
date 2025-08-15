using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class face : MonoBehaviour
{
    
    public GameObject[] enemy_faces;
    public int loaded_count;
    private Texture2D[] textures;
    private player_manager player_manager;
    void Start()
    {
        player_manager = UnityEngine.Object.FindFirstObjectByType<player_manager>();
        set_textures(player_manager.players_id,player_manager.player_count);
    }
    public void set_textures(int[] players_id,int player_count)
    {
        textures = new Texture2D[player_count];
        loaded_count = 0;
        for (int i = 0; i < player_count; i++)
        {
            StartCoroutine(SetTexture_coroutine(players_id,player_count,config_loader.config.texture_URL + players_id[i], i));
        }
    }
    private IEnumerator SetTexture_coroutine(int[] players_id,int player_count,string url, int i)
    {
        using UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);

        Debug.Log("[WebRequester] Send to " + url.ToString());
        yield return req.SendWebRequest();
        Debug.Log("[WebRequester] Recv");

        if (req.result == UnityWebRequest.Result.Success)
        {
            textures[i] = DownloadHandlerTexture.GetContent(req);
            loaded_count++;
            if (loaded_count == player_count)
                paste_image(player_count);
        }
        else
        {
            Debug.LogError($"[WebRequester:ERROR] {(!string.IsNullOrEmpty(req.downloadHandler.error) ? req.downloadHandler.error : req.error)}");
        }
    }
    public void paste_image(int player_count)
    {
        for (int i = 0; i < enemy_faces.Length; i++) {
            if (enemy_faces[i] != null && textures[i % player_count] != null) {
                Renderer renderer = enemy_faces[i].GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.mainTexture = textures[i % player_count];
                }

            }
        }
    }
}

