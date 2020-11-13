using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public AudioSource AS;//Steps Sound
    public AudioSource JumpSound;//Jump Sound
    public CharacterController controller;
    public Animator animator;
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
    private bool isMoving = false;
    private bool facingRight = true;
    private bool turning = false;
    private bool jumping = false;

    void Start()
    {
        if (!enableDoubleJump)
            ableToDoubleJump = false;
    }

    void Update()
    {
        if (!isGrounded) AS.gameObject.SetActive(false);//Object sound off
        else  AS.gameObject.SetActive(true); //Object sound on

        float horizontalInput = Input.GetAxis("Horizontal");
        facingRight = transform.localScale.z > 0;

        if (turning)
        {
            direction.x = 0;
        }
        else if (facingRight != (horizontalInput > 0) && horizontalInput != 0)
        {
            turning = true;
            animator.SetTrigger("Turn");
        }

        else if (wallJump)
        {
            direction.y = wallJumpYForce;
            direction.x = wallJumpDirection ? wallJumpXForce : -wallJumpXForce;
        }
        else
        {

            if (horizontalInput != 0)
            {
                if (!AS.isPlaying) AS.Play();//Steps Sound
                currentAccelaration += acceleration * Time.deltaTime;
                if (currentAccelaration > 1)
                    currentAccelaration = 1;
            }
            else
            {
                AS.Stop();//Steps Sound
                currentAccelaration = 0;
            }
            direction.x = horizontalInput * speed * currentAccelaration;
        }        

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
        bool isFacingWall = Physics.CheckSphere(wallCheck.position, 1.3f, wallLayer);
        isSliding = isFacingWall && direction.y < 0 && !wallJump; 

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump") && !jumping)
            {
                animator.SetTrigger("Jump");
            }
                
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
                    JumpSound.Play();//Jump sound
                }
                else
                    direction.y += gravity * Time.deltaTime * wallSlideSlowDown;
            }

            else if (Input.GetButtonDown("Jump") && !isFacingWall && ableToDoubleJump)
            {
                direction.y = secondJumpForce;
                ableToDoubleJump = false;
                JumpSound.Play();//Jump sound
                animator.SetTrigger("DoubleJump");
            }
        }

        if (!turning && isMoving != (direction.x != 0))
        {
            isMoving = direction.x != 0;
            animator.SetBool("Moving", isMoving);
        }




        controller.Move(direction * Time.deltaTime);



    }

    public void finishTurn()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
        turning = false;   
    }

    public void Jump()
    {
        direction.y = jumpForce;
        controller.Move(direction * Time.deltaTime);
        JumpSound.Play();//Jump sound
        jumping = false;
    }

}
