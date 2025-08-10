using UnityEngine;

public class test_script : MonoBehaviour
{
    private face face;  // これを追加！

    void Start()
    {
        face = GetComponent<face>();
        int[] players = { 32411 };
        face.get_textures(players);
        face.paste_image(players);
    }

    void Update()
    {
    }
}
