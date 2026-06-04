using UnityEngine;

/// <summary>
/// 玩家控制器 - 处理玩家移动、跳跃、交互
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("视角设置")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("地面检测")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    // 组件引用
    private CharacterController controller;
    private Camera playerCamera;
    private PlayerStats playerStats;

    // 状态变量
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;
    private float xRotation = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        playerStats = GetComponent<PlayerStats>();

        // 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleGravity();
        HandleMouseLook();
        HandleInteraction();
    }

    /// <summary>
    /// 地面检测
    /// </summary>
    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    /// <summary>
    /// 处理移动
    /// </summary>
    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        move = move.normalized;

        // 检查是否跑步
        isRunning = Input.GetKey(KeyCode.LeftShift) && playerStats.CurrentStamina > 0;
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        // 跑步消耗体力
        if (isRunning && move.magnitude > 0)
        {
            playerStats.ConsumeStamina(10f * Time.deltaTime);
        }

        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 处理跳跃
    /// </summary>
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            playerStats.ConsumeStamina(15f);
        }
    }

    /// <summary>
    /// 处理重力
    /// </summary>
    private void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// 处理鼠标视角
    /// </summary>
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    /// <summary>
    /// 处理交互
    /// </summary>
    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 射线检测交互物体
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3f))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                }
            }
        }
    }

    /// <summary>
    /// 获取是否在地面
    /// </summary>
    public bool IsGrounded => isGrounded;

    /// <summary>
    /// 获取是否在跑步
    /// </summary>
    public bool IsRunning => isRunning;
}

/// <summary>
/// 可交互接口
/// </summary>
public interface IInteractable
{
    void Interact(PlayerController player);
}