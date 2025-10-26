using UnityEngine;
using UnityEngine.InputSystem;

// [RequireComponent(typeof(CharacterController))] // これがあると自動でCCが追加される
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.8f;
    
    // ★★★ インスペクターからアタッチ必須 ★★★
    public CharacterController controller; 

    private Vector3 velocity;
    private bool isGrounded;
    private Vector2 moveInput;

    // Start() で controller がセットされているか確認
    void Start()
    {
        if (controller == null)
        {
            Debug.LogError("!!! PlayerMovement: 'Controller' がアタッチされていません !!!", this);
        }
    }

    // ★★★ PlayerInput (Send Messages) から呼ばれる ★★★
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        
        // ★ デバッグログ：OnMoveが呼ばれたか、入力値は何か
        Debug.Log("OnMove() が呼ばれました！ 入力値: " + moveInput);
    }

    void Update()
    {
        // controller が無い場合は、エラーを防ぐために処理を中断
        if (controller == null)
        {
            return; 
        }

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // OnMoveで受け取った値を使う
        float h = moveInput.x;
        float v = moveInput.y;

        Vector3 moveDirection = (transform.right * h + transform.forward * v) * moveSpeed;

        velocity.y += gravity * Time.deltaTime;

        controller.Move((moveDirection + velocity) * Time.deltaTime);
    }
}