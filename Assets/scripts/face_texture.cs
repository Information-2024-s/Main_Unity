using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class face : MonoBehaviour
{
    private Texture2D[] textures;
    public GameObject[] enemy_faces;
    public int loaded_count;
    void Start()
    {

    }
    public void set_textures(int[] players_id)
    {
        textures = new Texture2D[players_id.Length];
        loaded_count = 0;
        for (int i = 0; i < players_id.Length; i++)
        {
            StartCoroutine(SetTexture_coroutine(players_id,"http://127.0.0.1:5000/get/" + players_id[i], i));
        }
    }
    private IEnumerator SetTexture_coroutine(int[] players_id,string url, int i)
    {
        using UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);

        Debug.Log("[WebRequester] Send to " + url.ToString());
        yield return req.SendWebRequest();
        Debug.Log("[WebRequester] Recv");

        if (req.result == UnityWebRequest.Result.Success)
        {
            textures[i] = DownloadHandlerTexture.GetContent(req);
            loaded_count++;
            if (loaded_count == players_id.Length)
                paste_image(players_id);
        }
        else
        {
            Debug.LogError($"[WebRequester:ERROR] {(!string.IsNullOrEmpty(req.downloadHandler.error) ? req.downloadHandler.error : req.error)}");
        }
    }
    public void paste_image(int[] players_id)
    {
        for (int i = 0; i < enemy_faces.Length; i++) {
            if (enemy_faces[i] != null && textures[i % players_id.Length] != null) {
                Renderer renderer = enemy_faces[i].GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.mainTexture = textures[i % players_id.Length];
                }

            }
        }
    }
}

