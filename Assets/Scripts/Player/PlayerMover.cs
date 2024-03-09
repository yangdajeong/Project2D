using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] float StayWallJumpTime;
    [SerializeField] float RollTime;


    [Header("Checker")]
    [SerializeField] GroundChecker groundChecker;
    [SerializeField] WallChecker wallChecker;

    private bool isWallJump;
    private bool isRoll;

    private float isRight = 1;

    private Vector2 moveDir;




    private void FixedUpdate()
    {


        Move();

        // 홀수 점프 시 방향반전 방지
        if (groundChecker.IsGround)
        {
            if (isRight == -1)
            {
                FlipPlayer();
            }

            animator.SetBool("Flip", false);
        }

        if (wallChecker.IsWall)
        {
            isWallJump = true;
            Invoke("FreezeX", StayWallJumpTime);
            //rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * slidingSpeed);

        }
    }

    private void FreezeX()
    {
        isWallJump = false;
    }

    private void Move()
    {
        // 벽을 탔을 때 이동 잠시 멈춤
        if (!isWallJump)
        {

            if (moveDir.x < 0 && rigid.velocity.x > -maxXSpeed)
            {
                rigid.AddForce(Vector2.right * moveDir.x * movePower);
               
                //앞으로 가면 구르기
                if(isRoll)
                {
                    isRoll = false;
                    Roll();

                }


            }
            else if (moveDir.x > 0 && rigid.velocity.x < maxXSpeed)
            {
                rigid.AddForce(Vector2.right * moveDir.x * movePower);

                //뒤로 가면 구르기
                if (isRoll)
                {
                    isRoll = false;
                    Roll();

                }
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

    private void FlipPlayer()
    {
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        isRight = isRight * -1;
    }

    private void Roll()
    {

        animator.SetBool("IsRoll", true);
        StartCoroutine(StopRoll());
    }

    private IEnumerator StopRoll()
    {

        yield return new WaitForSeconds(RollTime);
        animator.SetBool("IsRoll", false);

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

    private void OnJump(InputValue value)
    {
        if(value.isPressed && groundChecker.IsGround) //땅에 있는 상태
        {
            Jump();
        }
        else if (value.isPressed && wallChecker.IsWall) //벽에 매달린 상태
        {
            isWallJump = true;
            animator.SetBool("Flip", true);
            WallJump();
        }
    }

    private void OnRoll(InputValue value)
    {

        if (value.isPressed)
        {
            isRoll = true;
        }
    }



}
