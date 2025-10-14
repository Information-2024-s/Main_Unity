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

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayerコンポーネントがありません！");
            return;
        }



    }

    public void transion_to_qr()
    {
        transion_next_scene();
    }



    IEnumerator transion_next_scene()
    {
        videoPlayer.Play();
        yield return new WaitWhile(() => videoPlayer.isPlaying);
        SceneManager.LoadScene("QR_read");
    }

}
