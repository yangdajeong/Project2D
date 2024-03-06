using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] Animator animator;

    [Header("Property")]
    [SerializeField] float movePower;
    [SerializeField] float brakePower;
    [SerializeField] float wallJumpPower;
    [SerializeField] float maxXSpeed;
    [SerializeField] float maxYSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float slidingSpeed;


    [SerializeField] GroundChecker groundChecker;
    [SerializeField] WallChecker wallChecker;

    private Vector2 moveDir;

    float isRight = 1;


    private void FixedUpdate()
    {
        Move();

        if(wallChecker.IsWall)
        {
            animator.SetBool("Flip", false);
            //rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * slidingSpeed);

        }
    }

    private void Move()
    {
        //최대 속도를 지정함
        if (moveDir.x < 0 && rigid.velocity.x > -maxXSpeed)
        {
            rigid.AddForce(Vector2.right * moveDir.x * movePower);
        }
        else if(moveDir.x > 0 && rigid.velocity.x < maxXSpeed)
        {
            rigid.AddForce(Vector2.right * moveDir.x * movePower);
        }
        //감속
        else if (moveDir.x == 0 && rigid.velocity.x > 0.1f)
        {
            rigid.AddForce(Vector2.left * brakePower);
        }
        else if (moveDir.x == 0 && rigid.velocity.x < -0.1f)
        {
            rigid.AddForce(Vector2.right * brakePower);
        }

        //낙하 지정속도
        if (rigid.velocity.y < -maxYSpeed)
        {
            Vector2 velocity = rigid.velocity;
            velocity.y = -maxYSpeed;
            rigid.velocity = velocity;

        }

        animator.SetFloat("YSpeed", rigid.velocity.y);
    }

    private void Jump()
    {
        Vector2 velocity = rigid.velocity;
        velocity.y = jumpSpeed;
        rigid.velocity = velocity;
    }

    private void WallJump()
    {
        rigid.velocity = new Vector2(-isRight * wallJumpPower, 0.9f * wallJumpPower);
        FlipPlayer();
    }

    void FlipPlayer()
    {
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        isRight = isRight * -1;
    }


    private void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();

        if(moveDir.x < 0) 
        {
            render.flipX = true;
            animator.SetBool("Run", true);
        }
        else if(moveDir.x > 0)
        {
            render.flipX = false;
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }
    }

    private bool Flip;

    private void OnJump(InputValue value)
    {
        if(value.isPressed && groundChecker.IsGround) //땅에 있는 상태
        {
            Jump();
        }
        else if (value.isPressed && wallChecker.IsWall) //벽에 매달린 상태
        {
            animator.SetBool("Flip", true);
            WallJump();


        }
    }





}
