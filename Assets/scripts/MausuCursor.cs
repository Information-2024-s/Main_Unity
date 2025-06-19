using UnityEngine;

public class MausuCursor : MonoBehaviour
{
    public Texture2D cursorTexture;

    void Start()
    {
        Vector2 offset = new Vector2(500f, 500f);  // X,Yに500ピクセルずらす
        Cursor.SetCursor(cursorTexture, offset, CursorMode.Auto);
    }
}
