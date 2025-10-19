using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using UnityEngine.EventSystems;  
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class TitleVideoController : MonoBehaviour
{

    private VideoPlayer videoPlayer;
    public GameObject loop_video;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayerコンポーネントがありません！");
            return;
        }
        loop_video.SetActive(true);



    }

    public void transion_to_qr()
    {
        StartCoroutine(transion_next_scene());
    }



    IEnumerator transion_next_scene()
    {
        
        videoPlayer.Play();
        yield return new WaitUntil(() => videoPlayer.isPlaying);
        loop_video.SetActive(false);
        yield return new WaitWhile(() => videoPlayer.isPlaying);
        SceneManager.LoadScene("QR_read");
    }

}
