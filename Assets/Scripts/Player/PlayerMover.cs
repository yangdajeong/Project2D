using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour, IDamagable
{
    [Header("Component")]
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer render;
    [SerializeField] Animator animator;
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] Grunt grunt;

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
    [SerializeField] float RollSpeed;
    [SerializeField] float RollMaxSpeed;

    [Header("DiedProperty")]
    [SerializeField] float xFallDownPower;
    [SerializeField] float yFallDownPower;
    [SerializeField] float hitPower;

    [Header("Checker")]
    [SerializeField] GroundChecker groundChecker;
    [SerializeField] WallChecker wallChecker;

    private bool isWallJump;
    private bool isRoll;

    private float isRight = 1;

    private Vector2 moveDir;

    private bool playerDied = false;
    public bool PlayerDied {  get { return playerDied; } }



    private void FixedUpdate()
    {
        //마우스 공격과 반대방향으로 뛸 때 버그 방어함수
        if (moveDir.x < 0)
        {
            render.flipX = true;
            animator.SetBool("Run", true);
        }
        else if (moveDir.x > 0)
        {
            render.flipX = false;
            animator.SetBool("Run", true);
        }



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
            //왼쪽
            if (moveDir.x < 0 && rigid.velocity.x > -maxXSpeed)
            {
                rigid.AddForce(Vector2.right * moveDir.x * movePower);
               
                //앞으로 가면 구르기
                if(isRoll)
                {
                    rigid.AddForce(Vector2.right * moveDir.x * RollSpeed, ForceMode2D.Impulse);
                    isRoll = false;
                    Roll();

                    // 구르기 x최대 속력

                    if (rigid.velocity.x < -RollMaxSpeed)
                    {
                        rigid.velocity = new Vector2(-RollMaxSpeed, rigid.velocity.y);
                    }

                }
            }


            //오른쪽
            else if (moveDir.x > 0 && rigid.velocity.x < maxXSpeed)
            {
                rigid.AddForce(Vector2.right * moveDir.x * movePower);

                //뒤로 가면 구르기
                if (isRoll)
                {
                    rigid.AddForce(Vector2.right * moveDir.x * RollSpeed, ForceMode2D.Impulse);
                    isRoll = false;
                    Roll();

                    // 구르기 x최대 속력
                    if (rigid.velocity.x > RollMaxSpeed)
                    {
                        rigid.velocity = new Vector2(RollMaxSpeed, rigid.velocity.y);
                    }

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


        if (moveDir.x < 0) 
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



    public virtual void Died()
    {
        Debug.Log("플레이어 다이");

        playerDied = true;
        animator.SetTrigger("IsDied");
        animator.SetBool("Dead", true);


        rigid.AddForce(grunt.GruntDirVec * hitPower, ForceMode2D.Impulse);
        //rigid.AddForce(Vector2.right * hitPower, ForceMode2D.Impulse);

        //hitEffect?.CreateHitEffect();


        //공격받아서 날라가는 y 최대 속력

        // x최대 속력
        if (rigid.velocity.x < -xFallDownPower)
        {
            rigid.velocity = new Vector2(-xFallDownPower, rigid.velocity.y);
        }
        else if (rigid.velocity.x > xFallDownPower)
        {
            rigid.velocity = new Vector2(xFallDownPower, rigid.velocity.y);
        }


        // y최대 속력
        if (rigid.velocity.y < -yFallDownPower)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, -yFallDownPower);
        }
        else if (rigid.velocity.y > yFallDownPower)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, yFallDownPower);
        }


    }


}

    