using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class face : MonoBehaviour
{
    private Texture2D[] textures;
    public GameObject[] enemy_faces;
    void Start()
    {

    }
    public void get_textures(int[] players_id)
    {
        for (int i = 0; i < players_id.Length; i++)
        {
            StartCoroutine(GetTexture("http://127.0.0.1:5000/get/" + players_id[i],i));
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
    private IEnumerator GetTexture(string url,int i){
        using UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);

        Debug.Log("[WebRequester] Send to "+url.ToString());
        yield return req.SendWebRequest();
        Debug.Log("[WebRequester] Recv");

        if (req.result == UnityWebRequest.Result.Success)
        {
            textures[i] = DownloadHandlerTexture.GetContent(req);

        }
        else
        {
            Debug.LogError($"[WebRequester:ERROR] {(!string.IsNullOrEmpty(req.downloadHandler.error) ? req.downloadHandler.error : req.error)}");
        }
    }
}

