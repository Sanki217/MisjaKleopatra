using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    
    public float speed = 8f;
    [Range(0, 1)] public float acceleration = 0.1f;

    public float gravity = 25f;
    public float jumpForce = 10f;
    public bool enableDoubleJump = true;
    public float secondJumpForce = 10f;
    [Range(0,1)] public float wallSlideSlowDown = 0.5f;
    public float wallJumpXForce = 10f;
    public float wallJumpYForce = 10f;
    public float wallJumpTime = 2f;

    private bool ableToDoubleJump = true;
    private Vector3 direction;
    private float currentAccelaration = 0f;
    private bool isGrounded;
    private bool isSliding;
    private bool wallJump = false;
    private bool wallJumpDirection;

    void Start()
    {
        if (!enableDoubleJump)
            ableToDoubleJump = false;
    }

    void Update()
    {
        if (wallJump)
        {
            direction.y = wallJumpYForce;
            direction.x = wallJumpDirection ? wallJumpXForce : -wallJumpXForce;
        }
        else
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            if (horizontalInput != 0)
            {
                currentAccelaration += acceleration * Time.deltaTime;
                if (currentAccelaration > 1)
                    currentAccelaration = 1;
            }
            else
            {
                currentAccelaration = 0;
            }
            direction.x = horizontalInput * speed * currentAccelaration;
        }        

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
        bool isFacingWall = Physics.CheckSphere(wallCheck.position, 1.3f, wallLayer);
        isSliding = isFacingWall && direction.y < 0 && !wallJump; 

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
                direction.y = jumpForce;
            else if (!ableToDoubleJump && enableDoubleJump)
                ableToDoubleJump = true;
        }
        else
        {
            // apply gravity
            direction.y -= gravity * Time.deltaTime;

            if (isSliding)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    wallJump = true;
                    wallJumpDirection = direction.x < 0;
                    Invoke("endWallJump", wallJumpTime);
                }
                else
                    direction.y += gravity * Time.deltaTime * wallSlideSlowDown;
            }

            else if (Input.GetButtonDown("Jump") && !isFacingWall && ableToDoubleJump)
            {
                direction.y = secondJumpForce;
                ableToDoubleJump = false;
            }
        }



        controller.Move(direction * Time.deltaTime);
        Debug.Log(direction.x);
    }

    void endWallJump()
    {
        wallJump = false;
    }

}
