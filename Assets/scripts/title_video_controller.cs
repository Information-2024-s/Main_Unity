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
    public GameObject logo_object;
    private Image logo_image; 


    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayerコンポーネントがありません！");
            return;
        }
        loop_video.SetActive(true);
        logo_image = logo_object.GetComponent<Image>();


    }

    public void transion_to_qr()
    {
        StartCoroutine(transion_next_scene());
    }



    IEnumerator transion_next_scene()
    {

        videoPlayer.Play();
        StartCoroutine(FadeImage(logo_image,1));
        yield return new WaitUntil(() => videoPlayer.isPlaying);
        loop_video.SetActive(false);
        yield return new WaitWhile(() => videoPlayer.isPlaying);
        SceneManager.LoadScene("QR_read");
    }
        private IEnumerator FadeImage(Image img, float duration)
    {
        Color color = img.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, time / duration);
            img.color = color;
            yield return null;
        }

        color.a = 0f;
        img.color = color;
        img.gameObject.SetActive(false); // 完全に非表示にする場合
    }

}
