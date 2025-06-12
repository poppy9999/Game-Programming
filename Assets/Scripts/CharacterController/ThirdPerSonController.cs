using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ThirdPerSonController : NetworkBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpForce = 6f;
    public float gravity = 20f;

    public Transform cameraTransform; // ✅ 可在 Inspector 拖入摄像机

    private CharacterController controller;
    private Animator animator;

    private Vector3 moveDirection = Vector3.zero;
    private bool isRunning = false;
    private bool isJumping = false;
    
    float startWalkSpeed;
    float startRunSpeed;

    private void Awake()
    {
        startWalkSpeed = walkSpeed;
        startRunSpeed = runSpeed;
    }


    public void Accelerate()
    {
        walkSpeed *= 2;
        runSpeed *= 2;
    }
    public void Decelerate()
    {
        walkSpeed = startWalkSpeed;
        runSpeed = startRunSpeed;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (isServer || isLocalPlayer)
        {
            // 只有服务端和本地玩家启用 CharacterController
            controller.enabled = true;
        }
        else
        {
            // 其他客户端禁用，避免冲突
            controller.enabled = false;
        }

        // 如果未手动指定摄像机，默认使用 Camera.main
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (isLocalPlayer)
        {
            cameraTransform.GetComponent<Camera>().depth = 10;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (cameraTransform == null)
        {
            Debug.LogWarning("cameraTransform 未设置！");
            return;
        }

        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement()
    {
        if (controller.isGrounded)
        {
            if (isJumping)
            {
                isJumping = false;
                animator.ResetTrigger("Jump");
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 inputDir = new Vector3(h, 0f, v).normalized;

            if (inputDir.magnitude > 0.1f)
            {
                isRunning = Input.GetKey(KeyCode.LeftShift);
                float speed = isRunning ? runSpeed : walkSpeed;

                // ✅ 按照摄像机的方向计算移动角度
                float angle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

                moveDirection = moveDir.normalized * speed;
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            else
            {
                moveDirection = Vector3.zero;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpForce;
                animator.SetTrigger("Jump");
                isJumping = true;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void HandleAnimation()
    {
        bool isMoving = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude > 0.1f;
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRunning", isRunning);
    }
}
