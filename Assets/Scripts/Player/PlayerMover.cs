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

        // Ȧ�� ���� �� ������� ����
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
        // ���� ���� �� �̵� ��� ����
        if (!isWallJump)
        {

            if (moveDir.x < 0 && rigid.velocity.x > -maxXSpeed)
            {
                rigid.AddForce(Vector2.right * moveDir.x * movePower);
               
                //������ ���� ������
                if(isRoll)
                {
                    isRoll = false;
                    Roll();

                }


            }
            else if (moveDir.x > 0 && rigid.velocity.x < maxXSpeed)
            {
                rigid.AddForce(Vector2.right * moveDir.x * movePower);

                //�ڷ� ���� ������
                if (isRoll)
                {
                    isRoll = false;
                    Roll();

                }
            }

            //����
            else if (moveDir.x == 0 && rigid.velocity.x > 0.1f)
            {
                rigid.AddForce(Vector2.left * brakePower);
            }
            else if (moveDir.x == 0 && rigid.velocity.x < -0.1f)
            {
                rigid.AddForce(Vector2.right * brakePower);
            }

        }

        //���� �����ӵ�
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
        if(value.isPressed && groundChecker.IsGround) //���� �ִ� ����
        {
            Jump();
        }
        else if (value.isPressed && wallChecker.IsWall) //���� �Ŵ޸� ����
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
