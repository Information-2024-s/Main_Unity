using UnityEngine;
using UnityEngine.InputSystem; // 1. InputSystemの名前空間を追加

public class PlayerPOV : MonoBehaviour
{
    // パラメータ
    // ★★★ インスペクターで「首」や「カメラ」のTransformを指定 ★★★
    public Transform neck;
    public float sensitivity = 2.0f;
    public float minVertical = -90.0f;
    public float maxVertical = 90.0f;

    // 演算用変数
    private float rotationX = 0f;
    private Vector2 lookInput; // 2. OnLookからマウス入力を受け取る変数

    // ゲーム開始時に呼ばれる
    void Start()
    {
        // カーソルを非表示＆ロック
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 3. neck がアタッチされているかデバッグ確認
        if (neck == null)
        {
            Debug.LogError("!!! PlayerPOV: 'Neck' がアタッチされていません !!!", this);
        }
    }

    // 4. PlayerInput (Send Messages) から "Look" アクションを
    //    受け取るための関数
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    // 毎フレーム実行される
    void Update()
    {
        // 5. neck が無い場合はエラーを防ぐ
        if (neck == null)
        {
            return;
        }

        // 6. OnLookで受け取った値を使う (Input.GetAxisは削除)
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        // Player（体）の回転（左右）
        transform.Rotate(0, mouseX, 0); // このスクリプトがアタッチされたオブジェクト(体)を回す

        // Neck（首）の回転（上下）
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minVertical, maxVertical);
        neck.localRotation = Quaternion.Euler(rotationX, 0, 0); // 首(カメラ)を回す
    }
}